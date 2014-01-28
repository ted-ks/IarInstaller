using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Windows.Forms;

namespace IarInstaller.IarInstaller.Logic
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.ShowDialog();
            return textBox.Text;
        }
    }

    public static class ShowResultsDialog
    {

        public static void ShowResultsDialogWindow(string caption, Dictionary<string, bool> installData)
        {
            Form dialogWindow = new Form();

            dialogWindow.Width = 500;
            dialogWindow.Height = 400;

            dialogWindow.Text = caption;

            Label textLabel = new Label() { Left = 50, Top = 20, Text = "Install Summary" };
            TextBox installResult = new TextBox() { Left = 50, Top = 50, Width = 400};
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 320 };
            installResult.Multiline = true;
            installResult.AcceptsReturn = true;
            installResult.ScrollBars = ScrollBars.Vertical;
            installResult.WordWrap = true;
            installResult.Height = 200;

            
            foreach(string folder in installData.Keys)
            {
                if (installData[folder] == true)
                {
                    installResult.AppendText("Copied - " + folder +"\r\n");
                }
            }
            installResult.Enabled = false;
            confirmation.Click += (sender, e) => { dialogWindow.Close(); };
            dialogWindow.Controls.Add(confirmation);
            dialogWindow.Controls.Add(installResult);
            dialogWindow.Controls.Add(textLabel);
            dialogWindow.ShowDialog();
            return;
        }

    }

    public static class GenerarlHelp
    {
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {

                
                //check if the target directory exists
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);                    
                }

                //copy all the files into the new directory

                foreach (FileInfo fi in source.GetFiles())
                {
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                }


                //copy all the sub directories using recursion

                foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                    CopyAll(diSourceDir, nextTargetDir);
                }
                //success here
            }
            catch (Exception ie)
            {
                MessageBox.Show("Some Exception Occured!!!" + ie.Message);
            }
        }

    }
}
