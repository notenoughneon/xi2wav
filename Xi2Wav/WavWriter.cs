using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xi2Wav
{
    public class WavWriter
    {
        public const UInt16 FORMAT_PCM = 0x0001;

        public static void Write(Stream stream, int rate, int bits, List<byte[]> channels)
        {
            if (channels.Count == 0)
                throw new InvalidDataException();
            var length = channels.First().Length;
            if (channels.Skip(1).Any(c => c.Length != length))
                throw new InvalidDataException();

            using (var br = new BinaryWriter(stream, Encoding.Default))
            {
                UInt32 dataChunkSize = (UInt32) channels.Sum(c => c.Length) + 4;
                UInt32 fmtChunkSize = 16;
                UInt32 chunkSize = 4 + 8 + fmtChunkSize + 8 + dataChunkSize;

                UInt16 numChannels = (UInt16)channels.Count;
                UInt32 sampleRate = (UInt32)rate;
                UInt16 bitsPerSample = (UInt16)bits;
                UInt32 byteRate = (UInt32)(sampleRate * numChannels * (bitsPerSample / 8));
                UInt16 blockAlign = (UInt16)(numChannels * (bitsPerSample / 8));

                // RIFF header
                br.Write("RIFF".ToCharArray());
                br.Write(chunkSize);
                br.Write("WAVE".ToCharArray());

                // fmt chunk
                br.Write("fmt ".ToCharArray());
                br.Write(fmtChunkSize);
                br.Write(FORMAT_PCM);
                br.Write(numChannels);
                br.Write(sampleRate);
                br.Write(byteRate);
                br.Write(blockAlign);
                br.Write(bitsPerSample);

                // data chunk
                br.Write("data".ToCharArray());
                br.Write(dataChunkSize);
                var bytesPerSample = bitsPerSample/8;
                for (var offset = 0; offset < length; offset += bytesPerSample)
                {
                    foreach (var channel in channels)
                    {
                        br.Write(channel, offset, bytesPerSample);
                    }
                }
            }
        }
    }
}
