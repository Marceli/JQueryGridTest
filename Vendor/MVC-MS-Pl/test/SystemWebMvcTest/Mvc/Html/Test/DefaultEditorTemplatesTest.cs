namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Data.Linq;
    using System.IO;
    using System.Web.UI.WebControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class DefaultEditorTemplatesTest {

        // BooleanTemplate

        [TestMethod]
        public void BooleanTemplateTests() {
            // Boolean values

            Assert.AreEqual(
                @"<input checked=""checked"" class=""check-box"" id=""FieldPrefix"" name=""FieldPrefix"" type=""checkbox"" value=""true"" /><input name=""FieldPrefix"" type=""hidden"" value=""false"" />",
                DefaultEditorTemplates.BooleanTemplate(MakeHtmlHelper<bool>(true)));

            Assert.AreEqual(
                @"<input class=""check-box"" id=""FieldPrefix"" name=""FieldPrefix"" type=""checkbox"" value=""true"" /><input name=""FieldPrefix"" type=""hidden"" value=""false"" />",
                DefaultEditorTemplates.BooleanTemplate(MakeHtmlHelper<bool>(false)));

            Assert.AreEqual(
                @"<input class=""check-box"" id=""FieldPrefix"" name=""FieldPrefix"" type=""checkbox"" value=""true"" /><input name=""FieldPrefix"" type=""hidden"" value=""false"" />",
                DefaultEditorTemplates.BooleanTemplate(MakeHtmlHelper<bool>(null)));

            // Nullable<Boolean> values

            Assert.AreEqual(
                @"<select class=""list-box tri-state"" id=""FieldPrefix"" name=""FieldPrefix""><option value="""">Not Set</option>
<option selected=""selected"" value=""true"">True</option>
<option value=""false"">False</option>
</select>",
                DefaultEditorTemplates.BooleanTemplate(MakeHtmlHelper<Nullable<bool>>(true)));

            Assert.AreEqual(
                @"<select class=""list-box tri-state"" id=""FieldPrefix"" name=""FieldPrefix""><option value="""">Not Set</option>
<option value=""true"">True</option>
<option selected=""selected"" value=""false"">False</option>
</select>",
                DefaultEditorTemplates.BooleanTemplate(MakeHtmlHelper<Nullable<bool>>(false)));

            Assert.AreEqual(
                @"<select class=""list-box tri-state"" id=""FieldPrefix"" name=""FieldPrefix""><option selected=""selected"" value="""">Not Set</option>
<option value=""true"">True</option>
<option value=""false"">False</option>
</select>",
                DefaultEditorTemplates.BooleanTemplate(MakeHtmlHelper<Nullable<bool>>(null)));
        }

        // HiddenInputTemplate

        [TestMethod]
        public void HiddenInputTemplateTests() {
            Assert.AreEqual(
                @"Hidden Value<input id=""FieldPrefix"" name=""FieldPrefix"" type=""hidden"" value=""Hidden Value"" />",
                DefaultEditorTemplates.HiddenInputTemplate(MakeHtmlHelper<string>("Hidden Value")));

            Assert.AreEqual(
                @"&lt;script&gt;alert('XSS!')&lt;/script&gt;<input id=""FieldPrefix"" name=""FieldPrefix"" type=""hidden"" value=""&lt;script>alert('XSS!')&lt;/script>"" />",
                DefaultEditorTemplates.HiddenInputTemplate(MakeHtmlHelper<string>("<script>alert('XSS!')</script>")));

            var helperWithInvisibleChrome = MakeHtmlHelper<string>("<script>alert('XSS!')</script>", "<b>Encode me!</b>");
            helperWithInvisibleChrome.ViewData.ModelMetadata.HideSurroundingChrome = true;
            Assert.AreEqual(
                @"<input id=""FieldPrefix"" name=""FieldPrefix"" type=""hidden"" value=""&lt;script>alert('XSS!')&lt;/script>"" />",
                DefaultEditorTemplates.HiddenInputTemplate(helperWithInvisibleChrome));

            byte[] byteValues = { 1, 2, 3, 4, 5 };

            Assert.AreEqual(
                @"&quot;AQIDBAU=&quot;<input id=""FieldPrefix"" name=""FieldPrefix"" type=""hidden"" value=""AQIDBAU="" />",
                DefaultEditorTemplates.HiddenInputTemplate(MakeHtmlHelper<Binary>(new Binary(byteValues))));

            Assert.AreEqual(
                @"System.Byte[]<input id=""FieldPrefix"" name=""FieldPrefix"" type=""hidden"" value=""AQIDBAU="" />",
                DefaultEditorTemplates.HiddenInputTemplate(MakeHtmlHelper<byte[]>(byteValues)));
        }

        // MultilineText

        [TestMethod]
        public void MultilineTextTemplateTests() {
            Assert.AreEqual(
                @"<textarea class=""text-box multi-line"" id=""FieldPrefix"" name=""FieldPrefix"">
Multiple
Line
Value!</textarea>",
                DefaultEditorTemplates.MultilineTextTemplate(MakeHtmlHelper<string>("", @"Multiple
Line
Value!")));

            Assert.AreEqual(
                @"<textarea class=""text-box multi-line"" id=""FieldPrefix"" name=""FieldPrefix"">
&lt;script&gt;alert('XSS!')&lt;/script&gt;</textarea>",
                DefaultEditorTemplates.MultilineTextTemplate(MakeHtmlHelper<string>("", "<script>alert('XSS!')</script>")));
        }

        // ObjectTemplate

        private static string SpyCallback(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string templateName, DataBoundControlMode mode) {
            return String.Format("Model = {0}, ModelType = {1}, PropertyName = {2}, HtmlFieldName = {3}, TemplateName = {4}, Mode = {5}",
                                 metadata.Model ?? "(null)",
                                 metadata.ModelType == null ? "(null)" : metadata.ModelType.FullName,
                                 metadata.PropertyName ?? "(null)",
                                 htmlFieldName == String.Empty ? "(empty)" : htmlFieldName ?? "(null)",
                                 templateName ?? "(null)",
                                 mode);
        }

        class ObjectTemplateModel {
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        [TestMethod]
        public void ObjectTemplateEditsAllPropertiesOnObjectByDefault() {
            string expected = @"<div class=""editor-label""><label for=""FieldPrefix_Property1"">Property1</label></div>
<div class=""editor-field"">Model = p1, ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = Edit </div>
<div class=""editor-label""><label for=""FieldPrefix_Property2"">Property2</label></div>
<div class=""editor-field"">Model = (null), ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = Edit </div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel { Property1 = "p1", Property2 = null };
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateWithModelError() {
            string expected = @"<div class=""editor-label""><label for=""FieldPrefix_Property1"">Property1</label></div>
<div class=""editor-field"">Model = p1, ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = Edit <span class=""field-validation-error"">*</span></div>
<div class=""editor-label""><label for=""FieldPrefix_Property2"">Property2</label></div>
<div class=""editor-field"">Model = (null), ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = Edit </div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel { Property1 = "p1", Property2 = null };
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            html.ViewData.ModelState.AddModelError("FieldPrefix.Property1", "Error Message");

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateWithDisplayNameMetadata() {
            string expected = @"<div class=""editor-field"">Model = (null), ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = Edit </div>
<div class=""editor-label""><label for=""FieldPrefix_Property2"">Custom display name</label></div>
<div class=""editor-field"">Model = (null), ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = Edit </div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property1") { DisplayName = String.Empty };
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property2") { DisplayName = "Custom display name" };
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateWithShowForEditorMetadata() {
            string expected = @"<div class=""editor-label""><label for=""FieldPrefix_Property1"">Property1</label></div>
<div class=""editor-field"">Model = (null), ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = Edit </div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property1") { ShowForEdit = true };
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property2") { ShowForEdit = false };
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplatePreventsRecursionOnModelValue() {
            string expected = @"<div class=""editor-label""><label for=""FieldPrefix_Property2"">Property2</label></div>
<div class=""editor-field"">Model = propValue2, ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = Edit </div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue1", typeof(string), "Property1");
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue2", typeof(string), "Property2");
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });
            html.ViewData.TemplateInfo.VisitedObjects.Add("propValue1");

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplatePreventsRecursionOnModelTypeForNullModelValues() {
            string expected = @"<div class=""editor-label""><label for=""FieldPrefix_Property2"">Property2</label></div>
<div class=""editor-field"">Model = propValue2, ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = Edit </div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property1");
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue2", typeof(string), "Property2");
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });
            html.ViewData.TemplateInfo.VisitedObjects.Add(typeof(string));

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateDisplaysNullDisplayTextWithNullModelAndTemplateDepthGreaterThanOne() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(null);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(ObjectTemplateModel));
            metadata.NullDisplayText = "Null Display Text";
            metadata.SimpleDisplayText = "Simple Display Text";
            html.ViewData.ModelMetadata = metadata;
            html.ViewData.TemplateInfo.VisitedObjects.Add("foo");
            html.ViewData.TemplateInfo.VisitedObjects.Add("bar");

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(metadata.NullDisplayText, result);
        }

        [TestMethod]
        public void ObjectTemplateDisplaysSimpleDisplayTextWithNonNullModelTemplateDepthGreaterThanOne() {
            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(ObjectTemplateModel));
            html.ViewData.ModelMetadata = metadata;
            metadata.NullDisplayText = "Null Display Text";
            metadata.SimpleDisplayText = "Simple Display Text";
            html.ViewData.TemplateInfo.VisitedObjects.Add("foo");
            html.ViewData.TemplateInfo.VisitedObjects.Add("bar");

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(metadata.SimpleDisplayText, result);
        }

        // PasswordTemplate

        [TestMethod]
        public void PasswordTemplateTests() {
            Assert.AreEqual(
                @"<input class=""text-box single-line password"" id=""FieldPrefix"" name=""FieldPrefix"" type=""password"" value=""Value"" />",
                DefaultEditorTemplates.PasswordTemplate(MakeHtmlHelper<string>("Value")));

            Assert.AreEqual(
                @"<input class=""text-box single-line password"" id=""FieldPrefix"" name=""FieldPrefix"" type=""password"" value=""&lt;script>alert('XSS!')&lt;/script>"" />",
                DefaultEditorTemplates.PasswordTemplate(MakeHtmlHelper<string>("<script>alert('XSS!')</script>")));
        }

        [TestMethod]
        public void ObjectTemplateWithHiddenChrome() {
            string expected = @"Model = propValue1, ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = Edit";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue1", typeof(string), "Property1") { HideSurroundingChrome = true };
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata });

            // Act
            string result = DefaultEditorTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        // StringTemplate

        [TestMethod]
        public void StringTemplateTests() {
            Assert.AreEqual(
                @"<input class=""text-box single-line"" id=""FieldPrefix"" name=""FieldPrefix"" type=""text"" value=""Value"" />",
                DefaultEditorTemplates.StringTemplate(MakeHtmlHelper<string>("Value")));

            Assert.AreEqual(
                @"<input class=""text-box single-line"" id=""FieldPrefix"" name=""FieldPrefix"" type=""text"" value=""&lt;script>alert('XSS!')&lt;/script>"" />",
                DefaultEditorTemplates.StringTemplate(MakeHtmlHelper<string>("<script>alert('XSS!')</script>")));
        }

        // Helpers

        private HtmlHelper MakeHtmlHelper<TModel>(object model) {
            return MakeHtmlHelper<TModel>(model, model);
        }

        private HtmlHelper MakeHtmlHelper<TModel>(object model, object formattedModelValue) {
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            viewData.TemplateInfo.HtmlFieldPrefix = "FieldPrefix";
            viewData.TemplateInfo.FormattedModelValue = formattedModelValue;
            viewData.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => model, typeof(TModel));

            ViewContext viewContext = new ViewContext(new ControllerContext(), new DummyView(), viewData, new TempDataDictionary());

            return new HtmlHelper(viewContext, new SimpleViewDataContainer(viewData));
        }

        private class DummyView : IView {
            public void Render(ViewContext viewContext, TextWriter writer) {
                throw new NotImplementedException();
            }
        }
    }
}
