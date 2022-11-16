using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1 : Form
    {
        #region SpritePalette - Sprite

        // 8c = first index

        private int AddressFromIndex(int current)
        {
            int address = 0;

            // Which index are we looking at?
            int index = gfxArray + current;

            // Address of index
            address = (rom.Read8(index + 2) << 16) | rom.Read16(index);

            return address;
        }

        private void button_p_loadGFX_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
            int address = AddressFromIndex(index);


            pictureBox_p_ObjectImage.Image = rom.ReadFromSpriteHeader(address, palette[0]);
        }

        private void button_p_loadPGFX_Click(object sender, EventArgs e)
        {
            int current = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
            int previous = current - 4;
            // Do we go below our bounds?
            if (previous >= 0x8c)
            {
                textBox_p_imgToLoad.Text = previous.ToString("X");
                button_p_loadGFX_Click(sender, e);
            }

        }

        private void button_p_loadNGFX_Click(object sender, EventArgs e)
        {
            int current = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
            int next = current + 4;
            if (rom.Read16(gfxArray + next + 1) != 0)
            {
                textBox_p_imgToLoad.Text = next.ToString("X");
                button_p_loadGFX_Click(sender, e);
            }
        }

        private void comboBox_p_zoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_p_zoom.SelectedIndex)
            {
                case 0:
                    int index = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
                    int address = AddressFromIndex(index);
                    SetScale(1, address);
                    //panel_p_ObjectImage.SetAutoScrollMargin(pictureBox_p_ObjectImage.Width / 2, pictureBox_p_ObjectImage.Height / 2);
                    break;
                case 1:
                    index = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
                    address = AddressFromIndex(index);
                    SetScale(2, address);
                    break;
                case 2:
                    index = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
                    address = AddressFromIndex(index);
                    SetScale(4, address);
                    break;
                default:
                    break;
            }


        }

        private void SetScale(int picture_scale, int address)
        {
            // Set the scale.
            PictureScale = picture_scale;

            pictureBox_p_ObjectImage.Image = rom.ReadFromSpriteHeader(address, palette[0]);

            pictureBox_p_ObjectImage.Width = (int)(PictureScale * 256);
            pictureBox_p_ObjectImage.Height = (int)(PictureScale * 256);
        }
        #endregion


    }
}
