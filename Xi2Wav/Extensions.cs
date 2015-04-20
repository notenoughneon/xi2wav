using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xi2Wav
{
    public static class Extensions
    {
        public static ushort[] ReadUInt16s(this BinaryReader reader, int count)
        {
            var result = new ushort[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = reader.ReadUInt16();
            }
            return result;
        }
    }
}
