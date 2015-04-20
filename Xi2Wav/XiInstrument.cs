using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xi2Wav
{
    // XI file format references used:
    // https://github.com/ckald/Samplicity/blob/master/docs/xi_specs.txt
    // http://sourceforge.net/p/modplug/code/HEAD/tree/trunk/OpenMPT/soundlib/XMTools.h

    public class XiInstrument
    {
        // file header
        public string Signature;
        public string Name;
        public string TrackerName;
        public ushort Version;

        // instrument header
        public byte[] SampleMap;
        public ushort[] VolumeEnvelope;
        public ushort[] PanEnvelope;
        public byte VolumePointCount;
        public byte PanPointCount;
        public byte VolumeSustain;
        public byte VolumeLoopStart;
        public byte VolumeLoopEnd;
        public byte PanSustain;
        public byte PanLoopStart;
        public byte PanLoopEnd;
        public byte VolumeFlags;
        public byte PanFlags;
        public byte VibratoType;
        public byte VibratoSweep;
        public byte VibratoDepth;
        public byte VibratoRate;
        public ushort VolumeFadeout;
        public byte MidiEnabled;
        public byte MidiChannel;
        public ushort MidiProgram;
        public ushort PitchWheelRange;
        public byte MuteComputer;
        public byte[] Reserved;
        public ushort SampleCount;

        public List<XiSample> Samples;

        public XiInstrument(Stream stream)
        {
            //TODO: handle endianness
            using (var r = new BinaryReader(stream, Encoding.Default, true))
            {
                // file header
                Signature = new String(r.ReadChars(21));
                if (Signature != "Extended Instrument: ")
                    throw new InvalidDataException("Unexpected signature");
                Name = new String(r.ReadChars(22));
                if (r.ReadByte() != '\x1a')
                    throw new InvalidDataException("Expected EOF");
                TrackerName = new String(r.ReadChars(20));
                Version = r.ReadUInt16();
                if (Version != 0x0102)
                    throw new InvalidDataException("Expected version 0x102");

                // instrument header
                SampleMap = r.ReadBytes(96);
                VolumeEnvelope = r.ReadUInt16s(24);
                PanEnvelope = r.ReadUInt16s(24);
                VolumePointCount = r.ReadByte();
                PanPointCount = r.ReadByte();
                VolumeSustain = r.ReadByte();
                VolumeLoopStart = r.ReadByte();
                VolumeLoopEnd = r.ReadByte();
                PanSustain = r.ReadByte();
                PanLoopStart = r.ReadByte();
                PanLoopEnd = r.ReadByte();
                VolumeFlags = r.ReadByte();
                PanFlags = r.ReadByte();
                VibratoType = r.ReadByte();
                VibratoSweep = r.ReadByte();
                VibratoDepth = r.ReadByte();
                VibratoRate = r.ReadByte();
                VolumeFadeout = r.ReadUInt16();
                MidiEnabled = r.ReadByte();
                MidiChannel = r.ReadByte();
                MidiProgram = r.ReadUInt16();
                PitchWheelRange = r.ReadUInt16();
                MuteComputer = r.ReadByte();
                Reserved = r.ReadBytes(15);
                SampleCount = r.ReadUInt16();
            }

            // sample headers
            Samples = new List<XiSample>();
            for (var i = 0; i < SampleCount; i++)
            {
                Samples.Add(new XiSample(stream));
            }

            foreach (var sample in Samples)
            {
                sample.LoadData(stream);
            }
        }
    }
}
