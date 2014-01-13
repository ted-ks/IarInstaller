using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace IarInstaller.IarInstaller.Logic
{
    class AppLogic
    {
        static private string PartPackFilePath;
        static private string IARInstallationPath;
        static private Dictionary<string,bool> FolderDict;



        public static void SetPartPackFilePathFromForm(string partPackPath)
        {
            PartPackFilePath = partPackPath;
        }

        public static string  GetInstallFolderFromUser()
        {
            MessageBox.Show("Please select the IAR installation directory", "Select IAR install folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            FolderBrowserDialog selectIARInstalDir = new FolderBrowserDialog();
            DialogResult resultDialog = selectIARInstalDir.ShowDialog();

            return (selectIARInstalDir.SelectedPath);   
        }

        public static void InstallPartPackToTheSystem()
        {
            bool IsIarInstalled;

            IsIarInstalled = FindIarInstallationDirectory();

            if (IsIarInstalled == false)
            {
                return;
            }

            if (IsCleanIARInstallation() == false)
            {
                MessageBox.Show("Error in the Installation of EWARM in selected Folder or Its a Bug!!!", "Error in IAR Install folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 

            VerifyPartPackZipFile();

            VerifyPartPackZipFileContent();

            InstallPartPackZipToIAR();

            NotifyUserResult();

        }

        public static bool IsCleanIARInstallation()
        {
            return (CheckIfEWARMIsInstalled() && CleanFolderStructureForEwarm());                            
        }

        private static bool CleanFolderStructureForEwarm()
        {
            string[] folderChecks = {"config", "examples", "inc", "config\\debugger\\Atmel", "config\\devices\\Atmel", "config\\flashloader\\Atmel", "config\\linker\\Atmel", "examples\\Atmel\\", "inc\\Atmel"};
            bool flag = false;

            foreach(string folder in folderChecks)
            {                
                if(Directory.GetDirectories(IARInstallationPath + "\\" + folderChecks).Length == 0)
                {
                    FolderDict.Add(folder, false);                    
                }
                else
                {
                    FolderDict.Add(folder, true);
                    flag = true;
                }
            }
            return flag;            
        }

        private static void NotifyUserResult()
        {
            throw new NotImplementedException();
        }

        private static void InstallPartPackZipToIAR()
        {
            throw new NotImplementedException();
        }

        private static void VerifyPartPackZipFileContent()
        {
            throw new NotImplementedException();
        }

        private static bool CheckIfEWARMIsInstalled()
        {
            string tempName = FindIARVersionInstalled();
            IARInstallationPath = tempName;
       
            return tempName == "Null"? false : true;
        }

        private static string FindIARVersionInstalled()
        {
            string tempPath = IARInstallationPath;
            string lastFolder = Path.GetFileName(tempPath);
            string returnPath = "Null";

            
            if (lastFolder == "IAR Systems")
            {
                
                
                string[] IARFolderContents = Directory.GetDirectories(tempPath);

                
                foreach (string folder in IARFolderContents)
                {
                    string highestVersion = "Embedded Workbench 0.0";
                    
                    if (folder.IndexOf("Embedded Workbench") != -1)
                    {
                        if(string.Compare(Path.GetFileName(folder), highestVersion) == 1)
                        {
                            highestVersion = Path.GetFileName(folder);
                        }
                    }

                    returnPath = tempPath + "\\" + highestVersion + "\\arm";
                }



            }
            else if (lastFolder.IndexOf("Embedded Workbench") != -1)
            {
                returnPath = tempPath + "\\arm";

            }
            else if (lastFolder == "arm" && tempPath.IndexOf("IAR Systems") != -1)
            {
                returnPath = tempPath;
            }  
            
            return returnPath;
        }

        private static bool VerifyPartPackZipFile()
        {
            return false;
        }

        private static bool  FindIarInstallationDirectory()
        {
            string checkRegistryKey1 = @"SOFTWARE\Wow6432Node\IAR Systems";
            string checkRegistryKey2 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{4D7DD9A6-4461-4CBE-AB31-F3949AB55BC1}";


            string keyData = "Null";

            using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(checkRegistryKey2))
            {
                keyData = tempKey.GetValue("InstallLocation").ToString();
            }

            keyData = "Null";

            if (keyData == "Null")
            {
                string dataFromUser = GetInstallFolderFromUser();
                keyData = dataFromUser.Length == 0 ? "null" : dataFromUser;
            }
            IARInstallationPath = keyData;

            return (keyData == "null") ? false : true;            
        }
    }
}
