namespace System.Web.Mvc.Test {
    using System;
    using System.Data.Linq;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelBindersTest {

        [TestMethod]
        public void BindersPropertyIsNotNull() {
            // Arrange & Act
            ModelBinderDictionary binders = ModelBinders.Binders;

            // Assert
            Assert.IsNotNull(binders, "Binders should not be null");
        }

        [TestMethod]
        public void DefaultModelBinders() {
            // Act
            ModelBinderDictionary binders = ModelBinders.Binders;

            // Assert
            Assert.AreEqual(3, binders.Count);
            Assert.IsTrue(binders.ContainsKey(typeof(byte[])), "Did not contain entry for byte[].");
            Assert.IsInstanceOfType(binders[typeof(byte[])], typeof(ByteArrayModelBinder), "Did not contain correct binder for byte[].");
            Assert.IsTrue(binders.ContainsKey(typeof(HttpPostedFileBase)), "Did not contain entry for HttpPostedFileBase.");
            Assert.IsInstanceOfType(binders[typeof(HttpPostedFileBase)], typeof(HttpPostedFileBaseModelBinder), "Did not contain correct binder for HttpPostedFileBase.");
            Assert.IsTrue(binders.ContainsKey(typeof(Binary)), "Did not contain entry for Binary.");
            Assert.IsInstanceOfType(binders[typeof(Binary)], typeof(LinqBinaryModelBinder), "Did not contain correct binder for Binary.");
        }
    }
}
