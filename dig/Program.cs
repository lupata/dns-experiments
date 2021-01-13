using System;
using System.Net;
using System.Net.Sockets;

namespace dig
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = args[0];
            string question = args[1];

            System.Net.IPHostEntry serverHost = System.Net.Dns.GetHostEntry(server);

            Dns.Packet packet = new Dns.Packet();
            packet.Flags.RD = true;
            packet.Question.Add(new Dns.Question(question, 1, 1));
            byte[] raw = packet.ToBytes();

            // debug: read it back
            Dns.Packet response = new Dns.Packet(ref raw);
            Console.WriteLine(response.Question[0].QNAME);
            Console.WriteLine(response.Question[0].QTYPE);
            Console.WriteLine(response.Question[0].QCLASS);

            UdpClient client = new UdpClient();
            int send = client.Send(raw, raw.Length, server, 53);
            Console.WriteLine(" send bytes: " + send.ToString());

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            raw = client.Receive(ref remote);

            foreach (var bt in raw)
            {
                Console.WriteLine(bt.ToString());
            }
        }
    }
}
