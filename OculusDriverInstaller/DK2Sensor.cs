using System.IO;
using System.Text;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class DK2Sensor : Driver
	{
		public DK2Sensor()
		{
			InfName = "RiftSensor.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "RiftSensor.cat";
			GUID = "{745a17a0-74d3-11d0-b6fe-00a0c90f57da}";
			DriverDesc = Constants.Rift.DK2Sensor.DriverDesc;
			SystemManagementName = null;
			ReadableName = "DK2Sensor Driver";
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
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.RiftSensorCat);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.RiftSensorInf));
		}
	}
}
