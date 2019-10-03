using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jurassic;
using Jurassic.Library;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    internal class ClrTypeConverter
    {
        private readonly ScriptEngine _engine;
        private readonly Func<object> _globalFn;

        private object ClrGlobal => _globalFn(); 

        public ScriptEngine Engine => _engine;

        public ClrTypeConverter(ScriptEngine engine, Func<object> globalFn)
        {
            _engine = engine;
            _globalFn = globalFn;
        }

        private readonly Dictionary<object, object> _clrToJsObjects = new Dictionary<object, object>();
        private readonly IDictionary<object, object> _jsToClrMap = new Dictionary<object, object>();
        private readonly IDictionary<Tuple<ObjectInstance, ObjectInstance>, Delegate> _jsToClrDelegatesMap = new Dictionary<Tuple<ObjectInstance, ObjectInstance>, Delegate>(
            new TupleComparer()); 

        void RegisterMap(object clrObject, object jsObject)
        {
            _jsToClrMap.Add(jsObject, clrObject);
            _clrToJsObjects.Add(clrObject, jsObject);
        }
        
        void RegisterMap(Delegate clrObject, FunctionInstance jsObject, ObjectInstance jsThis)
        {
            _jsToClrDelegatesMap.Add(new Tuple<ObjectInstance, ObjectInstance>(jsObject, jsThis), clrObject);
            _clrToJsObjects.Add(clrObject, jsObject);
        }
        
        public object ConvertToJs(object obj)
        {
            if (obj == null)
                return Null.Value;
            
            if(obj == _globalFn())
                return _engine.Global;

            if (obj is int || obj is double || obj is ushort || obj is short || obj is float || obj is decimal || obj is long || obj is ulong)
                return Convert.ToDouble(obj);
            
            if (obj is string || obj is bool)
                return obj;

            if (_clrToJsObjects.TryGetValue(obj, out var jsObj))
                return jsObj;
            
            //todo: check if the func and action are necessary
            if (obj is Func<object, object> func)
                jsObj = new FuncInst(this, func);
            else if(obj is Action<object, object[]> actObjs)
                jsObj= new ActInst(this, actObjs);
            else
                jsObj = new ClrObjectInstance(this, obj, GetPrototype(obj.GetType()));

            RegisterMap(obj, jsObj);
            
            return jsObj;
        }
        
        public object ConvertToClr(object jsObject)
        {
            if (jsObject == null || jsObject == Null.Value || jsObject == global::Jurassic.Undefined.Value)
                return null;
            
            if (jsObject == Engine.Global)
                return ClrGlobal;
            
            if (jsObject is ConcatenatedString cstr)
            {
                return cstr.ToString();
            }

            if (_jsToClrMap.TryGetValue(jsObject, out var clr))
                return clr;
            
            if (jsObject is ClrObjectInstance clrObjInstance)
            {
                return clrObjInstance.Target;
            }

            if (jsObject is FuncInst func)
            {
                return func.Target;
            }

            if (jsObject is ActInst act)
            {
                return act.Target;
            }

            if (jsObject is ArrayInstance arr)
            {
                return arr.ElementValues.ToArray();
            }

            if (jsObject is FunctionInstance functionInstance)
            {
                var clrObj = (Func<object, object[], object>)((@this, args) =>
                {
                    var jsThis = ConvertToJs(@this);
                    var jsArgs = args.Select(x => ConvertToJs(x)).ToArray();
                    var jsResult = functionInstance.Call(jsThis, jsArgs);
                    return ConvertToClr(jsResult);
                });
                
                RegisterMap(clrObj, jsObject);
                return clrObj;
            }
            
            
            return jsObject;
        }


        public object ConvertToClr(object jsObject, Type targetType, object jsThis) =>
            ConvertToClr(jsObject, targetType, jsThis, false);

        /// <summary>
        /// Used to convert method's arguments
        /// </summary>
        public object ConvertToClr(object jsObject, Type targetType, object jsThis, bool expandArrayArgs)
        {
            if (targetType == typeof(bool))
                return ConvertBoolToClr(jsObject);
            
            if (jsObject == null)
                return null;

            if (targetType == typeof(double))
                return Convert.ToDouble(jsObject);

            if (targetType == typeof(double?))
                return jsObject == null ? null : (double?)Convert.ToDouble(jsObject);

            if (targetType == typeof(int))
                return Convert.ToInt32(jsObject);

            if (targetType == typeof(uint))
                return Convert.ToUInt32(jsObject);

            if (targetType == typeof(ushort))
                return Convert.ToUInt16(jsObject);

            if (targetType == typeof(short))
                return Convert.ToInt16(jsObject);

            if (targetType == typeof(string))
                return jsObject.ToString();

            if (targetType == typeof(object) && jsObject is int || jsObject is string || jsObject is double)
            {
                return jsObject;
            }
            
            if (_jsToClrMap.TryGetValue(jsObject, out var exist))
                return exist;

            if (jsObject is FunctionInstance func && typeof(Delegate).IsAssignableFrom(targetType))
            {
                var jsThisInst = (ObjectInstance)(jsThis is global::Jurassic.Undefined ? Engine.Global : jsThis);
                
                //we have to pin 'this' to the handler.
                if (_jsToClrDelegatesMap.TryGetValue(new Tuple<ObjectInstance, ObjectInstance>(func, jsThisInst), out var del))
                    return del;
                
                if (targetType == typeof(System.Action))
                {
                    var expressionBody = Expression.Call(
                        Expression.Constant(func, typeof(FunctionInstance)),
                        "Call", 
                        new Type[0], 
                        Expression.Constant(jsThis, typeof(object)), 
                        Expression.Constant(new object[0], typeof(object[])));

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
                                var jsArgs = args.Select(ConvertToJs).ToArray();
                                func.Call(jsThis, jsArgs);
                            };
                            
                            RegisterMap(handler, func, jsThisInst);

                            return handler;
                        }
                        else if (generic == typeof(System.Action<>) || generic == typeof(Func<,>))
                        {
                            var genArgs = targetType.GetGenericArguments();

                            var parameterExpression = Expression.Parameter(genArgs[0], "arg1");

                            var argumentsArrayInit = Expression.NewArrayInit(typeof(object),
                                Expression.Call(
                                    Expression.Constant(this),
                                    "ConvertToJs", 
                                    new Type[0],
                                    Expression.Convert(parameterExpression, typeof(object))
                                )
                            );

                            var expressionBody = (Expression)Expression.Call(
                                Expression.Constant(func, func.GetType()),
                                "Call", new Type[0], 
                                Expression.Constant(jsThis, typeof(object)),
                                argumentsArrayInit);

                            if (generic == typeof(Func<,>))
                                expressionBody = Expression.Convert(expressionBody, targetType.GetGenericArguments().Last());

                            var lambda = Expression.Lambda(targetType, expressionBody, parameterExpression);
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

            if (jsObject is ArrayInstance jsArray && targetType == typeof(object[]))
            {
                return jsArray.ElementValues.Select(ConvertToClr).ToArray();
            }

            if (jsObject is ObjectInstance objectInstance && !(jsObject is ClrObjectInstance))
            {
                return GetCreator(targetType)(objectInstance);
            }
            
            return ConvertToClr(jsObject);
        }

        private static object ConvertBoolToClr(object jsObject)
        {
            return
                jsObject is bool b ? b :
                jsObject is BooleanInstance bi ? bi.Value :
                !(jsObject is global::Jurassic.Undefined) && (jsObject is ObjectInstance || jsObject is string);
        }

        private readonly IDictionary<Type, Func<ObjectInstance, object>> _creators = new Dictionary<Type, Func<ObjectInstance, object>>();

        private Func<ObjectInstance, object> GetCreator(Type type)
        {
            if(_creators.TryGetValue(type, out var creator))
                return creator;

            var parameterExpression = Expression.Parameter(typeof(ObjectInstance), "jsObj");

            //todo: for boolTypes. ConvertBooleanToClr can be called instead of "ConvertToClr"
            
            var binds = type.GetFields(BindingFlags.Public | BindingFlags.SetField | BindingFlags.Instance)
                .Select(fieldInfo => (MemberBinding)Expression.Bind(
                    fieldInfo,
                    Expression.Convert(Expression.Call(Expression.Constant(this, typeof(ClrTypeConverter)),
                        nameof(ConvertToClr),new Type[0],
                        Expression.Call(parameterExpression,
                            "get_Item", new Type[0],
                            Expression.Constant(fieldInfo.GetName(), typeof(string))),
                        Expression.Constant(fieldInfo.FieldType),
                        Expression.Constant(null)), fieldInfo.FieldType))).ToArray();
            
            var creatorExpr = Expression.Lambda<Func<ObjectInstance, object>>(
                    Expression.MemberInit(Expression.New(type), binds), parameterExpression);

            creator = creatorExpr.Compile();
            
            _creators.Add(type, creator);

            return creator;
        }

        private Dictionary<Type, ObjectInstance> _prototypes = new Dictionary<Type,ObjectInstance>();

        /// <summary>
        /// Creates prototype wrapper
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ObjectInstance GetPrototype(Type type)
        {
            if (type == typeof(object))
                return Engine.Object.InstancePrototype;
            
            if (_prototypes.TryGetValue(type, out var prototype))
                return prototype;

            prototype = new ClrPrototype(this, type);
            _prototypes.Add(type, prototype);
            return prototype;
        }

        //todo: use weakreference dictionary
        private Dictionary<object, Dictionary<EventInfo, FunctionInstance>> _attachedEvents = new Dictionary<object, Dictionary<EventInfo, FunctionInstance>>();
        public Dictionary<EventInfo, FunctionInstance> GetAttachedEventsFor(object clrThis)
        {
            if (_attachedEvents.TryGetValue(clrThis, out var events))
                return events;
            
            events = new Dictionary<EventInfo, FunctionInstance>();
            _attachedEvents.Add(clrThis, events);
            return events;
        }
    }

    internal class TupleComparer : IEqualityComparer<Tuple<ObjectInstance,ObjectInstance>>
    {
        public bool Equals(Tuple<ObjectInstance, ObjectInstance> x, Tuple<ObjectInstance, ObjectInstance> y) =>
            Equals(x?.Item1, y?.Item1) && Equals(x?.Item2, y?.Item2);

        public int GetHashCode(Tuple<ObjectInstance, ObjectInstance> obj) =>
            obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
    }

    class FuncInst : FunctionInstance
    {
        private readonly ClrTypeConverter _ctx;
        private readonly Func<object, object> _func;
        private readonly bool _doConvert;

        public Func<object, object> Target => _func;


        public FuncInst(ClrTypeConverter ctx, Func<object, object> func, bool doConvert = true) : base(ctx.Engine)
        {
            _ctx = ctx;
            _func = func;
            _doConvert = doConvert;
        }

        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            try
            {
                var clrThis = _ctx.ConvertToClr(thisObject);
                return _doConvert ? _ctx.ConvertToJs(_func(clrThis)) : _func(clrThis);
            }
            catch (Exception e)
            {
                throw new JavaScriptException(_ctx.Engine, ErrorType.Error, e.Message, e);
            }
        }
    }


    internal class ActInst : FunctionInstance
    {
        private readonly ClrTypeConverter _ctx;
        private readonly Action<object,object[]> _act;

        public Action<object,object[]> Target => _act;

        public ActInst(ClrTypeConverter ctx, Action<object, object[]> act) : base(ctx.Engine)
        {
            _ctx = ctx;
            _act = act;
        }

            
        public override object CallLateBound(object thisObject, params object[] argumentValues)
        {
            try
            {
                var clrThis = _ctx.ConvertToClr(thisObject);
                _act(clrThis, argumentValues);
            }
            catch (Exception e)
            {
                throw new JavaScriptException(Engine, ErrorType.Error, e.Message, e);
            }

            return null;
        }
    }
}