using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// This program was designed for test purposes only
/// Not for a review
/// </summary>

namespace EchoTcpServer
{
    [ExcludeFromCodeCoverage]
    public class EchoServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public EchoServer(int port)
        {
            _port = port;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}.");

            var handler = new EchoHandler();

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected.");


                    _ = Task.Run(async () => 
                    {
                        using (client)
                        using (var stream = client.GetStream())
                        {
                            await handler.HandleClientAsync(stream, _cancellationTokenSource.Token);
                        }
                        Console.WriteLine("Client disconnected.");
                    }, _cancellationTokenSource.Token);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept error: {ex.Message}");
                }
            }

            Console.WriteLine("Server shutdown.");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
            _cancellationTokenSource.Dispose();
            Console.WriteLine("Server stopped.");
        }

        public static async Task Main(string[] args)
        {
            EchoServer server = new EchoServer(5000);

            _ = Task.Run(() => server.StartAsync());

            string host = "127.0.0.1";
            int port = 60000;
            int intervalMilliseconds = 5000;

            using (var sender = new UdpTimedSender(host, port))
            {
                Console.WriteLine("Press any key to stop sending...");
                sender.StartSending(intervalMilliseconds);

                Console.WriteLine("Press 'q' to quit...");
                while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q)
                {
                }

                sender.StopSending();
                server.Stop();
                Console.WriteLine("Sender stopped.");
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class UdpTimedSender : IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private readonly UdpClient _udpClient;
        private Timer _timer;

        public UdpTimedSender(string host, int port)
        {
            _host = host;
            _port = port;
            _udpClient = new UdpClient();
        }

        public void StartSending(int intervalMilliseconds)
        {
            if (_timer != null)
                throw new InvalidOperationException("Sender is already running.");

            _timer = new Timer(SendMessageCallback, null, 0, intervalMilliseconds);
        }

        ushort i = 0;

        private void SendMessageCallback(object state)
        {
            try
            {
                Random rnd = new Random();
                byte[] samples = new byte[1024];
                rnd.NextBytes(samples);
                i++;

                byte[] msg = (new byte[] { 0x04, 0x84 }).Concat(BitConverter.GetBytes(i)).Concat(samples).ToArray();
                var endpoint = new IPEndPoint(IPAddress.Parse(_host), _port);

                _udpClient.Send(msg, msg.Length, endpoint);
                Console.WriteLine($"Message sent to {_host}:{_port} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public void StopSending()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public void Dispose()
        {
            StopSending();
            _udpClient.Dispose();
        }
    }
}