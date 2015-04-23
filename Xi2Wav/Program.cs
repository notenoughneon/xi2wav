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
            var xiPath = args[0];
            using (var filestream = new FileStream(xiPath, FileMode.Open))
            {
                var instrument = new XiInstrument(filestream);
                if (instrument.Samples.Count > 1)
                    throw new Exception("Multi-samples not yet supported");
                if (!instrument.Samples.First().Is16Bit)
                    throw new Exception("8 bit samples not supported");

                var wavPath = Path.GetFileNameWithoutExtension(xiPath) + ".wav";
                var channels = instrument.Samples.Select(s => s.PcmData).ToList();

                using (var outstream = new FileStream(wavPath, FileMode.Create))
                {
                    WavWriter.Write(outstream, 22050, 16, channels);
                }
            }
        }
    }
}
