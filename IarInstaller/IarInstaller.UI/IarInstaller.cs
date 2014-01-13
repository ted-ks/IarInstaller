using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using IarInstaller.IarInstaller.Logic;

namespace IarInstaller
{
    public partial class IarPartPackInstaller : Form
    {
        public IarPartPackInstaller()
        {
            InitializeComponent();                        
        }

        public IarPartPackInstaller(string partPackFile)
        {            
            InitializeComponent();
            this.PartPackName.Text = Path.GetFileName(partPackFile);
            AppLogic.SetPartPackFilePathFromForm(partPackFile);
            if (partPackFile == "Nothing To install")
            {
                this.InstallButton.Enabled = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            AppLogic.InstallPartPackToTheSystem();
        }
    }
}
