using System;
using System.IO;
using System.Text;
using Daybreak.Core;
using Daybreak.Win32;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class RemoteAudio : Driver
	{
		public const string DevicePath = "Root\\oculusvad_OculusVad";

		public RemoteAudio()
		{
			InfName = "oculusvad.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "oculusvad.cat";
			GUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";
			DriverDesc = Constants.Rift.RemoteAudio.DriverDesc;
			SystemManagementName = "oculusvad_oculusvad";
			ReadableName = "Oculus Virtual Audio Device";
		}

		public override bool Install()
		{
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) == Constants.Windows.Versions.Win10)
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "Installing driver");
				DevCon("remove", "Root\\oculusvad_OculusVad", Constants.InstallStep.InstallDriver);
				if (DevCon("install", "Root\\oculusvad_OculusVad", Constants.InstallStep.InstallDriver))
				{
					return CheckInstallHealth();
				}
				return false;
			}
			Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping installation of remove audio driver because it is only compatible with Windows 10");
			return true;
		}

		public override bool Uninstall()
		{
			_ = !DevCon("remove", "Root\\oculusvad_OculusVad", Constants.InstallStep.UninstallDriver);
			base.Uninstall();
			try
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "About to delete remove audio driver binary");
				File.Delete(Path.Combine(Constants.Windows.DriverBinaryPath, "oculusvad.sys"));
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.UninstallDriver, "Delete oculusvad.sys", Lumberjack.Severity.Warning, 50, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\DriverInstallers\\RemoteAudio.cs");
			}
			return CheckUninstallHealth();
		}

		public override void ExtractFiles()
		{
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "oculusvad.sys"), Resources.OculusVADSys);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "OculusVadApo.dll"), Resources.OculusVadApo);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.OculusVADCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.OculusVADInf));
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "devcon.exe"), Resources.devcon);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "WdfCoinstaller01009.dll"), Resources.WdfCoinstaller01009);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "WdfCoinstaller01011.dll"), Resources.WdfCoinstaller01011);
		}
	}
}
