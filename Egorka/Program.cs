using System;
using System.Threading;
using System.Windows.Forms;

namespace EgorkaGame.Egorka
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = EgorkaSettings.Instance.CultureInfo;

            Application.Run(new FullScreenForm());
        }
    }
}
