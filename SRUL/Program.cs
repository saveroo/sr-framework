using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SRUL
{
    static class Program
    {
        private static SRLoaderForm _loaderForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>s
        [STAThread]
        static void Main()
        {
            WindowsFormsSettings.LoadApplicationSettings();
            
            WindowsFormsSettings.ForceDirectXPaint();
            // WindowsFormsSettings.EnableFormSkins();
            // SkinManager.EnableFormSkins();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.ApplicationExit += (sender, args) =>
            {
                try
                {
                    SRLoaderForm._srLoader.apis.PostOfflineStatus();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };
            
            // TODO: (v3) Avoiding DPA Allocation Issues
            if(_loaderForm == null)
                _loaderForm = new SRLoaderForm();
            Application.Run(_loaderForm);
        }
    }
}
