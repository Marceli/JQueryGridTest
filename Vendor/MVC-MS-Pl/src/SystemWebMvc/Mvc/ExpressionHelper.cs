namespace System.Web.Mvc {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public static class ExpressionHelper {
        public static string GetExpressionText(string expression) {
            return
                String.Equals(expression, "model", StringComparison.OrdinalIgnoreCase)
                    ? String.Empty    // If it's exactly "model", then give them an empty string, to replicate the lambda behavior
                    : expression;
        }

        public static string GetExpressionText(LambdaExpression expression) {
            // Crack the expression string for property/field accessors to create its name
            Stack<string> nameParts = new Stack<string>();
            Expression part = expression.Body;

            while (part != null) {
                if (part.NodeType == System.Linq.Expressions.ExpressionType.MemberAccess) {
                    MemberExpression memberExpressionPart = (MemberExpression)part;
                    nameParts.Push(memberExpressionPart.Member.Name);
                    part = memberExpressionPart.Expression;
                }
                else {
                    break;
                }
            }

            // If it starts with "model", then strip that away
            if (nameParts.Count > 0 && String.Equals(nameParts.Peek(), "model", StringComparison.OrdinalIgnoreCase)) {
                nameParts.Pop();
            }

            if (nameParts.Count > 0) {
                return nameParts.Aggregate((left, right) => left + "." + right);
            }

            return String.Empty;
        }
    }
}
