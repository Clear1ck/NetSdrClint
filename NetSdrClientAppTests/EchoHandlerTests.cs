using NUnit.Framework;
using EchoTcpServer;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServerTests
{
    [TestFixture]
    public class EchoHandlerTests
    {
        [Test]
        public async Task HandleClientAsync_ShouldEchoMessage()
        {
            // Arrange
            var handler = new EchoHandler();
            var message = "Hello NUnit";
            var inputBytes = Encoding.UTF8.GetBytes(message);
            
            using var stream = new MemoryStream();
            stream.Write(inputBytes, 0, inputBytes.Length);
            stream.Position = 0;

            // Act
            await handler.HandleClientAsync(stream, CancellationToken.None);

            // Assert
            var resultBytes = stream.ToArray();
            var resultString = Encoding.UTF8.GetString(resultBytes);
            
            Assert.That(resultString, Does.Contain(message));
        }

        [Test]
        public async Task HandleClientAsync_EmptyStream_ShouldNotThrow()
        {
            // Arrange
            var handler = new EchoHandler();
            using var stream = new MemoryStream(); 

            // Act & Assert
            Assert.DoesNotThrowAsync(async () => 
                await handler.HandleClientAsync(stream, CancellationToken.None));
        }
    }
}