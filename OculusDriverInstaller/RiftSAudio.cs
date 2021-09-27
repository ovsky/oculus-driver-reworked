using System.IO;
using System.Text;
using Daybreak.Core;
using Daybreak.Win32;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class RiftSAudio : Driver
	{
		public RiftSAudio()
		{
			InfName = "OCULUSUD.Inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "oculusud.cat";
			GUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";
			DriverDesc = Constants.Rift.RiftSAudio.DriverDesc;
			SystemManagementName = null;
			ReadableName = "Rift S Audio Driver";
		}

		public override bool Install()
		{
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) != Constants.Windows.Versions.Win10)
			{
				Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping installation of RiftS audio driver because it is only compatible with Windows 10");
				return true;
			}
			if (base.Install())
			{
				return CheckInstallHealth();
			}
			return false;
		}

		public override bool Uninstall()
		{
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) != Constants.Windows.Versions.Win10)
			{
				Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping uninstallation of RiftS audio driver because it is only compatible with Windows 10");
				return true;
			}
			if (base.Uninstall())
			{
				return CheckUninstallHealth();
			}
			return false;
		}

		public override void ExtractFiles()
		{
			string text = Path.Combine(Constants.Windows.TempDirectory, "x64");
			Directory.CreateDirectory(text);
			File.WriteAllBytes(Path.Combine(text, "OCULUSUD.sys"), Resources.RiftSAudioSys);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.RiftSAudioCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.RiftSAudioInf));
		}
	}
}
