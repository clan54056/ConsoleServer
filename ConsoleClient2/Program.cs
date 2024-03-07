// A C# program for Client
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{

    class Program
    {

        // Main Method
        static void Main(string[] args)
        {
            ExecuteClient();
            Console.ReadLine();
        }

        // ExecuteClient() Method
        static void ExecuteClient()
        {

            try
            {

                // Establish the remote endpoint 
                // for the socket. This example 
                // uses port 11111 on the local 
                // computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

                // Creation TCP/IP Socket using 
                // Socket Class Constructor
                Socket sender = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(localEndPoint);
                    Console.WriteLine("Socket connected to -> {0}", sender.LocalEndPoint.ToString());

                    // Start a separate thread for receiving messages
                    new Thread(() =>
                    {
                        while (true)
                        {
                            byte[] messageReceived = new byte[1024];
                            int byteRecv = sender.Receive(messageReceived);
                            if (byteRecv == 0)
                            {
                                // Server disconnected
                                Console.WriteLine("Server disconnected.");
                                break;
                            }

                            Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));
                        }
                    }).Start();

                    // Send and receive messages in a loop
                    while (true)
                    {
                        string message = Console.ReadLine();
                        if (message.ToLower() == "exit")
                            break;

                        byte[] messageSent = Encoding.ASCII.GetBytes(message);
                        int byteSent = sender.Send(messageSent);
                    }
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
    }
}
