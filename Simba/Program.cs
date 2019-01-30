using System;
using System.Windows.Forms;

namespace Simba
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
         {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SavannahForm savannahForm = new SavannahForm();
            Application.Run(savannahForm);
        }
    }
}
