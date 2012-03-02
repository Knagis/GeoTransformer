/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GeoTransformer
{
    static class Program
    {
        public static Data.TransformerSchema.TransformerSchema Database
        {
            get
            {
                return Data.TransformerSchema.TransformerSchema.Instance;
            }
        }

        /// <summary>
        /// Sets the Internet Explorer document mode to IE9 in the registry for embedded browser controls.
        /// </summary>
        private static void SetInternetExplorerDocumentMode()
        {
            try
            {
                using (var regRead = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"))
                {
                    if (regRead.GetValue("GeoTransformer.exe") as int? != 9000)
                        using (var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true))
                            regkey.SetValue("GeoTransformer.exe", 9000, Microsoft.Win32.RegistryValueKind.DWord);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            System.Net.ServicePointManager.Expect100Continue = false;

            if (AutoUpdater.PerformUpdate(args))
                return;

            SetInternetExplorerDocumentMode();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new MainForm();
            Extensions.ExtensionLoader.ApplicationService = new ApplicationService(form);

            Application.Run(form);

            Database.Database().Dispose();

            // unfortunately there are still some COM object leaks in csExWB. this will make sure that the process does not stay
            // active after the user has closed the main form. 
            // Even though csExWB is no longer used we still prefer this approach because it makes sure that no extension can force
            // the application to be stuck in memory.
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var fname = "Error-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
            try
            {
                System.IO.File.WriteAllText(System.IO.Path.Combine(Application.StartupPath, fname), e.ExceptionObject.ToString());
            }
            catch { }

            MessageBox.Show("Unfortunately the application has crashed. Please send this error message to the developer (the message is also saved in the application folder in the file named " + fname+ "." + Environment.NewLine + Environment.NewLine + e.ExceptionObject.ToString(), "GeoTransformer - unhandled exception", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
    }
}
