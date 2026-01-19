using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EchoTcpServer
{
    public class EchoHandler
    {
        public async Task HandleClientAsync(Stream stream)
        {
            var buffer = new byte[1024];
            int bytesRead;

            try 
            {

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {message}");

                    var responseMessage = $"Echo: {message}";
                    var responseBytes = Encoding.UTF8.GetBytes(responseMessage);

                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }
    }
}