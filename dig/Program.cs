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
            IPAddress address = System.Net.IPAddress.Parse(args[2]);


            Dns.Packet packet = new Dns.Packet();
            packet.Flags.RD = true;
            packet.Question.Add(new Dns.Question(question, 1, 1));
            

            Dns.EDNS edns = new Dns.EDNS();

            Dns.Option subnet = new Dns.Option();
            subnet.Subnet(address);
            edns.Options.Add(subnet);
            Dns.Option cookie = new Dns.Option();
            cookie.Cookie(0x4444444444444444);
            edns.Options.Add(cookie);

            packet.Additional.Add(new Dns.Resource("", 41, 4096, 0, edns.ToBytes()));

            // add an non-standard record
            packet.Additional.Add(new Dns.Resource("", 40, 1, 0x11223344, new byte[0]));

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

                foreach (var bt in r.RDATA)
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

                foreach (var bt in r.RDATA)
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

                foreach (var bt in r.RDATA)
                {
                    Console.WriteLine(bt.ToString());
                }
            }
        }
    }
}
