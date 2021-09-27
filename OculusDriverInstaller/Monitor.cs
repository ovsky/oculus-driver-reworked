using System.IO;
using System.Text;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class Monitor : Driver
	{
		public Monitor()
		{
			InfName = "RiftDisplay.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "RiftDisplay.cat";
			GUID = "{4d36e96e-e325-11ce-bfc1-08002be10318}";
			DriverDesc = Constants.Rift.Monitor.DriverDesc;
			SystemManagementName = null;
			ReadableName = "Monitor Driver";
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
			if (base.Uninstall())
			{
				return CheckUninstallHealth();
			}
			return false;
		}

		public override void ExtractFiles()
		{
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.RiftDisplayCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.RiftDisplayInf));
		}
	}
}
