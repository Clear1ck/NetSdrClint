using System.Net;
using System.Net.Sockets;
using EchoTcpServer;


var port = 8888;
var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Echo Server started on port {port}...");


var handler = new EchoHandler();

while (true)
{

    var client = await listener.AcceptTcpClientAsync();
    Console.WriteLine("Client connected.");


    _ = Task.Run(async () => 
    {
        try
        {
            using (client)
            using (var stream = client.GetStream())
            {
                await handler.HandleClientAsync(stream);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
        Console.WriteLine("Client disconnected.");
    });
}