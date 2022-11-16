using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1 : Form
    {
        Bitmap ogPal;
        Bitmap editedImage;
        Color selected;
        List<Color> currentPalette;
        int size = 15;
        bool isDrawing;
        int penX,
            penY;
        Pen p;
        Bitmap copy;
        bool isPen, isDrawLine, isFloodFill, isReplace, isReplaceAll;
        Dictionary<Color, int> indexByColor;
        Color colorToReplace;
        private void TileArtInit()
        {
            pictureBox_tilePalette.Image = DrawImagePalette();

            //pictureBox_tilePalette_MouseDown(0, mouse);

            StartTile();
            ShowGrid(EnlargeImage(editedImage));
            Cursor = Cursors.Default;

            var temp = currentPalette.IndexOf(selected);
            if (temp == -1)
            {
                temp = 0;
            }
            labelIndex.Text = $"Index: {temp.ToString("X")}";
        }
        private Bitmap DrawImagePalette()
        {
            currentPalette = rom.ReadPalette(rom.palettePointers[palKey], 1, 15)[0];
            Bitmap currentPaletteDisplay = GetPalette(currentPalette.ToArray(), 16, 20);
            // Store to use in selection
            ogPal = new Bitmap(currentPaletteDisplay);
            return currentPaletteDisplay;

        }
        private void pictureBox_tilePalette_MouseDown(object sender, MouseEventArgs e)
        {
            // Apply the original
            // New bitmap as careful to leave our original untouched
            pictureBox_tilePalette.Image = new Bitmap(ogPal);

            selected = ((Bitmap)pictureBox_tilePalette.Image).GetPixel(e.X, e.Y);

            // Draw on the picture
            using (Graphics g = Graphics.FromImage(pictureBox_tilePalette.Image))
            {
                int x = e.X / 20;
                g.DrawRectangle(new Pen(Color.Black, 2f), x * 20, 0, 20, 20);
            }
        }
        private void StartTile()
        {
            // To precent future typing
            var thisTile = rom.tiles[listBox_i_tiles.SelectedIndex];
            Bitmap bmp;
            // Are we looking at a 2x2?
            if (thisTile.type == "2x2")
            {
                // 20, 29 - location
                pictureBox_tilePlayground.Size = new Size(size * 16, size * 16);
                pictureBox_tilePlayground.Location = new Point(800 / size, 815 / size);

                bmp = new Bitmap(16, 16);
            }
            else
            {
                pictureBox_tilePlayground.Size = new Size(size * 8, size * 8);
                pictureBox_tilePlayground.Location = new Point(1600 / size, 1630 / size);

                bmp = new Bitmap(8, 8);
            }
            Bitmap TL = GetTile(thisTile.TLtile), TR, BL, BR;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(TL, 0, 0);
                if (thisTile.type == "2x2")
                {
                    TR = GetTile(thisTile.TRtile);
                    BL = GetTile(thisTile.BLtile);
                    BR = GetTile(thisTile.BRtile);

                    g.DrawImage(TR, 8, 0);
                    g.DrawImage(BL, 0, 8);
                    g.DrawImage(BR, 8, 8);
                }
            }
            editedImage = new Bitmap(bmp);

            pictureBox_tilePlayground.Image = EnlargeImage(bmp);
            //pictureBox_tilePlayground.Image = bmp;




        }
        public Bitmap GetTile(byte[] array)
        {
            Bitmap bmp = new Bitmap(8, 8);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(rom.DecodeChar(array, 4, currentPalette), 0, 0);
            }

            return bmp;
        }
        private Bitmap EnlargeImage(Bitmap toEnlarge)
        {
            Bitmap @return = new Bitmap(size * toEnlarge.Width, size * toEnlarge.Height);

            using (Graphics g = Graphics.FromImage(@return))
            {
                for (int height = 0; height < toEnlarge.Height; height++)
                {
                    for (int width = 0; width < toEnlarge.Width; width++)
                    {
                        Color thisColor = toEnlarge.GetPixel(width, height);
                        g.FillRectangle(new SolidBrush(thisColor), width * size, height * size, size, size);
                    }
                }
            }


            return @return;
        }
        private void checkBox_grid_CheckedChanged(object sender, EventArgs e)
        {
            ShowGrid(EnlargeImage(editedImage));
        }
        private void ShowGrid(Bitmap bmp)
        {
            if (checkBox_grid.Checked)
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < pictureBox_tilePlayground.Height; i += size)
                    {
                        g.DrawLine(new Pen(Color.Black), 0, i, pictureBox_tilePlayground.Width, i);
                        g.DrawLine(new Pen(Color.Black), i, 0, i, pictureBox_tilePlayground.Height);
                    }
                    if (rom.tiles[listBox_i_tiles.SelectedIndex].type == "2x2")
                    {
                        g.DrawLine(new Pen(Color.Red), 8 * size, 0, 8 * size, 16 * size);
                        g.DrawLine(new Pen(Color.Red), 0, 8 * size, 16 * size, 8 * size);
                    }
                }
                pictureBox_tilePlayground.Image = bmp;
            }
            else
            {
                pictureBox_tilePlayground.Image = bmp;
                //EnlargeImage(sandbox);
            }

        }
        private void button_pen_Click(object sender, EventArgs e)
        {
            var send = (Button)sender;
            pictureBox_currentMode.Image = send.Image;
            isPen = false;
            isDrawLine = false;
            isFloodFill = false;
            isReplace = false;
            isReplaceAll = false;

            switch (send.Name)
            {
                case "button_pen":
                    isPen = true;
                    break;
                case "button_drawLine":
                    isDrawLine = true;
                    break;
                case "button_fill":
                    isFloodFill = true;
                    break;
                case "button_replaceAll":
                    isReplace = true;
                    MessageBox.Show("Select a color in the image to replace with the selected color");
                    break;
                case "button_replaceGlobal":
                    isReplaceAll = true;
                    MessageBox.Show("Select a color in the image to replace with the selected color");
                    break;

                    break;
                default:
                    break;
            }
        }

        // Keep the tile(s) stored in editedImage, their original 8x8 or 16x16 image. 
        // Enlarge when you want to display
        private void pictureBox_tilePlayground_MouseDown(object sender, MouseEventArgs e)
        {
            // If not drawing
            if (!isPen && !isDrawLine && !isFloodFill && !isReplace && !isReplaceAll)
            {
                var selected = ((Bitmap)pictureBox_tilePlayground.Image).GetPixel(e.X, e.Y);
                var temp = currentPalette.IndexOf(selected);
                if (temp == -1)
                { 
                    temp = 0;
                }
                labelIndex.Text = $"Index: {temp.ToString("X")}";


                return;
            }

            // Duplicate to use
            copy = new Bitmap(editedImage);
            penX = e.X / size;
            penY = e.Y / size;
            isDrawing = true;
            // Set first tile for instant feedback
            if (isPen)
            {
                int x = e.X / size,
                    y = e.Y / size;
                editedImage.SetPixel(x, y, selected);

                ShowGrid(EnlargeImage(editedImage));
            }
            if (isFloodFill)
            {
                int x = e.X / size,
                    y = e.Y / size;

                FloodFill(editedImage, x, y);

                ShowGrid(EnlargeImage(editedImage));
            }
            if (isReplace)
            {
                int x = e.X / size,
                    y = e.Y / size;
                Color toReplace = editedImage.GetPixel(x, y);

                // Loop through entire image
                for (int h = 0; h < editedImage.Height; h++)
                {
                    for (int w = 0; w < editedImage.Width; w++)
                    {
                        if (toReplace == editedImage.GetPixel(w, h))
                        {
                            editedImage.SetPixel(w, h, selected);
                        }
                    }
                }


                ShowGrid(EnlargeImage(editedImage));

            }
            if (isReplaceAll)
            {
                int x = e.X / size,
                    y = e.Y / size;
                Color toReplace = editedImage.GetPixel(x, y);
                ReplaceAll(editedImage, toReplace);
                button_applyTiles_Click(0, new EventArgs());

                for (int i = 0; i < listBox_i_tiles.Items.Count; i++)
                {
                    listBox_i_tiles.SelectedIndex = i;
                    ReplaceAll(editedImage, toReplace);
                    button_applyTiles_Click(0, new EventArgs());

                }
                listBox_i_tiles.SelectedIndex = 0;



                ShowGrid(EnlargeImage(editedImage));

            }
        }

        private Bitmap ReplaceAll(Bitmap bmp, Color clr)
        {
            // Loop through entire image
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++)
                {
                    if (clr == bmp.GetPixel(w, h))
                    {
                        bmp.SetPixel(w, h, selected);
                    }
                }
            }



            return bmp;
        } 
        private void pictureBox_tilePlayground_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                if (isPen)
                {
                    int x = e.X / size,
                        y = e.Y / size;
                    try
                    {
                        editedImage.SetPixel(x, y, selected);
                    }
                    catch (Exception ex)
                    {

                    }

                } else if (isDrawLine)
                {
                    // Restore copy on each preview
                    editedImage = new Bitmap(copy);

                    using (Graphics g = Graphics.FromImage(editedImage))
                    {
                        int x = e.X / size,
                            y = e.Y / size;
                        g.DrawLine(new Pen(selected), penX, penY, x, y);
                    }

                }

                ShowGrid(EnlargeImage(editedImage));

            }
        }

        private void pictureBox_tilePlayground_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;

        }
        private struct FillCoords 
        {
            public int x, y;
            public FillCoords(int x, int y) 
            {
                this.x = x;
                this.y = y;               
            }
        }
        private int FindIndex(int x, int y, int size)
        {
            return x * (size - 1) + y;
        }
        public struct Direction
        {
            public int x, y;
            public Direction (int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        private Bitmap FloodFill (Bitmap bmp, int x, int y)
        {
            bool[] visited = new bool[bmp.Width * bmp.Height];
            Color colorToFill = bmp.GetPixel(x, y);
            Queue<FillCoords> fillQueue = new Queue<FillCoords>();
            // Push current to queue
            fillQueue.Enqueue(new FillCoords(x, y));
            // Four orthogonal directions
            // Ordered like this to simulate going around the coordinate plane
            Direction[] directions = new Direction[]
            {
                new Direction(1, 0),
                new Direction(0, 1),
                new Direction(- 1, 0),
                new Direction(0, -1)
            };


            while (fillQueue.Count > 0)
            {
                // Shift queue
                var temp = fillQueue.Dequeue();
                x = temp.x;
                y = temp.y;

                if (colorToFill == bmp.GetPixel(x, y))
                {
                    // 'Selected' is a Color var of class scope
                    // That holds our currently selected color
                    bmp.SetPixel(x, y, selected);

                    // Loop through the directions
                    foreach (var direction in directions)
                    {
                        int nextX = x + direction.x,
                            nextY = y + direction.y;
                        int index = FindIndex(nextX, nextY, bmp.Width);
                        // Is our next index valid?
                        // Have we been here?
                        if (IsIndexInBitmap(nextX, nextY, bmp) && !visited[index])
                        {
                            // Is next color valid?
                            Color nextColor = bmp.GetPixel(nextX, nextY);
                            if (nextColor == colorToFill)
                            {
                                visited[index] = true;
                                fillQueue.Enqueue(new FillCoords(nextX, nextY));
                            }
                        }
                    }

                }

            }
            return bmp;
        }
        private bool IsIndexInBitmap (int indexX, int indexY, Bitmap bmp)
        {
            if (indexX < 0 || indexX >= bmp.Width || indexY < 0 || indexY >= bmp.Height)
            {
                return false;
            }
            return true;            
        }
        private void pictureBox_tilePlayground_MouseEnter(object sender, EventArgs e)
        {
            if (isDrawLine || isFloodFill || isPen)
            {
                Cursor = Cursors.Cross;
            }
        }

        private void pictureBox_tilePlayground_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox_currentMode.Image = null;
            isPen = false;
            isDrawLine = false;
            isFloodFill = false;
            isReplace = false;
            isReplaceAll = false;

        }

        private void trackBar_size_Scroll(object sender, EventArgs e)
        {
            size = trackBar_size.Value;
            TileInit();
            TileArtInit();

        }
        private int FindOffset (int address)
        {
            return address - rom.baseAddress;
        }
        private void button_applyTiles_Click(object sender, EventArgs e)
        {

            var tile = rom.tiles[listBox_i_tiles.SelectedIndex];

            Bitmap temp = new Bitmap(8, 8);
            using (Graphics g = Graphics.FromImage(temp))
            {
                g.DrawImage(editedImage, new Rectangle(0, 0, 8, 8), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
            }
            rom.@char = ConvertImageToBitplane(temp, rom.@char, FindOffset(tile.tileAddresses[0]));

            if (tile.type == "2x2")
            {
                temp = new Bitmap(8, 8);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.DrawImage(editedImage, new Rectangle(0, 0, 8, 8), new Rectangle(8, 0, 8, 8), GraphicsUnit.Pixel);
                }
                rom.@char = ConvertImageToBitplane(temp, rom.@char, FindOffset(tile.tileAddresses[1]));

                temp = new Bitmap(8, 8);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.DrawImage(editedImage, new Rectangle(0, 0, 8, 8), new Rectangle(0, 8, 8, 8), GraphicsUnit.Pixel);
                }
                rom.@char = ConvertImageToBitplane(temp, rom.@char, FindOffset(tile.tileAddresses[2]));

                temp = new Bitmap(8, 8);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.DrawImage(editedImage, new Rectangle(0, 0, 8, 8), new Rectangle(8, 8, 8, 8), GraphicsUnit.Pixel);
                }
                rom.@char = ConvertImageToBitplane(temp, rom.@char, FindOffset(tile.tileAddresses[3]));

            }


            rom.ReadFromSpriteHeader(0, currentPalette, true);
            RedrawPreview();
            
            //TileInit();
            //TileArtInit();
        
        }

        private void button_i_writeImage_Click(object sender, EventArgs e)
        {
            for (int i = 0, address = rom.baseAddress; i < rom.@char.Length; )
            {
                rom.Write8(ref address, rom.@char[i++]);
            }

        }
        private byte[] ConvertImageToBitplane(Bitmap bmp, byte[] imageInfo, int address, int bpp = 4)
        {
            // Since we use a 0-indexed array

            int bits = 1;
            // Loop through bpp
            for (int i = 0; i < (bpp >> 1); i++)
            {
                //int addressOG = address;
                // Loop through bytes
                for (int j = 0; j < 0x10; j += 2)
                {
                    byte @byte = 0;
                    byte @byte2 = 0;
                    // Loop through pixels of that row
                    for (int k = 0; k < 8; k++)
                    {
                        Color thiscolor = bmp.GetPixel(k, (j >> 1));
                        thiscolor = SimplifiedColor(thiscolor, Color.FromArgb(255,248,248,248));
                        int index = GetIndexInPalette(thiscolor);
                        if (index == -1)
                        {
                            index = 0;
                        }
                        if ((index & bits) > 0)
                        {
                            @byte |= (byte)(1 << (7 - k));
                        }
                        if ((index & (bits << 1)) > 0)
                        {
                            @byte2 |= (byte)(1 << (7 - k));
                        }




                    }

                    imageInfo[address++] = @byte;
                    imageInfo[address++] = @byte2;
                }
                bits <<= 2;
                //address = addressOG;
            }


            return imageInfo;
        }
        private int GetIndexInPalette (Color color)
        {
            int index = currentPalette.IndexOf(color);
            return index;
        }

        private void button_replaceAll_Click(object sender, EventArgs e)
        {
            timer_replace.Enabled = true;
            MessageBox.Show("Select a color in the image to replace");
        }
        private void timer_replace_Tick(object sender, EventArgs e)
        {

        }

    }
}
