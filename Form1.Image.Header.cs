using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1 : Form
    {
        private void comboBox_loadPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            palKey = comboBox_loadPalette.SelectedItem.ToString();

            // Write address
            textBox_addressPal.Text = (rom.palettePointers[palKey] | 0xb00000).ToString("X");
        }

        private void comboBox_i_palette_SelectedIndexChanged(object sender, EventArgs e)
        {
            palKey = comboBox_loadPalette.SelectedItem.ToString();

        }

        private void button_i_loadImage_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(textBox_i_image.Text, 16);
            if (index == 0x2320)
            {

            }

            int offset = rom.Read24(gfxArray + index);
            ReadHeader(offset);
            textBox_i_address.Text = offset.ToString("X6");

            _offset = offset;

            LoadHitbox(index / 2);

            Bitmap bmp = rom.ReadFromSpriteHeader(offset, palette[0]);
            if (checkBox_hitbox.Checked)
                bmp = DrawHitbox(bmp);
            pictureBox_i_preview.Image = bmp;
            listBox_i_tiles.Items.Clear();
            listBox_i_tiles.Items.AddRange(rom.tiles.ToArray());
            listBox_i_tiles.SelectedIndex = 0;

            //listBox_i_tiles_SelectedIndexChanged(sender, e);
            TileInit();
            TileArtInit();
        }

        private void ReadHeader(int index)
        {
            // Read each byte in header
            // To display
            byte b0 = (byte)rom.Read8(index++);
            byte b1 = (byte)rom.Read8(index++);
            byte b2 = (byte)rom.Read8(index++);
            byte b3 = (byte)rom.Read8(index++);
            byte b4 = (byte)rom.Read8(index++);
            byte b5 = (byte)rom.Read8(index++);
            byte b6 = (byte)rom.Read8(index++);
            byte b7 = (byte)rom.Read8(index++);

            // Allow to be edited
            textBox_iH0.Text = b0.ToString("X");
            textBox_iH_1.Text = b1.ToString("X");
            textBox_iH_2.Text = b2.ToString("X");
            textBox_iH_3.Text = b3.ToString("X");
            textBox_iH_4.Text = b4.ToString("X");
            textBox_iH_5.Text = b5.ToString("X");
            textBox_ih_6.Text = b6.ToString("X");
            textBox_iH_7.Text = b7.ToString("X");

            headerLength = 8 + (b0 * 2) + (b1 * 2) + (b3 * 2);
            // Display size of data
            int size = headerLength + (b5 << 5) + (b7 * 0x20);

            textBox_iH_total.Text = size.ToString("X");


        }

        private void button_i_loadPalette_Click(object sender, EventArgs e)
        {
            // Store to 'global'
            palKey = comboBox_i_palette.SelectedItem.ToString();

            // Load palette from rom
            palette = rom.ReadPalette(rom.palettePointers[palKey]);

            // Draw image again
            RedrawPreview();
        }

        private void button_i_prevImage_Click(object sender, EventArgs e)
        {
            int current = Convert.ToInt32(textBox_i_image.Text, 16);
            int previous = current - 4;
            // Do we go below our bounds?
            if (previous >= 0x8c)
            {
                textBox_i_image.Text = previous.ToString("X");
                // Draw image again
                button_i_loadImage_Click(sender, e);
            }
        }

        private void button_nextImage_Click(object sender, EventArgs e)
        {
            int current = Convert.ToInt32(textBox_i_image.Text, 16);
            int next = current + 4;
            // Is the next pointer 0?
            if (!(rom.Read16(gfxArray + next + 0) == 0 && rom.Read16(gfxArray + next + 1) == 0))
            {
                textBox_i_image.Text = next.ToString("X");
                // Draw image again
                button_i_loadImage_Click(sender, e);
            }
        }

        private void button_i_prevPal_Click(object sender, EventArgs e)
        {
            // Have we hit the bottom?
            int gotoNum = comboBox_i_palette.SelectedIndex - 1 > 0 ? comboBox_i_palette.SelectedIndex - 1 : 0;
            comboBox_i_palette.SelectedIndex = gotoNum;

            button_i_loadPalette_Click(sender, e);

        }

        private void button_i_nextPal_Click(object sender, EventArgs e)
        {
            // Have we hit the top?
            int top = comboBox_i_palette.SelectedIndex;
            int items = comboBox_i_palette.Items.Count;
            int gotoNum = top + 1 == items ? top : top + 1;
            comboBox_i_palette.SelectedIndex = gotoNum;

            button_i_loadPalette_Click(sender, e);
        }

        // Redraw but don't pull from rom again
        private void RedrawPreview()
        {
            pictureBox_tilePalette.Image = GetPalette(rom.ReadPalette(rom.palettePointers[palKey], 1, 16)[0].ToArray(), 16, 20);

            int index = Convert.ToInt32(textBox_i_image.Text, 16);

            int offset = rom.Read24(gfxArray + index);
            textBox_i_address.Text = offset.ToString("X6");

            Bitmap bmp = rom.ReadFromSpriteHeader(offset, palette[0], true);
            if (checkBox_hitbox.Checked)
                bmp = DrawHitbox(bmp);
            pictureBox_i_preview.Image = bmp;

        }
        private void button_header_write_Click(object sender, EventArgs e)
        {
            try
            {

                string warning = "WARNING! Edit with caution. Editing the header has \nthe possibility of corrupting your ROM. \nAlso, all unwritten data will be lost.\n\nEdit?";
                if (MessageBox.Show(warning, "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH0.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH_1.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH_2.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH_3.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH_4.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH_5.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_ih_6.Text, 16));
                    rom.Write8(ref _offset, Convert.ToInt32(textBox_iH_7.Text, 16));

                    button_i_loadImage_Click(sender, e);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                rom.RestoreFromBackup();
                button_i_loadImage_Click(sender, e);

            }
        }

    }
}
