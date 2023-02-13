using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1
    {
        Animation animation;
        int defaultAnimationPointer;
        int animationTimer = 0;
        float animationSpeed = 1;
        int defaultLimit = 0x1b8;
        List<AnimationIndex> copied = new List<AnimationIndex>();


        // 0xbe8572
        private void button_setBase_Click(object sender, EventArgs e)
        {

            try
            {
                
                animationSpeed = 1;
                defaultAnimationPointer = (int)numericUpDown_base.Value;
                pictureBox_animationPreview.Image = null;
                SetDefault();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void SetDefault (bool first = true)
        {
            Cursor.Current = Cursors.WaitCursor; 
            int offset = rom.palettePointers[comboBox_animation_palette.SelectedItem.ToString()];
            var palette = rom.ReadPalette(offset)[0].ToArray();
            animationTimer = 0;

            if (first)
                animation = new Animation(defaultAnimationPointer, palette, rom, animationSpeed, checkBox_altAnimation.Checked);

            listBox_animation.Items.Clear();
            listBox_animation.Items.AddRange(animation.animationIndices.ToArray());
            listBox_animation.SelectedIndex = 0;
            numericUpDown_animationAddress.Value = animation.addressStart;

            Cursor.Current = Cursors.Default;
            //timer_animationTimer.Enabled = !checkBox_altAnimation.Checked;
            timer_animationTimer.Enabled = true;

        }
        private void timer_animationTimer_Tick(object sender, EventArgs e)
        {
            var currFrame = animation.animationIndices[animation.currentIndex];
            animationTimer++;

            if (animationTimer >= currFrame.time)
            {
                animationTimer = 0;
                animation.currentIndex++;
            }
            if (currFrame.bmp != null)
            {
                LoadHitbox(currFrame.imgPointer / 2);
                pictureBox_animationPreview.Image = AnimationDrawImageAndHitbox(currFrame.bmp);
            }

            if (!currFrame.loop)
            {
                timer_animationTimer.Enabled = false;
            }

        }
        private void button_restartAnimation_Click(object sender, EventArgs e)
        {
            animation.currentIndex = 0;
            SetDefault(false);
        }

        private void button_previousAnimation_Click(object sender, EventArgs e)
        {
            if (numericUpDown_base.Value != 0)
                numericUpDown_base.Value -= 1;
            //button_setBase_Click(0, new EventArgs());
        }

        private void button_nextAnimation_Click(object sender, EventArgs e)
        {
            if (numericUpDown_base.Value != 878)
                numericUpDown_base.Value += 1;
            //button_setBase_Click(0, new EventArgs());

        }

        private void button_prevAnimPal_Click(object sender, EventArgs e)
        {
            if (comboBox_animation_palette.SelectedIndex != 0)
            {
                comboBox_animation_palette.SelectedIndex--;
                button_setBase_Click(0, new EventArgs());
            }
        }

        private void button_nextAnimPal_Click(object sender, EventArgs e)
        {
            if (comboBox_animation_palette.SelectedIndex != comboBox_animation_palette.Items.Count - 1)
            {
                comboBox_animation_palette.SelectedIndex++;
                button_setBase_Click(0, new EventArgs());
            }

        }
        private void listBox_animation_SelectedIndexChanged(object sender, EventArgs e)
        {
                var animationIndex = (AnimationIndex)listBox_animation.Items[listBox_animation.SelectedIndex];

                ReadHitbox(animationIndex);

                if (animationIndex.hitboxAddress != 0)
                {
                    label_hitboxAddress.Text = $"Hitbox address: \n{animationIndex.hitboxAddress.ToString("X6")}";
                }


                label_animationInfo.Text = animationIndex.ToString().Replace('*', ' ');
                var arr = animationIndex.arr;
                int offsetP = rom.palettePointers[comboBox_animation_palette.SelectedItem.ToString()];
                var palette = rom.ReadPalette(offsetP)[0].ToArray();
                int imgPointer = animationIndex.imgPointer;
                if (imgPointer != 0)
                    LoadHitbox(imgPointer / 2);
                Bitmap bmp = animationIndex.bmp;
                timer_animationTimer.Enabled = false;
                if (bmp != null)
                {
                    Bitmap output = AnimationDrawImageAndHitbox(bmp);
                    imgPointer = animationIndex.imgPointer2;
                    if (imgPointer != 0)
                    {
                        LoadHitbox(imgPointer / 2);
                        output = AnimationDrawImageAndHitbox(output);
                    }
                    pictureBox_animationPreview.Image = output;
                }
                textBox_frameData.Text = $"{animationIndex}";
                if (checkBox_quickEdit.Checked)
                    textBox_frameData.Focus();
            label_animationAddress.Text = animationIndex.address.ToString("X6");


        }
        private void button_animationMinus_Click(object sender, EventArgs e)
        {
            ModSpeed(2);
            button_restartAnimation_Click(0, new EventArgs());
        }

        private void button_animationPlus_Click(object sender, EventArgs e)
        {
            ModSpeed(0.5f);
            button_restartAnimation_Click(0, new EventArgs());

        }
        private void ModSpeed(float mod)
        {
            foreach (var index in animation.animationIndices)
            {
                index.time = (int)(index.time * mod);
                if (index.time == 0)
                {
                    index.time = 1;
                }
            }
        }
        private void button_animationWrite_Click(object sender, EventArgs e)
        {
            var offset = (int)numericUpDown_animationAddress.Value;
            int pointer = offset & 0xffff;
            int index = (int)numericUpDown_base.Value;
            // 0xbe8572
            rom.Write16(0xbe8572 + index * 2, pointer);


            for (int i = 0; i < listBox_animation.Items.Count; i++)
            {
                var animationIndex = (AnimationIndex)listBox_animation.Items[i];
                animationIndex.WriteIndexToROM(ref offset);
                if (animationIndex.ToString() == "80")
                {
                    break;
                }

            }

            MessageBox.Show("Done");
            //button_setBase_Click(0, new EventArgs());


        }

        private void button_animationApply_Click(object sender, EventArgs e)
        {
            var animationIndex = (AnimationIndex)listBox_animation.Items[listBox_animation.SelectedIndex];
            animationIndex.ApplyStringToArr(textBox_frameData.Text);
            int index = listBox_animation.SelectedIndex;

            listBox_animation.Items.Clear();
            listBox_animation.Items.AddRange(animation.animationIndices.ToArray());
            listBox_animation.SelectedIndex = index;

            //animationIndex.WriteIndexToROM();
            //button_setBase_Click(0, new EventArgs());

        }
        private void textBox_frameData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_animationApply_Click(0, new EventArgs());
                listBox_animation.Focus();
            }
        }
        private void button_animationInsert_Click(string str = "80")
        {
            int index = listBox_animation.SelectedIndex;
            var toAdd = new AnimationIndex(rom);
            toAdd.ApplyStringToArr(str);
            listBox_animation.Items.Insert(index, toAdd);
        }
        private void button_animationInsert_Click(object sender, EventArgs e)
        {
            int index = listBox_animation.SelectedIndex;
            var toAdd = new AnimationIndex(rom);
            toAdd.ApplyStringToArr("80");
            animation.animationIndices.Insert(index, toAdd);
            RefillListbox(animation.animationIndices.ToArray(), listBox_animation);
        }

        private void button_animationRemove_Click(object sender, EventArgs e)
        {
            int index = listBox_animation.SelectedIndex;
            animation.animationIndices.RemoveAt(index);
            RefillListbox(animation.animationIndices.ToArray(), listBox_animation);

        }
        private void button_animationMoveUp_Click(object sender, EventArgs e)
        {
            var min = 0;
            int index = listBox_animation.SelectedIndex;
            if (index > min)
            {
                var shifter = animation.animationIndices[index];
                var shiftee = animation.animationIndices[index - 1];
                animation.animationIndices[index] = shiftee;
                animation.animationIndices[index - 1] = shifter;
                RefillListbox(animation.animationIndices.ToArray(), listBox_animation);
                listBox_animation.SelectedIndex = index - 1;

            }
        }

        private void button_animationMoveDown_Click(object sender, EventArgs e)
        {
            var max = listBox_animation.Items.Count;
            int index = listBox_animation.SelectedIndex;
            if (index < max - 1)
            {
                var shifter = animation.animationIndices[index];
                var shiftee = animation.animationIndices[index + 1];
                animation.animationIndices[index] = shiftee;
                animation.animationIndices[index + 1] = shifter;
                RefillListbox(animation.animationIndices.ToArray(), listBox_animation);
                listBox_animation.SelectedIndex = index + 1;

            }
        }
        private void RefillListbox(object[] arr, ListBox lb)
        {
            var index = lb.SelectedIndex;
            lb.Items.Clear();
            lb.Items.AddRange(arr.ToArray());
            if (index <= lb.Items.Count - 1)
                lb.SelectedIndex = index;
            

        }
        private void button_animationScanAll_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string all = "";
            //for (int i = 0; i <= 10; i++)
            for (int i = 0; i < defaultLimit; i++)
            {
                numericUpDown_base.Value = i;
                numericUpDown_base.Refresh();
                button_setBase_Click(0, new EventArgs());
                all += $"{defaultAnimationPointer.ToString("X4")}: \n";
                all += $"{animation.commandsUsed}\n";
            }
            Cursor.Current = Cursors.Default;
            Clipboard.SetText(all);
            MessageBox.Show("Done");
        }
        private void button_animationReplace_Click(object sender, EventArgs e)
        {
            int listindex = listBox_animation.SelectedIndex;
            int replaceFrom = (int)numericUpDown_animationReplaceFrom.Value;
            int replaceTo = (int)numericUpDown_animationReplaceTo.Value;
            foreach (var index in animation.animationIndices)
            {
                if (index.imgPointer == replaceFrom)
                {
                    index.ReplaceImage(replaceTo);
                }
            }
            // Refresh listbox
            listBox_animation.Items.Clear();
            listBox_animation.Items.AddRange(animation.animationIndices.ToArray());
            listBox_animation.SelectedIndex = listindex;
        }
        private void numericUpDown_animationReplaceTo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button_animationReplace_Click(0, new EventArgs());
            }
        }
        private void button_lut_Click(object sender, EventArgs e)
        {
            var pasted = Prompt.ShowDialog("Enter imported pointer", "").ToUpper();

            // Load our dict
            var dict = sd.ReadCategory("Dixie");
            if (!dict.ContainsKey(pasted))
            {
                MessageBox.Show("Not uploaded");
                return;
            }
                //throw new Exception($"Image {pasted} never uploaded");
            string newPointerS = dict[pasted];
            MessageBox.Show(newPointerS);

        }

        private void RefreshAnimationListbox()
        {
            int listindex = listBox_animation.SelectedIndex;
            // Refresh listbox
            listBox_animation.Items.Clear();
            listBox_animation.Items.AddRange(animation.animationIndices.ToArray());
            listBox_animation.SelectedIndex = listindex;
        }
        string animationCopy = "Copy";
        private void button_animationCopy_Click(object sender, EventArgs e)
        {
            string str = "";
            for (int i = 0; i < listBox_animation.Items.Count; i++)
            {
                var item = (AnimationIndex)listBox_animation.Items[i];
                //copied.Add(item);

                str += item.ToString();
                str += "&";
            }
            Clipboard.Clear();
            Clipboard.SetText(str);

            button_animationCopy.Text = animationCopy == "Copy" ? "Copy!" : "Copy";
            animationCopy = animationCopy == "Copy" ? "Copy!" : "Copy";
        }

        private void button_animationClear_Click(object sender, EventArgs e)
        {
            copied = new List<AnimationIndex>();
            MessageBox.Show("Cleared!");
        }

        private void checkBox_animationShowHitbox_CheckedChanged(object sender, EventArgs e)
        {

        }
        private Bitmap AnimationDrawImageAndHitbox(Bitmap bmp)
        {
            if (checkBox_animationShowHitbox.Checked)
            {
                bmp = DrawHitbox(bmp);
            }

            return bmp;
        }
        private void listBox_animation_MouseDown(object sender, MouseEventArgs e)
        {
            if (animation == null && animation.animationIndices.Count == 0)
            {
                return;
            }
            // This index
            var index = (AnimationIndex)listBox_animation.SelectedItem;
            if (index.imgPointer == 0)
            {
                //return;
            }
            if (e.Button == MouseButtons.Right)
            {
                // FIXME
                PasteCustom();
                return;


                tabControl_sprites.SelectedIndex = 1;
                textBox_i_image.Text = index.imgPointer.ToString("X");
                button_i_loadImage_Click(0, new EventArgs());
                //tabControl.SelectedIndex = 2;
                //textBox_hitboxPointer.Focus();
                //textBox_hitboxPointer.SelectionStart = 4;
            }
        }

        private void ReadHitbox(AnimationIndex animationIndex)
        {
            int imgPointer = animationIndex.imgPointer;
            imgPointer /= 2;
            animationIndex.hitboxAddress = rom.Read16(0xbb8000 + imgPointer) + 0xbb0000;
        }

        private void PasteCustom()
        {
            var clippy = Clipboard.GetText();
            var spl = clippy.Split('&');
            for (int i = 0; i < spl.Length - 1; i++)
            {
                button_animationInsert_Click(0, new EventArgs());
                textBox_frameData.Refresh();
                textBox_frameData.Text = spl[i];
                button_animationApply_Click(0, new EventArgs());
                listBox_animation.SelectedIndex++;
            }
        }

        private void PasteString(string toInsert)
        {
            var animationIndex = (AnimationIndex)listBox_animation.SelectedItem;
            string @string = "";
            List<int> bytes = new List<int>();

            bytes = (toInsert.Split(',')).Select(e => Convert.ToInt32(e.Trim(), 16)).ToList();
            bytes[2] = LUTLookup(bytes[2]);
            bytes[3] = LUTLookup(bytes[3]);
            @string += $"{bytes[0].ToString("X2")},";
            @string += $"{bytes[1].ToString("X2")},";
            var temp0 = (byte)bytes[2];
            var temp1 = (byte)(bytes[2] >> 8);
            @string += $"{temp0.ToString("X2")},";
            @string += $"{temp1.ToString("X2")},";
            temp0 = (byte)bytes[3];
            temp1 = (byte)(bytes[3] >> 8);
            @string += $"{temp0.ToString("X2")},";
            @string += $"{temp1.ToString("X2")}";


            textBox_frameData.Text = @string;
            textBox_frameData.Refresh();
            button_animationApply_Click(0, new EventArgs());


            //if (!dict.ContainsKey(pasted.ToString("X")))
            //    throw new Exception($"Image {pasted.ToString("X")} never uploaded");
            //string newPointerS = dict[pasted.ToString("X")];
            //int num = Convert.ToInt32(newPointerS, 16);


            //MessageBox.Show("In");
        }

        private int LUTLookup(int pasted)
        {
            // Load our dict
            var dict = sd.ReadCategory("Dixie");

            if (!dict.ContainsKey(pasted.ToString("X")))
                throw new Exception($"Image {pasted.ToString("X")} never uploaded");
            string newPointerS = dict[pasted.ToString("X")];
            int num = Convert.ToInt32(newPointerS, 16);

            return num;
        }
        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var duration = 0;
            for (int i = 0; i < animation.animationIndices.Count; i++)
            {
                var arr = animation.animationIndices[i].arr;
                if (arr[0] < 0x80)
                {
                    duration += arr[0];
                    listBox_animation.SelectedIndex = i;
                    button_animationCopy_Click(0, new EventArgs());
                }
            }
            MessageBox.Show($"{duration} frames total");
        }
    }
}
