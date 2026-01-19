using NUnit.Framework;
using NetSdrClientApp.Networking;

namespace NetSdrClientAppTests
{
    [TestFixture]
    public class UdpClientWrapperTests
    {
        [Test]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            using var wrapper = new UdpClientWrapper(9999);

            // Assert
            Assert.That(wrapper, Is.Not.Null);
        }

        [Test]
        public void Dispose_ShouldNotThrow_WhenCalledTwice()
        {
            var wrapper = new UdpClientWrapper(9999);
            
            // Act
            wrapper.Dispose();
            
            // Assert
            Assert.DoesNotThrow(() => wrapper.Dispose());
        }
    }
}