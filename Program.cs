using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FutureInstaller
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            WindowsDefenderExceptionManager manager = new WindowsDefenderExceptionManager();
            manager.AddException("A:\\fpos");
            manager.AddException("B:\\fpos");
            manager.AddException("C:\\fpos");
            manager.AddException("D:\\fpos");
            manager.AddException("E:\\fpos");
            manager.AddException("A:\\fpos5");
            manager.AddException("B:\\fpos5");
            manager.AddException("C:\\fpos5");
            manager.AddException("D:\\fpos5");
            manager.AddException("E:\\fpos5");
            manager.AddException("A:\\fpos6");
            manager.AddException("B:\\fpos6");
            manager.AddException("C:\\fpos6");
            manager.AddException("D:\\fpos6");
            manager.AddException("E:\\fpos6");
        }
    }
}
