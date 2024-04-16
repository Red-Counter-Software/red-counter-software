namespace RedCounterSoftware.Common.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class LambdaExpressionExtensions
    {
        public static string GetPropertyName(this LambdaExpression selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            MemberExpression me;

            if (selector.Body is MemberExpression expression)
            {
                me = expression;
            }
            else
            {
                var op = ((UnaryExpression)selector.Body).Operand;
                me = (MemberExpression)op;
            }

            return ((PropertyInfo)me.Member).Name;
        }

        public static Type GetPropertyType(this LambdaExpression selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            MemberExpression me;

            if (selector.Body is MemberExpression expression)
            {
                me = expression;
            }
            else
            {
                var op = ((UnaryExpression)selector.Body).Operand;
                me = (MemberExpression)op;
            }

            return ((PropertyInfo)me.Member).PropertyType;
        }

        public static BinaryExpression CreateKeyComparisonExpression(this Expression leftExpression, Expression rightExpression)
        {
            ArgumentNullException.ThrowIfNull(leftExpression);
            ArgumentNullException.ThrowIfNull(rightExpression);

            if (leftExpression.Type == rightExpression.Type)
            {
                return Expression.Equal(leftExpression, rightExpression);
            }

            if (leftExpression.Type.IsNullableType())
            {
                rightExpression = Expression.Convert(rightExpression, leftExpression.Type);
            }
            else
            {
                leftExpression = Expression.Convert(leftExpression, rightExpression.Type);
            }

            return Expression.Equal(leftExpression, rightExpression);
        }

        public static ParameterExpression GetParameterExpression(this LambdaExpression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var memberExp = expression.Body;
            while (true)
            {
                if (memberExp is MemberExpression exp)
                {
                    switch (exp.Expression)
                    {
                        case ParameterExpression exp2:
                            return exp2;
                        case MemberExpression exp3:
                            memberExp = exp3;
                            continue;
                        default:
                            throw new NotSupportedException();
                    }
                }

                if (memberExp is UnaryExpression exp4)
                {
                    switch (exp4.Operand)
                    {
                        case ParameterExpression exp5:
                            return exp5;
                        case MemberExpression exp6:
                            memberExp = exp6;
                            continue;
                        default:
                            throw new NotSupportedException();
                    }
                }

                break;
            }

            throw new InvalidOperationException("No parameter expression found in provided expression");
        }

        public static Expression<Func<T, bool>> GetFilterExpression<T, TK>(this Expression<Func<T, TK>> selector, TK value)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var exp = selector.Body.CreateKeyComparisonExpression(Expression.Constant(value));
            var lambda = (Expression<Func<T, bool>>)Expression.Lambda(exp, false, selector.GetParameterExpression());
            return lambda;
        }
    }
}
