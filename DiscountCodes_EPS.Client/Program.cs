using System;
using System.Net.Sockets;
using System.Text;

namespace DiscountCodes_EPS.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            string serverIP = "127.0.0.1";
            int serverPort = 8888;

            try
            {
                using (TcpClient client = new TcpClient(serverIP, serverPort))
                using (NetworkStream stream = client.GetStream())
                {
                    while (true)
                    {
                        Console.WriteLine("Choose an action (Generate = 1, UseCode = 2, or Exit = 3):");
                        string action = Console.ReadLine();

                        if (action == "3")
                        {
                            break;
                        }

                        string request = "";
                        if (action == "1")
                        {
                            Console.WriteLine("Enter the number of codes to generate (between 1 and 2000):");
                            ushort count;
                            while (!ushort.TryParse(Console.ReadLine(), out count) || count < 1 || count > 2000)
                            {
                                Console.WriteLine("Invalid input. Please enter a number between 1 and 2000:");
                            }
                            Console.WriteLine("Enter the length of each code (7 or 8):");
                            byte length;
                            while (!byte.TryParse(Console.ReadLine(), out length) || (length != 7 && length != 8))
                            {
                                Console.WriteLine("Invalid input. Please enter 7 or 8:");
                            }
                            request = $"Generate;{count};{length}";
                        }
                        else if (action == "2")
                        {
                            Console.WriteLine("Enter the discount code:");
                            string code = Console.ReadLine();
                            request = $"UseCode;{code}";
                        }
                        else
                        {
                            Console.WriteLine("Invalid action. Please choose Generate = 1, UseCode = 2, or Exit = 3.");
                            continue;
                        }

                        SendRequest(stream, request);
                        string response = ReceiveResponse(stream);
                        PrintResponse(action, response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void SendRequest(NetworkStream stream, string request)
        {
            byte[] requestData = Encoding.ASCII.GetBytes(request);
            stream.Write(requestData, 0, requestData.Length);
            Console.WriteLine($"Sent request to server: {request}");
        }

        static string ReceiveResponse(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            return response;
        }

        static void PrintResponse(string action, string response)
        {
            if (action == "1")
            {
                if (response == "True")
                {
                    Console.WriteLine("Discount codes generated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to generate discount codes.");
                }
            }
            else if (action == "2")
            {
                if (response == "1")
                {
                    Console.WriteLine("Discount code used successfully.");
                }
                else if (response == "0")
                {
                    Console.WriteLine("Discount code not found.");
                }
                else if (response == "2")
                {
                    Console.WriteLine("Discount code already used.");
                }
                else
                {
                    Console.WriteLine("Invalid response from server.");
                }
            }
        }
    }
}
