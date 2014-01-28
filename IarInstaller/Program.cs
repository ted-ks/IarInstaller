using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows.Forms;

using IarInstaller.IarInstaller.Logic;

namespace IarInstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            
            if (!FileAssociation.IsAssociated(".iar"))
            {
                FileAssociation.AssociateIconAndApplication (".iar", "IarPartPackInstaller", "iar File", @"octave-logo.ico", Application.ExecutablePath);
                MessageBox.Show("File Assiciation added", "Iar Part Pack Installer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();                           
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);                
                

                Application.Run(new IarPartPackInstaller((args.Length > 0) ? args[0] : "Nothing To install"));                
            }
        }
    }
}
