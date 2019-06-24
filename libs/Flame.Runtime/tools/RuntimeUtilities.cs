namespace Flame.Runtime.tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text;

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
        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            var a = expr.Body.GetType();
            if (expr.Body is MemberExpression me)
                return me.Member.Name;
            if (expr.Body is BinaryExpression be)
                return be.ToString();
            return "<???>";
        }
    }
}