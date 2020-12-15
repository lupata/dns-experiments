using System;


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

            Console.WriteLine("dig: server:" + serverHost.AddressList[0].ToString() + " request:" + requestHost.AddressList[0].ToString());


        }
    }
}
