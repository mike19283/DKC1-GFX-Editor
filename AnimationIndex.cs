using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandAloneGFXDKC1
{
    class AnimationIndex
    {
        public Bitmap bmp;
        ROM rom;
        public byte[] arr;
        public bool loop = true;
        public int time;
        public int address = 0;
        public bool referenced;
        public int calledAddress;
        public bool imgPresent = false;
        public int imgPointer = 0;
        public bool altAnimation;
        public int hitboxAddress = 0;
        //public Bitmap bmp2;
        public int imgPointer2 = 0;
        public List<Color> palette;


        public AnimationIndex(ROM rom)
        {
            this.rom = rom;
        }
        public override string ToString()
        {
            string str = "";
            foreach (var d in arr)
            {
                str += $"{d.ToString("X2")}, ";
            }
            str = str.Trim();
            str = str.Substring(0, str.Length - 1);
            if (referenced)
            {
                str += " *";
            }
            return str;
        }
        public void WriteIndexToROM (ref int address)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                rom.Write8(address++, arr[i]);
            }
        }
        public void ApplyStringToArr(string str)
        {
            try
            {
                str = str.Replace('*', ' ');
                var split = str.Split(',');
                List<byte> newArr = new List<byte>();

                for (int i = 0; i < split.Length; i++)
                {
                    str = split[i].Trim();
                    newArr.Add(Convert.ToByte(str, 16));

                }

                arr = newArr.ToArray();
                if (arr[0] >= 0x80)
                {
                    switch (arr[0])
                    {
                        case 0x80:
                        case 0x81:
                        case 0x82:
                        case 0x83:
                        case 0x84:
                        case 0x88:
                        case 0x8c:
                        case 0x8d:
                        case 0x8e:
                        case 0x8f:
                        case 0x90:
                            break;
                        default:
                            imgPointer = (arr[3] << 8) | (arr[2] << 0);
                            int offset = rom.Read24(Form1.gfxArray + imgPointer);
                            bmp = rom.ReadFromSpriteHeader(offset, palette, false);
                            break;
                    }
                }
                else
                {
                    imgPointer = (arr[2] << 8) | (arr[1] << 0);
                    int offset = rom.Read24(Form1.gfxArray + imgPointer);
                    bmp = rom.ReadFromSpriteHeader(offset, palette, false);

                }
            }
            catch { }

        }
        public void ReplaceImage (int pointer)
        {
            List<byte> replaced = new List<byte>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (imgPointer == 0)
                {
                    replaced.Add(arr[i]);
                }
                else
                {
                    if (i == 1 || i == 2)
                    {
                        replaced.Add((byte)(pointer >> 0));
                        replaced.Add((byte)(pointer >> 8));
                        i++;
                    } else
                    {
                        replaced.Add(arr[i]);
                    }
                }
            }
            arr = replaced.ToArray();
        }
        public void ReplaceGFXPointer (int replace)
        {
            arr[1] = (byte)(replace >> 0);
            arr[2] = (byte)(replace >> 8);
            int offset = rom.Read24(Form1.gfxArray + replace);
            bmp = rom.ReadFromSpriteHeader(offset, palette, false);

        }
    }
}
