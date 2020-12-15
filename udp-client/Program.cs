using System;
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
                // there are multiple ovewrloaded versions of the constructor: this sets remote server/port 
                UdpClient client = new UdpClient();
                Byte[] raw = Encoding.ASCII.GetBytes(message);
                int send = client.Send(raw, raw.Length, server, 6653);
                Console.WriteLine(" send bytes: " + send.ToString());

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
