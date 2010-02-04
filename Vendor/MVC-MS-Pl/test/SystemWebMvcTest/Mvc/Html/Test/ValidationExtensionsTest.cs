namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Linq;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValidationExtensionsTest {

        [TestMethod]
        public void ValidationMessageAllowsEmptyModelName() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.ModelState.AddModelError("", "some error text");
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(vdd);

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">some error text</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsFirstError() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("foo");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsGenericMessageInsteadOfExceptionText() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("quux");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">The value 'quuxValue' is invalid.</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsNullForInvalidName() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("boo");

            // Assert
            Assert.IsNull(html, "html should be null if name is invalid.");
        }

        [TestMethod]
        public void ValidationMessageReturnsWithObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", new { bar = "bar" });

            // Assert
            Assert.AreEqual(@"<span bar=""bar"" class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsWithCustomClassOverridesDefault() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", new { @class = "my-custom-css-class" });

            // Assert
            Assert.AreEqual(@"<span class=""my-custom-css-class"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsWithCustomMessage() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", "bar error");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">bar error</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsWithCustomMessageAndObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", "bar error", new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<span baz=""baz"" class=""field-validation-error"">bar error</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageThrowsIfModelNameIsNull() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    htmlHelper.ValidationMessage(null);
                }, "modelName");
        }

        [TestMethod]
        public void ValidationMessageWithClientValidation() {
            var originalProvider = ModelValidatorProviders.Current;

            try {
                // Arrange
                HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());
                FormContext formContext = new FormContext() {
                    ClientValidationEnabled = true,
                    FormId = "form_id"
                };
                htmlHelper.ViewContext.FormContext = formContext;

                ModelClientValidationRule[] expectedValidationRules = new ModelClientValidationRule[] {
                    new ModelClientValidationRule() { ValidationType = "ValidationRule1" },
                    new ModelClientValidationRule() { ValidationType = "ValidationRule2" }
                };

                Mock<ModelValidator> mockValidator = new Mock<ModelValidator>(ModelMetadata.FromStringExpression("", htmlHelper.ViewContext.ViewData), htmlHelper.ViewContext);
                mockValidator.Expect(v => v.GetClientValidationRules())
                             .Returns(expectedValidationRules);
                Mock<ModelValidatorProvider> mockValidatorProvider = new Mock<ModelValidatorProvider>();
                mockValidatorProvider.Expect(vp => vp.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                                     .Returns(new[] { mockValidator.Object });
                ModelValidatorProviders.Current = mockValidatorProvider.Object;

                // Act
                MvcHtmlString html = htmlHelper.ValidationMessage("baz");

                // Assert
                Assert.AreEqual(@"<span class=""field-validation-error"" id=""form_id_baz_validator""></span>", html.ToHtmlString(),
                    "ValidationMessage() should always return something if client validation is enabled.");
                Assert.IsNotNull(formContext.GetValidationMetadataForField("baz"));
                Assert.AreEqual("form_id_baz_validator", formContext.FieldValidators["baz"].ValidatorId);
                CollectionAssert.AreEqual(expectedValidationRules, formContext.FieldValidators["baz"].ValidationRules.ToArray());
            }
            finally {
                ModelValidatorProviders.Current = originalProvider;
            }
        }

        [TestMethod]
        public void ValidationMessageWithModelStateAndNoErrors() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("baz");

            // Assert
            Assert.IsNull(html, "html should be null if there are no errors");
        }

        [TestMethod]
        public void ValidationSummary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary();

            // Assert
            Assert.AreEqual(@"<ul class=""validation-summary-errors""><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithDictionary() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes["class"] = "my-class";

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(null /* message */, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<ul class=""my-class""><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithDictionaryAndMessage() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes["class"] = "my-class";

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary("This is my message.", htmlAttributes);

            // Assert
            Assert.AreEqual(@"<span class=""my-class"">This is my message.</span>
<ul class=""my-class""><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithNoErrorsReturnsNull() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary();

            // Assert
            Assert.IsNull(html, "html should be null if there are no errors to report.");
        }

        [TestMethod]
        public void ValidationSummaryWithObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(null /* message */, new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<ul baz=""baz"" class=""validation-summary-errors""><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithObjectAttributesAndMessage() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary("This is my message.", new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<span baz=""baz"" class=""validation-summary-errors"">This is my message.</span>
<ul baz=""baz"" class=""validation-summary-errors""><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageWithPrefix() {
            // Arrange
            HtmlHelper htmlHelper = HtmlHelperTest.GetHtmlHelper(GetViewDataWithModelErrors("MyPrefix"));

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("foo");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        private static ViewDataDictionary GetViewDataWithModelErrors() {
            ViewDataDictionary viewData = new ViewDataDictionary();
            ModelState modelStateFoo = new ModelState();
            ModelState modelStateBar = new ModelState();
            ModelState modelStateBaz = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error <1>"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            modelStateBar.Errors.Add(new ModelError("bar error <1>"));
            modelStateBar.Errors.Add(new ModelError("bar error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            viewData.ModelState["bar"] = modelStateBar;
            viewData.ModelState["baz"] = modelStateBaz;
            viewData.ModelState.SetModelValue("quux", new ValueProviderResult(null, "quuxValue", null));
            viewData.ModelState.AddModelError("quux", new InvalidOperationException("Some error text."));
            return viewData;
        }

        private static ViewDataDictionary GetViewDataWithModelErrors(string prefix) {
            ViewDataDictionary viewData = new ViewDataDictionary();
            viewData.TemplateInfo.HtmlFieldPrefix = prefix;
            ModelState modelStateFoo = new ModelState();
            ModelState modelStateBar = new ModelState();
            ModelState modelStateBaz = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error <1>"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            modelStateBar.Errors.Add(new ModelError("bar error <1>"));
            modelStateBar.Errors.Add(new ModelError("bar error 2"));
            viewData.ModelState[prefix + ".foo"] = modelStateFoo;
            viewData.ModelState[prefix + ".bar"] = modelStateBar;
            viewData.ModelState[prefix + ".baz"] = modelStateBaz;
            viewData.ModelState.SetModelValue(prefix + ".quux", new ValueProviderResult(null, "quuxValue", null));
            viewData.ModelState.AddModelError(prefix + ".quux", new InvalidOperationException("Some error text."));
            return viewData;
        }

    }
}
