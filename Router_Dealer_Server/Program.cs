using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using NetMQ;
using NetMQ.Sockets;

namespace Router_Dealer_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new RouterSocket("@tcp://127.0.0.1:5000"))
            {
                while (true)
                {
                    Console.WriteLine("Server waiting for message");
                    var clientMessage = server.ReceiveMultipartMessage();
                    Console.WriteLine("======================================");
                    Console.WriteLine(" INCOMING CLIENT MESSAGE FROM CLIENT ");
                    Console.WriteLine("======================================");
                    PrintFrames("Server receiving", clientMessage);
                    if (clientMessage.FrameCount == 3)
                    {
                        var clientAddress = clientMessage[0];
                        var clientOriginalMessage = clientMessage[2].ConvertToString();
                        string response = string.Format("{0} back from server {1}",
                            clientOriginalMessage, DateTime.Now.ToLongTimeString());
                        var messageToClient = new NetMQMessage();
                        messageToClient.Append(clientAddress);
                        messageToClient.AppendEmptyFrame();
                        messageToClient.Append(response);
                        server.SendMultipartMessage(messageToClient);
                    }
                }
            }
        }

        public static void PrintFrames(string operationType, NetMQMessage message)
        {
            for (int i = 0; i < message.FrameCount; i++)
            {
                Console.WriteLine("{0} Socket : Frame[{1}] = {2}", operationType, i,
                    message[i].ConvertToString());
            }
        }
    }
}
