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
        static void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("xi2wav infile.xi [outfile.wav]");
            Console.WriteLine("xi2wav /r indirectory [outdirectory]");
        }

        static void Main(string[] args)
        {
            var parms = args.Where(a => !a.StartsWith("/")).ToArray();
            var switches = args.Where(a => a.StartsWith("/")).ToArray();
            if (switches.Length == 1 && switches[0] == "/r" && parms.Length > 0)
            {
                foreach (var xiPath in Directory.EnumerateFiles(parms[0], "*.xi", SearchOption.AllDirectories))
                {
                    var wavPath = Path.GetDirectoryName(xiPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(xiPath) + ".wav";
                    if (parms.Length >= 2)
                        wavPath = parms[1] + Path.DirectorySeparatorChar + wavPath;
                    Directory.CreateDirectory(Path.GetDirectoryName(wavPath));
                    Convert(xiPath, wavPath);
                }
            }
            else if (switches.Length == 0 && parms.Length > 0)
            {
                var xiPath = parms[0];
                string wavPath;
                if (parms.Length >= 2)
                    wavPath = parms[1];
                else
                    wavPath = Path.GetDirectoryName(xiPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(xiPath) + ".wav";
                Convert(xiPath, wavPath);
            }
            else
            {
                Usage();
            }
        }

        static void Convert(string xiPath, string wavPath)
        {
            Console.WriteLine("Converting {0}", xiPath);
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
                // convert dpcm to pcm
                foreach (var sample in instrument.Samples)
                {
                    Int16 last = 0;
                    for (var i = 0; i < sample.DpcmData.Length; i += 2)
                    {
                        last += BitConverter.ToInt16(sample.DpcmData, i);
                        Array.Copy(BitConverter.GetBytes(last), 0, sample.DpcmData, i, 2);
                    }
                }
                var channels = instrument.Samples.Select(s => s.DpcmData).ToList();
                WavWriter.Write(outstream, 44100, 16, channels);
            }
        }
    }
}
