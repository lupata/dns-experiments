namespace Dns
{
    class RandomIp4
    {
        // linear congruential generator (good enough, use parameters from numerical recipes)
        private uint value;
        private const uint c = 1013904223;
        private const uint a = 1664525;

        static uint[,] specialRanges;

        static RandomIp4()
        {
            void AddRange(byte b1, byte b2, byte b3, byte b4, byte net)
            {
                uint first = (uint)((b1 << 24) + (b2 << 16) + (b3 << 8) + b4);
                uint last = (uint)(first + (1 << (32 - net)) - 1);

               // specialRanges = new
            }


        }



    }
}
