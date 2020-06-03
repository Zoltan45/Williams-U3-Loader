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
        internal byte internalid = 0;
        internal byte offset3A = 0;

        internal string ToStringOut()
        {
            return ID + "," + Type+",0x" + offset1CPU.ToString("X4") + ",0x" + offset1ROM.ToString("X5") + ",0x" + priority.ToString("X2") + ",0x" +offset2.ToString("X2") + ",0x"
                            + offset2CPU.ToString("X4") + ",0x" + offset2ROM.ToString("X5") + ",0x" + offset3CPU.ToString("X4") + ",0x" + offset3ROM.ToString("X5")  + "," + voice.ToString("X2") + ",0x" + internalid.ToString("X2") 
                             + "\n";

        }
        internal string ToStringOutSam()
        {
            return ID + "," + Type + ",0x" + offset1CPU.ToString("X4") + ",0x" + offset1ROM.ToString("X5") + ",0x" + priority.ToString("X2") + ",0x" + offset2.ToString("X2") + ",0x"
                            + offset2CPU.ToString("X4") + ",0x" + offset2ROM.ToString("X5") + ",0x" + offset3CPU.ToString("X4") + ",0x" + offset3ROM.ToString("X5") + "," + voice.ToString("X2") + ",0x" + internalid.ToString("X2");
        }
    }
}