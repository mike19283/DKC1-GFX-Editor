using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1 : Form
    {
        #region SpritePalette - Palette
        private Bitmap GetPalette(Color[] palette, int width = 15, int size = 20)
        {
            Bitmap bmp = new Bitmap(size * width, size);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < palette.Length; i++)
                {
                    g.FillRectangle(new SolidBrush(palette[i]), (size * i), 0, size, size);
                }
            }

            return bmp;
        }

        private Color[] GetObjectPal(int offset)
        {
            return rom.ReadPalette(offset)[0].ToArray();
        }




        private void button_load_Click(object sender, EventArgs e)
        {
            palKey = comboBox_loadPalette.Text;
            int offset;
            if (!rom.palettePointers.ContainsKey(palKey))
            {
                offset = Convert.ToInt32(palKey, 16);
            }
            else
            {
                offset = rom.palettePointers[palKey];
            }
            
            palette = rom.ReadPalette(offset);
            var p = palette[0].Skip(1).Take(15);
            pictureBox_palEdit.Image = GetPalette(p.ToArray());

            // Clear others
            pictureBox_colorPreview.Image = null;
            textBox_p_snes.Clear();

            button_p_loadGFX_Click(sender, e);

            var mouse = new MouseEventArgs(MouseButtons, 1, 0, 0, 0);

            pictureBox_palEdit_MouseClick(sender, mouse);


        }

        private void button_writePal_Click(object sender, EventArgs e)
        {
            var p = palette[0].Skip(1).Take(15).ToList();


            rom.WritePaletteToROM(new List<List<Color>>() { p }, (Convert.ToInt32(textBox_addressPal.Text, 16) & 0x3fffff));
            button_p_loadGFX_Click(sender, e);
            button_load_Click(sender, e);        
        }

        private void button_previousPal_Click(object sender, EventArgs e)
        {
            int gotoNum = comboBox_loadPalette.SelectedIndex - 1 > 0 ? comboBox_loadPalette.SelectedIndex - 1 : 0;
            comboBox_loadPalette.SelectedIndex = gotoNum;
            button_load_Click(sender, e);

            button_p_loadGFX_Click(sender, e);
        }

        private void button_nextPal_Click(object sender, EventArgs e)
        {
            int top = comboBox_loadPalette.SelectedIndex;
            int items = comboBox_loadPalette.Items.Count;
            int gotoNum = top + 1 == items ? top : top + 1;
            comboBox_loadPalette.SelectedIndex = gotoNum;
            button_load_Click(sender, e);

            button_p_loadGFX_Click(sender, e);
        }


        private void pictureBox_palEdit_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X, y = e.Y;
            Color color = ((Bitmap)pictureBox_palEdit.Image).GetPixel(x, y);
            label_index.Text = (x / 20 + 1).ToString("X");

            pictureBox_colorPreview.Image = DrawColor(color);

            // Convert rgb24 into rgb15
            int r = color.R / 8;
            int g = color.G / 8;
            int b = color.B / 8;

            numericUpDown_r.Value = r;
            numericUpDown_g.Value = g;
            numericUpDown_b.Value = b;

            int snes = (r << 0) | (g << 5) | (b << 10);
            textBox_p_snes.Text = snes.ToString("X");

        }
        private Bitmap DrawColor(Color color)
        {
            Bitmap bmp = new Bitmap(100, 100);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(new SolidBrush(color), 0, 0, 100, 100);
            }

            return bmp;

        }

        private void ValueChanged(object sender, EventArgs e)
        {
            Color newColor = Color.FromArgb((int)(numericUpDown_r.Value * 8), (int)(numericUpDown_g.Value * 8), (int)(numericUpDown_b.Value * 8));

            pictureBox_colorPreview.Image = DrawColor(newColor);

            int snes = ((int)numericUpDown_r.Value << 0) | ((int)numericUpDown_g.Value << 5) | ((int)numericUpDown_b.Value << 10);
            textBox_p_snes.Text = snes.ToString("X");

        }

        private void button_apply_Click(object sender, EventArgs e)
        {
            Color toApply = ((Bitmap)pictureBox_colorPreview.Image).GetPixel(0, 0);

            // Apply at index - 1
            palette[0][Convert.ToInt32(label_index.Text, 16)] = toApply;
            var p = palette[0].Skip(1).Take(15);
            pictureBox_palEdit.Image = GetPalette(p.ToArray());
            button_p_loadGFX_Click(sender, e);
        }
        #endregion
        private void exportPalettebinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Temp array for obj palette
            var pal = new List<byte>();
            var picture = (Bitmap)pictureBox_palEdit.Image;

            for (int i = 0; i < pictureBox_palEdit.Width; i += 20)
            {
                var color = picture.GetPixel(i, 0);
                var snesNum = rom.ConvertColorToSNES(color);
                pal.Add((byte)snesNum);
                pal.Add((byte)(snesNum >> 8));
                
            }
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "BIN (*.bin)|*.bin";

            if (s.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(s.FileName, pal.ToArray());
                MessageBox.Show("Saved!");
            }
        }

        private void button_exportImage_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)pictureBox_p_ObjectImage.Image.Clone();
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Bitmap (*.bmp)|*.bmp";
            if (s.ShowDialog() == DialogResult.OK)
            {
                bmp.Save(s.FileName);
            }
        }


        private void importPalettebinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "BIN (*.bin)|*.bin";

            if (d.ShowDialog() == DialogResult.OK)
            {
                byte[] data = File.ReadAllBytes(d.FileName);

                int offset = rom.palettePointers[palKey];
                rom.WriteArr(offset, data);

            }
        }


    }
}
