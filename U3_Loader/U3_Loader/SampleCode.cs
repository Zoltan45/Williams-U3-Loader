using System;

namespace U3_Loader
{
    internal class SampleCode
    {
        internal string ID;
        internal string Type;
        internal int offset1CPU;
        internal int offset1ROM;
        internal byte priority;
        internal byte offset2;
        internal int offset2CPU;
        internal int offset2ROM;
        internal int offset3CPU;
        internal int offset3ROM;
        internal byte voice;
        internal byte volume;
        internal byte offset3A;
        internal int offset4CPU;
        internal int offset4ROM;
        internal byte bank;
        internal byte pad;
        internal byte sample;
        internal ushort offset5CPU;
        internal int offset5ROM;
        internal ushort length;

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