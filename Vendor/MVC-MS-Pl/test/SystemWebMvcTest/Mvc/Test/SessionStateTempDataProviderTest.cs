namespace System.Web.Mvc.Test {
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class SessionStateTempDataProviderTest {

        [TestMethod]
        public void Load_NullSession_ReturnsEmptyDictionary() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();

            // Act
            IDictionary<string, object> tempDataDictionary = testProvider.LoadTempData(GetControllerContext());

            // Assert
            Assert.AreEqual(0, tempDataDictionary.Count);
        }

        [TestMethod]
        public void Load_NonNullSession_NoSessionData_ReturnsEmptyDictionary() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            Mock<HttpSessionStateBase> mockSessionStateBase = new Mock<HttpSessionStateBase>();
            mockControllerContext.Expect(c => c.HttpContext.Session).Returns(mockSessionStateBase.Object);

            // Act
            var result = testProvider.LoadTempData(mockControllerContext.Object);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Load_NonNullSession_IncorrectSessionDataType_ReturnsEmptyDictionary() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            Mock<HttpSessionStateBase> mockSessionStateBase = new Mock<HttpSessionStateBase>();
            mockControllerContext.Expect(c => c.HttpContext.Session).Returns(mockSessionStateBase.Object);
            mockSessionStateBase.ExpectGetItem<HttpSessionStateBase, string, object>(SessionStateTempDataProvider.TempDataSessionStateKey).Returns(42);

            // Act
            var result = testProvider.LoadTempData(mockControllerContext.Object);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Load_NonNullSession_CorrectSessionDataType_ReturnsSessionData() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();
            Dictionary<string, object> tempData = new Dictionary<string, object> { { "foo", "bar" } };
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            Mock<HttpSessionStateBase> mockSessionStateBase = new Mock<HttpSessionStateBase>();
            mockControllerContext.Expect(c => c.HttpContext.Session).Returns(mockSessionStateBase.Object);
            mockSessionStateBase.ExpectGetItem<HttpSessionStateBase, string, object>(SessionStateTempDataProvider.TempDataSessionStateKey).Returns(tempData);

            // Act
            var result = testProvider.LoadTempData(mockControllerContext.Object);

            // Assert
            Assert.AreSame(tempData, result);
        }

        [TestMethod]
        public void Save_NullSession_NullDictionary_DoesNotThrow() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();

            // Act
            testProvider.SaveTempData(GetControllerContext(), null);
        }


        [TestMethod]
        public void Save_NullSession_EmptyDictionary_DoesNotThrow() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();

            // Act
            testProvider.SaveTempData(GetControllerContext(), new Dictionary<string, object>());
        }

        [TestMethod]
        public void Save_NullSession_NonEmptyDictionary_Throws() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    testProvider.SaveTempData(GetControllerContext(), new Dictionary<string, object> { { "foo", "bar" } });
                },
                "The SessionStateTempDataProvider class requires session state to be enabled.");
        }

        [TestMethod]
        public void Save_NonNullSession_AssignsTempDataDictionaryIntoSession() {
            // Arrange
            SessionStateTempDataProvider testProvider = new SessionStateTempDataProvider();
            Dictionary<string, object> tempData = new Dictionary<string, object> { { "foo", "bar" } };
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            Mock<HttpSessionStateBase> mockSessionStateBase = new Mock<HttpSessionStateBase>();
            mockControllerContext.Expect(c => c.HttpContext.Session).Returns(mockSessionStateBase.Object);
            mockSessionStateBase.ExpectSetItem(SessionStateTempDataProvider.TempDataSessionStateKey, tempData);

            // Act
            testProvider.SaveTempData(mockControllerContext.Object, tempData);

            // Assert
            mockSessionStateBase.VerifyAll();
        }

        private static ControllerContext GetControllerContext() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Session).Returns((HttpSessionStateBase)null);
            return mockControllerContext.Object;
        }
    }
}
