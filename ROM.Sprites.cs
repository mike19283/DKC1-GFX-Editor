using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class ROM
    {
        public struct CharsToRender
        {
            public int x, y, r, c, xAddress, yAddress;
            public string type;
            public CharsToRender(int x, int y, int r, int c, int xAddress, int yAddress, string type)
            {
                this.x = x;
                this.y = y;
                this.r = r;
                this.c = c;
                this.xAddress = xAddress;
                this.yAddress = yAddress;
                this.type = type;
            }
        }
        public List<Tiles> tiles = new List<Tiles>();
        public List<byte[]> charArray = new List<byte[]>();

        public class Tiles
        {
            public int x, y, xAddress, yAddress;
            public string type;
            public int[] tileAddresses;
            public byte[] TLtile;
            public byte[] TRtile;
            public byte[] BLtile;
            public byte[] BRtile;
            public Tiles(int x, int y, int xAddress, int yAddress, string type)
            {
                this.x = x;
                this.y = y;
                this.xAddress = xAddress;
                this.yAddress = yAddress;
                this.type = type;
            }
            public override string ToString()
            {
                return $"({x.ToString("X2")}, {y.ToString("X2")})";
            }
        }
        public List<List<VRAM>> _vram;
        public class VRAM
        {
            public int address;
            public byte[] @char;
            public int x;
            public int y;
            public int xAddress;
            public int yAddress;
            public string type;
            public VRAM(int address, byte[] @char)
            {
                this.address = address;
                this.@char = @char;
            }
        }
        public byte[] @char;
        public int baseAddress;
        public byte[] imageInfo;
        public int sizeOfNonTiles;

        public Bitmap ReadFromSpriteHeader(Int32 address, List<Color> palette, bool preview = false)
        {
            //FIXME
            //return null;

            // Save base for later
            baseAddress = address;

            int addressOG = address;
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);

            // Create a new array every time else bad things
            tiles = new List<Tiles>();
            // Read entire sprite
            if (!preview)
            {
                @char = ReadHeader(address);
            }
            // Set our index accordingly
            address = 0;

            imageInfo = @char.Take(8).ToArray();

            /*            Sprite Header
                        byte 0 is number of 2x2 chars
                        byte 1 is number of 1x1 chars in group 1
                        byte 2 is relative position of first 8x8 char of group 1
                        byte 3 is number of 1x1 chars in group 2
                        byte 4 is position of group 2
                        byte 5 is size to send to vram shifted 5 times (dma group 1)
                        byte 6 is where to place dma group 2 (0 if none)
                        byte 7 is number of chars in group dma group 2*/
            int byte0 = @char[address++];
            int byte1 = @char[address++];
            int byte2 = @char[address++];
            int byte3 = @char[address++];
            int byte4 = @char[address++];
            int byte5 = @char[address++];
            int byte6 = @char[address++];
            int byte7 = @char[address++];

            sizeOfNonTiles = (byte0 + byte1 + byte3) * 2 + 8;



            // Chars to render
            var cTR = new List<CharsToRender>();
            cTR.AddRange(Read2x2(@char, ref address, byte0, addressOG));
            cTR.AddRange(Read1x1(@char, ref address, byte1, byte2, addressOG, "Group 1"));
            cTR.AddRange(Read1x1(@char, ref address, byte3, byte4, addressOG, "Group 2"));

            // Loop through 2x2 tiles
            // Tile index
            int tileIndex = 0;
            for (int i = 0; byte0 - i != 0; i++)
            {
                int a0, a1, a2, a3;
                a0 = address + addressOG + 0;
                a1 = a0 + 0x20;
                // Do we have 8 or more 2x2's?
                if (byte0 >= 8 && i < 8)
                {
                    a2 = a0 + 0x200;
                    a3 = a1 + 0x200;
                }
                else
                {
                    // Find size of leftover row
                    int _2x2 = byte0 / 8;
                    int temp = byte0 - _2x2 * 8;
                    int sizeOfRow = (temp * 2 + byte1);

                    a2 = (sizeOfRow * 0x20) + a0;
                    a3 = a2 + 0x20;

                }
                int TLindex = a0 - addressOG;
                tiles[tileIndex].TLtile = @char.Skip(TLindex).Take(0x20).ToArray();
                int TRindex = a1 - addressOG;
                tiles[tileIndex].TRtile = @char.Skip(TRindex).Take(0x20).ToArray();

                int BLindex = a2 - addressOG;
                tiles[tileIndex].BLtile = @char.Skip(BLindex).Take(0x20).ToArray();
                int BRindex = a3 - addressOG;
                tiles[tileIndex].BRtile = @char.Skip(BRindex).Take(0x20).ToArray();


                int[] arr = new int[] { a0, a1, a2, a3 };
                tiles[tileIndex++].tileAddresses = arr;

                if ((i + 1) % 8 == 0)
                {
                    address += 0x200;
                }
                address += 0x40;
                // FIX?
                //address += 0x20;



            }
            // Loop through group1 tiles
            for (int i = 0; byte1 - i != 0; i++)
            {
                int a0;
                a0 = address + addressOG + 0;

                int TLindex = a0 - addressOG;
                tiles[tileIndex].TLtile = @char.Skip(TLindex).Take(0x20).ToArray();

                int[] arr = new int[] { a0 };
                tiles[tileIndex++].tileAddresses = arr;

                if ((i + 1) % 16 == 0)
                {
                    // FIX??
                    //address += 0x200;
                }
                address += 0x20;


            }


            // Loop through group2 tiles starting at proper address
            address = sizeOfNonTiles + (byte4 * 0x20);
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);


            // Loop through group2 tiles
            for (int i = 0; byte3 - i != 0; i++)
            {
                int a0;
                a0 = address + addressOG + 0;

                int TLindex = a0 - addressOG;
                tiles[tileIndex].TLtile = @char.Skip(TLindex).Take(0x20).ToArray();

                int[] arr = new int[] { a0 };
                tiles[tileIndex++].tileAddresses = arr;

                if ((i + 1) % 16 == 0)
                {
                    // FIX??
                    //address += 0x200;
                }
                address += 0x20;


            }

            address = sizeOfNonTiles;
            if (true)
            {
                // Emulate vram (with addresses)
                _vram = SetupVRAM(@char, address, addressOG, byte5, byte6, byte7);
            }



            Bitmap bmp = new Bitmap(256, 256);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.Clear(Color.Transparent);
                //gr.FillRectangle(new SolidBrush(Color.FromArgb(69, 42, 69)), 0, 0, 256, 256);


                //gr.DrawImage(DecodeChar(address, 4, palette), index.x, index.y);
                foreach (var index in cTR)
                {
                    var v = _vram[index.r][index.c];
                    var addr = v.@char;


                    int x = index.x, y = index.y;
                    v.xAddress = index.xAddress;
                    v.yAddress = index.yAddress;
                    v.x = x;
                    v.y = y;
                    v.type = index.type;
                    gr.DrawImage(DecodeChar(addr, 4, palette), x, y);

                }


            }
            return bmp;
        }
        private CharsToRender[] Read2x2(byte[] @char, ref int address, int size, int addressOG)
        {
            var cTR = new List<CharsToRender>();
            int r = 0, c = 0;

            // Are there more to place?
            while (--size >= 0)
            {
                int xAddress = address + addressOG;
                var x = @char[address++];
                int yAddress = address + addressOG;
                var y = @char[address++];
                // Add each to our 'sprites to render'
                var temp = new CharsToRender(x + 0, y + 0, r + 0, c + 0, xAddress, yAddress, "2x2");
                cTR.Add(temp);
                temp = new CharsToRender(x + 8, y + 0, r + 0, c + 1, xAddress, yAddress, "2x2");
                cTR.Add(temp);
                temp = new CharsToRender(x + 0, y + 8, r + 1, c + 0, xAddress, yAddress, "2x2");
                cTR.Add(temp);
                temp = new CharsToRender(x + 8, y + 8, r + 1, c + 1, xAddress, yAddress, "2x2");
                cTR.Add(temp);
                tiles.Add(new Tiles(x, y, xAddress, yAddress, "2x2"));
                // Increment column
                c += 2;
                // Did we reach boundary?
                if (c == 16)
                {
                    r += 2;
                    c = 0;
                }
            }
            return cTR.ToArray();
        }
        private CharsToRender[] Read1x1(byte[] @char, ref int address, int size, int start, int addressOG, string group)
        {
            var cTR = new List<CharsToRender>();
            int r = start >> 4, c = start & 0xf;

            // Are there more to place?
            while (--size >= 0)
            {
                int xAddress = address + addressOG;
                var x = @char[address++];
                int yAddress = address + addressOG;
                var y = @char[address++];
                // Add each to our 'sprites to render'
                var temp = new CharsToRender(x, y, r + 0, c + 0, xAddress, yAddress, "1x1 " + group);
                cTR.Add(temp);

                tiles.Add(new Tiles(x, y, xAddress, yAddress, "1x1 " + group));

                // Increment column
                c++;
                // Did we reach boundary?
                if (c == 16)
                {
                    r += 1;
                    c = 0;
                }
            }
            return cTR.ToArray();
        }
        // Returns 8x8 bitmap
        public Bitmap DecodeChar(byte[] @char, int bpp, List<Color> palette)
        {

            // Create image to return
            Bitmap bmp = new Bitmap(8, 8);

            // Rows of resulting image
            for (int i = 0, index = 0; i < 8; i++)
            {
                // Columns of resulting image
                for (int j = 0; j < 8; j++)
                {
                    if (j == 2)
                    {

                    }

                    int colorIndex = 0;

                    // Loop for bpp
                    for (int k = 0; k < bpp / 2; k++)
                    {
                        // >> To get the right bit, << to get the right bpp
                        var x = (((@char[index + k * 16] >> (7 - j)) & 1) << (k * 2));
                        var y = (((@char[index + 1 + k * 16] >> (7 - j)) & 1) << (k * 2 + 1));
                        colorIndex |= x | y;
                    }

                    bmp.SetPixel(j, i, palette[colorIndex]);
                }
                index += 2;

            }


            return bmp;
        }
        private Int32[] GetArrayOfBytes(Int32 address, int bpp)
        {
            List<int> temp = new List<int>();

            for (int i = 0; i < bpp / 2 * 16; i++)
            {
                temp.Add(Read8(address++));
            }

            return temp.ToArray();
        }
        public void PositionChars(int _16Num, int _8Num, int _8start, Graphics gr, int[] _16X, int[] _16Y, int[] _8X, int[] _8Y, List<List<int>> vram)
        {


        }
        private List<List<VRAM>> SetupVRAM(byte[] @char, int address, int addressOG, int endGroup1, int startGroup2, int countGroup2)
        {
            List<List<VRAM>> @return = new List<List<VRAM>>();
            int currentIndex = 0;
            // Fill group 1
            for (int i = 0; currentIndex < endGroup1; i++)
            {
                // Add new row to vram
                @return.Add(new List<VRAM>());
                // Loop through chars
                for (int j = 0; j < 16 && currentIndex++ < endGroup1; j++)
                {
                    @return[i].Add(new VRAM(address + addressOG, @char.Skip(address).Take(0x20).ToArray()));
                    address += 0x20;
                }
            }

            // Fill group 2
            while (@return.Count <= startGroup2 >> 4)
            {
                @return.Add(new List<VRAM>());
                @return.Add(new List<VRAM>());
            }
            for (int i = 0; i < countGroup2; i++)
            {
                @return[startGroup2 >> 4].Add(new VRAM(address + addressOG, @char.Skip(address).Take(0x20).ToArray()));
                address += 0x20;
            }


            return @return;

        }
        public byte[] ReadHeader(int offset)
        {
            int offsetOG = offset;

            // Read each byte in header
            // To display
            byte b0 = (byte)Read8(offset++);
            byte b1 = (byte)Read8(offset++);
            byte b2 = (byte)Read8(offset++);
            byte b3 = (byte)Read8(offset++);
            byte b4 = (byte)Read8(offset++);
            byte b5 = (byte)Read8(offset++);
            byte b6 = (byte)Read8(offset++);
            byte b7 = (byte)Read8(offset++);


            // Display size of data
            int size = 8 + (b0 * 2) + (b1 * 2) + (b3 * 2) + (b5 << 5) + (b7 * 0x20);

            return rom.Skip(offsetOG).Take(size).ToArray();
        }
        public List<byte[]> FillCharData(int address, int b0, int b1, int b3)
        {
            List<byte[]> @return = new List<byte[]>();
            if (b0 >= 8)
            {

            }
            int x = 0;

            return @return;
        }

    }
}
