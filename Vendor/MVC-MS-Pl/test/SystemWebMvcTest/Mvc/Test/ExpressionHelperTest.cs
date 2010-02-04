namespace System.Web.Mvc.Test {
    using System;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionHelperTest {
        [TestMethod]
        public void StringBasedExpressionTests() {
            ViewDataDictionary vdd = new ViewDataDictionary();

            // Uses the given expression as the expression text
            Assert.AreEqual("?", ExpressionHelper.GetExpressionText("?"));

            // Exactly "Model" (case-insensitive) is turned into empty string
            Assert.AreEqual(String.Empty, ExpressionHelper.GetExpressionText("Model"));
            Assert.AreEqual(String.Empty, ExpressionHelper.GetExpressionText("mOdEl"));

            // Beginning with "Model" is untouched
            Assert.AreEqual("Model.Foo", ExpressionHelper.GetExpressionText("Model.Foo"));
        }

        [TestMethod]
        public void LambdaBasedExpressionTextTests() {
            // "Model" at the front of the expression is excluded (case insensitively)
            DummyContactModel Model = null;
            Assert.AreEqual(String.Empty, ExpressionHelper.GetExpressionText(Lambda<object, DummyContactModel>(m => Model)));
            Assert.AreEqual("FirstName", ExpressionHelper.GetExpressionText(Lambda<object, string>(m => Model.FirstName)));

            DummyContactModel mOdeL = null;
            Assert.AreEqual(String.Empty, ExpressionHelper.GetExpressionText(Lambda<object, DummyContactModel>(m => mOdeL)));
            Assert.AreEqual("FirstName", ExpressionHelper.GetExpressionText(Lambda<object, string>(m => mOdeL.FirstName)));

            // "Model" in the middle of the expression is not excluded
            DummyModelContainer container = null;
            Assert.AreEqual("container.Model", ExpressionHelper.GetExpressionText(Lambda<object, DummyContactModel>(m => container.Model)));
            Assert.AreEqual("container.Model.FirstName", ExpressionHelper.GetExpressionText(Lambda<object, string>(m => container.Model.FirstName)));

            // The parameter is excluded
            Assert.AreEqual(String.Empty, ExpressionHelper.GetExpressionText(Lambda<DummyContactModel, DummyContactModel>(m => m)));
            Assert.AreEqual("FirstName", ExpressionHelper.GetExpressionText(Lambda<DummyContactModel, string>(m => m.FirstName)));
        }

        // Helpers

        private LambdaExpression Lambda<T1, T2>(Expression<Func<T1, T2>> expression) {
            return expression;
        }

        class DummyContactModel {
            public string FirstName { get; set; }
        }

        class DummyModelContainer {
            public DummyContactModel Model { get; set; }
        }
    }
}
