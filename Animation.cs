using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandAloneGFXDKC1
{
    class Animation
    {
        public string commandsUsed = "";
        public int addressStart = 0;
        int defaultAnimation;
        Color[] palette;
        ROM rom;
        public List<AnimationIndex> animationIndices;
        public int currentIndex;
        int altCount = 0;
        int[] animationParams = new int[] 
        {
            0, 3, 2, 2, 3, 5, 9, 7, 4, 5, 9, 7, 4, 4, 1, 1, 1, 0
        };
        float timeModifier;

        public Animation (int defaultAnimation, Color[] palette, ROM rom, float timeModifier, bool altAnimation)
        {
            this.defaultAnimation = defaultAnimation;
            this.palette = palette;
            this.rom = rom;
            this.timeModifier = timeModifier;
            // Get animation pointer
            if (!altAnimation)
            {
                // 0xbe8572
                int pointer = rom.Read16(0xbe8572 + defaultAnimation * 2);
                animationIndices = ParseAnimation(pointer).ToList();
            }
            else
            {
                int pointer = rom.Read16(0xbcc388 + defaultAnimation * 2);
                animationIndices = ParseAltAnimation(pointer).ToList();
            }
            //ScanCalls();
        }
        private void ScanCalls()
        {
            for (int i = 0; i < animationIndices.Count; i++)
            {
                var caller = animationIndices[i];
                if (caller.calledAddress != 0)
                {
                    for (int j = 0; j < animationIndices.Count; j++)
                    {
                        var callee = animationIndices[j];
                        if (caller.calledAddress == callee.address)
                        {
                            callee.referenced = true;
                            break;
                        }
                    }
                }
            }
        }
        private AnimationIndex[] ParseAnimation(int pointer)
        {
            List<AnimationIndex> @return = new List<AnimationIndex>();
            Bitmap mounter = new Bitmap(256,256);
            int xLoc = 0;
            int yLoc = 0;
            bool seenMount = false;

            while (true)
            {
                int address = 0xbe0000 | pointer;
                if (addressStart == 0)
                {
                    addressStart = address;
                }
                int command = rom.Read8(address);
                AnimationIndex animationIndex = new AnimationIndex(rom);
                animationIndex.palette = palette.ToList();
                animationIndex.address = address;
                if (command > 0x91)
                {
                    throw new Exception($"Command out of range\n{command.ToString("X")}");
                }
                if (command >= 0x80)
                {
                    int count = animationParams[(command & 0x7f)];
                    var arr = rom.ReadSubArray(address, count + 1);
                    animationIndex.arr = arr;
                    pointer += count + 1;

                    switch (command)
                    {
                        case 0x80:
                            animationIndex.loop = false;
                            @return.Add(animationIndex);
                            return @return.ToArray();
                        case 0x81:
                            break;
                        case 0x82:
                            animationIndex.calledAddress = (arr[2] << 8) | (arr[1] << 0) | 0xbe0000;
                            break;
                        case 0x83:
                            //animationIndex.loop = false;
                            break;
                        case 0x84:
                            //animationIndex.loop = false;
                            break;
                        case 0x89:
                        case 0x8a:
                        case 0x8b:
                            animationIndex.arr = arr;
                            animationIndex.time = (int)(arr[1] * timeModifier);
                            int imgPointer = (arr[3] << 8) | (arr[2] << 0);
                            int offset = rom.Read24(Form1.gfxArray + imgPointer);
                            Bitmap bmp = rom.ReadFromSpriteHeader(offset, palette.ToList(), false);
                            if (seenMount)
                            {
                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    g.DrawImage(mounter, xLoc, yLoc);
                                }

                            }
                            animationIndex.bmp = bmp;

                            break;
                        case 0x85:
                            seenMount = true;
                            animationIndex.time = (int)(arr[1] * timeModifier);
                            imgPointer = (arr[3] << 8) | (arr[2] << 0);
                            offset = rom.Read24(Form1.gfxArray + imgPointer);
                            bmp = rom.ReadFromSpriteHeader(offset, palette.ToList(), false);
                            int imgPointer2 = (arr[5] << 8) | (arr[4] << 0);
                            offset = rom.Read24(Form1.gfxArray + imgPointer2);
                            var dkPal = rom.ReadPalette(0x3C849A)[0];

                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                mounter = rom.ReadFromSpriteHeader(offset, dkPal);
                                xLoc = 0;
                                yLoc = 0;
                                g.DrawImage(rom.ReadFromSpriteHeader(offset, dkPal), 0, 0);
                            }


                            animationIndex.bmp = bmp;
                            animationIndex.imgPointer = imgPointer;


                            break;
                        case 0x86:
                            seenMount = true;
                            animationIndex.time = (int)(arr[1] * timeModifier);
                            imgPointer = (arr[3] << 8) | (arr[2] << 0);
                            offset = rom.Read24(Form1.gfxArray + imgPointer);
                            bmp = rom.ReadFromSpriteHeader(offset, palette.ToList(), false);
                            imgPointer2 = (arr[5] << 8) | (arr[4] << 0);
                            offset = rom.Read24(Form1.gfxArray + imgPointer2);
                            dkPal = rom.ReadPalette(0x3C849A)[0];
                            int xOffset = rom.Read16Signed(animationIndex.address + 6);
                            int yOffset = rom.Read16Signed(animationIndex.address + 8) * -1;
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                mounter = rom.ReadFromSpriteHeader(offset, dkPal);
                                xLoc = xOffset;
                                yLoc = yOffset;
                                g.DrawImage(rom.ReadFromSpriteHeader(offset, dkPal), xOffset, yOffset);
                            }


                            animationIndex.bmp = bmp;
                            animationIndex.imgPointer = imgPointer;


                            break;
                        case 0x87:
                            seenMount = true;
                            animationIndex.time = (int)(arr[1] * timeModifier);
                            imgPointer = (arr[3] << 8) | (arr[2] << 0);
                            offset = rom.Read24(Form1.gfxArray + imgPointer);
                            bmp = new Bitmap(256, 256);
                            xOffset = rom.Read16Signed(animationIndex.address + 4);
                            yOffset = rom.Read16Signed(animationIndex.address + 6) * -1;
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.DrawImage(rom.ReadFromSpriteHeader(offset, palette.ToList(), false), xOffset, yOffset);
                            }


                            animationIndex.bmp = bmp;
                            animationIndex.imgPointer = imgPointer;


                            break;
                        case 0x88:
                            break;
                        case 0x8c:
                            break;
                        case 0x8d:
                            break;
                        case 0x8e:
                            break;
                        case 0x8f:
                            break;
                        case 0x90:
                            break;
                        case 0x91:
                            animationIndex.loop = false;
                            @return.Add(animationIndex);
                            return @return.ToArray();
                        default:
                            break;
                    }
                    commandsUsed += $"\t{animationIndex}\n";

                }
                else
                {
                    var arr = rom.ReadSubArray(address, 3);
                    animationIndex.arr = arr;
                    pointer += 3;
                    animationIndex.time = (int)(arr[0] * timeModifier);
                    int imgPointer = (arr[2] << 8) | (arr[1] << 0);
                    int offset = rom.Read24(Form1.gfxArray + imgPointer);
                    Bitmap bmp = rom.ReadFromSpriteHeader(offset, palette.ToList(), false);
                    if (seenMount)
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.DrawImage(mounter, xLoc, yLoc);
                        }

                    }


                    animationIndex.bmp = bmp;
                    animationIndex.imgPointer = imgPointer;

                }
                @return.Add(animationIndex);
            }

        }
        private AnimationIndex[] ParseAltAnimation(int pointer)
        {
            List<AnimationIndex> @return = new List<AnimationIndex>();

            while (true)
            {
                int address = 0xbc0000 | pointer;
                if (addressStart == 0)
                {
                    addressStart = address;
                }
                AnimationIndex animationIndex = new AnimationIndex(rom);
                animationIndex.address = address;
                animationIndex.altAnimation = true;


                var arr = rom.ReadSubArray(address, 8);
                animationIndex.arr = arr;
                pointer += 8;
                
                // Ending criteria
                if (rom.Read16(address + 4) >= 0xfffe || rom.Read16(address + 14) > 0x2a00)
                {
                    @return[altCount - 1].loop = false;
                    return @return.ToArray();
                }

                if (rom.Read16(address + 4) == 0x0)
                {
                    animationIndex.time = 4;
                }
                else
                {
                    animationIndex.time = (int)(arr[3] * timeModifier);
                }
                int imgPointer = rom.Read16(address + 6);
                int offset = rom.Read24(Form1.gfxArray + imgPointer);
                Bitmap bmp = rom.ReadFromSpriteHeader(offset, palette.ToList(), false);
                animationIndex.bmp = bmp;
                animationIndex.imgPointer = imgPointer;



                @return.Add(animationIndex);
                altCount++;
            }

        }
        private string GetCommandString (int pointer, byte[] arr, AnimationIndex animationIndex)
        {
            string @return = "";
            @return += $"{defaultAnimation.ToString("16")}:\n";
            @return += animationIndex.ToString() + "\n";
            @return += "\n";
            return @return;
        }
    }
}
