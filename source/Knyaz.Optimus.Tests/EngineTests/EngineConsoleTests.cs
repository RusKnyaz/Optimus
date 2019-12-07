using System;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.Scripting.Jurassic;
using Knyaz.Optimus.Tests.ScriptExecuting;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.EngineTests
{
	[TestFixture(JsEngines.Jint)]
	[TestFixture(JsEngines.Jurassic)]
	public class EngineConsoleTests
	{
		private readonly JsEngines _engine;

		public EngineConsoleTests(JsEngines engine) => _engine = engine;

		private Engine CreateEngine(IConsole console)
		{
			var builder = EngineBuilder.New().Window(w => w.SetConsole(console));
			switch (_engine)
			{
				case JsEngines.Jint: builder.UseJint();
					break;
					case JsEngines.Jurassic: builder.UseJurassic();
						break;
					default:
						throw new ArgumentException();
			}

			return builder.Build();
		}
		
		
		[TestCase("console.clear()")]
		[TestCase("console.clear('string to be ignored')")]
		public void Clear(string code)
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", code);
			
			consoleMock.Verify(x => x.Clear(), Times.Once);
		}
		
		
		[Test]
		public void LogOneObject()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.log(window)");
			
			consoleMock.Verify(x => x.Log(engine.Window), Times.Once);
		}

		[Test]
		public void LogMultipleObjects()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.log(window, 1.1)");
			
			consoleMock.Verify(x => x.Log(engine.Window, 1.1d), Times.Once);
		}

		[Test]
		public void LogString()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.log('hello')");
			
			consoleMock.Verify(x => x.Log("hello"), Times.Once);
		}

		[Test]
		public void LogFormattedString()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.log('hello', window)");
			
			consoleMock.Verify(x => x.Log("hello", engine.Window), Times.Once);
		}
		
		
		[Test]
		public void WarnOneObject()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.warn(window)");
			
			consoleMock.Verify(x => x.Warn(It.IsAny<object>()), Times.Once);
		}

		[Test]
		public void WarnMultipleObjects()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.warn(window, 1.1)");
			
			consoleMock.Verify(x => x.Warn(engine.Window, 1.1d), Times.Once);
		}

		[Test]
		public void WarnString()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.warn('hello')");
			
			consoleMock.Verify(x => x.Warn("hello"), Times.Once);
		}

		[Test]
		public void WarnFormattedString()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.warn('hello', window)");
			
			consoleMock.Verify(x => x.Warn("hello", engine.Window), Times.Once);
		}

		[Test]
		public void Group()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.group()");
			
			consoleMock.Verify(x => x.Group(), Times.Once);
		}
		
		[Test]
		public void GroupLabel()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.group('group name')");
			
			consoleMock.Verify(x => x.Group("group name"), Times.Once);
		}

		[TestCase("console.groupEnd()")]
		[TestCase("console.groupEnd('argument to be ignored')")]
		public void GroupEnd(string code)
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", code);
			
			consoleMock.Verify(x => x.GroupEnd(), Times.Once);
		}

		[Test]
		public void AssertObjs()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.assert(true, window)");
			
			consoleMock.Verify(x => x.Assert(true, engine.Window), Times.Once);
		}

		[Test]
		public void AssertFormattedString()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.assert(false, 'msg', 1.1)");
			
			consoleMock.Verify(x => x.Assert(false, "msg", 1.1d), Times.Once);
		}

		[Test, Ignore("Behaviour is undefined")]
		public void LogJsObjectArrayAndFunc()
		{
			var consoleMock = new Mock<IConsole>();

			var engine = CreateEngine(consoleMock.Object);
			
			engine.ScriptExecutor.Execute("text/javascript", "console.log({x:1}, [1,2,{x:3}], function(){})");
		}
	}
}