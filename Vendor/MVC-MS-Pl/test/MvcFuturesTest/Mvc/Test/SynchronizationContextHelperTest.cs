namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Threading;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class SynchronizationHelperContextTest {

        [TestMethod]
        public void SyncWithAction() {
            // Arrange
            bool actionWasCalled = false;
            bool sendWasCalled = false;

            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(sc => sc.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        sendWasCalled = true;
                        d(state);
                    });

            // Act
            SynchronizationContextHelper.Sync(mockSyncContext.Object, () => { actionWasCalled = true; });

            // Assert
            Assert.IsTrue(actionWasCalled);
            Assert.IsTrue(sendWasCalled);
        }

        [TestMethod]
        public void SyncWithActionCapturesException() {
            // Arrange
            InvalidOperationException exception = new InvalidOperationException("Some exception text.");

            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(sc => sc.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        try {
                            d(state);
                        }
                        catch {
                            // swallow exceptions, just like AspNetSynchronizationContext
                        }
                    });

            // Act & assert
            AsyncException thrownException = ExceptionHelper.ExpectException<AsyncException>(
                delegate {
                    SynchronizationContextHelper.Sync(mockSyncContext.Object, () => { throw exception; });
                },
                @"An operation that crossed a synchronization context failed. See the inner exception for more information.");

            Assert.AreEqual(exception, thrownException.InnerException);
        }

        [TestMethod]
        public void SyncWithFunc() {
            // Arrange
            bool sendWasCalled = false;

            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(sc => sc.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        sendWasCalled = true;
                        d(state);
                    });

            // Act
            int retVal = SynchronizationContextHelper.Sync(mockSyncContext.Object, () => 42);

            // Assert
            Assert.AreEqual(42, retVal);
            Assert.IsTrue(sendWasCalled);
        }

    }
}
