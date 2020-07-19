using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StandAloneGFXDKC1
{
    public class StoredData
    {
        /*
            This class emulates an ini file. All data
            is kept in dictionaries and represented
            as strings
        */

        private string name = "SDName.rbs";
        Dictionary<string, Dictionary<string, string>> data;

        public StoredData()
        {
            string[] lines = LoadFile();
            data = Convert(lines);
        }
        // Read file
        private string[] LoadFile()
        {
            string[] lines = new string[] { };
            // If it doesn't exist, create it
            try
            {
                lines = File.ReadAllLines(name);
                return lines;
            }
            catch (Exception)
            {
                System.IO.File.WriteAllText(name, "");
                return lines;

            }
        }
        // Convert to dictionary format
        private Dictionary<string, Dictionary<string,string>> Convert(string[] lines)
        {
            Dictionary<string, Dictionary<string, string>> @return = new Dictionary<string, Dictionary<string, string>>();

            string category = "";
            // Loop through lines
            foreach (var line in lines)
            {
                // Do 1 of 3 things based on contents
                if (line == "")
                {
                    continue;
                }
                else if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // New category
                    category = line.Substring(1, line.Length - 2);
                    //category = line;
                    @return[category] = new Dictionary<string, string>();
                    continue;
                }
                else
                {
                    // Line of format "id=value"
                    string[] split = line.Split('=');
                    string id = split[0].Trim(),
                           value = split[1].Trim();
                    @return[category][id] = value;
                }

            }
            return @return;
        }
        // Read .rbs
        public string Read(string cat, string id)
        {
            try
            {
                return data[cat][id];
            }
            catch
            {
                //MessageBox.Show("Not present");
                return "";
            }
        }
        // Read category
        public Dictionary<string, string> ReadCategory(string cat)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();

            try
            {

                // Set up as a dictionary of dictionaries
                foreach (var line in data[cat])
                {
                    string key = line.Key;
                    string value = line.Value;
                    temp[key] = value;
                }
            }
            catch
            {
                //MessageBox.Show("Not present");
            }
            return temp;
        }
        public void Write(string cat, string id, string value)
        {
            // Add if not present
            if (!data.ContainsKey(cat))
            {
                data[cat] = new Dictionary<string, string>();
            }
            data[cat][id] = value;
        }
        public void SaveRbs()
        {
            // Write dictionary
            System.IO.File.WriteAllLines(name, GetDAsLnes());
        }

        public void RefreshRbs()
        {
            string[] lines = LoadFile();
            data = Convert(lines);
        }

        private string[] GetDAsLnes()
        {
            List<string> @return = new List<string>();

            // Loop through each category
            foreach (var cat in data.Keys)
            {
                @return.Add($"[{cat}]");
                // Then through each id
                foreach (var pair in data[cat])
                {
                    string temp = $"{pair.Key}={pair.Value}";
                    @return.Add(temp);
                }
                @return.Add("");
            }

            return @return.ToArray();
        }

    }
}
