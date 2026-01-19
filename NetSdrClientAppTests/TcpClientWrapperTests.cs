using NUnit.Framework;
using NetSdrClientApp.Networking;

namespace NetSdrClientAppTests
{
    [TestFixture]
    public class TcpClientWrapperTests
    {
        [Test]
        public void Constructor_ShouldSetDisconnectedState()
        {
            using var wrapper = new TcpClientWrapper("127.0.0.1", 8888);
            
            Assert.That(wrapper.Connected, Is.False);
        }
    }
}