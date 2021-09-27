using System.IO;
using System.Text;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class RiftSUSB : Driver
	{
		public RiftSUSB()
		{
			InfName = "riftsusb.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "riftsusb.cat";
			GUID = "{36fc9e60-c465-11cf-8056-444553540000}";
			DriverDesc = Constants.Rift.RiftSUSB.DriverDesc;
			SystemManagementName = null;
			ReadableName = "Rift S USB Driver";
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
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.RiftSUSBCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.RiftSUSBInf));
		}
	}
}
