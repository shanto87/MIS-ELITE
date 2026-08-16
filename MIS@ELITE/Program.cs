using PdfSharp.Fonts;
using System;
using System.Windows.Forms;

namespace MIS_ELITE
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
            if (Properties.Settings.Default.IsLoggedIn)
            {
                Application.Run(new Inventory());
            }
            else
            {
                Application.Run(new LoginPage());
            }
            //Application.Run(new Inventory());
        }
    }
}
