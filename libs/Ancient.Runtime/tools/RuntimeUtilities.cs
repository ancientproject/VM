namespace ancient.runtime.tools
{
    using System;
    using System.Linq.Expressions;

    public static class RuntimeUtilities
    {
        /// <summary>
        /// Returns a string path from an expression. This is mostly used to retrieve serialized
        /// properties without hardcoding the field path as a string and thus allowing proper
        /// refactoring features.
        /// </summary>
        /// <typeparam name="TType">The class type where the member is defined</typeparam>
        /// <typeparam name="TValue">The member type</typeparam>
        /// <param name="expr">An expression path fo the member</param>
        /// <returns>A string representation of the expression path</returns>
        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr) => GetFieldPath(expr.Body);
        public static string GetFieldPath(Expression body)
        {
            switch (body)
            {
                case UnaryExpression unary:
                    return GetFieldPath(unary.Operand);
                case MemberExpression me:
                    return me.Member.Name;
                case BinaryExpression be:
                    return be.ToString();
                default:
                    return "<???>";
            }
        }
    }
}