using System.Collections.Generic;
using System.Linq.Expressions;

namespace Flow.Domain.Patterns
{
    /// <summary>
    /// see https://codereview.stackexchange.com/questions/116530/in-lining-invocationexpressions
    /// </summary>
    internal static class ExpressionHelpers
    {
        public static TExpressionType InlineInvokes<TExpressionType>(this TExpressionType expression)
            where TExpressionType : Expression
        {
            return (TExpressionType)new InvokeInliner().Inline(expression);
        }

        public static Expression InlineInvokes(this InvocationExpression expression)
        {
            return new InvokeInliner().Inline(expression);
        }

        private class InvokeInliner : ExpressionVisitor
        {
            private readonly Stack<Dictionary<ParameterExpression, Expression>> context = new Stack<Dictionary<ParameterExpression, Expression>>();
            
            public Expression Inline(Expression expression)
            {
                return Visit(expression)!;
            }

            protected override Expression VisitInvocation(InvocationExpression e)
            {
                var callingLambda = e.Expression as LambdaExpression;
                if (callingLambda == null) //Fix as per comment
                    return base.VisitInvocation(e);
                var currentMapping = new Dictionary<ParameterExpression, Expression>();
                for (var i = 0; i < e.Arguments.Count; i++)
                {
                    var argument = Visit(e.Arguments[i]);
                    var parameter = callingLambda.Parameters[i];
                    if (parameter != argument)
                        currentMapping.Add(parameter, argument);
                }
                context.Push(currentMapping);
                var result = Visit(callingLambda.Body);
                context.Pop();
                return result!;
            }

            protected override Expression VisitParameter(ParameterExpression e)
            {
                if (context.Count > 0)
                {
                    var currentMapping = context.Peek();
                    if (currentMapping.ContainsKey(e))
                        return currentMapping[e];
                }
                return e;
            }
        }
    }
}