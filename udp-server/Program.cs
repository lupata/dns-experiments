using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace udp_server
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    UdpClient server = new UdpClient(6653);
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    Console.WriteLine(" waiting");
                    Byte[] raw = server.Receive(ref remote);
                    string received = Encoding.ASCII.GetString(raw);
                    Console.WriteLine(" received: " + received);
                    Console.WriteLine(" from: " + remote.Address.ToString() + ":" + remote.Port.ToString());
                    server.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
