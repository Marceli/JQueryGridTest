namespace Microsoft.Web.Mvc.Test {
    using System.Data.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class BinaryHtmlExtensionsTest {
        internal static readonly byte[] Base64TestBytes = new byte[] { 23, 43, 53 };

        [TestMethod]
        public void HiddenForWithByteArrayValueRendersBase64EncodedValue() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Gallery { Image = Base64TestBytes });
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            mockViewContext.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Gallery> htmlHelper = new HtmlHelper<Gallery>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            MvcHtmlString result = htmlHelper.HiddenFor(g => g.Image);

            // Assert
            Assert.AreEqual("<input id=\"Image\" name=\"Image\" type=\"hidden\" value=\"Fys1\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithBinaryArrayValueRendersBase64EncodedValue() {
            // Arrange
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            Mock<IViewDataContainer> mockIViewDataContainer = new Mock<IViewDataContainer>();
            ViewDataDictionary viewData = new ViewDataDictionary(new Gallery { TimeStamp = new Binary(Base64TestBytes) });
            mockIViewDataContainer.Expect(c => c.ViewData).Returns(viewData);
            mockViewContext.Expect(c => c.ViewData).Returns(viewData);
            HtmlHelper<Gallery> htmlHelper = new HtmlHelper<Gallery>(mockViewContext.Object, mockIViewDataContainer.Object);

            // Act
            MvcHtmlString result = htmlHelper.HiddenFor(g => g.TimeStamp);

            // Assert
            Assert.AreEqual("<input id=\"TimeStamp\" name=\"TimeStamp\" type=\"hidden\" value=\"Fys1\" />", result.ToHtmlString());
        }

        private class Gallery {
            public byte[] Image {
                get;
                set;
            }

            public Binary TimeStamp {
                get;
                set;
            }
        }
    }
}
