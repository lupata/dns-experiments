using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Dns;

namespace dig
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = args[0];
            string request = args[1];

            Console.WriteLine("dig: server:" + server + " request:" + request);

            // resolve server and request via system DNS (at least for server this is anyway required)
            System.Net.IPHostEntry serverHost = System.Net.Dns.GetHostEntry(server);
            System.Net.IPHostEntry requestHost = System.Net.Dns.GetHostEntry(request);
            if (request[request.Length - 1] != '.')
            {
                request += '.';
            }

            Console.WriteLine("dig: server: " + serverHost.AddressList[0].ToString() + " request: " + requestHost.AddressList[0].ToString());

            // should use dns packet class, but now do it manually
            int size = 0;
            string[] labels = request.Split('.');
            foreach (var label in labels)
            {
                size += label.Length + 1;
                System.Console.WriteLine($"<{label}>");
            }

            byte[] raw = new byte[12 + size + 4];

            raw[0] = 0; // id
            raw[1] = 0;
            raw[2] = 1; // flags
            raw[3] = 0;
            raw[4] = 0; // one query
            raw[5] = 1;
            raw[6] = 0; // no answer
            raw[7] = 0;
            raw[8] = 0; // no auth
            raw[9] = 0;
            raw[10] = 0; // no additional
            raw[11] = 0;

            int pos = 12;
            foreach (var label in labels)
            {
                raw[pos++] = (byte)label.Length;
                foreach (char c in label)
                {
                    raw[pos++] = (byte)c;
                }
            }
            // the rfc is a bit misleading, no 16 bit padding
            raw[pos++] = 0; // type a
            raw[pos++] = 1;
            raw[pos++] = 0; // class in
            raw[pos++] = 1;

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
