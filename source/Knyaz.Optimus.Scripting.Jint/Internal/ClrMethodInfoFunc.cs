using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Scripting.Jint.Internal
{
	/// <summary>
	/// Adapter to convert the MethodInfo to FunctionInstance
	/// </summary>
	internal class ClrMethodInfoFunc : FunctionInstance
	{
		private readonly DomConverter _converter;
		private readonly JsValue _owner;
		private readonly MethodInfo[] _methods;
		private readonly Func<object, JsValue[], object>[] _invokers;

		public ClrMethodInfoFunc(DomConverter converter, global::Jint.Engine engine, MethodInfo[] methods, JsValue owner)
		    : base(engine, null, null, false)
		{
		    _converter = converter;
		    _owner = owner;
		    _methods = methods.OrderByDescending(x => x.GetParameters().Length).ToArray();
		    _invokers = new Func<object, JsValue[], object>[methods.Length];
		    Prototype = engine.Function.PrototypeObject;

		    var name = methods.First().GetName();
		    FastAddProperty("name", new JsValue(name), false, false, false);
		}

		public override JsValue Call(JsValue thisObject, JsValue[] argumentValues)
		{
		    int methodIndex =  _methods.IndexOf(x => x.GetParameters().IsAppropriate(argumentValues, true));    
		    if(methodIndex < 0)
		        methodIndex =_methods.IndexOf(x => x.GetParameters().IsAppropriate(argumentValues, false));
		    
		    if(methodIndex < 0)
			    return null; //or throw exception;
		    
		    var invoker = _invokers[methodIndex];
		    if (invoker == null)
		    {
		        _invokers[methodIndex] = invoker = CreateInvoker(_methods[methodIndex]);
		    }
		    
		    var clrObject = _converter.ConvertFromJs(thisObject == JsValue.Undefined ? _owner : thisObject);

		    if (clrObject is ClrPrototype prototype)
		    {
			    if (_methods.Length == 1 && _methods[0].Name == "ToString")
			    {
				    return $"[object {prototype.Class}]";
			    }
			    
			    throw new JavaScriptException("Illegal method invocation.");
		    }
		    
		    try
		    {
		        return JsValue.FromObject(Engine, invoker(clrObject, argumentValues));
		    }
		    catch (Exception e)
		    {
		        throw new JavaScriptException(e.ToString());
		    }
		}

		public static JsValue[] Slice(JsValue[] original, int idx)
		{
			if(idx >= original.Length)
				return new JsValue[0];
			
			var arr = new JsValue[original.Length - idx];
			Array.Copy(original, idx, arr, 0, arr.Length);
			return arr;
		}

		private Func<object, JsValue[], object> CreateInvoker(MethodInfo method)
		{
		    var methodParameters = method.GetParameters();

		    var expressionParamThis = Expression.Parameter(typeof(object), "clrThis");
		    var expressionParamArgs = Expression.Parameter(typeof(JsValue[]), "jsArgs");

		    var arguments =
		        methodParameters.Select((x, idx) => {
			        if (x.GetCustomAttribute<ParamArrayAttribute>() != null)
			        {
				        var getItemExpression = 
					        Expression.Call(GetType(), nameof(Slice), new Type[0], expressionParamArgs, Expression.Constant(idx));

				        return _converter.CreateConverterExpr(x.ParameterType, getItemExpression, expressionParamThis, false);
			        }
			        else
			        {
				        var getItemExpression = (Expression) Expression.ArrayAccess(expressionParamArgs, Expression.Constant(idx));

				        return
					        Expression.Condition(
						        Expression.LessThan(Expression.Constant(idx),
							        Expression.ArrayLength(expressionParamArgs)),
						        _converter.CreateConverterExpr(x.ParameterType, getItemExpression, expressionParamThis,
							        method.GetInterfaceDeclarationsForMethod().Any(p =>
								        p.GetParameters()[idx].GetCustomAttribute<JsExpandArrayAttribute>() != null)),
						        Expression.Convert(
							        x.HasDefaultValue
								        ? Expression.Constant(x.DefaultValue)
								        : Expression.Constant(GetDefault(x.ParameterType)),
							        x.ParameterType)
					        );
			        }
		        }).ToArray();

		    var expressionCall = Expression.Call(
		        Expression.Convert(expressionParamThis, method.DeclaringType), 
		        method,
		        arguments);


		    if (method.ReturnParameter.ParameterType == typeof(void))
		    {
			    var act = Expression.Lambda<Action<object, JsValue[]>>(
				    expressionCall, 
				    expressionParamThis, 
				    expressionParamArgs).Compile();
			    
		        return (clrThis, jsArgs) => { act(clrThis, jsArgs);return null;};
		    }

		    var invoker = Expression.Lambda<Func<object, JsValue[], object>>(
				    Expression.Convert(expressionCall, typeof(object)),
				    expressionParamThis,
				    expressionParamArgs).Compile(); 
		    
		    return invoker;    
		}

		public static object GetDefault(Type type)
		{
		    if(type.IsValueType)
		    {
		        return Activator.CreateInstance(type);
		    }
		    return null;
		}

		public static PropertyDescriptor Create(global::Jint.Engine engine,
		    DomConverter converter, 
		    string propertyName, 
		    MethodInfo[] methods,
		    JsValue owner)
		{
		    var func = new ClrMethodInfoFunc(converter, engine, methods, owner);
		    //todo: check if this necessary
		    func.FastAddProperty("toString",
		        new JsValue(new ClrFunctionInstance(engine,
			        (value, values) => new JsValue("function " + propertyName + "() { [native code] }"))),
		        false, false, false);

		    return new PropertyDescriptor(func, false, true, false);
		}        
    }
}