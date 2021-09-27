namespace OculusDriverInstaller
{
	public static class Constants
	{
		public static class ReturnCodes
		{
			public const int SUCCESS = 0;

			public const int FAILURE = -1;

			public const int EXCEPTION = -2;

			public const int MULTIPLE_INSTANCES = -3;
		}

		public static class Rift
		{
			public static class Monitor
			{
				public const string InfName = "RiftDisplay.inf";

				public const string CatName = "RiftDisplay.cat";

				public const string SysName = "RiftDisplay.sys";

				public const string GUID = "{4d36e96e-e325-11ce-bfc1-08002be10318}";

				public static readonly string[] DriverDesc = new string[1] { "Rift" };

				public const string SystemManagementName = null;

				public const string ReadableName = "Monitor Driver";
			}

			public static class DK2Sensor
			{
				public const string InfName = "RiftSensor.inf";

				public const string CatName = "RiftSensor.cat";

				public const string SysName = "RiftSensor.sys";

				public const string GUID = "{745a17a0-74d3-11d0-b6fe-00a0c90f57da}";

				public static readonly string[] DriverDesc = new string[1] { "Rift DK2 Sensor USB" };

				public const string SystemManagementName = null;

				public const string ReadableName = "DK2Sensor Driver";
			}

			public static class Tracker
			{
				public const string InfName = "OCUSBVID.inf";

				public const string CatName = "OCUSBVID.Cat";

				public const string SysName = "ocusbvid111.sys";

				public const string SysNameOld = "ocusbvid109.sys";

				public const string GUID = "{A2DB2D36-9CA9-4DD8-9E46-3121A16063D2}";

				public static readonly string[] DriverDesc = new string[1] { "Rift Sensor" };

				public const string SystemManagementName = "OCUSBVID";

				public const string ReadableName = "Tracker Driver";
			}

			public static class GamepadEmulation
			{
				public const string InfName = "Oculus_ViGEmBus.inf";

				public const string SysName = "Oculus_ViGEmBus.sys";

				public const string CatName = "oculus_vigembus.cat";

				public const string GUID = "{4D36E97D-E325-11CE-BFC1-08002BE10318}";

				public static readonly string[] DriverDesc = new string[1] { "Oculus Virtual Gamepad Emulation Bus" };

				public const string SystemManagementName = "Oculus_ViGEmBus";

				public const string ReadableName = "Gamepad Emulation Driver";

				public const string DevicePath = "Root\\Oculus_ViGEmBus";
			}

			public static class Audio
			{
				public const string InfName = "oculus119b.inf";

				public const string CatName = "oculus119b.cat";

				public const string SysName = "oculus119b.sys";

				public const string GUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";

				public const string Enabler = "Audio_Enable.exe";

				public static readonly string[] DriverDesc = new string[1] { "Oculus VR Headset" };

				public const string SystemManagementName = "OCULUSVRHEADSET";

				public const string ReadableName = "Audio Driver";
			}

			public static class RiftSAudio
			{
				public const string InfName = "OCULUSUD.Inf";

				public const string CatName = "oculusud.cat";

				public const string SysName = "OCULUSUD.sys";

				public const string GUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";

				public static readonly string[] DriverDesc = new string[1] { "Oculus VR Headset" };

				public const string SystemManagementName = null;

				public const string ReadableName = "Rift S Audio Driver";
			}

			public static class RiftSSensor
			{
				public const string InfName = "riftssensor.inf";

				public const string CatName = "riftssensor.cat";

				public const string GUID = "{ca3e7ab9-b4c3-4ae6-8251-579ef933890f}";

				public static readonly string[] DriverDesc = new string[1] { "Rift S" };

				public const string SystemManagementName = null;

				public const string ReadableName = "Rift S Sensor Driver";
			}

			public static class RiftSUSB
			{
				public const string InfName = "riftsusb.inf";

				public const string CatName = "riftsusb.cat";

				public const string GUID = "{36fc9e60-c465-11cf-8056-444553540000}";

				public static readonly string[] DriverDesc = new string[1] { "Rift S USB Hub" };

				public const string SystemManagementName = null;

				public const string ReadableName = "Rift S USB Driver";
			}

			public static class RemoteAudio
			{
				public const string InfName = "oculusvad.inf";

				public const string CatName = "oculusvad.cat";

				public const string SysName = "oculusvad.sys";

				public const string DllName = "OculusVadApo.dll";

				public const string GUID = "{4d36e96c-e325-11ce-bfc1-08002be10318}";

				public static readonly string[] DriverDesc = new string[1] { "Oculus Virtual Audio Device" };

				public const string SystemManagementName = "oculusvad_oculusvad";

				public const string ReadableName = "Oculus Virtual Audio Device";

				public const string DevicePath = "Root\\oculusvad_OculusVad";
			}

			public static class AndroidUSB
			{
				public const string InfName = "android_winusb.inf";

				public const string CatName = "androidwinusba64.cat";

				public const string WdfCoInstaller01009Name = "WdfCoInstaller01009.dll";

				public const string winusbcoinstaller2Name = "winusbcoinstaller2.dll";

				public const string WUDFUpdate_01009Name = "WUDFUpdate_01009.dll";

				public const string GUID = "{3F966BD9-FA04-4ec5-991C-D326973B5128}";

				public static readonly string[] DriverDesc = new string[5] { "Oculus XRSP Interface", "Oculus ADB Interface", "Oculus Composite ADB Interface", "Oculus Composite XRSP Interface", "Oculus Bootloader Interface" };

				public const string SystemManagementName = null;

				public const string ReadableName = "Oculus XRSP Interface";
			}
		}

		public static class Certs
		{
			public const string CMedia = "oculus-cmedia.cer";

			public const string EV = "oculus-ev.cer";

			public const string LLC2018 = "oculus-llc-2018.cer";

			public const string LLC = "oculus-llc.cer";
		}

		public static class Windows
		{
			public static class InstallFlags
			{
				public const int DRIVER_PACKAGE_REPAIR = 1;

				public const int DRIVER_PACKAGE_SILENT = 2;

				public const int DRIVER_PACKAGE_FORCE = 4;

				public const int DRIVER_PACKAGE_ONLY_IF_DEVICE_PRESENT = 8;

				public const int DRIVER_PACKAGE_LEGACY_MODE = 16;

				public const int DRIVER_PACKAGE_DELETE_FILES = 32;
			}

			public static class ErrorCodes
			{
				public const int ERROR_SUCCESS = 0;

				private const int APPLICATION_ERROR_MASK = 536870912;

				private const int ERROR_SEVERITY_ERROR = -1073741824;

				public const int ERROR_NO_SUCH_DEVINST = -536870389;

				public const int ERROR_DRIVER_NOT_INSTALLED = -536870142;

				public const int ERROR_NO_MORE_ITEMS = 259;

				public const int ERROR_APP_INIT_FAILURE = 575;

				public const int ERROR_INSUFFICIENT_BUFFER = 122;

				public const int ERROR_AUTHENTICODE_TRUST_NOT_ESTABLISHED = -536870334;

				public const int ERROR_AUTHENTICODE_PUBLISHER_NOT_TRUSTED = -536870333;

				public const int ERROR_DEVICE_INSTALLER_NOT_READY = -536870330;

				public const int ERROR_DRIVER_STORE_ADD_FAILED = -536870329;

				public const int ERROR_FILE_HASH_NOT_IN_CATALOG = -536870325;
			}

			public static class DevCon
			{
				public const string DevconExe = "devcon.exe";

				public const string Coinstaller = "WdfCoinstaller01011.dll";

				public const string CoinstallerOld = "WdfCoinstaller01009.dll";

				public const string Install = "install";

				public const string Remove = "remove";

				public const int SUCCESS = 0;

				public const int REQUIRES_REBOOT = 1;

				public const int FAILURE = 2;

				public const int SYNTAX_ERROR = 3;
			}

			public enum Versions
			{
				Win7,
				Win8,
				Win8_1,
				Win10
			}

			public static string INFPath = "";

			public static string DriverLog = "";

			public static string TempDirectory = "";

			public static string DriverStagingPath = "";

			public static string DriverBinaryPath = "";

			public const string RegistryCheckLoc = "SYSTEM\\ControlSet001\\Control\\Class\\";
		}

		public enum InstallStep
		{
			InstallDriver,
			UninstallDriver,
			VerifyDriver,
			AddCert,
			StartService,
			StopService,
			InstallDone,
			UninstallDone,
			VerifyDone,
			CopyFiles,
			CheckInstallHealth,
			CheckUninstallHealth,
			ParseArgs,
			RegistryCheck,
			GetMutex,
			Unknown
		}

		public static string ODIVersionString = "1.1.1.1";

		public const string OVRServiceName = "OVRService";

		public static string RebootRequiredCaption = "Restart your computer?";

		public static string RebootRequiredText = "Please restart your computer. Your Oculus headset may not work correctly until you do.";
	}
}
