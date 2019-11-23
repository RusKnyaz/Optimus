using System;

namespace Knyaz.Optimus.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private readonly Func<IJsScriptExecutor> _getJsScriptExecutor;
		private IJsScriptExecutor _jsEngine;
		

		
		public ScriptExecutor(Func<IJsScriptExecutor> getJsScriptExecutor)
		{
			_getJsScriptExecutor = getJsScriptExecutor;
			_jsEngine = getJsScriptExecutor();
		}

		
		public void Execute(string type, string code)
		{
			if (code == null) //if error occurred on script loading.
				return;

			if (string.IsNullOrEmpty(type) || type.ToLowerInvariant() == "text/javascript")
			{
				try
				{
					_jsEngine.Execute(code);
				}
				catch (Exception e)
				{
					if (OnException != null)
						OnException(e);
				}
			}
		}

		public object Evaluate(string type, string code)
		{
			if (string.IsNullOrEmpty(type) || type.ToLowerInvariant() == "text/javascript")
			{
				return _jsEngine.Evaluate(code);
			}

			throw new Exception("Unsupported script type: " + type);
		}

		public event Action<Exception> OnException;
		public void Clear()
		{
			(_jsEngine as IDisposable)?.Dispose();
			_jsEngine = _getJsScriptExecutor();
		}

		public object EvalFuncAndCall(string code, object @this, params object[] args)
		{
			var funcCode = "(function(){ return "+code+";})()";

			var func = Evaluate("text/javascript", funcCode) as Func<object, object[], object>;

			try
			{
				return func(@this, args);
			}
			catch (Exception e)
			{
				OnException?.Invoke(e);
			}
			return null;
		}
	}


	[Serializable]
	public class ScriptExecutingException : Exception
	{
		public ScriptExecutingException()
		{
		}

		public ScriptExecutingException(string message) : base(message)
		{
		}

		public ScriptExecutingException(string message, Exception inner) : base(message, inner)
		{
		}

		public ScriptExecutingException(string message, Exception inner, string code)
			: base(message ?? "Script executing error.", inner)
		{
			Code = code;
		}

		public string Code { get; private set; }
	}
}
