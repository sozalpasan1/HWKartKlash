using System;
using System.Net;
using System.Net.Sockets;

namespace KartServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Kart Game Server");
            Console.WriteLine("=================");
            
            int port = 7777;
            if (args.Length > 0 && int.TryParse(args[0], out int customPort))
            {
                port = customPort;
            }
            
            GameServer server = new GameServer(port);
            server.Start();
            
            Console.WriteLine($"Server started on port {port}");
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            
            server.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}