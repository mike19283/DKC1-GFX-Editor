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
        private void LoadHitbox(int index)
        {
            int offset = 0xbb8000 + index;
            offset = 0xbb0000 + rom.Read16(offset);

            x_hitbox = rom.Read16(offset + 0);
            y_hitbox = rom.Read16(offset + 2);
            width_hitbox = rom.Read16(offset + 4);
            height_hitbox = rom.Read16(offset + 6);

            var pointer = offset & 0xffff;
            textBox_hitboxPointer.Text = pointer.ToString("X4");
            textBox_hitbox_x.Text = x_hitbox.ToString("X4");
            textBox_hitbox_y.Text = y_hitbox.ToString("X4");
            textBox_hitbox_width.Text = width_hitbox.ToString("X4");
            textBox_hitbox_height.Text = height_hitbox.ToString("X4");

            textBox_hitbox_address.Text = offset.ToString("X6");
        }

        private void button_hitbox_write_Click(object sender, EventArgs e)
        {
            int x = Convert.ToInt32(textBox_hitbox_x.Text, 16),
                y = Convert.ToInt32(textBox_hitbox_y.Text, 16),
                width = Convert.ToInt32(textBox_hitbox_width.Text, 16),
                height = Convert.ToInt32(textBox_hitbox_height.Text, 16),
                address = Convert.ToInt32(textBox_hitbox_address.Text, 16);

            rom.Write16(ref address, x);
            rom.Write16(ref address, y);
            rom.Write16(ref address, width);
            rom.Write16(ref address, height);

            button_i_loadImage_Click(sender, e);

        }
        private int ConvertToSNESInt(int toConvert)
        {
            return toConvert >= 0x8000 ? (0x10000 - toConvert) * -1 : toConvert;
        }
        private Bitmap DrawHitbox(Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawRectangle(new Pen(Color.Black),
                    +bmp.Width / 2 + ConvertToSNESInt(x_hitbox),
                    +bmp.Height / 2 + ConvertToSNESInt(y_hitbox),
                    +width_hitbox,
                    +height_hitbox);
            }

            return bmp;
        }

        private void checkBox_hitbox_CheckedChanged(object sender, EventArgs e)
        {
            RedrawPreview();
        }
        private void SetScale_i(int picture_scale, int address)
        {
            // Set the scale.
            PictureScale = picture_scale;

            RedrawPreview();

            pictureBox_i_preview.Width = (int)(PictureScale * 256);
            pictureBox_i_preview.Height = (int)(PictureScale * 256);
        }

        private void comboBox_i_zoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_i_zoom.SelectedIndex)
            {
                case 0:
                    SetScale_i(1, 0);
                    break;
                case 1:
                    SetScale_i(2, 0);
                    break;
                case 2:
                    SetScale_i(4, 0);
                    break;
                default:
                    break;
            }
        }

        private void button_exportHitbox_Click(object sender, EventArgs e)
        {
            int hitA = Convert.ToInt32(textBox_hitbox_address.Text, 16);
            var arr = rom.ReadSubArray(hitA, 8);
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Bin (*.bin)|*.bin";

            if (s.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(s.FileName, arr);
                MessageBox.Show("Exported!");
            }

        }
        private void button_hitboxSetPointer_Click(object sender, EventArgs e)
        {
            var image = Convert.ToInt32(textBox_i_image.Text, 16);
            var pointer = Convert.ToInt32(textBox_hitboxPointer.Text, 16);
            // Base hitbox address
            // bb8000
            rom.Write16(0xbb8000 + image / 2, pointer);
            LoadHitbox(image /2);
            button_i_loadImage_Click(0, new EventArgs());


        }


    }
}
