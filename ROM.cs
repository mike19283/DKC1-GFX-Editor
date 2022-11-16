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
    public partial class ROM
    {
        public List<byte> rom = new List<byte>();
        public byte[] backupRom;
        public string fileName;
        public bool loadROMSuccess = false;
        public bool saved = false;
        public static int seed = 0;
        StoredData sd;
        public int maxFileNameLength = 80;
        public static string gameTitleAsString;



        public ROM(StoredData sd)
        {
            this.sd = sd;
        }

        public void Load () 
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "ROM file (*.smc;*.sfc)|*.smc;*.sfc";
            d.Title = "Select a proper DKC ROM";

            while (d.ShowDialog() == DialogResult.OK)
            {
                fileName = d.FileName;
                if (LoadROM(d.FileName))
                    break;
            }
        }
        public  bool LoadROM (string path)
        {
            // Refresh Ini
            sd.RefreshRbs();
            //Loading my file and displaying all my content.
            backupRom = File.ReadAllBytes(path);
            backupRom = backupRom.Skip(backupRom.Length == 0x400200 ? 0x200 : 0).ToArray();


            // As seen in header
            var gameTitle = new ArraySegment<byte>(backupRom, 0XFFC0, 21).ToArray();
            gameTitleAsString = GetTitleFromHeader(gameTitle);
            // Verify checksum
            //if (GetChecksum(backupRom) == 0x163e1202.ToString("x"))
            if (backupRom[0xffdb] == 0 && (VerifyROM(gameTitle, "DONKEY KONG COUNTRY  ") || VerifyROM(gameTitle, "DKC Hack   [DKC v1.0]")))
            {
                // Copy backup to main
                RestoreFromBackup();
                loadROMSuccess = true;

                fileName = path;
                // Add to recents
                //sd.AddToRecents(path);

                // TODO add check for recents
                // Write to ini as recent
                sd.Write("File", "Path", path);
                sd.SaveRbs();



                return true;
            }
            else
            {
                MessageBox.Show("Invalid file");
                return false;
            }

        }
        public UInt16 Read8(Int32 address)
        {
            //address &= 0x3fffff;
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (UInt16)rom[address++];
        }
        public UInt16 Read8(ref Int32 address)
        {
            //address &= 0x3fffff;
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (UInt16)rom[address++];
        }
        public UInt16 Read16(Int32 address)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (UInt16)(
                (rom[address++] << 0) |
                (rom[address++] << 8));
        }
        public int Read16Signed(Int32 address)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            int @return = (UInt16)(
                (rom[address++] << 0) |
                (rom[address++] << 8));
            return ConvertToSNESInt(@return);
        }
        public UInt16 Read16(ref Int32 address)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (UInt16)(
                (rom[address++] << 0) |
                (rom[address++] << 8));
        }
        public Int32 Read24(ref Int32 address)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (UInt16)(
                (rom[address++] << 0) |
                (rom[address++] << 8) |
                (rom[address++] << 16));
        }
        public Int32 Read32(Int32 address)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (UInt16)(
                (rom[address++] << 0) |
                (rom[address++] << 8) |
                (rom[address++] << 16) |
                (rom[address++] << 24));
        }
        public byte[] ReadSubArray(Int32 address, int size)
        {
            //address &= 0x3fffff;
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            var temp = rom.Skip(address).Take(size).ToArray();
            List<byte> arr = new List<byte>();
            arr.AddRange(temp);
            return arr.ToArray();
        }

        public Int32 Read24(Int32 address)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            return (Int32)(
                (rom[address++] << 0) |
                (rom[address++] << 8) |
                (rom[address++] << 16));
        }
        private int ConvertToSNESInt(int toConvert)
        {
            return toConvert >= 0x8000 ? (0x10000 - toConvert) * -1 : toConvert;
        }

        public void Write8(ref Int32 address, Int32 value)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            // Actually write
            rom[address++] = (byte)(value >> 0);
        }
        public void Write8(Int32 address, Int32 value)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            // Actually write
            rom[address++] = (byte)(value >> 0);
        }
        public void Write16(Int32 address, Int32 value)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            // Actually write
            rom[address++] = (byte)(value >> 0);
            rom[address++] = (byte)(value >> 8);
        }
        public void Write16(ref Int32 address, Int32 value)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            // Actually write
            rom[address++] = (byte)(value >> 0);
            rom[address++] = (byte)(value >> 8);
        }
        public void WriteArr (int address, byte[] arr)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            foreach (var b in arr)
            {
                rom[address++] = b;
            }
        }

        public void WriteString(Int32 address, string str)
        {
            address &= (address > 0x7fffff ? 0x3fffff : 0xffffff);
            foreach (var letter in str)
            {
                rom[address++] = (byte)(letter);
            }
        }
        public void RestoreFromBackup()
        {
            // Make sure rom is clear
            rom = new List<byte>();
            // Copy Over
            rom.AddRange(backupRom);
        }

        // For ROM validation
        private string GetChecksum(byte[] tempArr)
        {
            Int32 checksum = 0;
            foreach (var @byte in tempArr)
                checksum += @byte;
            return checksum.ToString("x");
        }
        // Compare header title
        public bool VerifyROM (byte[] arr, string headerString)
        {
            // Loop through string
            for (int i = 0; i < headerString.Length; i++)
            {
                if (headerString[i] != (char)arr[i])
                {
                    return false;
                }
            }

            return true;
        }


        // Save file
        public void SaveROM(string @string)
        {
            System.IO.File.WriteAllBytes(@string, rom.ToArray());
            MessageBox.Show("Saved!");
            WriteToBackup();

        }

        // Save As file
        public void SaveAsROM()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "ROM file (*.smc)|*.smc;";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(dialog.FileName, rom.ToArray());
                fileName = dialog.FileName;
                

                saved = true;

                sd.Write("File", "Path", fileName);
                sd.SaveRbs();

                MessageBox.Show("Saved!");

                //RestoreFromBackup();

                WriteToBackup();
            }
        }
        public bool IsROMChanged ()
        {
            // Loop through every byte and check
            for (int i = 0; i < rom.Count; i++)
            {
                if (rom[i] != backupRom[i])
                {
                    return true;
                }
            }
            return false;
        }
        private void WriteToBackup ()
        {
            for (int i = 0; i < backupRom.Length; i++)
            {
                backupRom[i] = rom[i];
            }
        }

        public string GetTitle()
        {
            var @return = (fileName.Length > maxFileNameLength) ? fileName.Substring(fileName.Length - maxFileNameLength) : fileName;

            return @return;
        }

        public string GetTitleFromHeader (byte[] arr)
        {
            var @return = "";
            foreach (var @byte in arr)
            {
                @return += (char)@byte;
            }
            return @return;
        }
    }
}
