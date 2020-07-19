using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1 : Form
    {

        ROM rom;
        StoredData sd = new StoredData();
        List<List<Color>> palette;
        int PictureScale = 1;
        string palKey;
        //List<TileCoords> tiles = new List<TileCoords>();
        // Make 'global' so I can use everywhere
        int x_hitbox;
        int y_hitbox;
        int width_hitbox;
        int height_hitbox;
        int _offset;
        int timer = 0;
        int timerConstant = 1;
        public Form1()
        {
            InitializeComponent();
            Version.OnLoad();
            rom = new ROM(sd);

            try
            {
                string path = sd.Read("File", "Path");
                rom.LoadROM(path);
                Init();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }

        private void Init ()
        {
            comboBox_loadPalette.Items.AddRange(rom.palettePointers.Keys.ToArray());
            comboBox_i_palette.Items.AddRange(rom.palettePointers.Keys.ToArray());

            comboBox_loadPalette.SelectedIndex = 0;

            // Get palette
            palKey = rom.palettePointers.Keys.ToArray()[0];
            palette = rom.ReadPalette(rom.palettePointers[palKey]);

            tabControl_sprites.SelectedIndex = 1;
            tabControl_sprites.SelectedIndex = 0;
            tabControl_sprites_SelectedIndexChanged(1, new EventArgs());

            // Display everything
            tabControl_sprites.Visible = true;

            AddGlobalHotkeyToAll(this);

            timer_100ticks.Enabled = true;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rom.IsROMChanged())
            {
                if (MessageBox.Show("Unsaved changes detected. Really continue?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }


            rom.Load();
            if (rom.loadROMSuccess)
            {
                Init();
            }

        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rom.SaveAsROM();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rom.IsROMChanged())
            {
                if (MessageBox.Show("Unsaved changes detected. Really quit?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();


        private void tabControl_sprites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl_sprites.SelectedIndex == 0)
            {
                textBox_p_imgToLoad.Text = textBox_i_image.Text;

                panel_paletteEdit.Visible = true;

                button_load_Click(sender, e);
                var mouse = new MouseEventArgs(MouseButtons, 1, 0, 0, 0);

                pictureBox_palEdit_MouseClick(sender, mouse);

                button_p_loadGFX_Click(sender, e);

                comboBox_p_zoom.SelectedIndex = 0;
                comboBox_loadPalette.SelectedItem = palKey;
            }
            else if (tabControl_sprites.SelectedIndex == 1)
            {
                textBox_i_image.Text = textBox_p_imgToLoad.Text;

                comboBox_i_zoom.SelectedIndex = 0;
                comboBox_i_palette.SelectedItem = palKey;

                button_i_loadImage_Click(sender, e);
                button_load_Click(sender, e);
                //button_i_loadImage_Click(sender, e);
                var mouse = new MouseEventArgs(MouseButtons, 1, 0, 0, 0);

                pictureBox_tilePalette_MouseDown(0, mouse);
            }
        }

        private void AddGlobalHotkeyToAll(Control ctrl)
        {
            ctrl.KeyDown += new System.Windows.Forms.KeyEventHandler(GlobalHotkey);
            foreach (Control child in ctrl.Controls)
            {
                AddGlobalHotkeyToAll(child);
            }

        }
        private void GlobalHotkey(object  sender, KeyEventArgs e)
        {
            // Is our timer active?
            if (timer > 0)
            {
                return;
            }

            if (e.KeyCode == Keys.F3)
            {
                timer = timerConstant;
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_p_loadPGFX_Click(sender, e);
                }
                else
                {
                    button_i_prevImage_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F4)
            {
                timer = timerConstant;
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_p_loadNGFX_Click(sender, e);
                }
                else
                {
                    button_nextImage_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F5)
            {
                timer = timerConstant;
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_previousPal_Click(sender, e);
                }
                else
                {
                    button_i_prevPal_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F6)
            {
                timer = timerConstant; 
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_nextPal_Click(sender, e);
                }
                else
                {
                    button_i_nextPal_Click(sender, e);
                }
            }

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawPreview();
        }

        private void timer_100ticks_Tick(object sender, EventArgs e)
        {
            if (timer > 0)
            {
                timer--;
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version.ManualCheck();
        }
    }

}
