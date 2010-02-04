namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class ScriptExtensionsTest {
        [TestMethod]
        public void ScriptWithoutFileThrowsArgumentNullException() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(() => html.Script(null), "file");
        }

        [TestMethod]
        public void ScriptWithRootedPathRendersProperElement() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("~/Correct/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/$(SESSION)/Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithRelativePathRendersProperElement() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("../../Correct/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"../../Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithRelativeCurrentPathRendersProperElement() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("/Correct/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithScriptRelativePathRendersProperElement() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("Correct/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/$(SESSION)/Scripts/Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithUrlRendersProperElement() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("http://ajax.Correct.com/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"http://ajax.Correct.com/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithSecureUrlRendersProperElement() {
            // Arrange
            HtmlHelper html = TestHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("https://ajax.Correct.com/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"https://ajax.Correct.com/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

    }
}
