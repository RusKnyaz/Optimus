using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Css.Expression
{
	internal abstract class CssExpression
	{
		public static readonly CssExpression All = new AllExpression();
		public static readonly CssExpression None = new EmptyExpression();
		public static CssExpression Class(string className) => new ClassExpression(className);
		public static CssExpression Tag(string tagName) => new TagExpression(tagName);
		public static CssExpression Id(string id) => new IdExpression(id);
		public static CssExpression Or(IReadOnlyCollection<CssExpression> expression) => 
			new OrExpression(expression);
		
		public static CssExpression Binary(BinaryOperator op, CssExpression left, CssExpression right) => 
			new BinaryExpression(op, left, right);

		public static CssExpression Attr(AttributeOperator op, string attribute, string value) =>
			new AttributeExpression(attribute, value, op);

		public static CssExpression State(CssExpression expression, string state) => 
			new StateExpression(expression, state);
		
		public abstract bool Match(Element elt);

		public virtual int GetSpecificity() => 1;

		public enum BinaryOperator { Ancestor, Parent, And, PrevSibling, Prev }

		class EmptyExpression : CssExpression { public override bool Match(Element elt) => false; }

		class OrExpression : CssExpression
		{
			private readonly IReadOnlyCollection<CssExpression> _expression;

			public OrExpression(IReadOnlyCollection<CssExpression> expression) => _expression = expression;


			public override bool Match(Element elt) => _expression.Any(e => e.Match(elt));

			public override string ToString() => string.Join(",", _expression);

			public override int GetSpecificity() => _expression.Sum(x => x.GetSpecificity());
		}
		
		class BinaryExpression : CssExpression
		{
			private BinaryOperator _operator;
			private CssExpression _left;
			private CssExpression _right;

			public BinaryExpression(BinaryOperator @operator, CssExpression left, CssExpression right)
			{
				_operator = @operator;
				_left = left;
				_right = right;
			}


			public override bool Match(Element elt)
			{
				switch (_operator)
				{
					case BinaryOperator.Parent:
					{
						return (elt.ParentNode is Element parentElt) && (_left.Match(parentElt) && _right.Match(elt));
					}
					case BinaryOperator.Ancestor:
					{
						return
							elt.ParentNode is Element parentElt && parentElt.GetRecursive(x => x.ParentNode as Element)
								.Any(x => x != null && (_left.Match(x) && _right.Match(elt)));
					}
					case BinaryOperator.And: return _left.Match(elt) && _right.Match(elt);
					case BinaryOperator.PrevSibling:
						return GetPrevElement(elt) is Element prevElement && _left.Match(prevElement) && _right.Match(elt);
					case BinaryOperator.Prev:
						return _right.Match(elt) && 
						       GetPrevElement(elt).GetRecursive(GetPrevElement)
						       .Any(x => _left.Match(x));
					default:
						throw new InvalidOperationException("Unknown operator: " + _operator);
				}
			}

			public override string ToString() => _left.ToString() + ConvertOp(_operator) + _right.ToString();

			private static string ConvertOp(BinaryOperator @operator)
			{
				switch (@operator)
				{
					case BinaryOperator.Ancestor: return " ";
					case BinaryOperator.And: return "";
					case BinaryOperator.Parent: return ">";
					case BinaryOperator.Prev: return "~";
					case BinaryOperator.PrevSibling: return "+";
					default:throw new InvalidOperationException();
				}
			}

			public override int GetSpecificity() => _left.GetSpecificity() + _right.GetSpecificity();
		}

		private static Element GetPrevElement(Element elt)
		{
			var result = elt.PreviousSibling;
			while (!(result is Element) && result != null)
				result = result.PreviousSibling;//can be [text] or comment and etc.
			return (Element)result;
		}

		class StateExpression : CssExpression
		{
			private readonly CssExpression _expression;
			private readonly string _state;
			public override bool Match(Element elt) => false; //todo: implement

			public StateExpression(CssExpression expression, string state)
			{
				_expression = expression;
				_state = state;
			}

			public override string ToString() => _expression + ":" + _state;

			public override int GetSpecificity() => (_expression?.GetSpecificity() ?? 0) + 1;
		}
		
		class AllExpression : CssExpression
		{
			public override bool Match(Element elt) => true;
			public override string ToString() => "*";

			public override int GetSpecificity() => 0;
		}
		
		class ClassExpression : CssExpression
		{
			readonly string _className;
			public ClassExpression(string className) => _className = className;

			public override bool Match(Element elt) => ((Element)elt).ClassList.Contains(_className);
			public override int GetSpecificity() => 256;

			public override string ToString() => "." + _className;
		}

		class TagExpression : CssExpression
		{
			private readonly string _tagName;
			public TagExpression(string tagName) => _tagName = tagName;
			public override bool Match(Element elt) => string.Equals(elt.TagName, _tagName, StringComparison.OrdinalIgnoreCase);

			public override string ToString() => _tagName;
		}

		class IdExpression : CssExpression
		{
			private readonly string _id;

			public IdExpression(string id) => _id = id;
			public override bool Match(Element elt) => elt.Id == _id;
			public override int GetSpecificity() => 65536;

			public override string ToString() => "#" + _id;
		}

		public enum AttributeOperator
		{
			Equals,
			StartWith,
			EndWith,
			Contains,
			DelimitedContains,
			DelimitedStartWith,
			Exists
		}
		
		class AttributeExpression : CssExpression
		{
			readonly string _attribute;
			readonly string _value;
			readonly AttributeOperator _operator;

			public AttributeExpression(string attribute, string value, AttributeOperator @operator)
			{
				_attribute = attribute;
				_value = value;
				_operator = @operator;
			}

			public override bool Match(Element elt)
			{
				if (_operator == AttributeOperator.Exists)
					return elt.HasAttribute(_attribute);
				
				if (_value == string.Empty)
					return _operator == AttributeOperator.Equals &&
						elt.GetAttribute(_attribute) == "";
				
				var attr = elt.GetAttribute(_attribute);
				switch (_operator)
				{
					case AttributeOperator.Equals:
						return attr != null && attr.Equals(_value);
					case AttributeOperator.Contains:
						return attr != null && attr.Contains(_value);
					case AttributeOperator.EndWith:
						return attr != null && attr.EndsWith(_value);
					case AttributeOperator.StartWith:
						return attr != null && attr.StartsWith(_value);
					case AttributeOperator.DelimitedStartWith:
						return attr != null && (attr.Equals(_value) || attr.StartsWith(_value+'-'));
					case AttributeOperator.DelimitedContains:
						return attr != null && attr.Split(' ').Contains(_value);
					default:
						throw new InvalidOperationException("Unknown css operator: " + _operator);
				}
			}

			public override int GetSpecificity() => 256;

			public override string ToString() => _attribute + ConvertOp(_operator) + (_value ?? "");


			private static string ConvertOp(AttributeOperator @operator)
			{
				switch (@operator)
				{
					case AttributeOperator.Contains: return "*=";
					case AttributeOperator.Equals: return "=";
					case AttributeOperator.Exists: return "*=";
					case AttributeOperator.DelimitedContains: return "~=";
					case AttributeOperator.EndWith: return "$=";
					case AttributeOperator.StartWith: return "^=";
					case AttributeOperator.DelimitedStartWith: return "|=";
					default:throw new InvalidOperationException();
				}
			}
		}

	}
}