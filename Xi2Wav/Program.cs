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
            using (var filestream = new FileStream(args[0], FileMode.Open))
            {
                var instrument = new XiInstrument(filestream);
            }
        }
    }
}
