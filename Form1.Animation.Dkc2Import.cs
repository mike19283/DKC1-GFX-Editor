using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public partial class Form1
    {
        int[] animationDkc2Params = new int[]
        {
            0, 2, 2, 2, 2, 5, 9, 7, 4, 5, 9, 7, 4, 7, 2, 4, 4, 3, 1, 2, 0
        };
        private void button_animationLoadFromClipboard_Click(object sender, EventArgs e)
        {
            string clipText = Clipboard.GetText();
            var spl = clipText.Split('\n');
            button_setBase_Click(0, new EventArgs());
            timer_animationTimer.Enabled = false;
            animation.animationIndices = new List<AnimationIndex>();
            foreach (var line in spl)
            {

                if (line.Length > 0)
                {
                    AnimationIndex animationIndex = new AnimationIndex(rom);
                    animationIndex.palette = palette[0].ToList();
                    animationIndex.ApplyStringToArr(line);
                    // Load our dict
                    var dict = sd.ReadCategory("Dixie");
                    int pasted = (animationIndex.arr[1] << 0) | (animationIndex.arr[2] << 8);
                    if (!dict.ContainsKey(pasted.ToString("X")))
                        throw new Exception($"Image {pasted.ToString("X")} never uploaded");
                    string newPointerS = dict[pasted.ToString("X")];
                    int num = Convert.ToInt32(newPointerS, 16);
                    animationIndex.ReplaceGFXPointer(num);


                    animation.animationIndices.Add(animationIndex);

                }
            }

            // Refresh listbox
            listBox_animation.Items.Clear();
            listBox_animation.Items.AddRange(animation.animationIndices.ToArray());

        }


    }
}
