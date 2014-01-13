using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace IarInstaller.IarInstaller.Logic
{
    public class FileAssociation
    {
        public static void AssociateIconAndApplication(string extension, string progId, string description, string icon, string application)
        {

            Registry.ClassesRoot.CreateSubKey(extension).SetValue("", progId);

            if (progId.Length > 0)
            {
                using (RegistryKey tempKey = Registry.ClassesRoot.CreateSubKey(progId))
                {
                    tempKey.SetValue("", description);
                    tempKey.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
                    tempKey.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(application) + " \"%1\"");
                }
            }
        }

        public static bool IsAssociated(string extension)
        {
            return (Registry.ClassesRoot.OpenSubKey(extension, false) != null);
        }

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath, [Out] StringBuilder lpszShortPath, uint cchBuffer);

        private static object ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}
