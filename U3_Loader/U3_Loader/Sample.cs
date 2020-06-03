using System;

namespace U3_Loader
{
    internal class Sample
    {
        internal string ID;
        internal int offset4CPU = 0;
        internal int offset4ROM = 0;
        internal byte bank = 0;
        internal byte pad = 0;
        internal byte sample = 0;
        internal ushort offset5CPU = 0;
        internal int offset5ROM = 0;
        internal ushort length = 0;
        internal double time = 0;
        internal string ToStringOut()
        {
            return ID + ",0x" + offset4CPU.ToString("X4") + ",0x" + offset4ROM.ToString("X5") + ",0x"
                            + offset5CPU.ToString("X4") + ",0x" + offset5ROM.ToString("X5") + ",0x" + bank.ToString("X2") + ",0x" + pad.ToString("X2") + ",0x" + sample.ToString("X2") + ",0x" + length.ToString("X5") + "," + time.ToString()+
                            "\n";
        }

        internal string ToStringOutSam()
        {
            return "0x" + offset4CPU.ToString("X4") + ",0x" + offset4ROM.ToString("X5") + ",0x"
                            + offset5CPU.ToString("X4") + ",0x" + offset5ROM.ToString("X5") + ",0x" + bank.ToString("X2") + ",0x" + pad.ToString("X2") + ",0x" + sample.ToString("X2") + ",0x" + length.ToString("X5") + "," + time.ToString() +
                            "\n";
        }

    }
}