using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace U3_Loader
{
    public partial class Form1 : Form
    {
        private string ImportDir;
        private string ExportDir;
        private List<SampleCode> Samples7a;
        private List<SampleCode> Samples00;
        private List<Sample> SamplesINT;
        string String7A = "CMD,Type,Offset1 Address(CPU),Offset1 Address(ROM),Type Code,Offset2,Offset2 Address(CPU),Offset2 Address(ROM),Offset3 Address(CPU),Offset3 Address(ROM),Voice,Sample Internal ID,Sample Offset Address(CPU),Sample Offset Address(ROM),Sample Jump Address(CPU),Sample Jump Address(ROM),Bank,Delay(if 2 or less),SampleNo,Length,Time(Ticks)\n";
        string String00 = "CMD,Type,Offset1 Address(CPU),Offset1 Address(ROM),Type Code,Offset2,Offset2 Address(CPU),Offset2 Address(ROM),Offset3 Address(CPU),Offset3 Address(ROM),Voice,Sample Internal ID,Sample Offset Address(CPU),Sample Offset Address(ROM),Sample Jump Address(CPU),Sample Jump Address(ROM),Bank,Delay(if 2 or less),SampleNo,Length,Time(Ticks)\n";
        string StringSamp = "ID,Offset Address(CPU),Offset Address(ROM),Jump Address(CPU),Jump Address(ROM),Bank,Delay(if 2 or less),Sample,Length,Time(Ticks)\n";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Samples7a = new List<SampleCode>();
            Samples00 = new List<SampleCode>();
            SamplesINT = new List<Sample>();
            int baseadd = 0x4000;
            OpenFileDialog OF = new OpenFileDialog
            {
                Title = "Open U3",
            };
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                ImportDir = System.IO.Path.GetDirectoryName(OF.FileName);
                ExportDir = ImportDir;
                byte[] tmp = File.ReadAllBytes(OF.FileName);
                int start = 0x10000;
                for (int bnk = 1; bnk < 3; bnk++) //find bank 3
                {
                    start += 0x8000;
                    if (tmp.Length <= start)
                    {
                        start = 0;
                    }
                }
                int origstart = start;

                ushort start_7A = getShort(tmp, origstart + 0x19);
                ushort start_00 = getShort(tmp, origstart + 0x0f);

                ushort base2 = getShort(tmp, origstart + 0x0b);

                // samples
                ushort base3 = getShort(tmp, origstart + 0x15);
                bool terminated = false;
                int i = 0;
                while (terminated == false)
                {
                    Sample sample = new Sample();
                    sample.ID = i.ToString("X2");
                    sample.offset4CPU = (int)(base3 + (2 * i));
                    sample.offset4ROM = origstart + sample.offset4CPU - baseadd;

                    sample.offset5CPU = (ushort)getShort(tmp, sample.offset4ROM); //short address
                    sample.offset5ROM = origstart + sample.offset5CPU - baseadd;

                    if (sample.offset5CPU < base3) //out of data
                    {
                        terminated = true;
                    }
                    else
                    {
                        sample.bank = tmp[sample.offset5ROM];
                        sample.pad = tmp[sample.offset5ROM + 1];
                        sample.sample = tmp[sample.offset5ROM + 2];
                        sample.length = (ushort)getShort(tmp, sample.offset5ROM + 3);
                        sample.time = sample.length / 7575.75 * 2;

                        SamplesINT.Add(sample);
                        StringSamp += sample.ToStringOut();
                        i++;
                    }
                }


                for (int ia = 0; ia < 0x100; ia++)
                {
                    bool synth = false;
                    SampleCode samplecode = new SampleCode();
                    samplecode.ID = ia.ToString("X");
                    samplecode.Type = "Sample";
                    samplecode.offset1CPU = (int)(start_7A + (2 * ia)); //5f44 + 2 x id
                    samplecode.offset1ROM = (int)(origstart + samplecode.offset1CPU - baseadd); //1f44 + 2 x id

                    if (samplecode.offset1CPU >= base2)
                    {
                        samplecode.Type = "INVALID";
                        Samples7a.Add(samplecode);
                        String7A += samplecode.ToStringOut();
                        continue;
                    }

                    if (((samplecode.offset1ROM + 1) > tmp.Length) || samplecode.offset1ROM < 0)
                    {
                        samplecode.Type = "Synth (Off1)";
                        synth = true;
                    }

                    if (synth == false)
                    {
                        samplecode.priority = tmp[samplecode.offset1ROM];

                        switch (samplecode.priority)
                        {
                            case 0x01:
                                {
                                    samplecode.Type = "Music";
                                    break;
                                }

                            case 0x04:
                                {
                                    samplecode.Type = "SFX";
                                    break;
                                }

                            case 0x07:
                                {
                                    samplecode.Type = "Sample";
                                    break;
                                }

                            case 0x0F:
                                {
                                    samplecode.Type = "Code";
                                    break;
                                }

                            case 0x10:
                                {
                                    samplecode.Type = "NOP";
                                    break;
                                }
                            default:
                                {
                                    samplecode.Type = "0x" + samplecode.priority.ToString("X2");
                                    break;
                                }
                        }

                        samplecode.offset2 = tmp[samplecode.offset1ROM + 1];
                        samplecode.offset2CPU = (int)(base2 + (2 * samplecode.offset2)); //6255 + 2 x (byte2)
                        samplecode.offset2ROM = (int)(origstart + samplecode.offset2CPU - baseadd); //2255 + 2 x (byte2)
                    }
                    else
                    {
                        samplecode.priority = 0;
                        samplecode.offset2 = 0;
                        samplecode.offset2CPU = 0;
                        samplecode.offset2ROM = 0;
                    }

                    if (synth == false)
                    {
                        if (((samplecode.offset2ROM + 2) > tmp.Length) || samplecode.offset2ROM < 0)
                        {
                            samplecode.Type = "Synth (Off2)";
                            synth = true;
                        }
                    }
                    if (synth == false)
                    {
                        samplecode.offset3CPU = (ushort)getShort(tmp, samplecode.offset2ROM); //short address
                        samplecode.offset3ROM = origstart + samplecode.offset3CPU - baseadd; //short address
                    }
                    else
                    {
                        samplecode.offset3CPU = 0;
                        samplecode.offset3ROM = 0;
                    }

                    if (synth == false)
                    {
                        if (((samplecode.offset3ROM + 2) > tmp.Length) || samplecode.offset3ROM < 0)
                        {
                            samplecode.Type = "Synth (Off3)";
                            synth = true;
                        }
                    }

                    if (synth == false && samplecode.Type == "Sample")
                    {
                        bool vterminated = false;
                        int offset = samplecode.offset3ROM;
                        while (vterminated == false)
                        {
                            byte tmpvoice = tmp[offset];
                            offset++;
                            byte internalid = tmp[offset];
                            offset++;

                            if (internalid == 0xff)
                            {
                                vterminated = true;
                                if (tmpvoice != 0x00)
                                {
                                    samplecode.internalid = tmpvoice;
                                    Samples7a.Add(samplecode);
                                    String7A += samplecode.ToStringOutSam();
                                    Sample loader = SamplesINT.ElementAt(samplecode.internalid);
                                    String7A += "," + loader.ToStringOutSam();
                                }
                            }
                            else
                            {
                                if (tmpvoice != 0x00)
                                {
                                    samplecode.voice = tmpvoice;
                                }
                                samplecode.internalid = internalid;
                                Samples7a.Add(samplecode);
                                String7A += samplecode.ToStringOutSam();
                                Sample loader = SamplesINT.ElementAt(samplecode.internalid);
                                String7A += "," + loader.ToStringOutSam();
                            }
                        }
                    }
                    else
                    {
                        samplecode.voice = 0;
                        samplecode.internalid = 0;
                        Samples7a.Add(samplecode);
                        String7A += samplecode.ToStringOut();
                    }

                }


                for (int ib = 0; ib < 0x100; ib++)
                {
                    bool synth = false;
                    SampleCode samplecode = new SampleCode();
                    samplecode.ID = ib.ToString("X2");
                    samplecode.Type = "Sample";
                    samplecode.offset1CPU = (int)(start_00 + (2 * ib)); //5d44 + 2 x id
                    samplecode.offset1ROM = (int)(origstart + samplecode.offset1CPU - baseadd); //1f44 + 2 x id

                    if (((samplecode.offset1ROM + 1) > tmp.Length) || samplecode.offset1ROM < 0)
                    {
                        samplecode.Type = "Synth (Off1)";
                        synth = true;
                    }

                    if (synth == false)
                    {
                        samplecode.priority = tmp[samplecode.offset1ROM];
                        switch (samplecode.priority)
                        {
                            case 0x01:
                                {
                                    samplecode.Type = "Music";
                                    break;
                                }

                            case 0x04:
                                {
                                    samplecode.Type = "SFX";
                                    break;
                                }

                            case 0x07:
                                {
                                    samplecode.Type = "Sample";
                                    break;
                                }

                            case 0x0F:
                                {
                                    samplecode.Type = "Code";
                                    break;
                                }
                            case 0x10:
                                {
                                    samplecode.Type = "NOP";
                                    break;
                                }

                            default:
                                {
                                    samplecode.Type = "0x" + samplecode.priority.ToString("X2");
                                    break;
                                }
                        }
                        samplecode.offset2 = tmp[samplecode.offset1ROM + 1];
                        samplecode.offset2CPU = (int)(base2 + (2 * samplecode.offset2)); //6255 + 2 x (byte2)
                        samplecode.offset2ROM = (int)(origstart + samplecode.offset2CPU - baseadd); //2255 + 2 x (byte2)
                    }
                    else
                    {
                        samplecode.priority = 0;
                        samplecode.offset2 = 0;
                        samplecode.offset2CPU = 0;
                        samplecode.offset2ROM = 0;
                    }

                    if (synth == false)
                    {
                        if (((samplecode.offset2ROM + 2) > tmp.Length) || samplecode.offset2ROM < 0)
                        {
                            samplecode.Type = "Synth (Off2)";
                            synth = true;
                        }
                    }
                    if (synth == false)
                    {
                        samplecode.offset3CPU = (int)getShort(tmp, samplecode.offset2ROM); //short address
                        samplecode.offset3ROM = origstart + samplecode.offset3CPU - baseadd; //short address
                    }
                    else
                    {
                        samplecode.offset3CPU = 0;
                        samplecode.offset3ROM = 0;
                    }

                    if (synth == false)
                    {
                        if (((samplecode.offset3ROM + 2) > tmp.Length) || samplecode.offset3ROM < 0)
                        {
                            samplecode.Type = "Synth (Off3)";
                            synth = true;
                        }
                    }


                    if (synth == false && samplecode.Type == "Sample")
                    {
                        bool vterminated = false;
                        int offset = samplecode.offset3ROM;
                        while (vterminated == false)
                        {
                            byte tmpvoice = tmp[offset];
                            offset++;
                            byte internalid = tmp[offset];
                            offset++;
                            if (internalid == 0xff)
                            {
                                vterminated = true;
                                if (tmpvoice != 0x00)
                                {
                                    samplecode.internalid = tmpvoice;
                                    Samples00.Add(samplecode);
                                    String00 += samplecode.ToStringOutSam();
                                    Sample loader = SamplesINT.ElementAt(samplecode.internalid);
                                    String00 += "," + loader.ToStringOutSam();
                                }
                            }
                            else
                            {
                                if (tmpvoice != 0x00)
                                {
                                    samplecode.voice = tmpvoice;
                                }
                                samplecode.internalid = internalid;
                                Samples00.Add(samplecode);
                                String00 += samplecode.ToStringOutSam();
                                Sample loader = SamplesINT.ElementAt(samplecode.internalid);
                                String00 += "," + loader.ToStringOutSam();
                            }
                        }
                    }
                    else
                    {
                        samplecode.voice = 0;
                        samplecode.internalid = 0;
                        Samples00.Add(samplecode);
                        String00 += samplecode.ToStringOut();
                    }


                }

            }


            TextBoxSamp.Text = StringSamp;
            TextBox7A.Text = String7A;
            TextBox00.Text = String00;
        }

        private ushort getShort(byte[] tmp, int v)
        {
            ushort ret = 0;
            ret += (ushort) (tmp[v] << 8);
            ret += (ushort) tmp[v+1];
            return ret;
        }


        private void CSV7A_Click(object sender, EventArgs e)
        {
            SaveFileDialog SF = new SaveFileDialog
            {
                Title = "Save 7A File",
                InitialDirectory = ExportDir
            };
            if (SF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                ExportDir = System.IO.Path.GetDirectoryName(SF.FileName);
                File.WriteAllText(SF.FileName, String7A);
            }
        }

        private void CSV00_Click(object sender, EventArgs e)
        {
            SaveFileDialog SF = new SaveFileDialog
            {
                Title = "Save 00 File",
                InitialDirectory = ExportDir
            };
            if (SF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                ExportDir = System.IO.Path.GetDirectoryName(SF.FileName);
                File.WriteAllText(SF.FileName, String00);
            }
        }

        private void SampleCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog SF = new SaveFileDialog
            {
                Title = "Save Sample File",
                InitialDirectory = ExportDir
            };
            if (SF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                ExportDir = System.IO.Path.GetDirectoryName(SF.FileName);
                File.WriteAllText(SF.FileName, StringSamp);
            }
        }
    }
}
