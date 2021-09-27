using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Daybreak.Core;
using Daybreak.Win32;

namespace OculusDriverInstaller
{
	internal abstract class Driver
	{
		protected string InfName;

		protected string InfLoc;

		protected string CatName;

		protected string GUID;

		protected string[] DriverDesc;

		protected string SystemManagementName;

		public string ReadableName;

		protected bool DevPresentOnInstall;

		public bool NeedsRebootAfterOperation;

		[DllImport("Newdev.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DiInstallDriver(IntPtr hwndParent, [MarshalAs(UnmanagedType.LPTStr)] string DriverPackageInfPath, int Flags, out bool pNeedReboot);

		[DllImport("DIFxAPI.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DriverPackageInstall([MarshalAs(UnmanagedType.LPTStr)] string DriverPackageInfPath, int Flags, IntPtr pInstallerInfo, out bool pNeedReboot);

		[DllImport("DIFxAPI.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DriverPackageUninstall([MarshalAs(UnmanagedType.LPTStr)] string DriverPackageInfPath, int Flags, IntPtr pInstallerInfo, out bool pNeedReboot);

		public virtual bool Install()
		{
			string text = Path.Combine(InfLoc, InfName);
			Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning installation of " + ReadableName + " - Driver package '" + text + "'");
			bool pNeedReboot = false;
			int num = DriverPackageInstall(text, 8, IntPtr.Zero, out pNeedReboot);
			DevPresentOnInstall = num == 0;
			Lumberjack.Log(Lumberjack.Severity.Debug, $"DevPresentOnInstall = {DevPresentOnInstall}");
			switch (num)
			{
			case -536870389:
				Lumberjack.Log(Lumberjack.Severity.Info, "DriverPackageInstall(DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT) result: ERROR_NO_SUCH_DEVINST - Device not detected (not plugged in) right now");
				break;
			case 259:
				Lumberjack.Log(Lumberjack.Severity.Info, "DriverPackageInstall(DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT) result: ERROR_NO_MORE_ITEMS - Device already detected");
				break;
			default:
				Lumberjack.Log(Lumberjack.Severity.Debug, $"DriverPackageInstall(DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT) result: {num}");
				break;
			}
			bool flag;
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) > Constants.Windows.Versions.Win7)
			{
				flag = DiInstallDriver(IntPtr.Zero, text, 2, out pNeedReboot);
				num = Marshal.GetLastWin32Error();
				if (num == 259)
				{
					Lumberjack.Log(Lumberjack.Severity.Info, "DiInstallDriver() result: ERROR_NO_MORE_ITEMS - Device already detected");
					flag = true;
				}
			}
			else
			{
				num = DriverPackageInstall(text, 0, IntPtr.Zero, out pNeedReboot);
				switch (num)
				{
				case 0:
					Lumberjack.Log(Lumberjack.Severity.Info, "DriverPackageInstall() result: Success");
					flag = true;
					break;
				case -536870389:
					Lumberjack.Log(Lumberjack.Severity.Info, "DriverPackageInstall() result: ERROR_NO_SUCH_DEVINST - Device not detected (not plugged in) right now");
					flag = true;
					break;
				case 259:
					Lumberjack.Log(Lumberjack.Severity.Info, "DriverPackageInstall() result: ERROR_NO_MORE_ITEMS - Device already detected");
					flag = true;
					break;
				default:
					Lumberjack.Log(Lumberjack.Severity.Debug, $"DriverPackageInstall() result: {num}");
					flag = false;
					break;
				}
			}
			if (pNeedReboot)
			{
				NeedsRebootAfterOperation = true;
				Lumberjack.Log(Lumberjack.Severity.Info, "Reboot required after installation of " + ReadableName + " : DriverPackageInstall " + text + " requested reboot");
			}
			if (flag)
			{
				Lumberjack.Log(Lumberjack.Severity.Info, "DiInstallDriver() Successful install");
				num = 0;
			}
			else
			{
				string text2 = new Win32Exception(num).Message;
				switch (num)
				{
				case -536870334:
					text2 = $"ERROR_AUTHENTICODE_TRUST_NOT_ESTABLISHED (0x{num:X})";
					break;
				case -536870333:
					text2 = $"ERROR_AUTHENTICODE_PUBLISHER_NOT_TRUSTED (0x{num:X})";
					break;
				case -536870330:
					text2 = $"ERROR_DEVICE_INSTALLER_NOT_READY (0x{num:X})";
					break;
				case -536870329:
					text2 = $"ERROR_DRIVER_STORE_ADD_FAILED (0x{num:X})";
					break;
				case -536870325:
					text2 = $"ERROR_FILE_HASH_NOT_IN_CATALOG (0x{num:X})";
					break;
				}
				Lumberjack.Log(Lumberjack.Severity.Error, "DiInstallDriver() error message: " + text2);
				ODIAnalytics.LogMessage(text2, num, Constants.InstallStep.InstallDriver, ReadableName, Lumberjack.Severity.Error, 159, "Install", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending installation of " + ReadableName);
			return num == 0;
		}

		public virtual bool Uninstall()
		{
			string text = Path.Combine(InfLoc, InfName);
			Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning uninstallation of: " + ReadableName + " - Driver package '" + text + "'");
			bool pNeedReboot = false;
			int num = DriverPackageUninstall(text, 4, IntPtr.Zero, out pNeedReboot);
			if (pNeedReboot)
			{
				NeedsRebootAfterOperation = true;
				Lumberjack.Log(Lumberjack.Severity.Info, "Reboot required after uninstallation of " + ReadableName + " : Main DriverPackageUninstall " + text + " requested reboot");
			}
			num = ((num != -536870142) ? num : 0);
			if (num != 0)
			{
				string message = new Win32Exception(num).Message;
				Lumberjack.Log(Lumberjack.Severity.Warning, "DriverPackageUninstall() return code: " + message);
				ODIAnalytics.LogMessage(message, num, Constants.InstallStep.UninstallDriver, ReadableName, Lumberjack.Severity.Error, 192, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			bool flag = false;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(InfName);
			string[] directories = Directory.GetDirectories(Constants.Windows.DriverStagingPath, fileNameWithoutExtension + "*");
			for (int i = 0; i < directories.Length; i++)
			{
				string[] files = Directory.GetFiles(directories[i], InfName);
				foreach (string text2 in files)
				{
					pNeedReboot = false;
					int num2 = DriverPackageUninstall(text2, 4, IntPtr.Zero, out pNeedReboot);
					if (num2 != 0)
					{
						flag = true;
						ODIAnalytics.LogMessage(new Win32Exception(num2).Message, num, Constants.InstallStep.UninstallDriver, ReadableName + ": remove old driver versions", Lumberjack.Severity.Warning, 216, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
					}
					if (pNeedReboot)
					{
						NeedsRebootAfterOperation = true;
						Lumberjack.Log(Lumberjack.Severity.Info, "Reboot required after uninstallation of " + ReadableName + " : DriverPackageUninstall of " + text2 + " requested reboot");
					}
				}
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending uninstallation of: " + ReadableName);
			if (num == 0)
			{
				return !flag;
			}
			return false;
		}

		protected bool DevCon(string cmd, string devicePath, Constants.InstallStep installStep)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to invoke devcon with cmd: " + cmd + ", in install step: " + installStep);
			try
			{
				Process process = new Process();
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.StartInfo.FileName = Path.Combine(Constants.Windows.TempDirectory, "devcon.exe");
				process.StartInfo.Arguments = cmd + " \"" + InfName + "\" " + devicePath;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.WorkingDirectory = Constants.Windows.TempDirectory;
				if (!process.Start())
				{
					Lumberjack.Log(Lumberjack.Severity.Error, "Failed to start process");
				}
				else
				{
					process.WaitForExit();
					string text = process.StandardOutput.ReadToEnd();
					string text2 = process.StandardError.ReadToEnd();
					Lumberjack.Log(Lumberjack.Severity.Debug, "StdOut: " + text);
					Lumberjack.Log(Lumberjack.Severity.Debug, "StdErr: " + text2);
				}
				string msg = "";
				switch (process.ExitCode)
				{
				case 1:
					msg = "Requires Reboot";
					break;
				case 2:
					msg = "Failure";
					break;
				case 3:
					msg = "Syntax Error";
					break;
				default:
					msg = "Unknown Error";
					break;
				case 0:
					break;
				}
				if (process.ExitCode != 0)
				{
					ODIAnalytics.LogMessage(msg, process.ExitCode, installStep, "devcon: " + ReadableName, Lumberjack.Severity.Error, 291, "DevCon", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
					return false;
				}
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message + " : " + ex.StackTrace, ex.GetType().Name, installStep, "devcon: " + ReadableName, Lumberjack.Severity.Error, 302, "DevCon", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
				return false;
			}
			DevPresentOnInstall = true;
			Lumberjack.Log(Lumberjack.Severity.Debug, "Devcon completed successfully");
			return true;
		}

		public virtual bool CheckInstallHealth()
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning check install health of: " + ReadableName);
			bool result = true;
			Lumberjack.Log(Lumberjack.Severity.Debug, "Checking if oem*.inf exists");
			if (Mixins.GetOEMName(CatName) == null)
			{
				result = false;
				ODIAnalytics.LogMessage("Couldn't find matching oem*.inf file in " + Constants.Windows.INFPath, -1, Constants.InstallStep.CheckInstallHealth, "GetOEMName: " + ReadableName, Lumberjack.Severity.Warning, 329, "CheckInstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			else
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "oem*.inf exists");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Checking if the staging directory exists");
			string infDriverStorePath;
			int num = Mixins.DriverStagingExists(Path.Combine(InfLoc, InfName), out infDriverStorePath);
			if (num != 0)
			{
				result = false;
				ODIAnalytics.LogMessage("Couldn't find driver in the driver store", num, Constants.InstallStep.CheckInstallHealth, "DriverPackageGetPath: " + ReadableName, Lumberjack.Severity.Warning, 344, "CheckInstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			else
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "The staging directory exists");
			}
			if (DevPresentOnInstall && !NeedsRebootAfterOperation)
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "Checking if " + ReadableName + " is fully installed");
				if (SystemManagementName != null && !Mixins.DoesDriverExist(SystemManagementName))
				{
					result = false;
					ODIAnalytics.LogMessage("Driver does not exist", -1, Constants.InstallStep.CheckInstallHealth, "DoesDriverExist: " + ReadableName, Lumberjack.Severity.Warning, 362, "CheckInstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
				}
				try
				{
					Lumberjack.Log(Lumberjack.Severity.Debug, "Checking if the registry entry exists");
					bool flag = false;
					string[] driverDesc = DriverDesc;
					foreach (string value in driverDesc)
					{
						if (Mixins.RegistrySubKeyExists("SYSTEM\\ControlSet001\\Control\\Class\\" + GUID, "DriverDesc", value))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						result = false;
						ODIAnalytics.LogMessage("Registry entry does not exist", -1, Constants.InstallStep.RegistryCheck, ReadableName + ": SYSTEM\\ControlSet001\\Control\\Class\\" + GUID, Lumberjack.Severity.Warning, 383, "CheckInstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
					}
					else
					{
						Lumberjack.Log(Lumberjack.Severity.Debug, "The registry entry exists");
					}
				}
				catch (Exception ex)
				{
					result = false;
					ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.CheckInstallHealth, ReadableName + ": check registry subkey exists", Lumberjack.Severity.Warning, 397, "CheckInstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
				}
			}
			string text = ((num == 0) ? "HEALTHY" : "BAD");
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending check install health of: " + ReadableName);
			Lumberjack.Log(Lumberjack.Severity.Info, "Health status: " + text);
			return result;
		}

		public virtual bool CheckUninstallHealth()
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning check uninstall health of: " + ReadableName);
			bool result = true;
			if (Mixins.GetOEMName(CatName) != null)
			{
				result = false;
				ODIAnalytics.LogMessage("Found matching oem*.inf file in " + Constants.Windows.INFPath, -1, Constants.InstallStep.CheckUninstallHealth, "GetOEMName: " + ReadableName, Lumberjack.Severity.Warning, 420, "CheckUninstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			string infDriverStorePath;
			int num = Mixins.DriverStagingExists(Path.Combine(InfLoc, InfName), out infDriverStorePath);
			if (num != -536870142)
			{
				result = false;
				ODIAnalytics.LogMessage("Found driver in the driver store", num, Constants.InstallStep.CheckUninstallHealth, "DriverPackageGetPath: " + ReadableName, Lumberjack.Severity.Warning, 432, "CheckUninstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			if (SystemManagementName != null && Mixins.DoesDriverExist(SystemManagementName))
			{
				result = false;
				ODIAnalytics.LogMessage("Driver exists", -1, Constants.InstallStep.CheckUninstallHealth, "DoesDriverExist: " + ReadableName, Lumberjack.Severity.Warning, 441, "CheckUninstallHealth", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Driver.cs");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending check uninstall health of: " + ReadableName);
			Lumberjack.Log(Lumberjack.Severity.Info, $"Health status: {num}");
			return result;
		}

		public abstract void ExtractFiles();
	}
}
