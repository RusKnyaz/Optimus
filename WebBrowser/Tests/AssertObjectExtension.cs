#if NUNIT
using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebBrowser.Tests
{
	public static class AssertObjectExtension
	{
		public static void Assert<T>(this T obj, Expression<Func<T, bool>> predicate)
		{
			Assert(obj, predicate, predicate.Body);
		}

		private static void Assert<T>(T obj, Expression<Func<T, bool>> parent, Expression expression)
		{
			var binary = expression as BinaryExpression;
			if (binary != null)
			{
				Assert(obj, parent, binary);
				return;
			}

			var unary = expression as UnaryExpression;
			if (unary != null && unary.NodeType == ExpressionType.Not)
			{
				var result = Invoke(obj, parent, unary.Operand);
				NUnit.Framework.Assert.IsFalse((bool)result, GetMessage(unary.Operand));
			}
			else
			{
				var result = Invoke(obj, parent, expression);
				NUnit.Framework.Assert.IsTrue((bool)result, GetMessage(expression));
			}
		}

		private static void Assert<T>(T obj, Expression<Func<T, bool>> parent, BinaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.AndAlso)
			{
				Assert(obj, parent, expression.Left);
				Assert(obj, parent, expression.Right);
				return;
			}

			var leftResult = Invoke(obj, parent, expression.Left);
			var rightResult = Invoke(obj, parent, expression.Right);

			switch (expression.NodeType)
			{
				case ExpressionType.Equal:
					NUnit.Framework.Assert.AreEqual(rightResult, leftResult, GetMessage(expression.Left));
					break;
				case ExpressionType.NotEqual:
					NUnit.Framework.Assert.AreNotEqual(rightResult, leftResult, GetMessage(expression.Left));
					break;
				case ExpressionType.GreaterThan:
					//todo: be carefull with unconditional convertation to decimal
					NUnit.Framework.Assert.Greater((decimal)rightResult, (decimal)leftResult, GetMessage(expression.Left));
					break;
				case ExpressionType.GreaterThanOrEqual:
					NUnit.Framework.Assert.GreaterOrEqual((decimal)rightResult, (decimal)leftResult, GetMessage(expression.Left));
					break;
				case ExpressionType.LessThan:
					NUnit.Framework.Assert.Less((decimal)rightResult, (decimal)leftResult, GetMessage(expression.Left));
					break;
				case ExpressionType.LessThanOrEqual:
					NUnit.Framework.Assert.LessOrEqual((decimal)rightResult, (decimal)leftResult, GetMessage(expression.Left));
					break;
				default:
					throw new InvalidOperationException("invalid assertion expression");
			}
		}

		private static object Invoke<T>(T obj, Expression<Func<T, bool>> parent, Expression expression)
		{
			if (expression.NodeType == ExpressionType.MemberAccess)
			{
				var memberAcc = expression as MemberExpression;
				var memberResult = Invoke(obj, parent, memberAcc.Expression);
				NUnit.Framework.Assert.IsNotNull(memberResult, memberAcc.Expression.ToString());
			}

			if (expression.NodeType == ExpressionType.Call)
			{
				var call = (MethodCallExpression)expression;
				var callObj = call.Object ?? call.Arguments[0];
				var objResult = Invoke(obj, parent, callObj);
				NUnit.Framework.Assert.IsNotNull(objResult, callObj.ToString());
			}

			var lambdaExpr = Expression.Lambda<Func<T, object>>(
				Expression.Convert(expression, typeof(object)), parent.Parameters.ToArray());
			var exprCompiled = lambdaExpr.Compile();
			return exprCompiled.Invoke(obj);
		}

		private static string GetMessage(Expression expr)
		{
			var convert = expr as UnaryExpression;
			if (convert != null && convert.NodeType == ExpressionType.Convert)
			{
				return GetMessage(convert.Operand);
			}

			return expr.ToString();
		}
	}
}
#endif