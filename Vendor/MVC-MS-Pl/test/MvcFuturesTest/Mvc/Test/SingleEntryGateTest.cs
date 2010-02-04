namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class SingleEntryGateTest {

        [TestMethod]
        public void TryEnter() {
            // Arrange
            SingleEntryGate gate = new SingleEntryGate();

            // Act
            bool firstCall = gate.TryEnter();
            bool secondCall = gate.TryEnter();

            // Assert
            Assert.IsTrue(firstCall, "TryEnter() should return TRUE on first call.");
            Assert.IsFalse(secondCall, "TryEnter() should return FALSE on each subsequent call.");
        }

    }
}
