using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

using Ionic.Zip;
using IarInstaller.IarInstaller.Logic;

namespace IarInstaller.IarInstaller.Logic
{
    public class AppLogic
    {
        static private string PartPackFilePath;
        static private string IARInstallationPath;
        static private Dictionary<string,bool> FolderDict;
        static private Dictionary<string, bool> PartPackDict;
        static private string TempPartpackFolder;
        static private string DeviceName;



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
                MessageBox.Show("Error in the Installation of EWARM in selected Folder or Its a Bug!!", "Error in IAR Install folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (VerifyPartPackZipFile() == false)
            {

                MessageBox.Show("Invalid Zip file!!!", "Error in IAR partpack file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetupPartPackUnzipArea();

            UnzipPartpackInTempFolder();

            if (VerifyPartPackZipFileContent() == false)
            {
                MessageBox.Show("Partpack file is completely Invalid ", "Error in IAR partpack file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            GetDeviceName();

            if (string.IsNullOrEmpty(DeviceName))
            {
                MessageBox.Show("Empty Device name, Re-Install partpack", "Error in User Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }            
            
            
            
            InstallPartPackZipToIAR();

            NotifyUserResult();

        }

        private static void GetDeviceName()
        {
            try
            {
                string[] deviceNameList = Directory.GetDirectories(TempPartpackFolder + "\\config\\devices\\Atmel\\");
                DeviceName = deviceNameList[0];
            }
            catch(Exception e)
            {            
                DeviceName =  Prompt.ShowDialog("Enter Device Name (Folder Name for Partpack in Installation):", "Device Name");
            }                        
        }

        private static void UnzipPartpackInTempFolder()
        {            
            using (ZipFile tempZip = ZipFile.Read(PartPackFilePath))
            {
                tempZip.ExtractAll(TempPartpackFolder);                
            }            
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        private static void SetupPartPackUnzipArea()
        {
            TempPartpackFolder = GetTemporaryDirectory();                        
        }

        public static bool IsCleanIARInstallation()
        {
            return (CheckIfEWARMIsInstalled() && CleanFolderStructureForEwarm());                            
        }

        public static bool CleanFolderStructureForEwarm()
        {
            string[] folderChecks = {"config\\debugger\\Atmel", "config\\devices\\Atmel", "config\\flashloader\\Atmel", "config\\linker\\Atmel", "examples\\Atmel\\", "inc\\Atmel"};            

            FolderDict = new Dictionary<string, bool>();

            return DirectoryVerification(IARInstallationPath, ref FolderDict, folderChecks);            
        }

        private static bool DirectoryVerification(string pathToVerify, ref Dictionary<string, bool> objectDict, string[] folderChecks)
        {
            bool flag = false;

            foreach (string folder in folderChecks)
            {
                try
                {
                    var tempObject = Directory.GetDirectories(pathToVerify + "\\" + folder).Length;
                    objectDict.Add(folder, true);
                    flag = true;
                }
                catch (Exception e)
                {
                    objectDict.Add(folder, false);
                }
            }

            return flag;

        }

        private static void NotifyUserResult()
        {
            throw new NotImplementedException();
        }

        private static bool InstallPartPackZipToIAR()
        {
            bool flagInstall = CheckWhetherPartpackIsInstalled();
            if (flagInstall == true)
            {
                DialogResult userInp = MessageBox.Show("Partpack Already Installed in Your computer. Re-Install?", "Already Installed!", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (userInp == DialogResult.Cancel)
                    return false;
            }
            else
            {
                CopyFilesFromPartpackToIAR();
                return true;
            }
            return true;
        }

        private static void CopyFilesFromPartpackToIAR()
        {
            foreach(string folder in FolderDict.Keys)
            {
                if (PartPackDict.Keys.Contains(folder) && PartPackDict[folder] == true && FolderDict[folder] == true)
                {
                    MakeDeviceFolderInIAR(folder+ "\\" +DeviceName);
                    string tempDeviceName = getPartpackDeviceFolder(folder);
                    string copyDeviceName = DeviceName;

                    if (DeviceName.ToLower() == tempDeviceName.ToLower() && DeviceName != tempDeviceName)
                    {
                        copyDeviceName = tempDeviceName;
                    }

                    CopyFilesToNewFolder(folder + "\\" + copyDeviceName, folder + "\\" + tempDeviceName);
                }                
            }
        }

        private static string getPartpackDeviceFolder(string sFolder)
        {
            try
            {
                string[] listDir = Directory.GetDirectories(TempPartpackFolder + "\\" + sFolder);
                if (listDir.Length == 1)
                {
                    return listDir[0];
                }
                else
                {
                    MessageBox.Show("Bug!! Better Call Vishnu " + sFolder);
                    return null;
                }
            }
            catch (Exception e)
            {

                MessageBox.Show("Some Exception Wanna Check");
                return null;
            }

        }

        private static void CopyFilesToNewFolder(string dFolder, string sFolder)
        {

            DirectoryInfo sourcedinfo = new DirectoryInfo(TempPartpackFolder + "\\" + sFolder);
            DirectoryInfo destinfo = new DirectoryInfo(IARInstallationPath + "\\" + dFolder);
            GenerarlHelp.CopyAll(sourcedinfo, destinfo);            
        }

        public static void MakeDeviceFolderInIAR(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    return;
                }
                string tempDir = Directory.GetCurrentDirectory();
                IARInstallationPath = @"C:\Program Files (x86)\IAR Systems\Embedded Workbench 6.5\arm\";

                Directory.SetCurrentDirectory(IARInstallationPath);

                Directory.CreateDirectory( folder);
                Directory.SetCurrentDirectory(tempDir);
            }
            catch (Exception e)
            {
                MessageBox.Show("Some Bug, Go and Tell Vishnu!!!");
            }
        }

        private static bool CheckWhetherPartpackIsInstalled()
        {
            try
            {
                Directory.GetDirectories(IARInstallationPath + "\\config\\devices\\Atmel\\" + DeviceName);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        private static bool VerifyPartPackZipFileContent()
        {
            PartPackDict = new Dictionary<string, bool>();

            return DirectoryVerification(IARInstallationPath, ref PartPackDict, FolderDict.Keys.ToArray<string>());     
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
            try
            {
                ZipFile zipCheck = ZipFile.Read(PartPackFilePath);
                
                return true;
                
            }

            catch (Exception e)
            {

                return false;
            }

            
        }

        public static bool  FindIarInstallationDirectory()
        {
            string checkRegistryKey1 = @"SOFTWARE\Wow6432Node\IAR Systems";
            string checkRegistryKey2 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{4D7DD9A6-4461-4CBE-AB31-F3949AB55BC1}";


            string keyData = "Null";

            using (RegistryKey tempKey = Registry.LocalMachine.OpenSubKey(checkRegistryKey2))
            {
                keyData = tempKey.GetValue("InstallLocation").ToString();
            }

//            keyData = "Null";

            if (string.IsNullOrEmpty(keyData))
            {
                string dataFromUser = GetInstallFolderFromUser();
                keyData = dataFromUser.Length == 0 ? "null" : dataFromUser;
            }
            IARInstallationPath = keyData;

            return (string.IsNullOrEmpty(keyData)) ? false : true;            
        }
    }
}
