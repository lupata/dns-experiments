using System;
using System.Collections.Generic;

namespace Dns
{
    // see
    // https://tools.ietf.org/html/rfc6895
    // https://www.iana.org/assignments/dns-parameters/dns-parameters.xhtml

    public class Packet
    {
        public ushort ID { get; set; }

        public Flags Flags = new Flags();
        public List<Question> Question = new List<Question>();
        public List<Resource> Answer = new List<Resource>();
        public List<Resource> Authority = new List<Resource>();
        public List<Resource> Additional = new List<Resource>();

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>
            {
                (byte)(ID >> 8),
                (byte)ID,
                (byte)(Flags.Data >> 8),
                (byte)Flags.Data,
                (byte)(Question.Count >> 8),
                (byte)Question.Count,
                (byte)(Answer.Count >> 8),
                (byte)Answer.Count,
                (byte)(Authority.Count >> 8),
                (byte)Authority.Count,
                (byte)(Additional.Count >> 8),
                (byte)Additional.Count
            };

            foreach (var question in Question)
            {
                question.ToBytes(ref bytes);
            }

            return bytes.ToArray();
        }

        public Packet()
        {

        }
        public Packet(ref byte[] bytes)
        {
            if (bytes.Length < 12) return;

            ID = (ushort)((bytes[0] << 8) | bytes[1]);
            Flags.Data = (ushort)((bytes[2] << 8) | bytes[3]);
            var questions = (ushort)((bytes[4] << 8) | bytes[5]);
            var answers = (ushort)((bytes[6] << 8) | bytes[7]);
            var authorities = (ushort)((bytes[8] << 8) | bytes[9]);
            var additionals = (ushort)((bytes[10] << 8) | bytes[11]);

            int pos = 12;
            for (int i = 0; i < questions; ++i)
            {
                Question.Add(new Question(ref bytes, ref pos));
            }
            for (int i = 0; i < answers; ++i)
            {
                Answer.Add(new Resource(ref bytes, ref pos));
            }
            for (int i = 0; i < authorities; ++i)
            {
                Authority.Add(new Resource(ref bytes, ref pos));
            }
            for (int i = 0; i < additionals; ++i)
            {
                Additional.Add(new Resource(ref bytes, ref pos));
            }
        }

    }

    public class Flags
    {
        // painful bit calculations, I really miss c++ bitfields
        internal ushort data;

        public ushort Data
        {
            get => data;
            set => data = value;
        }
        public bool QR
        {
            get => (data & 0x8000) != 0;
            set => data = (ushort)(value ? data | 0x8000 : data & 0x7FFF);
        }
        public byte OpCode
        {
            get => (byte)((data & 0x7800) >> 11);
            set => data = (ushort)((data & 0x87FF) | (value << 11));
        }
        public bool AA
        {
            get => (data & 0x0400) != 0;
            set => data = (ushort)(value ? data | 0x0400 : data & 0xFBFF);
        }
        public bool TC
        {
            get => (data & 0x0200) != 0;
            set => data = (ushort)(value ? data | 0x0200 : data & 0xFDFF);
        }
        /// <summary>
        /// Recursion Desired
        /// </summary>
        public bool RD
        {
            get => (data & 0x0100) != 0;
            set => data = (ushort)(value ? data | 0x0100 : data & 0xFEFF);
        }
        public bool RA
        {
            get => (data & 0x0080) != 0;
            set => data = (ushort)(value ? data | 0x0080 : data & 0xFF7F);
        }
        public bool Z
        {
            get => (data & 0x0040) != 0;
            set => data = (ushort)(value ? data | 0x0040 : data & 0xFFBF);
        }
        public bool AD
        {
            get => (data & 0x0020) != 0;
            set => data = (ushort)(value ? data | 0x0020 : data & 0xFFDF);
        }
        public bool CD
        {
            get => (data & 0x0010) != 0;
            set => data = (ushort)(value ? data | 0x0010 : data & 0xFFEF);
        }
        public byte RCode
        {
            get => (byte)(data & 0x000F);
            set => data = (ushort)((data & 0xFFF0) | value);
        }
    }

    public class Record
    {
        public string ReadName(ref byte[] bytes, ref int pos)
        {
            string name = "";
            int oldPos = 0;

            while (true)
            {
                byte typeSize = bytes[pos++];
                ushort type = (ushort)(typeSize >> 6);
                if (type == 0)
                {
                    ushort size = (ushort)(typeSize & 0x3F);
                    if (size == 0)
                    {
                        if (name.Length == 0)
                        {
                            name = ".";
                        }
                        if (oldPos != 0)
                        {
                            pos = oldPos;
                        }
                        return name;
                    }
                    for (int i = 0; i < size; ++i)
                    {
                        name += (char)bytes[pos++];
                    }
                    name += '.';
                }
                // FIXME: unsafe for well crafted packets (infinite rekursion)
                if (type == 3)
                {
                    int newPos = (ushort)(((typeSize & 0x3F) << 8) | bytes[pos++]);
                    if (oldPos == 0)
                    {
                        oldPos = pos;
                    }
                    pos = newPos;
                }
            }
        }
    }

    public class Question : Record
    {
        public string QNAME { get; set; }
        public ushort QTYPE { get; set; }
        public ushort QCLASS { get; set; }

        public Question(string qname, ushort qtype, ushort qclass)
        {
            if (!qname.EndsWith('.'))
            {
                qname += '.';
            }
            QNAME = qname;
            QTYPE = qtype;
            QCLASS = qclass;
        }

        public Question(ref byte[] bytes, ref int pos)
        {
            QNAME = ReadName(ref bytes, ref pos);
            QTYPE = (ushort)((bytes[pos++] << 8) | bytes[pos++]); // don't do this in c++ (no sequence point)
            QCLASS = (ushort)((bytes[pos++] << 8) | bytes[pos++]);
        }

        public void ToBytes(ref List<byte> bytes)
        {
            var labels = QNAME.Split('.');

            foreach (var label in labels)
            {
                bytes.Add((byte)label.Length);
                foreach (char c in label)
                {
                    bytes.Add((byte)c);
                }
            }

            bytes.Add((byte)(QTYPE >> 8));
            bytes.Add((byte)QTYPE);
            bytes.Add((byte)(QCLASS >> 8));
            bytes.Add((byte)QCLASS);
        }
    }

    public class Resource : Record
    {
        public string NAME { get; set; }
        public ushort TYPE { get; set; }
        public ushort CLASS { get; set; }
        public uint TTL { get; set; }
        public byte[] RDATA { get; set; }

        public Resource(string name, ushort type, ushort cls, uint ttl, byte[] rdata)
        {
            if (!name.EndsWith('.'))
            {
                name += '.';
            }
            NAME = name;
            TYPE = type;
            CLASS = cls;
            TTL = ttl;
            RDATA = rdata;
        }
        public Resource(ref byte[] bytes, ref int pos)
        {
            NAME = ReadName(ref bytes, ref pos);
            TYPE = (ushort)((bytes[pos++] << 8) | bytes[pos++]); // don't do this in c++ (no sequence point)
            CLASS = (ushort)((bytes[pos++] << 8) | bytes[pos++]);
            TTL = (ushort)((bytes[pos++] << 24) | (bytes[pos++] << 16) | (bytes[pos++] << 8) | bytes[pos++]);

            ushort rdlength = (ushort)((bytes[pos++] << 8) | bytes[pos++]);
            RDATA = new byte[rdlength];
            Buffer.BlockCopy(bytes, pos, RDATA, 0, rdlength);
        }
    }
}
