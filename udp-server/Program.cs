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
            string message = args[0];

            UdpClient server = new UdpClient(6653);

            while (true)
            {
                try
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

                    Console.WriteLine(" waiting");

                    Byte[] raw = server.Receive(ref remote);
                    string received = Encoding.ASCII.GetString(raw);
                    Console.WriteLine(" received: " + received);
                    Console.WriteLine(" from: " + remote.Address.ToString() + ":" + remote.Port.ToString());

                    raw = Encoding.ASCII.GetBytes(
                        remote.Address.ToString() + ":" + remote.Port.ToString()
                        + " " + received + " -> " + message);

                    int send = server.Send(raw, raw.Length, remote);
                    Console.WriteLine(" send bytes: " + send.ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            server.Close();
        }
    }
}
