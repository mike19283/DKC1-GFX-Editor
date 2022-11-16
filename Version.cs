using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    static class Version
    {
        private static string versionString = "Version 0.057";
        private static string linkString = "https://pastebin.com/B1dwHzQS";
        public static string downloadLink;
        public static bool currentVersion;

        private static void IsUpdateAvailable()
        {
            try
            {
                    //throw new System.ArgumentException();

                // Get current version from my pastebin
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] raw = wc.DownloadData(linkString);

                // Parse the string
                String webData = System.Text.Encoding.UTF8.GetString(raw);
                    // Split data at my chosen point
                var websiteArr = Regex.Split(webData, "\\>__\\.\\.//\\r\\n");
                string websiteStr = websiteArr[1];
                websiteStr = Regex.Split(websiteStr, "\\<")[0];

                    // Update log consists of version[0] and link[1]
                    string[] log = Regex.Split(websiteStr, "\\r\\n");
                    downloadLink = log[1];

                    currentVersion = !(versionString == log[0]);
            }
                
            catch (Exception ex)
            {
                    MessageBox.Show("Trouble connecting to the internet. You may be running outdated software!");
                    currentVersion = false;
            }

            
        }

        public static void ManualCheck()
        {
            IsUpdateAvailable();

            if (currentVersion)
            {
                WillUserUpdate();
            }
            else
            {
                MessageBox.Show("You are running the current version!");
            }


        }

        public static void OnLoad()
        {

            IsUpdateAvailable();
            if (currentVersion)
            {
                WillUserUpdate();
            }

        }

        public static void WillUserUpdate()
        {
            // Does the user want to update now?
            if (MessageBox.Show("Your version is outdated. Update now?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MessageBox.Show("Please contact RainbowSprinklez on discord (Rainbow #2405) for any updates");
            }
            else
            {
                MessageBox.Show("Not recommended");
            }
        }


        public static string GetVersion() => versionString;

    }
}
