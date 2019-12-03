using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Interfaces
{
	/// <summary>
	/// Represents browser console
	/// </summary>
	[DomItem]
	public interface IConsole
	{
		/// <summary>
		/// Writes an error message to the console if the assertion is false
		/// </summary>
		void Assert(bool assertion, params object[] objs);

		/// <summary>
		/// Writes an error message to the console if the assertion is false
		/// </summary>
		void Assert(bool assertion, string format, params object[] objs);

		/// <summary>
		/// Clears the console
		/// </summary>
		void Clear();

		
		/// <summary>
		/// Outputs an error message to the console
		/// </summary>
		void Error(params object[] objs);
		
		/// <summary>
		/// Outputs an formatted error message to the console
		/// </summary>
		void Error(string format, params object[] objs);

		/// <summary>
		/// Creates a new inline group in the console. This indents following console messages by an additional level, until console.groupEnd() is called
		/// </summary>
		void Group();
		
		/// <summary>
		/// Creates a new inline group in the console. This indents following console messages by an additional level, until console.groupEnd() is called
		/// </summary>
		void Group(string label);

		/// <summary>
		/// Creates a new inline group in the console. However, the new group is created collapsed. The user will need to use the disclosure button to expand it
		/// </summary>
		void GroupCollapsed();

		/// <summary>
		/// Exits the current inline group in the console
		/// </summary>
		void GroupEnd();

		/// <summary>
		/// Outputs an informational message to the console
		/// </summary>
		void Info(params object[] objs);
		
		/// <summary>
		/// Outputs a formatted informational message to the console
		/// </summary>
		void Info(string format, params object[] objs);

		/// <summary>
		/// Outputs a message to the console
		/// </summary>
		void Log(string format, params object[] objs);
		
		/// <summary>
		/// Outputs a objects to the console
		/// </summary>
		void Log(params object[] objs);

		/// <summary>
		/// Starts a timer (can track how long an operation takes)
		/// </summary>
		void Time(string label);

		/// <summary>
		/// Stops a timer that was previously started by console.time()
		/// </summary>
		void TimeEnd(string label);

		/// <summary>
		/// Outputs a warning message to the console
		/// </summary>
		void Warn(params object[] objs);

		/// <summary>
		/// Outputs a formatted warning message to the console
		/// </summary>
		void Warn(string format, params object[] objs);
		
		/*
		/// <summary>
		/// Outputs a stack trace to the console.
		/// </summary>
		void Trace();
		
		/// <summary>
		/// Logs the number of times that this particular call to count() has been called
		/// </summary>
		void Count();
		
		/// <summary>
		/// Logs the number of times that this particular call to count() has been called
		/// </summary>
		void Count(string label);
		*/
	}
}