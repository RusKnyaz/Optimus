#if NUNIT
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class TokenListTests
	{
		TokenList _target;

		[SetUp]
		public void SetUp()
		{
			_target = new TokenList(() => string.Empty);
		}


		[Test]
		public void Add()
		{
			_target.Add("A");
			_target.Add("B");
			Assert.AreEqual(2, _target.Count);
			Assert.AreEqual("A", _target[0]);
			Assert.AreEqual("B", _target[1]);
		}

		[Test]
		public void AddTheSameTwice()
		{
			_target.Add("A");
			_target.Add("B");
			_target.Add("A");
			Assert.AreEqual(2, _target.Count);
			Assert.AreEqual("A", _target[0]);
			Assert.AreEqual("B", _target[1]);
		}

		[Test]
		public void Remove()
		{
			_target.Add("A");
			_target.Remove("A");
			Assert.AreEqual(0, _target.Count);
		}

		[Test]
		public void Toggle()
		{
			Assert.IsTrue(_target.Toggle("A"));
			Assert.AreEqual(1, _target.Count);
			Assert.AreEqual("A", _target[0]);

			Assert.IsTrue(_target.Toggle("B"));
			Assert.AreEqual(2, _target.Count);
			Assert.AreEqual("A", _target[0]);
			Assert.AreEqual("B", _target[1]);

			Assert.IsFalse(_target.Toggle("A"));
			Assert.AreEqual(1, _target.Count);
			Assert.AreEqual("B", _target[0]);

			Assert.IsFalse(_target.Toggle("B"));
			Assert.AreEqual(0, _target.Count);
		}

		[Test]
		public void ToggleForce()
		{
			Assert.IsFalse(_target.Toggle("A", false));
			Assert.AreEqual(0, _target.Count);

			Assert.IsTrue(_target.Toggle("A", true));
			Assert.AreEqual(1, _target.Count);
			Assert.AreEqual("A", _target[0]);

			Assert.IsTrue(_target.Toggle("A", true));
			Assert.AreEqual(1, _target.Count);
			Assert.AreEqual("A", _target[0]);

			Assert.IsFalse(_target.Toggle("A", false));
			Assert.AreEqual(0, _target.Count);
		}

	}
}
#endif