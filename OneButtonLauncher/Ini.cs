using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OneButtonLauncher
{
    public class Ini
    {
        string Path;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileStringW(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileStringW(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public Ini(string fileName)
        {
            Path = fileName;
        }

        public string Read(string section, string key, string defaultValue = "")
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileStringW(section, key, defaultValue, sb, 255, Path);
            return sb.ToString().Trim();
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileStringW(section, key, value, Path);
        }

        public void DeleteKey(string section, string key)
        {
            Write(key, null, section);
        }

        public void DeleteSection(string section)
        {
            Write(null, null, section);
        }

        public bool KeyExists(string section, string key)
        {
            return !string.IsNullOrEmpty(Read(key, section));
        }
    }
}
