using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SRUL
{
    static class Program
    {
        private static SRLoaderForm? _loaderForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>s
        
        // Merge .dll assemblies

        [STAThread]
        static void Main()
        {
            // AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
            //     String resourceName = "AssemblyLoadingAndReflection." +
            //                           new AssemblyName(args.Name).Name + ".dll";
            //     Debug.WriteLine(resourceName, "AssemblyResolve");
            //
            //     foreach (var vrb in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            //     {
            //         Debug.WriteLine(vrb.Name, "AssemblyResolve");
            //         using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(vrb.Name + ".dll")) {
            //             Byte[] assemblyData = new Byte[stream.Length];
            //             stream.Read(assemblyData, 0, assemblyData.Length);
            //             Assembly.Load(assemblyData);
            //         }   
            //     }
            //     
            //     return null;
            // };
            
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
                    if(_loaderForm != null)
                        if(!SRLoaderForm._srLoader.apis.IsOfflineMode()) 
                            SRLoaderForm._srLoader.apis.PostOfflineStatus();
                }
                catch (Exception e)
                {
                    throw new Exception($"{e}");
                }
            };
            
            // TODO: (v3) Avoiding DPA Allocation Issues
            if(_loaderForm == null || _loaderForm.IsDisposed)
                _loaderForm = new SRLoaderForm();
            Application.Run(_loaderForm);
        }
    }
}
