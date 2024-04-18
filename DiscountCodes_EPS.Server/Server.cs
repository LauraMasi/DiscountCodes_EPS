using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscountCodes_EPS.Server
{
    class Server
    {
        private TcpListener tcpListener;
        private DiscountCodeManager discountCodeManager;

        public Server()
        {
            discountCodeManager = new DiscountCodeManager();
        }

        public void Start()
        {
            int port = 8888;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            tcpListener = new TcpListener(localAddr, port);
            tcpListener.Start();

            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        }

        public void Stop()
        {
            tcpListener.Stop();
            Console.WriteLine("Server stopped.");
        }

        private async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {data}");

                string[] request = data.Split(';');

                if (request.Length >= 1)
                {
                    string response = "";

                    if (request[0] == "Generate")
                    {
                        ushort count;
                        byte length;
                        if (ushort.TryParse(request[1], out count) && byte.TryParse(request[2], out length))
                        {
                            bool result = discountCodeManager.GenerateAndSaveDiscountCodes(count, length);
                            response = result.ToString();
                        }
                    }
                    else if (request[0] == "UseCode")
                    {
                        string code = request[1];
                        byte result = discountCodeManager.UseDiscountCode(code);
                        response = result.ToString();
                    }

                    byte[] responseData = Encoding.ASCII.GetBytes(response);
                    await stream.WriteAsync(responseData, 0, responseData.Length);
                    Console.WriteLine($"Sent response to client: {response}");
                }
            }

            client.Close();
        }
    }
}
