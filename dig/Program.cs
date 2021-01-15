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

            UdpClient client = new UdpClient();
            int send = client.Send(raw, raw.Length, server, 53);

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            raw = client.Receive(ref remote);

            Dns.Packet response = new Dns.Packet(ref raw);
            Console.WriteLine(" -- Question --");
            foreach (var q in response.Question)
            {
                Console.WriteLine(q.QNAME);
                Console.WriteLine(q.QTYPE);
                Console.WriteLine(q.QCLASS);
            }

            Console.WriteLine(" -- Answer --");
            foreach (var r in response.Answer)
            {
                Console.WriteLine(r.NAME);
                Console.WriteLine(r.TYPE);
                Console.WriteLine(r.CLASS);
                Console.WriteLine(r.TTL);

                foreach (var bt in response.Answer[0].RDATA)
                {
                    Console.WriteLine(bt.ToString());
                }
            }

            Console.WriteLine(" -- Authority --");
            foreach (var r in response.Authority)
            {
                Console.WriteLine(r.NAME);
                Console.WriteLine(r.TYPE);
                Console.WriteLine(r.CLASS);
                Console.WriteLine(r.TTL);

                foreach (var bt in response.Answer[0].RDATA)
                {
                    Console.WriteLine(bt.ToString());
                }
            }

            Console.WriteLine(" -- Additional --");
            foreach (var r in response.Additional)
            {
                Console.WriteLine(r.NAME);
                Console.WriteLine(r.TYPE);
                Console.WriteLine(r.CLASS);
                Console.WriteLine(r.TTL);

                foreach (var bt in response.Answer[0].RDATA)
                {
                    Console.WriteLine(bt.ToString());
                }
            }
        }
    }
}
