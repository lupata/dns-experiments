using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_client
{
    class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string server = args[0];
            int count = int.Parse(args[1]);

            static void Receive(IAsyncResult ar)
            {
                try
                {
                    UdpClient client = (UdpClient)ar.AsyncState;
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    byte[] raw = Encoding.ASCII.GetBytes("connection error");
                    raw = client.EndReceive(ar, ref remote);
                    client.BeginReceive(new AsyncCallback(Receive), client);

                    string received = Encoding.ASCII.GetString(raw);
                    Console.WriteLine(" received: " + received);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            try
            {
                UdpClient client = new UdpClient(); // an appropriate local port number is assigned

                // client.BeginReceive(new AsyncCallback(Receive), client);

                for (int i = 0; i < count; ++i)
                {
                    Byte[] raw = Encoding.ASCII.GetBytes(i.ToString());
                    client.Send(raw, raw.Length, server, 6653);
                }

                client.BeginReceive(new AsyncCallback(Receive), client);

                // Thread.Sleep(1000);
                Console.ReadKey();
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
