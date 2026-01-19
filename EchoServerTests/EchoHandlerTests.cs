using Xunit;
using EchoTcpServer;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EchoServerTests
{
    public class EchoHandlerTests
    {
        [Fact]
        public async Task HandleClientAsync_Should_Echo_Input_Message()
        {

            var handler = new EchoHandler();
            var testMessage = "Hello World";
            var inputBytes = Encoding.UTF8.GetBytes(testMessage);


            using var stream = new MemoryStream();
            

            stream.Write(inputBytes, 0, inputBytes.Length);
            

            stream.Position = 0;


            await handler.HandleClientAsync(stream);


            var resultBytes = stream.ToArray();
            var resultString = Encoding.UTF8.GetString(resultBytes);


            Assert.Contains($"Echo: {testMessage}", resultString);
        }
    }
}