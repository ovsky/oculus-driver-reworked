using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Daybreak.Core;
using Daybreak.Util;
using Daybreak.Win32;
using Microsoft.Win32;

namespace OculusDriverInstaller
{
	internal class Mixins
	{
		public static bool RegistrySubKeyExists(string keyName, string valueName, string value)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyName, writable: false))
			{
				if (registryKey == null)
				{
					return false;
				}
				if (registryKey.GetValue(valueName)?.ToString() == value)
				{
					return true;
				}
				string[] subKeyNames = registryKey.GetSubKeyNames();
				foreach (string text in subKeyNames)
				{
					try
					{
						using RegistryKey registryKey2 = registryKey.OpenSubKey(text, writable: false);
						if (registryKey2 == null || !(registryKey2.GetValue(valueName)?.ToString() == value))
						{
							continue;
						}
						return true;
					}
					catch (SecurityException ex)
					{
						ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.RegistryCheck, "Reading registry subkey: " + text, Lumberjack.Severity.Warning, 37, "RegistrySubKeyExists", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Mixins.cs");
					}
				}
			}
			return false;
		}

		public static bool DoesDriverExist(string driverName)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "Checking if the " + driverName + " driver exists in system management");
			try
			{
				ManagementObjectCollection managementObjectCollection = new ManagementObjectSearcher(new SelectQuery("Win32_SystemDriver")
				{
					Condition = "Name = '" + driverName + "'"
				}).Get();
				if (managementObjectCollection == null)
				{
					ODIAnalytics.LogMessage("Could not query list of drivers from system management.", -1, Constants.InstallStep.CheckInstallHealth, driverName, Lumberjack.Severity.Warning, 63, "DoesDriverExist", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Mixins.cs");
					Lumberjack.Log(Lumberjack.Severity.Error, "Could not query list of drivers from system management.");
					return false;
				}
				int count = managementObjectCollection.Count;
				if (count <= 0)
				{
					Lumberjack.Log(Lumberjack.Severity.Error, "Driver could not be found in system management");
					return false;
				}
				if (count == 1)
				{
					Lumberjack.Log(Lumberjack.Severity.Debug, "Driver exists.");
				}
				else if (count > 1)
				{
					ODIAnalytics.LogMessage("More than one instance of the driver was found in system management", -1, Constants.InstallStep.CheckInstallHealth, driverName, Lumberjack.Severity.Warning, 88, "DoesDriverExist", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Mixins.cs");
					Lumberjack.Log(Lumberjack.Severity.Info, "More than one instance of the driver was found in system management");
				}
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message + "\n" + ex.StackTrace, ex.GetType().Name, Constants.InstallStep.CheckInstallHealth, "Exception while querying system management", Lumberjack.Severity.Warning, 103, "DoesDriverExist", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Mixins.cs");
				Lumberjack.Log(Lumberjack.Severity.Warning, "Exception while querying system management: Returning that it exists" + ex.Message + "\n" + ex.StackTrace);
			}
			return true;
		}

		public static bool AddCertificate(string certName)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to add certificate: " + certName);
			try
			{
				X509Certificate2 certificate = new X509Certificate2(certName);
				X509Store x509Store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadWrite);
				x509Store.Add(certificate);
				x509Store.Close();
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.AddCert, certName, Lumberjack.Severity.Warning, 126, "AddCertificate", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Mixins.cs");
				return false;
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Done installing certificate: " + certName);
			return true;
		}

		public static void ReallyDeleteDirectory(string path)
		{
			ReallyDelete(path, () => Directory.Exists(path), delegate
			{
				Directory.Delete(path, recursive: true);
			});
		}

		public static void ReallyDeleteFile(string path)
		{
			ReallyDelete(path, () => File.Exists(path), delegate
			{
				File.Delete(path);
			});
		}

		private static void ReallyDelete(string path, Func<bool> exists, Action function)
		{
			if (exists())
			{
				try
				{
					function();
				}
				catch (Exception ex)
				{
					Kernel.DeleteOnReboot(path);
					throw ex;
				}
			}
		}

		public static int ExecuteProcessInt(string path, string arguments, List<int> additionalErrorCodes = null)
		{
			int? num = Daybreak.Util.Mixins.ExecuteProcessMixin(path, arguments, additionalErrorCodes);
			if (num.HasValue)
			{
				return num.Value;
			}
			return 575;
		}

		public static Constants.Windows.Versions ConvertVersion(Version version)
		{
			switch (version.Major)
			{
			case 10:
				if (version.Minor == 0)
				{
					return Constants.Windows.Versions.Win10;
				}
				throw new InvalidOperationException();
			case 6:
				switch (version.Minor)
				{
				case 1:
					return Constants.Windows.Versions.Win7;
				case 2:
					return Constants.Windows.Versions.Win8;
				case 3:
					if (WindowsEnvironment.OSName.Contains("Windows 10"))
					{
						return Constants.Windows.Versions.Win10;
					}
					return Constants.Windows.Versions.Win8_1;
				default:
					throw new InvalidOperationException();
				}
			default:
				throw new InvalidOperationException();
			}
		}

		public static string VersionToString(Constants.Windows.Versions version)
		{
			if (version == Constants.Windows.Versions.Win8_1)
			{
				return "Win8.1";
			}
			return version.ToString();
		}

		public static string GetOEMName(string searchText)
		{
			try
			{
				string[] files = Directory.GetFiles(Constants.Windows.INFPath, "oem*.inf");
				foreach (string text in files)
				{
					foreach (string item in File.ReadLines(text))
					{
						if (item.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							return text.Substring(text.LastIndexOf('\\') + 1);
						}
					}
				}
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, -1, Constants.InstallStep.CheckInstallHealth, "GetOEMName for " + searchText, Lumberjack.Severity.Warning, 229, "GetOEMName", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Mixins.cs");
			}
			return null;
		}

		[DllImport("DIFxAPI.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int DriverPackageGetPath([MarshalAs(UnmanagedType.LPTStr)] string DriverPackageInfPath, [MarshalAs(UnmanagedType.LPTStr)] string pDestInfPath, out int pNumOfChars);

		public static int DriverStagingExists(string infPath, out string infDriverStorePath)
		{
			int pNumOfChars = 256;
			infDriverStorePath = string.Empty;
			infDriverStorePath = infDriverStorePath.PadLeft(pNumOfChars - 1);
			int num = DriverPackageGetPath(infPath, infDriverStorePath, out pNumOfChars);
			if (num == 122)
			{
				infDriverStorePath = infDriverStorePath.PadLeft(pNumOfChars - 1);
				num = DriverPackageGetPath(infPath, infDriverStorePath, out pNumOfChars);
			}
			return num;
		}
	}
}
