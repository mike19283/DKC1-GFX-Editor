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

namespace StandAloneGFXDKC1
{
    public partial class CreateSpriteSheet : Form
    {
        ROM rom;
        int tileCount = 0;
        int charCount = 0;
        int _16x16 = 0;
        int _8x8 = 0;
        List<string> oaminfo;
        List<Color> palette;
        // Find start point end endpoint
        Point earliest = new Point((int)10e7, (int)10e7);
        Point latest = new Point(0, 0);
        public CreateSpriteSheet(int current, ROM rom, List<Color> palette)
        {
            InitializeComponent();
            richTextBox_spriteList.Text = current.ToString("X");
            richTextBox_spriteList.Text += '\n';
            this.rom = rom;
            this.palette = palette;
        }
        public void AddToList(int current)
        {
            richTextBox_spriteList.Text += current.ToString("X");
            richTextBox_spriteList.Text += '\n';
        }
        public void AddStartToList(int current)
        {
            richTextBox_spriteList.Text += current.ToString("X");
            richTextBox_spriteList.Text += '-';
        }


        private void button_extractExport_Click(object sender, EventArgs e)
        {
            oaminfo = new List<string>();
            Cursor.Current = Cursors.WaitCursor;
            // Parse textbox to find values
            List<int> allPointers = new List<int>();
            ParseTextbox();
            allPointers = (richTextBox_spriteList.Lines.Select(el => Convert.ToInt32(el, 16))).ToList();

            List<Bitmap> sprites = new List<Bitmap>();
            // Get a sprite for each pointer
            GetSprites(allPointers, sprites);

            // Find start point end endpoint
            earliest = new Point((int)10e7, (int)10e7);
            latest = new Point(0, 0);

            FindExtremes(sprites, ref earliest, ref latest);

            int padding = 30;
            Cursor.Current = Cursors.Default;
            int spriteWidth = latest.X - earliest.X + padding;
            int spriteHeight = latest.Y - earliest.Y + padding;
            int sheetWidth;
            int sheetHeight;
            int spritesPerRow = 15;
            sheetWidth = sprites.Count >= spritesPerRow ? spritesPerRow * spriteWidth : sprites.Count * spriteWidth;
            sheetHeight = spriteHeight + ((int)(sprites.Count / spritesPerRow) * spriteHeight);
            Bitmap bmp = new Bitmap(sheetWidth, sheetHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < sprites.Count; i++)
                {
                    Bitmap sprite = sprites[i];
                    int x = (i % spritesPerRow) * spriteWidth + padding / 2;
                    int y = (i / spritesPerRow) * spriteHeight + padding / 2;
                    Rectangle source = new Rectangle(earliest.X, earliest.Y, spriteWidth, spriteHeight);
                    Rectangle dest = new Rectangle(x, y, spriteWidth, spriteHeight);
                    g.DrawImage(sprite, dest, source, GraphicsUnit.Pixel);
                }
            }
            string[] temp = (string[])(allPointers.Select(el => el.ToString("X")).ToArray());
            string temps = String.Join(",", temp);

            // Construct string arr
            List<string> arr = new List<string>()
            {
                "TOTALS",
                temps,
                $"OAM tiles: {tileCount}",
                $"Char count: {charCount}",
                $"16x16 tile count: {_16x16}",
                $"8x8 tile count: {_8x8}",
                ""
            };
            arr.AddRange(oaminfo);
            SaveOAMSpriteSheet(arr.ToArray());
            SaveSpriteSheet(bmp);
        }
        private string[] ExportInfo ()
        {
            List<string> ExportSpriteInfo = new List<string>();

            

            return ExportSpriteInfo.ToArray();
        }

        private static void FindExtremes(List<Bitmap> sprites, ref Point earliest, ref Point latest)
        {
            foreach (var sprite in sprites)
            {
                // Find earliest x
                for (int x = 0; x < sprite.Width; x++)
                {
                    for (int y = 0; y < sprite.Height; y++)
                    {
                        Color clr = sprite.GetPixel(x, y);
                        if (clr != Color.FromArgb(0, 0, 0, 0))
                        {
                            if (x < earliest.X)
                            {
                                earliest.X = x;
                            }
                            goto LoopX;
                        }
                    }
                }
            LoopX:;
                // Find earliest y
                for (int y = 0; y < sprite.Height; y++)
                {
                    for (int x = 0; x < sprite.Width; x++)
                    {
                        Color clr = sprite.GetPixel(x, y);
                        if (clr != Color.FromArgb(0, 0, 0, 0))
                        {
                            if (y < earliest.Y)
                            {
                                earliest.Y = y;
                            }
                            goto LoopY;
                        }
                    }
                }
            LoopY:;
                // Find latest x
                for (int x = sprite.Width - 1; x >= 0; x--)
                {
                    for (int y = sprite.Height - 1; y >= 0; y--)
                    {
                        Color clr = sprite.GetPixel(x, y);
                        if (clr != Color.FromArgb(0, 0, 0, 0))
                        {
                            if (x > latest.X)
                            {
                                latest.X = x;
                            }
                            goto LoopEndX;
                        }
                    }
                }
            LoopEndX:;
                // Find latest y
                for (int y = sprite.Height - 1; y >= 0; y--)
                {
                    for (int x = sprite.Width - 1; x >= 0; x--)
                    {
                        Color clr = sprite.GetPixel(x, y);
                        if (clr != Color.FromArgb(0, 0, 0, 0))
                        {
                            if (y > latest.Y)
                            {
                                latest.Y = y;
                            }
                            goto LoopEndY;
                        }
                    }
                }
            LoopEndY:;
            }
        }

        private void GetSprites(List<int> allPointers, List<Bitmap> sprites)
        {
            int spritesPerRow = 15;
            tileCount = 0; charCount = 0;
            _16x16 = 0; _8x8 = 0;
            foreach (var pointer in allPointers)
            {
                Bitmap sprite = rom.ReadFromSpriteHeader(AddressFromIndex(pointer), palette);
                sprites.Add(sprite);
                var tiles = rom.tiles;                
                tileCount += tiles.Count;
                charCount = GetCharCount();

                int index = allPointers.IndexOf(pointer);
                int coordsX = index % spritesPerRow + 1;
                int coordsY = index / spritesPerRow + 1;
                int s16x16 = 0, s8x8 = 0, schars = 0;
                foreach (var t in tiles)
                {
                    if (t.type == "2x2")
                    {
                        schars += 4;
                        s16x16++;
                    }
                    else
                    {
                        schars++;
                        s8x8++;
                    }
                }
                oaminfo.Add(pointer.ToString("X") + $" ({coordsX},{coordsY})");
                oaminfo.Add($"OAM tiles: {tiles.Count}");
                oaminfo.Add($"Char tiles count: {schars}");
                oaminfo.Add($"Tile 16x16: {s16x16}");
                oaminfo.Add($"Tile 8x8: {s8x8}");
                oaminfo.Add("");
            }
            var w = tileCount;
            var x = charCount;
            var y = _16x16;
            var z = _8x8;
        }

        private int GetCharCount()
        {
            for (int i = 0; i < rom.tiles.Count; i++)
            {
                var tile = rom.tiles[i];
                if (tile.type == "2x2")
                {
                    charCount += 4;
                    _16x16++;
                } else
                {
                    charCount++;
                    _8x8++;
                }

            }
            return charCount;
        }

        private void ParseTextbox()
        {
            string[] linesSource = richTextBox_spriteList.Lines;
            List<string> linesMod = new List<string>();
            for (int i = 0; i < linesSource.Length; i++)
            {
                var line = linesSource[i];
                if (line.Contains('-'))
                {
                    var split = line.Split('-');
                    split[0] = split[0].Trim();
                    split[1] = split[1].Trim();
                    // [start-end]
                    int start = Convert.ToInt32(split[0], 16);
                    int end = Convert.ToInt32(split[1], 16);
                    for (int j = start; j <= end; j += 4)
                    {
                        linesMod.Add(j.ToString("X"));
                    }
                }
                else if (line == "")
                {
                    continue;
                }
                else
                {
                    linesMod.Add(line);
                }
            }
            richTextBox_spriteList.Lines = linesMod.ToArray();
        }

        private static void SaveSpriteSheet(Bitmap bmp)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "BMP (*.bmp)|*.bmp";
            if (s.ShowDialog() == DialogResult.OK)
            {
                bmp.Save(s.FileName);
            }
        }
        private static void SaveOAMSpriteSheet(string[] arr)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Text (*.txt)|*.txt";
            if (s.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(s.FileName, arr);
            }
        }
        private int AddressFromIndex(int current)
        {
            int address = 0;

            // Which index are we looking at?
            int index = Form1.gfxArray + current;

            // Address of index
            address = (rom.Read8(index + 2) << 16) | rom.Read16(index);

            return address;
        }

    }
}
