using System.IO;
using System.Text;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class AndroidUSB : Driver
	{
		public AndroidUSB()
		{
			InfName = "android_winusb.inf";
			InfLoc = Constants.Windows.TempDirectory;
			CatName = "androidwinusba64.cat";
			GUID = "{3F966BD9-FA04-4ec5-991C-D326973B5128}";
			DriverDesc = Constants.Rift.AndroidUSB.DriverDesc;
			SystemManagementName = null;
			ReadableName = "Oculus XRSP Interface";
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
			string text = Path.Combine(Constants.Windows.TempDirectory, "amd64");
			Directory.CreateDirectory(text);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, CatName), Resources.androidwinusba64);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, InfName), Encoding.ASCII.GetBytes(Resources.android_winusb));
			File.WriteAllBytes(Path.Combine(text, "WdfCoInstaller01009.dll"), Resources.WdfCoInstaller010091);
			File.WriteAllBytes(Path.Combine(text, "winusbcoinstaller2.dll"), Resources.winusbcoinstaller2);
			File.WriteAllBytes(Path.Combine(text, "WUDFUpdate_01009.dll"), Resources.WUDFUpdate_01009);
		}
	}
}
