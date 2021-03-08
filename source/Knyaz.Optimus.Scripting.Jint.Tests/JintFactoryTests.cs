using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using NUnit.Framework;

namespace Knyaz.Optimus.Scripting.Jint.Tests
{
	public class JintFactoryTests
	{
		[Test]
		public void DefineDefaultTypes()
		{
			var eng = JintFactory.Create(new ScriptExecutionContext(null));
			Assert.AreEqual("function", eng.Evaluate("typeof Int8Array"));
		}
	}
}