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
        private void TileInit()
        {
            // To save on typing
            var thisTile = rom.tiles[listBox_i_tiles.SelectedIndex];
            textBox_type.Text = thisTile.type;
            textBox_y.Text = thisTile.y.ToString("X2");
            textBox_x.Text = thisTile.x.ToString("X2");

            // Fill in tile addresses
            // TL is always first
            textBox_tl.Text = thisTile.tileAddresses[0].ToString("X6");
            if (thisTile.type == "2x2")
            {
                textBox_tr.Text = thisTile.tileAddresses[1].ToString("X6");
                textBox_bl.Text = thisTile.tileAddresses[2].ToString("X6");
                textBox_br.Text = thisTile.tileAddresses[3].ToString("X6");
            }
            else
            {
                textBox_tr.Clear();
                textBox_bl.Clear();
                textBox_br.Clear();
            }

        }

        private void listBox_i_tiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            TileInit();
            TileArtInit();
            checkBox_grid_CheckedChanged(sender, e);

        }

        private void button_i_tileWrite_Click(object sender, EventArgs e)
        {
            // To save on typing
            var thisTile = rom.tiles[listBox_i_tiles.SelectedIndex];
            int x = Convert.ToInt32(textBox_x.Text, 16);
            int y = Convert.ToInt32(textBox_y.Text, 16);

            int xAddress = thisTile.xAddress - rom.baseAddress;
            int yAddress = xAddress + 1;
            rom.@char[xAddress] = (byte)x;
            rom.@char[yAddress] = (byte)y;

            RedrawPreview();

        }

    }
}
