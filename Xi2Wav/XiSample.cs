using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xi2Wav
{
    public class XiSample
    {
        public const byte FLAG_LOOP = 0x01;
        public const byte FLAG_PINGPONG = 0x02;
        public const byte FLAG_16BIT = 0x10;
        public const byte FLAG_STEREO = 0x20;

        public UInt32 Length;
        public UInt32 LoopStart;
        public UInt32 LoopLength;
        public byte Volume;
        public sbyte FineTune;
        public byte Flags;
        public byte Pan;
        public sbyte Transpose;
        public byte Reserved;
        public string Name;

        public bool IsLoop { get { return (Flags & FLAG_LOOP) != 0; } }
        public bool IsPingPong { get { return (Flags & FLAG_PINGPONG) != 0; } }
        public bool Is16Bit { get { return (Flags & FLAG_16BIT) != 0; } }
        public bool IsStereo { get { return (Flags & FLAG_STEREO) != 0; } }

        public byte[] DpcmData;

        internal XiSample(Stream stream)
        {
            using (var r = new BinaryReader(stream, Encoding.Default, true))
            {
                Length = r.ReadUInt32();
                LoopStart = r.ReadUInt32();
                LoopLength = r.ReadUInt32();
                Volume = r.ReadByte();
                FineTune = r.ReadSByte();
                Flags = r.ReadByte();
                Pan = r.ReadByte();
                Transpose = r.ReadSByte();
                Reserved = r.ReadByte();
                Name = new String(r.ReadChars(22));
            }
        }

        internal void LoadData(Stream stream)
        {
            DpcmData = new byte[Length];
            int offset = 0;
            int length = (int) Length;
            var read = 0;
            do
            {
                read = stream.Read(DpcmData, offset, length - offset);
                offset += read;
            }
            while (read > 0 && (length - offset) > 0);
            if (offset != length)
                throw new InvalidDataException("Unable to read entire data stream");
        }
    }
}
