using System;

namespace DiscountCodes_EPS.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();

            Console.WriteLine("Server started. Press any key to stop the server...");
            Console.ReadKey();

            server.Stop();
        }
    }
}
