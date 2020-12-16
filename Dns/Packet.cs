using System;

namespace Dns
{
    public class Packet
    {
        public Header header;
        public Question question;
        public Answer answer;
    }

    public class Header
    {
        // no c++ bitfield in C# and no pythcon pack -- todo: find a better way
        public ushort id; // id to identify response (udp -> no connection, also a sort of security feature)
        public bool qr; //query=0, response=1
        public byte opcode; // actually only 4 bits
        public bool aa; //authoritative answer
        public bool tc; // truncation
        public bool rd; // recursion desired
        public bool ra; // recurion available
        public byte z; // actually only 3 bits, reserved
        public byte rcode; // actually only 4 bits,

    }

    public class Question
    {

    }

    public class Answer
    {

    }
}
