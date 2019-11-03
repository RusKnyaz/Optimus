﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ScriptExecuting
{
	using Engine = global::Jint.Engine;
	
	internal class DomConverter : IObjectConverter
	{
		private Func<Engine> _getEngine;

		private List<Type> _bindedTypes = new List<Type>();
		private Dictionary<object, JsValue> _cache = new Dictionary<object, JsValue>();
		
		public DomConverter(Func<Engine> getEngine)
		{
			_getEngine = getEngine;

			var domTypes = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttributes<DomItemAttribute>().Any());
			foreach (var domType in domTypes)
			{
				_bindedTypes.Add(domType);
			}
		}

		public void SetGlobal(object global) => _global = global;
		
		public bool TryConvert(object value, out JsValue result)
		{
			var engine = Engine;
			if (engine == null)
			{
				result = JsValue.Undefined;
				return false;
			}

			if (value is Undefined)
			{
				result = JsValue.Undefined;
				return true;
			}

			if (value is Window)
			{
				result = engine.Global;
				return true;
			}

			if (value == null)
			{
				result = JsValue.Null;
				return true;
			}

			if (_cache.TryGetValue(value, out result))
				return true;

			if (value is IList list)
			{
				result = new ListAdapterEx(engine, list);
				_cache[value] = result;
				return true;
			}

			if (_bindedTypes.Any(x => x.IsInstanceOfType(value)))
			{
				result = new ClrObject(engine, value, GetPrototype(value.GetType()));
				_cache[value] = result;
				return true;
			}

			switch (value)
			{
				case string stringValue:
					result = new JsValue(stringValue);
					return true;
				case double d:
					result = new JsValue(d);
					return true;
				case bool b:
					result = new JsValue(b);
					return true;
				case int i:
					result = new JsValue(i);
					return true;
				case JsValue js: result = js;
					return true;
			}

			result = JsValue.Null;
			return false;
		}

		public JsValue ConvertToJs(object clrValue)
		{
			TryConvert(clrValue, out var result);
			return result;
		} 

		protected Engine Engine => _getEngine();

		public Action<T> ConvertDelegate<T>(JsValue @this, JsValue jsFunc)
		{
			if (jsFunc.IsNull())
				return null;
			
			var callable = jsFunc.AsObject() as ICallable;
			Action<T> handler = null;
			if (callable != null)
			{
				handler = (Action<T>) _delegatesCache.GetValue(callable, key => (Action<T>) (e =>
				{
					JsValue val;
					TryConvert(e, out val);
					key.Call(@this, new[] {val});
				}));
			}

			return handler;
		}
		
		/// <summary>
        /// Converts js function to c# delegate with conversion array of arguments.
        /// </summary>
		public Action<object[]> ConvertDelegate(JsValue @this, ICallable callable)
		{
			Action<object[]> handler = null;
			if (callable != null)
			{
				handler = (Action<object[]>) _delegatesCache.GetValue(callable, key =>
					(Action<object[]>) (e =>
					{
						var args = e == null
							? new JsValue[0]
							: e.Select(x =>
							{
								TryConvert(x, out var val);
								return val;
							}).ToArray();

						key.Call(@this, args);
					}));
			}

			return handler;
		}


		readonly ConditionalWeakTable<ICallable, object> _delegatesCache = new ConditionalWeakTable<ICallable, object>();

		public object ConvertFromJs(JsValue jsValue)
		{
			if (jsValue == _getEngine().Global)
				return _global;
			
			if (jsValue.IsUndefined())
				return null;
			
			if (jsValue.IsObject())
			{
				switch (jsValue.AsObject())
				{
					case ClrObject clr:
						return clr.Target;
					case ObjectInstance objInst:
						
						//todo: instantiate object instead of distionary
						return objInst.GetOwnProperties().ToDictionary(x => x.Key, x => 
							ConvertFromJs(objInst.Get(x.Key)));
				}

				return jsValue;
			}
			
			if (jsValue.IsBoolean())
				return jsValue.AsBoolean();

			if (jsValue.IsString())
				return jsValue.AsString();

			if (jsValue.IsNumber())
				return jsValue.AsNumber();

			return null;
		}
		
		private readonly IDictionary<Type, Func<ObjectInstance, object>> _creators = new Dictionary<Type, Func<ObjectInstance, object>>();
		
		private Func<ObjectInstance, object> GetCreator(Type type)
		{
			if(_creators.TryGetValue(type, out var creator))
				return creator;

			var parameterExpression = Expression.Parameter(typeof(ObjectInstance), "jsObj");

			var binds = type.GetFields(BindingFlags.Public | BindingFlags.SetField | BindingFlags.Instance)
				.Select(fieldInfo => (MemberBinding)Expression.Bind(
					fieldInfo,
					CreateConverterExpr(fieldInfo.FieldType, 
						Expression.Call(parameterExpression,
							nameof(ObjectInstance.Get),
							new Type[0], Expression.Constant(fieldInfo.GetName(), typeof(string))),
						Expression.Constant(JsValue.Undefined)))).ToArray();
            
			var creatorExpr = Expression.Lambda<Func<ObjectInstance, object>>(
				Expression.MemberInit(Expression.New(type), binds), parameterExpression);

			creator = creatorExpr.Compile();
            
			_creators.Add(type, creator);

			return creator;
		}

		private readonly Dictionary<Type, ObjectInstance> _prototypes = new Dictionary<Type, ObjectInstance>();

		public ObjectInstance GetPrototype(Type type)
		{
			if (_prototypes.TryGetValue(type, out var prototype))
				return prototype;

			var baseType = type.BaseType;
			var basePrototype = baseType == typeof(object) ? Engine.Object.PrototypeObject : GetPrototype(baseType); 
			prototype = new ClrPrototype(Engine, this,type, basePrototype);
			_prototypes.Add(type, prototype);
			return prototype;
		}
		
		public static short ConvertToShort(JsValue value) => 
			value.IsString() ? short.Parse(value.AsString()) : (short) value.AsNumber();

		public static int ConvertToInt(JsValue value) => value.IsString() ? int.Parse(value.AsString()) : (int) value.AsNumber();

		public static int? ConvertToIntOption(JsValue value) =>
			value.IsNull() || value.IsUndefined() ? null :
			value.IsNumber() ? (int)value.AsNumber() :
			value.IsString() && int.TryParse(value.AsString(), out var i) ? i : (int?)null;

		public static double ConvertToDouble(JsValue value) => 
			value.IsString() ? double.Parse(value.AsString()) :value.AsNumber();

		public static string ConvertToString(JsValue value) => 
			value.IsNull() || value.IsUndefined() ? null : value.ToString();

		public static bool ConvertToBool(JsValue value) =>
			value.IsBoolean() ? value.AsBoolean()
			: value.IsNumber() ? (int) value.AsNumber() != 0
			: !value.IsNull() && !value.IsUndefined();
		
		public static bool? ConvertToBoolOption(JsValue value) =>
			value.IsBoolean() ? value.AsBoolean()
			: value.IsNumber() ? (int) value.AsNumber() != 0
			: (value.IsNull() || value.IsUndefined()) ? (bool?)null : true;

		//todo: try to use generic
		public Delegate ConvertToDelegate(JsValue value, Type type, JsValue jsThis) =>
			ConvertToClr((FunctionInstance)value.AsObject(), type, jsThis);
		
		public Delegate ConvertToDelegate(JsValue value, Type type, JsValue jsThis, bool expandArrays) =>
			ConvertToClr((FunctionInstance)value.AsObject(), type, jsThis, expandArrays);

		public object ConvertToObject(JsValue jsValue, Type targetType)
		{
			if (jsValue == _getEngine().Global)
				return _global;
			
			if (jsValue.IsUndefined())
				return null;
			
			if (jsValue.IsObject())
			{
				switch (jsValue.AsObject())
				{
					case ClrObject clr:
						return clr.Target;
					case ObjectInstance objInst:
						return GetCreator(targetType)(objInst);
				}

				return jsValue;
			}
			
			if (jsValue.IsBoolean())
				return jsValue.AsBoolean();

			if (jsValue.IsString())
				return jsValue.AsString();

			if (jsValue.IsNumber())
				return jsValue.AsNumber();

			return null;
		} 

		public object[] ConvertToObjectArray(JsValue value) => 
			((JsValue[]) value.ToObject()).Select(x => x.ToObject()).ToArray();


		public IDictionary<EventInfo, FunctionInstance> GetAttachedEventsFor(JsValue clrThis)
		{
			var clrObjectInst = (ClrObject) clrThis.AsObject();
			return clrObjectInst.AttachedEvents;
		}
		
		private readonly IDictionary<Tuple<FunctionInstance, JsValue>, Delegate> _jsToClrDelegatesMap = new Dictionary<Tuple<FunctionInstance, JsValue>, Delegate>(
			new TupleComparer<FunctionInstance,JsValue>());

		private object _global;

		void RegisterMap(Delegate clrObject, FunctionInstance jsObject, JsValue jsThis)
		{
			_jsToClrDelegatesMap.Add(new Tuple<FunctionInstance, JsValue>(jsObject, jsThis), clrObject);
			_cache.Add(clrObject, jsObject);
		}
		
		
		public Delegate ConvertToClr(FunctionInstance func, Type targetType, JsValue jsThis, bool expandArrayArgs = false)
		{
			if (typeof(Delegate).IsAssignableFrom(targetType))
            {
                var jsThisInst = jsThis == JsValue.Undefined ? Engine.Global : jsThis;
                
                //we have to pin 'this' to the handler.
                if (_jsToClrDelegatesMap.TryGetValue(new Tuple<FunctionInstance, JsValue>(func, jsThisInst), out var del))
                    return del;
                
                if (targetType == typeof(System.Action))
                {
                    var expressionBody = Expression.Call(
                        Expression.Constant(func, typeof(FunctionInstance)),
                        nameof(FunctionInstance.Call), 
                        new Type[0], 
                        Expression.Constant(jsThis), 
                        Expression.Constant(new JsValue[0]));

                    var lambda = Expression.Lambda<Action>(expressionBody);
                    var compiled = lambda.Compile();

                    RegisterMap(compiled, func, jsThisInst);

                    return compiled;
                }
                else
                {
                    var generic = targetType.IsGenericTypeDefinition ? targetType :
                        targetType.IsGenericType ? targetType.GetGenericTypeDefinition() : null;

                    if (generic != null)
                    {
                        if (targetType == typeof(Action<object[]>) && expandArrayArgs)
                        {
                            //special case
                            Action<object[]> handler = args => {
                                var jsArgs = 
	                                args != null ? 
	                                args.Select(ConvertToJs).ToArray()
	                                : new JsValue[0];
                                func.Call(jsThis, jsArgs);
                            };
                            
                            RegisterMap(handler, func, jsThisInst);

                            return handler;
                        }
                        else if (generic == typeof(System.Action<>) || generic == typeof(Func<,>))
                        {
                            var genArgs = targetType.GetGenericArguments();

                            var parameterExpression = Expression.Parameter(genArgs[0], "arg1");

                            var argumentsArrayInit = Expression.NewArrayInit(typeof(JsValue),
                                Expression.Call(
                                    Expression.Constant(this),
                                    nameof(ConvertToJs),
                                    new Type[0],
                                    Expression.Convert(parameterExpression, typeof(object))
                                )
                            );

                            var callJsFuncExpr = (Expression)Expression.Call(
                                Expression.Constant(func, func.GetType()),
                                nameof(func.Call), new Type[0], 
                                Expression.Constant(jsThis),
                                argumentsArrayInit);
                            

                            //for Func we have to convert jsValue to the target type.
                            var funcReturnType = targetType.GetGenericArguments().Last();
                            if (generic == typeof(Func<,>))
                                callJsFuncExpr = CreateConverterExpr(
	                                funcReturnType, 
	                                callJsFuncExpr, 
	                                Expression.Constant(jsThis));

                            var lambda = Expression.Lambda(targetType, callJsFuncExpr, parameterExpression);
                            var compiled = lambda.Compile();

                            RegisterMap(compiled, func, jsThisInst);

                            return compiled;
                        }
                        else if (generic == typeof(System.Action<,>)) //todo: two and more arguments callback
                        {
                            
                            throw new NotImplementedException("Support of callback with multiple arguments not implemented");
                        }
                    }
                }
                
                throw new NotImplementedException();
            }
			throw new InvalidOperationException($"The FunctionInstance can not be converted to the target type: {targetType.Name}");
		}

		public Expression CreateConverterExpr(Type type, Expression getItemExpression, Expression getThisExpression) =>
			CreateConverterExpr(type, getItemExpression, getThisExpression, false);
		
		public Expression CreateConverterExpr(Type type, Expression getItemExpression, Expression getThisExpression, bool expandArray)
		{
			var typeEmptyArray = new Type[0];
	        
			if (type == typeof(int))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToInt), typeEmptyArray, getItemExpression);
			
			if (type == typeof(int?))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToIntOption), typeEmptyArray, getItemExpression);
	        
			if(type == typeof(bool))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToBool), typeEmptyArray, getItemExpression);
	        
			if(type == typeof(bool?))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToBoolOption), typeEmptyArray, getItemExpression);
	        
			if(type == typeof(string))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToString), typeEmptyArray, getItemExpression);
	        
			if(type == typeof(double))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToDouble), typeEmptyArray, getItemExpression);
			
			if(type == typeof(short))
				return Expression.Call(typeof(DomConverter), nameof(ConvertToShort), typeEmptyArray, getItemExpression);

			var converterConst = Expression.Constant(this);
			
			if (type.IsSubclassOf(typeof(Delegate)))
			{
				return 
					Expression.Convert(
						Expression.Call(converterConst, nameof(ConvertToDelegate), typeEmptyArray, 
							getItemExpression,
							//Expression.Constant(JsValue.Undefined),
							Expression.Constant(type),
							Expression.Call(converterConst, nameof(ConvertToJs), typeEmptyArray, getThisExpression),
							Expression.Constant(expandArray)),
						type);
			}
			
			if(type == typeof(object[]))
			{
				return Expression.Call(converterConst, nameof(ConvertToObjectArray), typeEmptyArray, getItemExpression);
			}

			var result = (Expression)Expression.Call(converterConst, nameof(ConvertToObject), typeEmptyArray,
				getItemExpression,
				Expression.Constant(type));
	        
			return  type != typeof(object) ? Expression.Convert(result,type) : result;
		}

		public void DefineProperties(ObjectInstance jsObject, Type type)
		{
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
			//register properties
			foreach (var property in type.GetProperties(bindingFlags)
				.Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null))
			{
				var name = property.GetName();

				var jsProperty = new ClrProperty(jsObject.Engine, this, property);

				jsObject.DefineOwnProperty(name, jsProperty, false);
			}

			//register methods
			foreach (var methods in type
				.GetMethods(bindingFlags)
				.Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null)
				.GroupBy(p => p.GetName()))
			{
				var methodsArray = methods.ToArray();
				var name = methods.Key;
				if (name == "toString")
					continue;

				var jsProperty = ClrMethodInfoFunc.Create(jsObject.Engine, this, name, methodsArray, jsObject);

				jsObject.DefineOwnProperty(name, jsProperty, false);
			}

			//register events
			foreach (var evt in type.GetEvents(bindingFlags)
				.Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null))
			{
				var name = evt.GetName().ToLowerInvariant();
				var jsProperty = new ClrEvent(jsObject.Engine, this, evt);
				jsObject.DefineOwnProperty(name, jsProperty, false);
			}
			
			
			DefineStatic(jsObject, type);
		}

		public static void DefineStatic(ObjectInstance jsObject, Type type)
		{
			foreach (var staticField in type.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(p => p.GetCustomAttribute<JsHiddenAttribute>() == null))
			{
				var clrValue = staticField.GetValue(null);

				jsObject.FastAddProperty(staticField.Name, JsValue.FromObject(jsObject.Engine, clrValue), false, false, false);
			}
		}
	}

	internal class DomItemAttribute : Attribute
	{
	}

	/// <summary>
	/// For of jint's list adapter
	/// </summary>
	internal class ListAdapterEx : ArrayInstance, IObjectWrapper
	{
		private readonly Engine _engine;
		private readonly IList _list;

		public ListAdapterEx(Engine engine, IList list)
			: base(engine)
		{
			_engine = engine;
			_list = list;
			Prototype = engine.Array;
		}

		public override void Put(string propertyName, JsValue value, bool throwOnError)
		{
			int index;
			if (int.TryParse(propertyName, out index))
			{
				//todo: resize the list if index is greater then count

				if (_list.Count > index)
					_list[index] = value.ToObject();
			}

			//base.Put(propertyName, value, throwOnError);
		}

		public override JsValue Get(string propertyName)
		{
			int index;
			if (int.TryParse(propertyName, out index))
			{
				return _list.Count > index ? JsValue.FromObject(_engine, _list[index]) : JsValue.Undefined;
			}

			return base.Get(propertyName);
		}

		public override PropertyDescriptor GetOwnProperty(string propertyName)
		{
			if (Properties.ContainsKey(propertyName))
				return Properties[propertyName];

			if (propertyName == "length")
			{
				var p = new PropertyDescriptor(
					new ClrFunctionInstance(_engine, (value, values) => _list.Count),
					new ClrFunctionInstance(_engine, (value, values) =>
					{
						//todo: resize list
						return value;
					}));

				Properties.Add(propertyName, p);
			}

			var index = 0u;
			if (uint.TryParse(propertyName, out index))
			{
				return new IndexDescriptor(Engine, propertyName, Target);
			}

			return base.GetOwnProperty(propertyName);
		}

		public object Target { get { return _list; } }
	}
}