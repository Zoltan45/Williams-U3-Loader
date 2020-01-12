using System;

namespace U3_Loader
{
    internal class SampleCode
    {
        internal string ID;
        internal string Type;
        internal int offset1CPU = 0;
        internal int offset1ROM = 0;
        internal byte priority = 0;
        internal byte offset2 = 0;
        internal int offset2CPU = 0;
        internal int offset2ROM = 0;
        internal int offset3CPU = 0;
        internal int offset3ROM = 0;
        internal byte voice = 0;
        internal byte volume = 0;
        internal byte offset3A = 0;
        internal int offset4CPU = 0;
        internal int offset4ROM = 0;
        internal byte bank = 0;
        internal byte pad = 0;
        internal byte sample = 0;
        internal ushort offset5CPU = 0;
        internal int offset5ROM = 0;
        internal ushort length = 0;

        internal string ToStringOut()
        {
            return ID + "," + Type+",0x" + offset1CPU.ToString("X4") + ",0x" + offset1ROM.ToString("X5") + ",0x" + priority.ToString("X2") + ",0x" +offset2.ToString("X2") + ",0x"
                            + offset2CPU.ToString("X4") + ",0x" + offset2ROM.ToString("X5") + ",0x" + voice.ToString("X2") + ",0x" + volume.ToString("X2") + ",0x"
                            + offset3CPU.ToString("X4") + ",0x" + offset3ROM.ToString("X5") + ",0x"
                            + offset4CPU.ToString("X4") + ",0x" + offset4ROM.ToString("X5") + ",0x" 
                            + offset5CPU.ToString("X4") + ",0x" + offset5ROM.ToString("X5") + ",0x" + bank.ToString("X2") + ",0x" + pad.ToString("X2") + ",0x" + sample.ToString("X2") + ",0x" + length.ToString("X5") + "\n";

        }
    }
}