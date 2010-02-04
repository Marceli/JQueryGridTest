namespace System.Web.TestUtil {
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Moq;
    using Moq.Language.Flow;

    public static class MockHelpers {
        // Would be nice to get rid of TResult so everything can be implicitly generic
        public static IExpect<TResult> ExpectGetItem<T, TIndex, TResult>(this Mock<T> mock, TIndex index) where T : class {
            MethodInfo getMethod = GetIndexGetter(typeof(T), typeof(TIndex), typeof(TResult));
            var objExpr = Expression.Parameter(typeof(T), "o");
            var methodCallExpr = Expression.Call(objExpr, getMethod, Expression.Constant(index, typeof(TIndex)));
            var lambdaExpr = Expression.Lambda<Func<T, TResult>>(methodCallExpr, objExpr);
            return mock.Expect<TResult>(lambdaExpr);
        }

        public static IExpect ExpectSetItem<T, TIndex, TProperty>(this Mock<T> mock, TIndex index, TProperty value) where T : class {
            // get property info, gen an expression to the setter
            MethodInfo setMethod = GetIndexSetter(typeof(T), typeof(TIndex));
            var objExpr = Expression.Parameter(typeof(T), "o");
            var methodCallExpr = Expression.Call(objExpr, setMethod, Expression.Constant(index, typeof(TIndex)), Expression.Constant(value, typeof(TProperty)));
            var lambdaExpr = Expression.Lambda<Action<T>>(methodCallExpr, objExpr);
            return mock.Expect(lambdaExpr);
        }

        public static IExpect ExpectSetItem<T, TInner, TIndex, TProperty>(this Mock<T> mock, Expression<Func<T, TInner>> property, TIndex index, TProperty value) where T : class {
            // get property info, gen an expression to the setter
            MethodInfo setMethod = GetIndexSetter(typeof(T), typeof(TIndex));
            var propertyExpr = property.Body;
            var methodCallExpr = Expression.Call(propertyExpr, setMethod, Expression.Constant(index, typeof(TIndex)), Expression.Constant(value, typeof(TProperty)));
            var lambdaExpr = Expression.Lambda<Action<T>>(methodCallExpr, property.Parameters);
            return mock.Expect(lambdaExpr);
        }

        // this is only required until Moq can support property setters
        public static IExpect ExpectSetProperty<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> property, TResult value) where T : class {
            // get the property info
            var oldLambdaExpr = property as LambdaExpression;
            var memberExpr = oldLambdaExpr.Body as MemberExpression;
            var propInfo = memberExpr.Member as PropertyInfo;

            // now gen a call to the setter
            var setter = propInfo.GetSetMethod();
            var paramExpr = Expression.Parameter(typeof(T), null);
            var newCallExpr = Expression.Call(paramExpr, setter, Expression.Constant(value, typeof(TResult)));
            var newLambdaExpr = Expression.Lambda<Action<T>>(newCallExpr, paramExpr);
            return mock.Expect(newLambdaExpr);
        }

        private static MethodInfo GetIndexGetter(Type type, Type indexParameterType, Type returnType) {
            PropertyInfo itemProperty = type.GetProperty("Item", new Type[] { indexParameterType });
            if (itemProperty == null) {
                throw new ArgumentException("Could not find indexer: " + type.FullName + "[" + indexParameterType.FullName + "]", "index");
            }

            MethodInfo getMethod = itemProperty.GetGetMethod();
            if (getMethod == null) {
                throw new ArgumentException("No public getter for indexer: " + type.FullName + "[" + indexParameterType.FullName + "]", "index");
            }
            if (getMethod.ReturnType != returnType) {
                throw new ArgumentException("Public getter for indexer " + type.FullName + "[" + indexParameterType.FullName + "] returns wrong type; expected " + returnType.FullName + " but was " + getMethod.ReturnType.FullName);
            }

            return getMethod;
        }

        private static MethodInfo GetIndexSetter(Type type, Type indexParameterType) {
            PropertyInfo itemProperty = type.GetProperty("Item", new Type[] { indexParameterType });
            if (itemProperty == null) {
                throw new ArgumentException("Could not find indexer: " + type.FullName + "[" + indexParameterType.FullName + "]", "index");
            }

            MethodInfo setMethod = itemProperty.GetSetMethod();
            if (setMethod == null) {
                throw new ArgumentException("No public setter for indexer: " + type.FullName + "[" + indexParameterType.FullName + "]", "index");
            }

            return setMethod;
        }

        public static StringBuilder SwitchResponseMockOutputToStringBuilder(this HttpContextBase httpContext) {
            return httpContext.Response.SwitchResponseMockOutputToStringBuilder();
        }

        public static StringBuilder SwitchResponseMockOutputToStringBuilder(this HttpResponseBase response) {
            return Mock.Get(response).SwitchResponseMockOutputToStringBuilder();
        }

        public static StringBuilder SwitchResponseMockOutputToStringBuilder(this IMock<HttpResponseBase> responseMock) {
            var sb = new StringBuilder();
            responseMock.Expect(response => response.Write(It.IsAny<string>())).Callback<string>(output => sb.Append(output));
            return sb;
        }

    }

    // helper class for making sure that we're performing culture-invariant string conversions
    public class CultureReflector : IFormattable {
        string IFormattable.ToString(string format, IFormatProvider formatProvider) {
            CultureInfo cInfo = (CultureInfo)formatProvider;
            return cInfo.ThreeLetterISOLanguageName;
        }
    }
}
