using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasteAsFile
{
	static class Program
	{
        static String NON_STOP_ARG = "-nonstop";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (args.Length>0)
			{
				if (args[0] == "/reg")
				{
					RegisterApp(false);
					return;
				}
				else if (args[0] == "/reg-non-stop")
				{
					RegisterApp(true);
					return;
				}
				else if (args[0] == "/unreg")
				{
					UnRegisterApp();
					return;
				}
				else if (args[0] == "/filename")
                {
                    if (args.Length > 1) {
                        RegisterFilename(args[1]);
                    }
                    return;
                }

				// Check for non-stop argument.
				if (args[0] == NON_STOP_ARG)
                {
					// Check if location is provided as well.
					if (args.Length > 1)
                    {
						Application.Run(new frmMain(true, args[1]));
					} else
                    { // No location is provided
						MessageBox.Show("-nonstop argument needs a location.", "Paste As File", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
                } else
                { // Only location is provided
					Application.Run(new frmMain(false, args[0]));
				}
			}
			else
			{
				Application.Run(new frmMain());
			}
			
		}

		public static void RegisterFilename(string filename)
		{
			try
			{
                var key = OpenDirectoryKey().CreateSubKey("shell").CreateSubKey("Paste Into File");
                key = key.CreateSubKey("filename");
                key.SetValue("", filename);

                MessageBox.Show("Filename has been registered with your system", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				//throw;
				MessageBox.Show(ex.Message + "\nPlease run the application as Administrator !", "Paste As File", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static void UnRegisterApp()
		{
			try
			{
				var key = OpenDirectoryKey().OpenSubKey(@"Background\shell", true);
				key.DeleteSubKeyTree("Paste Into File");

				key = OpenDirectoryKey().OpenSubKey("shell", true);
				key.DeleteSubKeyTree("Paste Into File");

				MessageBox.Show("Application has been Unregistered from your system", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\nPlease run the application as Administrator !", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Error);
				
			}
		}

		public static void RegisterApp(Boolean nonStop)
		{
			String nonStopArg = nonStop? NON_STOP_ARG : String.Empty;
			try
			{
				var key = OpenDirectoryKey().CreateSubKey(@"Background\shell").CreateSubKey("Paste Into File");
				key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
				key = key.CreateSubKey("command");
				key.SetValue("" , "\"" + Application.ExecutablePath + "\" " + NON_STOP_ARG + " \"%V\"");

				key = OpenDirectoryKey().CreateSubKey("shell").CreateSubKey("Paste Into File");
				key.SetValue("Icon", "\"" + Application.ExecutablePath + "\",0");
				key = key.CreateSubKey("command");
				key.SetValue("", "\"" + Application.ExecutablePath + "\" " + NON_STOP_ARG + " \"%V\"");
				MessageBox.Show("Application has been registered with your system", "Paste Into File", MessageBoxButtons.OK, MessageBoxIcon.Information);

			}
			catch (Exception ex)
			{
				//throw;
				MessageBox.Show(ex.Message + "\nPlease run the application as Administrator !", "Paste As File", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static void RestartApp()
		{
			ProcessStartInfo proc = new ProcessStartInfo();
			proc.UseShellExecute = true;
			proc.WorkingDirectory = Environment.CurrentDirectory;
			proc.FileName = Application.ExecutablePath;
			proc.Verb = "runas";

			try
			{
				Process.Start(proc);
			}
			catch
			{
				// The user refused the elevation.
				// Do nothing and return directly ...
				return;
			}
			Application.Exit();
		}

		static RegistryKey OpenDirectoryKey()
		{
			return Registry.CurrentUser.CreateSubKey(@"Software\Classes\Directory");
		}
	}
}
