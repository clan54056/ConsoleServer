// A C# Program for Server
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{

    class Program
    {

        // Main Method
        static void Main(string[] args)
        {
            ExecuteServer();
        }

        public static void ExecuteServer()
        {

            bool wer = true;
            bool ert = true;

            // Establish the local endpoint 
            // for the socket. Dns.GetHostName
            // returns the name of the host 
            // running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            // Creation TCP/IP Socket using 
            // Socket Class Constructor
            Socket listener = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

            try
            {

                // Using Bind() method we associate a
                // network address to the Server Socket
                // All client that will connect to this 
                // Server Socket must know this network
                // Address
                listener.Bind(localEndPoint);

                // Using Listen() method we create 
                // the Client list that will want
                // to connect to Server
                listener.Listen(10);

                List<Socket> clients = new List<Socket>();

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    Socket clientSocket = listener.Accept();
                    clients.Add(clientSocket);

                    // Create a new thread to handle the client
                    new System.Threading.Thread(() =>
                    {
                        while (true)
                        {
                            byte[] bytes = new Byte[1024];
                            string data = null;

                            try
                            {
                                int numByte = clientSocket.Receive(bytes);

                                if (numByte == 0)
                                {
                                    // Client disconnected
                                    clients.Remove(clientSocket);
                                    break;
                                }

                                data = Encoding.ASCII.GetString(bytes, 0, numByte);
                                Console.WriteLine("Received from client: {0}", data);

                                // Broadcast the message to all clients
                                foreach (var otherClient in clients)
                                {
                                    if (otherClient != clientSocket)
                                    {
                                        otherClient.Send(Encoding.ASCII.GetBytes(data));
                                    }
                                }
                            }
                            catch (SocketException)
                            {
                                // Client disconnected
                                clients.Remove(clientSocket);
                                break;
                            }
                        }

                        // Close client Socket
                        clientSocket.Close();
                    }).Start();
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
