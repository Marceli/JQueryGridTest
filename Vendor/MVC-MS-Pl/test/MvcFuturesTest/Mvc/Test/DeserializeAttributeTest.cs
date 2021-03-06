﻿namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class DeserializeAttributeTest {

        [TestMethod]
        public void BinderReturnsDeserializedValue() {
            // Arrange
            DeserializeAttribute attr = new DeserializeAttribute();
            IModelBinder binder = attr.GetBinder();
            ModelBindingContext mbContext = new ModelBindingContext() {
                ModelName = "someKey",
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "someKey", new ValueProviderResult("/wECKg==", "/wECKg==", CultureInfo.InvariantCulture) }
                }
            };

            // Act
            object retVal = binder.BindModel(null, mbContext);

            // Assert
            Assert.AreEqual(42, retVal, "Object was not properly deserialized.");
        }

        [TestMethod]
        public void BinderReturnsNullIfValueProviderDoesNotContainKey() {
            // Arrange
            DeserializeAttribute attr = new DeserializeAttribute();
            IModelBinder binder = attr.GetBinder();
            ModelBindingContext mbContext = new ModelBindingContext() {
                ModelName = "someKey",
                ValueProvider = new Dictionary<string, ValueProviderResult>()
            };

            // Act
            object retVal = binder.BindModel(null, mbContext);

            // Assert
            Assert.IsNull(retVal, "Binder should return null if no data was present.");
        }

        [TestMethod]
        public void BinderThrowsIfBindingContextIsNull() {
            // Arrange
            DeserializeAttribute attr = new DeserializeAttribute();
            IModelBinder binder = attr.GetBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BinderThrowsIfDataCorrupt() {
            // Arrange
            DeserializeAttribute attr = new DeserializeAttribute();
            IModelBinder binder = attr.GetBinder();
            ModelBindingContext mbContext = new ModelBindingContext() {
                ModelName = "someKey",
                ValueProvider = new Dictionary<string, ValueProviderResult>() {
                    { "someKey", new ValueProviderResult("This data is corrupted.", "This data is corrupted.", CultureInfo.InvariantCulture) }
                }
            };

            // Act & assert
            Exception exception = ExceptionHelper.ExpectException<SerializationException>(
                delegate {
                    binder.BindModel(null, mbContext);
                },
                @"Deserialization failed. Verify that the data is being deserialized using the same SerializationMode with which it was serialized. Otherwise see the inner exception.");

            Assert.IsNotNull(exception.InnerException, "Inner exception was not propagated correctly.");
        }

        [TestMethod]
        public void ModeDefaultsToPlaintext() {
            // Arrange
            DeserializeAttribute attr = new DeserializeAttribute();

            // Act
            SerializationMode defaultMode = attr.Mode;

            // Assert
            Assert.AreEqual(SerializationMode.Plaintext, defaultMode);
        }

    }
}
