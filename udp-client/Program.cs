using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_client
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = args[0];
            string message = args[1];

            try {
                UdpClient client = new UdpClient();
                Byte[] raw = Encoding.ASCII.GetBytes(message);
                int send = client.Send(raw, raw.Length, server, 6653);
                Console.WriteLine(" send bytes: " + send.ToString());

                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                raw = client.Receive(ref remote);
                string received = Encoding.ASCII.GetString(raw);
                Console.WriteLine(" received: " + received);
                Console.WriteLine(" from: " + remote.Address.ToString() + ":" + remote.Port.ToString());

                client.Close();

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
