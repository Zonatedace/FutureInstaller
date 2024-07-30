using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FutureInstaller
{
    public partial class Form3 : Form
    {

        private WindowsDefenderExceptionManager exceptionManager;

        public Form3()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            exceptionManager = new WindowsDefenderExceptionManager();


        }

       

        private async void Form3_Load_1(object sender, EventArgs e)
        {
            // Example usage of the exception manager
            progressBar1.Value = 10;

            await exceptionManager.ChangeNetworkProfileAsync();

            progressBar1.Value = 30;

            await exceptionManager.AddInstallFolderExceptionAsync("A:\\FPOS");
            await exceptionManager.AddInstallFolderExceptionAsync("A:\\FPOS5");
            await exceptionManager.AddInstallFolderExceptionAsync("A:\\FPOS6");
            await exceptionManager.AddInstallFolderExceptionAsync("B:\\FPOS");
            await exceptionManager.AddInstallFolderExceptionAsync("B:\\FPOS5");
            await exceptionManager.AddInstallFolderExceptionAsync("B:\\FPOS6");
            await exceptionManager.AddInstallFolderExceptionAsync("C:\\FPOS");
            await exceptionManager.AddInstallFolderExceptionAsync("C:\\FPOS5");

            progressBar1.Value = 40;

            

            await exceptionManager.AddInstallFolderExceptionAsync("C:\\FPOS6");
            await exceptionManager.AddInstallFolderExceptionAsync("D:\\FPOS");
            await exceptionManager.AddInstallFolderExceptionAsync("D:\\FPOS5");
            await exceptionManager.AddInstallFolderExceptionAsync("D:\\FPOS6");
            await exceptionManager.AddInstallFolderExceptionAsync("E:\\FPOS");
            await exceptionManager.AddInstallFolderExceptionAsync("E:\\FPOS5");
            await exceptionManager.AddInstallFolderExceptionAsync("E:\\FPOS6");
            await exceptionManager.AddInstallFolderExceptionAsync("F:\\FPOS");
            await exceptionManager.AddInstallFolderExceptionAsync("F:\\FPOS5");
            await exceptionManager.AddInstallFolderExceptionAsync("F:\\FPOS6");

            progressBar1.Value = 65;

            await exceptionManager.AddFirewallRulesAsync();

            progressBar1.Value = 100;

            await Task.Delay(1000);
            this.Close();
            
        }
    }
}
