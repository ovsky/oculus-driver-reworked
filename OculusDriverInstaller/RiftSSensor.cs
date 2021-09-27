using System.IO;
using System.Text;
using Daybreak.Core;
using Daybreak.Win32;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class RiftSSensor : Driver
	{
		public RiftSSensor()
		{
			InfName = "riftssensor.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "riftssensor.cat";
			GUID = "{ca3e7ab9-b4c3-4ae6-8251-579ef933890f}";
			DriverDesc = Constants.Rift.RiftSSensor.DriverDesc;
			SystemManagementName = null;
			ReadableName = "Rift S Sensor Driver";
		}

		public override bool Install()
		{
			if (Mixins.ConvertVersion(WindowsEnvironment.OSVersion) != Constants.Windows.Versions.Win10)
			{
				Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping installation of RiftS sensor driver because it is only compatible with Windows 10");
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
				Lumberjack.Log(Lumberjack.Severity.Warning, "Skipping uninstallation of RiftS sensor driver because it is only compatible with Windows 10");
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
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.RiftSSensorCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.RiftSSensorInf));
		}
	}
}
