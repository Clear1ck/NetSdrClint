using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EchoTcpServer
{
    public class EchoHandler
    {
        public async Task HandleClientAsync(Stream stream, CancellationToken token)
        {
            byte[] buffer = new byte[8192];
            int bytesRead;

            try
            {

                while (!token.IsCancellationRequested && 
                       (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                {

                    await stream.WriteAsync(buffer, 0, bytesRead, token);
                    Console.WriteLine($"Echoed {bytesRead} bytes to the client.");
                }
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }

        }
    }
}