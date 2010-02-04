namespace System.Web.Mvc.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelValidatorProvidersTest {

        [TestMethod]
        public void DefaultModelValidatorProviderIsDataAnnotations() {
            // Act
            ModelValidatorProvider provider = ModelValidatorProviders.Current;

            // Assert
            Assert.AreEqual(typeof(DataAnnotationsModelValidatorProvider), provider.GetType());
        }

        [TestMethod]
        public void SettingNullModelValidatorProviderUsesEmptyModelValidatorProvider() {
            ModelValidatorProvider original = ModelValidatorProviders.Current;

            try {
                // Act
                ModelValidatorProviders.Current = null;

                // Assert
                Assert.AreEqual(typeof(EmptyModelValidatorProvider), ModelValidatorProviders.Current.GetType());
            }
            finally {
                ModelValidatorProviders.Current = original;
            }
        }

    }
}
