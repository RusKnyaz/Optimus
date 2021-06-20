using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Moq;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class EventTargetTests
	{
		public interface IEventHandlers
		{
			void OnParentBubble(Event evt);
			void OnChildBubble(Event evt);
			void OnRootBubble(Event evt);
			void OnRootCapture(Event evt);
		}

		private object _childOriginal;
		private object _parentOriginal;
		private object _rootOriginal;

		private EventTarget _child;
		private EventTarget _parent;
		private EventTarget _root;

		private Mock<IEventHandlers> _handlers;

		[SetUp]
		public void SetUp()
		{
			_handlers = new Mock<IEventHandlers>();

			_rootOriginal = new object();
			_root = new EventTarget(_rootOriginal, () => null);
			_root.AddEventListener("click", _handlers.Object.OnRootBubble, false);
			_root.AddEventListener("click", _handlers.Object.OnRootCapture, true);

			_parentOriginal = new object();
			_parent = new EventTarget(_parentOriginal, () => _root);
			_parent.AddEventListener("click", _handlers.Object.OnParentBubble, false);

			_childOriginal = new object();
			_child = new EventTarget(_childOriginal, () => _parent);
			_child.AddEventListener("click", _handlers.Object.OnChildBubble, false);
		}

		[Test]
		public void UnbubblableEvent()
		{
			var doc = new HtmlDocument();
			var evt = doc.CreateEvent("Event");
			evt.InitEvent("click", false, false);

			_child.DispatchEvent(evt);

			_handlers.Verify(x => x.OnChildBubble(evt), Times.Once());
			_handlers.Verify(x => x.OnParentBubble(evt), Times.Never());
			_handlers.Verify(x => x.OnRootBubble(evt), Times.Never());
		}

		[Test]
		public void RootEvent()
		{
			var doc = new HtmlDocument();
			var evt = doc.CreateEvent("Event");
			evt.InitEvent("click", true, true);
			_root.DispatchEvent(evt);
			_handlers.Verify(x => x.OnRootBubble(evt), Times.Once());
			_handlers.Verify(x => x.OnRootCapture(evt), Times.Once());
		}
	}
}