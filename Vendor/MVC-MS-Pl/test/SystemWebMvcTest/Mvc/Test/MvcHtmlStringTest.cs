namespace System.Web.Mvc.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MvcHtmlStringTest {

        [TestMethod]
        public void ToHtmlStringReturnsOriginalString() {
            // Arrange
            MvcHtmlString htmlString = MvcHtmlString.Create("some value");

            // Act
            string retVal = htmlString.ToHtmlString();

            // Assert
            Assert.AreEqual("some value", retVal);
        }

        [TestMethod]
        public void ToStringReturnsOriginalString() {
            // Arrange
            MvcHtmlString htmlString = MvcHtmlString.Create("some value");

            // Act
            string retVal = htmlString.ToString();

            // Assert
            Assert.AreEqual("some value", retVal);
        }

        [TestMethod]
        public void ToStringReturnsEmptyStringIfOriginalStringWasNull() {
            // Arrange
            MvcHtmlString htmlString = MvcHtmlString.Create(null);

            // Act
            string retVal = htmlString.ToString();

            // Assert
            Assert.AreEqual("", retVal);
        }

    }
}
