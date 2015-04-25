using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xi2Wav
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists(args[0]))
            {
                foreach (var xiPath in Directory.EnumerateFiles(args[0], "*.xi", SearchOption.AllDirectories))
                {
                    Convert(xiPath);
                }
            }
            else
            {
                Convert(args[0]);
            }

        }

        static void Convert(string xiPath)
        {
            Console.WriteLine("Converting {0}", xiPath);
            var wavPath = Path.GetDirectoryName(xiPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(xiPath) + ".wav";
            using (var instream = new FileStream(xiPath, FileMode.Open))
            using (var outstream = new FileStream(wavPath, FileMode.Create))
            {
                var instrument = new XiInstrument(instream);
                if (instrument.Samples.Count == 0)
                    throw new Exception("Instrument has no samples");
                if (instrument.Samples.Count > 2)
                    throw new Exception("Multi-samples not yet supported");
                if (instrument.Samples.Any(s => !s.Is16Bit))
                    throw new Exception("8 bit samples not yet supported");
                var channels = instrument.Samples.Select(s => s.PcmData).ToList();
                WavWriter.Write(outstream, 44100, 16, channels);
            }
        }
    }
}
