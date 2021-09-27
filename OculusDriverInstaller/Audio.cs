using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Daybreak.Core;
using Daybreak.Win32;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class Audio : Driver
	{
		public Audio()
		{
			InfName = "oculus119b.inf";
			InfLoc = Path.Combine(Constants.Windows.TempDirectory, Mixins.ConvertVersion(WindowsEnvironment.OSVersion).ToString());
			CatName = "oculus119b.cat";
			GUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";
			DriverDesc = Constants.Rift.Audio.DriverDesc;
			SystemManagementName = "OCULUSVRHEADSET";
			ReadableName = "Audio Driver";
		}

		public override bool Install()
		{
			StartAudio(Constants.InstallStep.InstallDriver);
			if (base.Install())
			{
				return CheckInstallHealth();
			}
			return false;
		}

		public override bool Uninstall()
		{
			StopAudio(Constants.InstallStep.UninstallDriver);
			if (!base.Uninstall())
			{
				return false;
			}
			try
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "About to delete audio driver binary");
				File.Delete(Path.Combine(Constants.Windows.DriverBinaryPath, "oculus119b.sys"));
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.UninstallDriver, "Delete oculus119b.sys", Lumberjack.Severity.Warning, 38, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\DriverInstallers\\Audio.cs");
			}
			return CheckUninstallHealth();
		}

		private void StopAudio(Constants.InstallStep installStep)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to stop audio device");
			StartStopAudio(start: false, installStep);
		}

		private void StartAudio(Constants.InstallStep installStep)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to start audio device");
			StartStopAudio(start: true, installStep);
		}

		private void StartStopAudio(bool start, Constants.InstallStep installStep)
		{
			string text = (start ? "start" : "stop");
			try
			{
				Mixins.ExecuteProcessInt(Path.Combine(Constants.Windows.TempDirectory, "Audio_Enable.exe"), Convert.ToInt32(start).ToString(), new List<int> { -1 });
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, installStep, ReadableName + " " + text + " audio", Lumberjack.Severity.Warning, 70, "StartStopAudio", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\DriverInstallers\\Audio.cs");
			}
		}

		public override void ExtractFiles()
		{
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "Audio_Enable.exe"), Resources.Audio_Enable);
			Constants.Windows.Versions versions = Mixins.ConvertVersion(WindowsEnvironment.OSVersion);
			byte[] bytes;
			byte[] bytes2;
			byte[] bytes3;
			switch (versions)
			{
			case Constants.Windows.Versions.Win7:
				bytes = Encoding.ASCII.GetBytes(Resources.oculus119bInf_Win7);
				bytes2 = Resources.oculus119bCat_Win7;
				bytes3 = Resources.oculus119bSys_Win7;
				break;
			case Constants.Windows.Versions.Win8:
				bytes = Encoding.ASCII.GetBytes(Resources.oculus119bInf_Win8);
				bytes2 = Resources.oculus119bCat_Win8;
				bytes3 = Resources.oculus119bSys_Win8;
				break;
			case Constants.Windows.Versions.Win8_1:
				bytes = Encoding.ASCII.GetBytes(Resources.oculus119bInf_Win8_1);
				bytes2 = Resources.oculus119bCat_Win8_1;
				bytes3 = Resources.oculus119bSys_Win8_1;
				break;
			case Constants.Windows.Versions.Win10:
				bytes = Encoding.ASCII.GetBytes(Resources.oculus119bInf_Win10);
				bytes2 = Resources.oculus119bCat_Win10;
				bytes3 = Resources.oculus119bSys_Win10;
				break;
			default:
				throw new InvalidOperationException();
			}
			string text = Path.Combine(Constants.Windows.TempDirectory, versions.ToString());
			string text2 = Path.Combine(text, "x64");
			Directory.CreateDirectory(text);
			Directory.CreateDirectory(text2);
			File.WriteAllBytes(Path.Combine(text, InfName), bytes);
			File.WriteAllBytes(Path.Combine(text2, "oculus119b.sys"), bytes3);
			File.WriteAllBytes(Path.Combine(text, CatName), bytes2);
		}
	}
}
