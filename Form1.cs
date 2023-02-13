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
        int palKeyNum = 0;
        public static int gfxArray = 0xbbcc9c;
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
        bool deleted;
        int index;
        bool single;
        int headerLength;
        CreateSpriteSheet spriteSheet;
        Keys keyDown;

        public Form1()
        {
            InitializeComponent();
            // Pastebin format changed
            //Version.OnLoad();
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
                editToolStripMenuItem.Enabled = false;
            }
            

        }

        private void Controller ()
        {
            var ctrl = this.ActiveControl;
            switch(ctrl.Name)
            {
                case "textbox_x":
                    button_i_tileWrite_Click(0, new EventArgs());
                    break;
                case "textbox_y":
                    button_i_tileWrite_Click(0, new EventArgs());
                    break;
                default:
                    break;
            }
        }

        private void Init ()
        {
            if (comboBox_loadPalette.Items.Count == 0)
            {
                comboBox_loadPalette.Items.AddRange(rom.palettePointers.Keys.ToArray());
                comboBox_i_palette.Items.AddRange(rom.palettePointers.Keys.ToArray());
                comboBox_animation_palette.Items.AddRange(rom.palettePointers.Keys.ToArray());
            }
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

            this.Text = rom.fileName;
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
            this.Text = rom.fileName;
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
                button_i_import_apply.Enabled = false;
                textBox_i_image.Text = textBox_p_imgToLoad.Text;

                comboBox_i_zoom.SelectedIndex = 0;
                comboBox_i_palette.SelectedItem = palKey;

                button_i_loadImage_Click(sender, e);
                button_load_Click(sender, e);
                //button_i_loadImage_Click(sender, e);
                var mouse = new MouseEventArgs(MouseButtons, 1, 0, 0, 0);

                pictureBox_tilePalette_MouseDown(0, mouse);
            }
            else if (tabControl_sprites.SelectedIndex == 2)
            {
                comboBox_animation_palette.SelectedIndex = 0;
                comboBox_animation_palette.Refresh();
                //comboBox_animation_palette.SelectedItem = 
                //numericUpDown_base.Value = 0;
                //button_setBase_Click(0, new EventArgs());
            }
        }

        private void AddGlobalHotkeyToAll(Control ctrl)
        {
            ctrl.KeyDown += new System.Windows.Forms.KeyEventHandler(GlobalHotkey);
            ctrl.KeyUp += new System.Windows.Forms.KeyEventHandler(GlobalHotkeyKeyUp);
            foreach (Control child in ctrl.Controls)
            {
                AddGlobalHotkeyToAll(child);
            }

        }

        private void GlobalHotkeyKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyDown)
            {
                keyDown = Keys.Home;
            }
        }

        private void GlobalHotkey(object  sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyDown)
            {
                return;
            }
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
                else if (tabControl_sprites.SelectedIndex == 1)
                {
                    button_i_prevImage_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 2)
                {
                    button_previousAnimation_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F4)
            {
                timer = timerConstant;
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_p_loadNGFX_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 1)
                {
                    button_nextImage_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 2)
                {
                    button_nextAnimation_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F5)
            {
                timer = timerConstant;
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_previousPal_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 1)
                {
                    button_i_prevPal_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 2)
                {
                    button_prevAnimPal_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F6)
            {
                timer = timerConstant; 
                if (tabControl_sprites.SelectedIndex == 0)
                {
                    button_nextPal_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 1)
                {
                    button_i_nextPal_Click(sender, e);
                }
                else if (tabControl_sprites.SelectedIndex == 2)
                {
                    button_nextAnimPal_Click(sender, e);
                }
            }
            if (e.KeyCode == Keys.F8 && tabControl_sprites.SelectedIndex == 1)
            {
                timer = timerConstant;
                button_import_Click(0, new EventArgs());
            }
            if (e.KeyCode == Keys.F9 && tabControl_sprites.SelectedIndex == 2)
            {
                timer = timerConstant;
                PasteCustom();
            }
            if (e.KeyCode == Keys.F7 && tabControl_sprites.SelectedIndex == 0 && spriteSheet != null)
            {

                keyDown = e.KeyCode;
                int current = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
                spriteSheet.AddToList(current);

            }
            if (e.KeyCode == Keys.F8 && tabControl_sprites.SelectedIndex == 0 && spriteSheet != null)
            {

                keyDown = e.KeyCode;
                int current = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
                spriteSheet.AddStartToList(current);

            }

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawPreview();
        }

        private void timer_100ticks_Tick(object sender, EventArgs e)
        {
            editToolStripMenuItem.Enabled = rom.loadROMSuccess;
            if (timer > 0)
            {
                timer--;
            }
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version.ManualCheck();
        }

        private void button_export_Click(object sender, EventArgs e)
        {
            var offset = Convert.ToInt32(textBox_i_address.Text, 16);
            var size = Convert.ToInt32(textBox_iH_total.Text, 16);

            if (!checkBox_exportHeader.Checked)
            {
                offset += headerLength;
                size -= headerLength;
            }

            var sub = rom.ReadSubArray(offset, size);

            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "Bin (*.bin)|*.bin";

            if (s.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(s.FileName, sub);
                MessageBox.Show("Exported!");
            }
        }

        private void button_import_Click(object sender, EventArgs e)
        {
            single = true;
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "Bin (*.bin)|*.bin";

            if (d.ShowDialog() == DialogResult.OK)
            {
                ImportFile(d.FileName);
            }
        }
        private void ImportFile(string fileName)
        {
            deleted = false;
            byte[] data = System.IO.File.ReadAllBytes(fileName);
            var offset = Convert.ToInt32(textBox_i_address.Text, 16);
            var dataLength = data.Length;
            var currentSize = Convert.ToInt32(textBox_iH_total.Text, 16);
            string text = "Data is longer than original. Continue?";
            string caption = "WARNING";
            if (dataLength > currentSize)
            {
                
                if (single && MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    WriteArrFromFile(offset, data, fileName);
                }
                else
                {

                }
            }
            else
            {
                WriteArrFromFile(offset, data, fileName);
            }

        }
        private void WriteArrFromFile(int offset, byte[] data, string fileName)
        {
            rom.WriteArr(offset, data);
            button_i_loadImage_Click(0, new EventArgs());
            //MessageBox.Show("Imported!");

            deleted = true;


            // Get pointer from file
            int lastIndex = fileName.LastIndexOf('\\');
            string pointerS = fileName.Substring(lastIndex + 1);
            pointerS = pointerS.Substring(0, pointerS.Length - 4);
            //int filePointer = Convert.ToInt32(pointerS, 16);

            //int currentPointer = Convert.ToInt32(textBox_i_image.Text, 16);

            // Load dictionary from rbs
            var dict = sd.ReadCategory("Dixie");
            dict[pointerS] = textBox_i_image.Text;
            sd.Write("Dixie", pointerS, textBox_i_image.Text);
            sd.SaveRbs();

            if (!single)
                File.Delete(fileName);

        }

        private void textBox_i_image_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_i_loadImage_Click(0, new EventArgs());
            }
        }

        private void button_i_importBulk_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            single = false;
            string startString = "Enter in starting index";
            string endString = "Enter in ending index";
            int start = Convert.ToInt32(Prompt.ShowDialog(startString,""),16);
            int end = Convert.ToInt32(Prompt.ShowDialog(endString,""),16);
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                index = 0;
                Cursor.Current = Cursors.WaitCursor;
                for (int i = start; i < end; i += 4)
                {
                    deleted = false;
                    index = 0;
                    textBox_i_image.Text = i.ToString("X");
                    textBox_i_image.Refresh();
                    button_i_loadImage_Click(0, new EventArgs());

                    var files = Directory.GetFiles(fbd.SelectedPath).Where(f => f.EndsWith(".bin")).ToArray();
                    while(!deleted && index < files.Length)
                        ImportFile(files[index++]);                    
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void button_animationPaste_Click(object sender, EventArgs e)
        {
            PasteCustom();
            return;


            int pasteIndex = listBox_animation.SelectedIndex;
            animation.animationIndices.InsertRange(pasteIndex, copied);
            RefreshAnimationListbox();
        }

        private void textBox_hitboxPointer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_hitboxSetPointer_Click(0, new EventArgs());
            }
        }

        private void textBox_hitbox_x_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_hitbox_write_Click(0, new EventArgs());
            }
        }

        private void button_control_Click(object sender, EventArgs e)
        {
            Controller();
        }
        #region import image
        private void button_i_import_load_Click(object sender, EventArgs e)
        {
            pictureBox_i_import.Size = new Size(256, 256);
            OpenFileDialog d = new OpenFileDialog();
            d.Title = "Image to import";
            d.Filter = "PNG/BIN/JPG(*.bmp;*.png;*.jpg;*.jpeg)|*.bmp;*.png;*.jpg;*.jpeg";
            if (d.ShowDialog() == DialogResult.OK)
            {
                pictureBox_i_import.Image = new Bitmap(d.FileName);
            }
        }

        private void button_i_import_recolor_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)pictureBox_i_import.Image;
            bmp = SimplifyImage(bmp);
            pictureBox_i_import.Image = bmp;
            button_i_import_apply.Enabled = true;
        }
        public Bitmap SimplifyImage(Bitmap sprite)
        {
            Bitmap og = (Bitmap)sprite.Clone();
            for (int y = 0; y < sprite.Height; y++)
            {
                for (int x = 0; x < sprite.Width; x++)
                {
                    Color thisPixel = sprite.GetPixel(x, y);
                    Color match = SimplifiedColor(thisPixel, og.GetPixel(0,0));
                    sprite.SetPixel(x, y, match);
                }
            }

            return sprite;
        }
        public double Compare2Colors(Color a, Color b)
        {
            // a2 + b2 = c2
            int avgR = a.R - b.R;
            int avgG = a.G - b.G;
            int avgB = a.B - b.B;
            return avgR * avgR + avgG * avgG + avgB * avgB;
        }
        public Color SimplifiedColor(Color og, Color transparent)
        {
            if (og.A == 0)
                return og;
            Color newClr = currentPalette[0];
            Color closestColor = currentPalette[0];
            double closestVal = Compare2Colors(og, currentPalette[0]);
            for (int i = 0; i < currentPalette.Count; i++)
            {
                double tempVal = Compare2Colors(og, currentPalette[i]);
                if (tempVal < closestVal)
                {
                    closestVal = tempVal;
                    closestColor = currentPalette[i];
                }
            }
            if (og == transparent)
            {
                closestColor = Color.Transparent;
            }

            return closestColor;
        }


        private void button_i_import_apply_Click(object sender, EventArgs e)
        {
            rom.ReadFromSpriteHeader(Convert.ToInt32(textBox_i_address.Text, 16), palette[0], true);
            Bitmap bmp = (Bitmap)pictureBox_i_import.Image.Clone();
            var tiles = rom.tiles.ToArray();
            int length = tiles.Length;
            listBox_i_tiles.Items.Clear();
            listBox_i_tiles.Items.AddRange(tiles);
            for (int i = 0; i < length; i++)
            {
                var tile = tiles[i];
                listBox_i_tiles.SelectedIndex = i;
                // Get TL of image
                int x = tile.x;
                int y = tile.y;
                int w, h;
                if (tile.type == "2x2")
                {
                    w = 16;
                    h = 16;
                }
                else
                {
                    w = 8;
                    h = 8;
                }
                editedImage = new Bitmap(w, h);
                using (Graphics g = Graphics.FromImage(editedImage))
                {
                    g.DrawImage(bmp, 0, 0, new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
                }

                //pictureBox_i_import.Image = editedImage;
                //pictureBox_i_import.Refresh();
                //return;

                button_applyTiles_Click(0, new EventArgs());


            }
            //button_i_writeImage_Click(0, new EventArgs());
        }

        #endregion

        private void button_createSpriteSheet_Click(object sender, EventArgs e)
        {
            int current = Convert.ToInt32(textBox_p_imgToLoad.Text, 16);
            spriteSheet = new CreateSpriteSheet(current, rom, palette[0]);
            spriteSheet.Show();
            this.SetTopLevel(true);
        }

        private void checkBox_charBorder_CheckedChanged(object sender, EventArgs e)
        {
            RedrawPreview();
        }

    }

}
