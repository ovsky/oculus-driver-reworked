using System;
using System.IO;
using System.Text;
using Daybreak.Core;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class Tracker : Driver
	{
		public Tracker()
		{
			InfName = "OCUSBVID.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "OCUSBVID.Cat";
			GUID = "{A2DB2D36-9CA9-4DD8-9E46-3121A16063D2}";
			DriverDesc = Constants.Rift.Tracker.DriverDesc;
			SystemManagementName = "OCUSBVID";
			ReadableName = "Tracker Driver";
		}

		public override bool Install()
		{
			if (base.Install())
			{
				return CheckInstallHealth();
			}
			return false;
		}

		public override bool Uninstall()
		{
			bool flag = base.Uninstall();
			if (flag)
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "About to delete tracker driver binary");
				try
				{
					File.Delete(Path.Combine(Constants.Windows.DriverBinaryPath, "ocusbvid111.sys"));
				}
				catch (Exception ex)
				{
					ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.UninstallDriver, "Delete ocusbvid111.sys", Lumberjack.Severity.Warning, 35, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\DriverInstallers\\Tracker.cs");
				}
			}
			if (flag)
			{
				return CheckUninstallHealth();
			}
			return false;
		}

		public override void ExtractFiles()
		{
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.OCUSBVIDCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.OCUSBVIDInf));
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "ocusbvid109.sys"), Resources.ocusbvid109);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "ocusbvid111.sys"), Resources.ocusbvid111);
		}
	}
}
