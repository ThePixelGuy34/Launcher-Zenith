using IniParser;
using IniParser.Model;
using System;
using System.IO;

namespace UML.Services
{
    public static class UpdateINI
    {
        private static readonly string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static readonly string DataFolder = Path.Combine(BaseFolder, "Unknown Multiplayer Launcher");
        private static readonly string FilePath = Path.Combine(DataFolder, "config.ini");

        public static void WriteToConfig(string SectionName, string PathKey, string NewValue)
        {
            Directory.CreateDirectory(DataFolder); 

            FileIniDataParser parser = new FileIniDataParser();
            IniData iniData;

            if (File.Exists(FilePath))
            {
                iniData = parser.ReadFile(FilePath);

                if (iniData.Sections.ContainsSection(SectionName))
                {
                    if (iniData[SectionName].ContainsKey(PathKey))
                    {
                        iniData[SectionName].RemoveKey(PathKey);
                    }
                }
            }
            else
            {
                iniData = new IniData();
            }

            iniData[SectionName][PathKey] = NewValue;
            parser.WriteFile(FilePath, iniData);
        }

        public static string ReadValue(string SectionName, string PathKey)
        {
            if (File.Exists(FilePath))
            {
                FileIniDataParser parser = new FileIniDataParser();
                IniData iniData = parser.ReadFile(FilePath);
                if (iniData.Sections.ContainsSection(SectionName) && iniData[SectionName].ContainsKey(PathKey))
                {
                    return iniData[SectionName][PathKey];
                }
            }
            return string.Empty;
        }
    }
}
