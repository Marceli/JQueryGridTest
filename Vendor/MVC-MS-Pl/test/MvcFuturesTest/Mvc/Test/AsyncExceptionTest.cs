namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AsyncExceptionTest {

        [TestMethod]
        public void ConstructorWithMessageAndInnerExceptionParameter() {
            // Arrange
            Exception innerException = new Exception();

            // Act
            AsyncException ex = new AsyncException("the message", innerException);

            // Assert
            Assert.AreEqual("the message", ex.Message);
            Assert.AreEqual(innerException, ex.InnerException);
        }

        [TestMethod]
        public void ConstructorWithMessageParameter() {
            // Act
            AsyncException ex = new AsyncException("the message");

            // Assert
            Assert.AreEqual("the message", ex.Message);
        }

        [TestMethod]
        public void ConstructorWithoutParameters() {
            // Act & assert
            ExceptionHelper.ExpectException<AsyncException>(
                delegate {
                    throw new AsyncException();
                });
        }

        [TestMethod]
        public void TypeIsSerializable() {
            // Arrange
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            AsyncException ex = new AsyncException("the message", new Exception("inner exception"));

            // Act
            formatter.Serialize(ms, ex);
            ms.Position = 0;
            AsyncException deserialized = formatter.Deserialize(ms) as AsyncException;

            // Assert
            Assert.IsNotNull(deserialized, "Deserialization process did not return the exception.");
            Assert.AreEqual("the message", deserialized.Message);
            Assert.IsNotNull(deserialized.InnerException);
            Assert.AreEqual("inner exception", deserialized.InnerException.Message);
        }

    }
}
