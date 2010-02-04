namespace System.Web.Mvc.Test {
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ViewContextTest {

        [TestMethod]
        public void ConstructorCopiesFormContextReferenceFromOriginalViewContext() {
            // Arrange
            FormContext formContext = new FormContext();
            ViewContext originalViewContext = new ViewContext() {
                FormContext = formContext
            };

            // Act
            ViewContext newViewContext = new ViewContext(originalViewContext, new Mock<IView>().Object, new ViewDataDictionary(), new TempDataDictionary());

            // Assert
            Assert.AreEqual(formContext, newViewContext.FormContext, "FormContext should have been propagated between ViewContexts.");
        }

        [TestMethod]
        public void ConstructorThrowsIfTempDataIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData);
                }, "tempData");
        }

        [TestMethod]
        public void ConstructorThrowsIfViewDataIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = null;
            TempDataDictionary tempData = new TempDataDictionary();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData);
                }, "viewData");
        }

        [TestMethod]
        public void ConstructorThrowsIfViewIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = null;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = new TempDataDictionary();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData);
                }, "view");
        }

        [TestMethod]
        public void PropertiesAreSet() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = new TempDataDictionary();

            // Act
            ViewContext viewContext = new ViewContext(controllerContext, view, viewData, tempData);

            // Assert
            Assert.AreEqual(view, viewContext.View);
            Assert.AreEqual(viewData, viewContext.ViewData);
            Assert.AreEqual(tempData, viewContext.TempData);
            Assert.IsNull(viewContext.FormContext, "FormContext shouldn't be set unless Html.BeginForm() has been called.");
        }

    }
}
