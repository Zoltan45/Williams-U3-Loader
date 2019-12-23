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
        string String7A = "CMD,Type,Offset1 Address(CPU),Offset1 Address(ROM),Type Code,Offset2,Offset2 Address(CPU),Offset2 Address(ROM),Voice,Volume?,Offset3 Address(CPU),Offset3 Address(ROM),Offset4 Address(CPU),Offset4 Address(ROM),Offset5 Address(CPU),Offset5 Address(ROM),Bank,Pad?,Sample\n";
        string String00 = "CMD,Type,Offset1 Address(CPU),Offset1 Address(ROM),Type Code,Offset2,Offset2 Address(CPU),Offset2 Address(ROM),Voice,Volume?,Offset3 Address(CPU),Offset3 Address(ROM),Offset4 Address(CPU),Offset4 Address(ROM),Offset5 Address(CPU),Offset5 Address(ROM),Bank,Pad?,Sample\n";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Samples7a = new List<SampleCode>();
            Samples00 = new List<SampleCode>();
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
                for (int i=1; i < 3; i++) //find bank 3
                {
                    start += 0x8000;
                    if (tmp.Length <= start)
                    {
                        start = 0;
                    }
                }
                int origstart = start;

                ushort start_7A = getShort(tmp,origstart + 0x19);
                ushort start_00 = getShort(tmp, origstart + 0x0f);

                ushort base2 = getShort(tmp,origstart + 0x0b);
                ushort base3 = getShort(tmp,origstart + 0x15);

                for (int i=0; i <0x100; i++)
                {
                    bool synth = false;
                    SampleCode samplecode = new SampleCode();
                    samplecode.ID = i.ToString("X");
                    samplecode.Type = "Sample";
                    samplecode.offset1CPU = (int)(start_7A + (2 * i)); //5f44 + 2 x id
                    samplecode.offset1ROM = (int)(origstart +samplecode.offset1CPU - baseadd); //1f44 + 2 x id

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

                            default:
                                {
                                    samplecode.Type = "0x"+ samplecode.priority.ToString("X2");
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
                        if ( ((samplecode.offset3ROM+2) > tmp.Length) || samplecode.offset3ROM < 0)
                        {
                            samplecode.Type = "Synth (Off3)";
                            synth = true;
                        }
                    }

                    if (synth == false)
                    {
                        samplecode.voice = tmp[samplecode.offset3ROM];
                        samplecode.offset3A = tmp[samplecode.offset3ROM + 1];
                        samplecode.volume = tmp[samplecode.offset3ROM + 2];
                        samplecode.offset4CPU = (int)(base3 + (2 * samplecode.offset3A)); //684F + 2 x (off3a)
                        samplecode.offset4ROM = (int)(origstart + samplecode.offset4CPU - baseadd); //684F + 2 x (off3a)
                    }
                    else
                    {
                        samplecode.voice = 0;
                        samplecode.offset3A = 0;
                        samplecode.volume = 0;
                        samplecode.offset4CPU = 0;
                        samplecode.offset4ROM = 0;
                    }

                    if (synth == false)
                    {
                        if (((samplecode.offset4ROM + 2) > tmp.Length) || samplecode.offset4ROM < 0)
                        {
                            samplecode.Type = "Synth (Off4)";
                            synth = true;
                        }
                    }

                    if (synth == false)
                    {
                        samplecode.offset5CPU = (ushort) getShort(tmp,samplecode.offset4ROM);
                        samplecode.offset5ROM = origstart + samplecode.offset5CPU - baseadd;

                        samplecode.bank = tmp[samplecode.offset5ROM];
                        samplecode.pad = tmp[samplecode.offset5ROM + 1];
                        samplecode.sample = tmp[samplecode.offset5ROM + 2];
                    }
                    else
                    {
                        samplecode.offset5CPU = 0;
                        samplecode.offset5ROM = 0;
                        samplecode.bank = 0;
                        samplecode.pad = 0;
                        samplecode.sample = 0;
                    }
                    Samples7a.Add(samplecode);
                    String7A += samplecode.ToStringOut();
                }


                for (int i = 0; i < 0x100; i++)
                {
                    bool synth = false;
                    SampleCode samplecode = new SampleCode();
                    samplecode.ID = i.ToString("X2");
                    samplecode.Type = "Sample";
                    samplecode.offset1CPU = (int)(start_00 + (2 * i)); //5d44 + 2 x id
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

                    if (synth == false)
                    {
                        samplecode.voice = tmp[samplecode.offset3ROM];
                        samplecode.offset3A = tmp[samplecode.offset3ROM + 1];
                        samplecode.volume = tmp[samplecode.offset3ROM + 2];
                        samplecode.offset4CPU = (int)(base3 + (2 * samplecode.offset3A)); //684F + 2 x (off3a)
                        samplecode.offset4ROM = (int)(origstart + samplecode.offset4CPU - baseadd); //684F + 2 x (off3a)
                    }
                    else
                    {
                        samplecode.voice = 0;
                        samplecode.offset3A = 0;
                        samplecode.volume = 0;
                        samplecode.offset4CPU = 0;
                        samplecode.offset4ROM = 0;
                    }

                    if (synth == false)
                    {
                        if (((samplecode.offset4ROM + 2) > tmp.Length) || samplecode.offset4ROM < 0)
                        {
                            samplecode.Type = "Synth (Off4)";
                            synth = true;
                        }
                    }

                    if (synth == false)
                    {
                        samplecode.offset5CPU = (ushort)getShort(tmp, samplecode.offset4ROM);
                        samplecode.offset5ROM = origstart + samplecode.offset5CPU - baseadd;

                        samplecode.bank = tmp[samplecode.offset5ROM];
                        samplecode.pad = tmp[samplecode.offset5ROM + 1];
                        samplecode.sample = tmp[samplecode.offset5ROM + 2];
                    }
                    else
                    {
                        samplecode.offset5CPU = 0;
                        samplecode.offset5ROM = 0;
                        samplecode.bank = 0;
                        samplecode.pad = 0;
                        samplecode.sample = 0;
                    }
                    Samples00.Add(samplecode);
                    String00 += samplecode.ToStringOut();
                }


            }


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
    }
}
