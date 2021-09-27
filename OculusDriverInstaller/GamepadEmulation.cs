using System;
using System.IO;
using System.Text;
using Daybreak.Core;
using Daybreak.Win32;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class GamepadEmulation : Driver
	{
		public const string DevicePath = "Root\\Oculus_ViGEmBus";

		public GamepadEmulation()
		{
			InfName = "Oculus_ViGEmBus.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "oculus_vigembus.cat";
			GUID = "{4D36E97D-E325-11CE-BFC1-08002BE10318}";
			DriverDesc = Constants.Rift.GamepadEmulation.DriverDesc;
			SystemManagementName = "Oculus_ViGEmBus";
			ReadableName = "Gamepad Emulation Driver";
		}

		public override bool Install()
		{
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) != Constants.Windows.Versions.Win10)
			{
				Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping installation of gamepad driver because it is only compatible with Windows 10");
				return true;
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Installing driver");
			DevCon("remove", "Root\\Oculus_ViGEmBus", Constants.InstallStep.InstallDriver);
			if (DevCon("install", "Root\\Oculus_ViGEmBus", Constants.InstallStep.InstallDriver))
			{
				return CheckInstallHealth();
			}
			return false;
		}

		public override bool Uninstall()
		{
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) != Constants.Windows.Versions.Win10)
			{
				Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping uninstallation of gamepad driver because it is only compatible with Windows 10");
				return true;
			}
			_ = !DevCon("remove", "Root\\Oculus_ViGEmBus", Constants.InstallStep.UninstallDriver);
			base.Uninstall();
			try
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "About to delete gamepad emulation driver binary");
				File.Delete(Path.Combine(Constants.Windows.DriverBinaryPath, "Oculus_ViGEmBus.sys"));
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.UninstallDriver, "Delete Oculus_ViGEmBus.sys", Lumberjack.Severity.Warning, 57, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\DriverInstallers\\GamepadEmulation.cs");
			}
			return CheckUninstallHealth();
		}

		public override void ExtractFiles()
		{
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "Oculus_ViGEmBus.sys"), Resources.Oculus_ViGEmBus);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.Oculus_ViGEmBusCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.Oculus_ViGEmBusInf));
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "devcon.exe"), Resources.devcon);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "WdfCoinstaller01009.dll"), Resources.WdfCoinstaller01009);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "WdfCoinstaller01011.dll"), Resources.WdfCoinstaller01011);
		}
	}
}
