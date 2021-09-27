using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Daybreak.Core;
using Daybreak.Win32;
using Microsoft.Win32;
using OculusDriverInstaller.Properties;

namespace OculusDriverInstaller
{
	internal class Installer
	{
		[Flags]
		public enum ExitWindows : uint
		{
			LogOff = 0x0u,
			ShutDown = 0x1u,
			Reboot = 0x2u,
			PowerOff = 0x8u,
			RestartApps = 0x40u,
			Force = 0x4u,
			ForceIfHung = 0x10u
		}

		[Flags]
		private enum ShutdownReason : uint
		{
			MajorApplication = 0x40000u,
			MajorHardware = 0x10000u,
			MajorLegacyApi = 0x70000u,
			MajorOperatingSystem = 0x20000u,
			MajorOther = 0x0u,
			MajorPower = 0x60000u,
			MajorSoftware = 0x30000u,
			MajorSystem = 0x50000u,
			MinorBlueScreen = 0xFu,
			MinorCordUnplugged = 0xBu,
			MinorDisk = 0x7u,
			MinorEnvironment = 0xCu,
			MinorHardwareDriver = 0xDu,
			MinorHotfix = 0x11u,
			MinorHung = 0x5u,
			MinorInstallation = 0x2u,
			MinorMaintenance = 0x1u,
			MinorMMC = 0x19u,
			MinorNetworkConnectivity = 0x14u,
			MinorNetworkCard = 0x9u,
			MinorOther = 0x0u,
			MinorOtherDriver = 0xEu,
			MinorPowerSupply = 0xAu,
			MinorProcessor = 0x8u,
			MinorReconfig = 0x4u,
			MinorSecurity = 0x13u,
			MinorSecurityFix = 0x12u,
			MinorSecurityFixUninstall = 0x18u,
			MinorServicePack = 0x10u,
			MinorServicePackUninstall = 0x16u,
			MinorTermSrv = 0x20u,
			MinorUnstable = 0x6u,
			MinorUpgrade = 0x3u,
			MinorWMI = 0x15u,
			FlagUserDefined = 0x40000000u,
			FlagPlanned = 0x80000000u
		}

		private struct LUID
		{
			public uint LowPart;

			public int HighPart;
		}

		private struct LUID_AND_ATTRIBUTES
		{
			public LUID Luid;

			public uint Attributes;
		}

		private struct TOKEN_PRIVILEGES
		{
			public uint PrivilegeCount;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public LUID_AND_ATTRIBUTES[] Privileges;
		}

		private readonly Constants.Windows.Versions WinVersion;

		private readonly string WinVersionStr;

		private readonly Driver[] VerifyList;

		private readonly Driver[] Drivers;

		private readonly int SlowDownMillis;

		public static string ODIVersionInstallString = "";

		public static bool UnattendedInstall = false;

		private const uint TOKEN_QUERY = 8u;

		private const uint TOKEN_ADJUST_PRIVILEGES = 32u;

		private const uint SE_PRIVILEGE_ENABLED = 2u;

		private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, uint Zero, IntPtr Null1, IntPtr Null2);

		private void rebootPC()
		{
			IntPtr TokenHandle = IntPtr.Zero;
			try
			{
				if (!OpenProcessToken(Process.GetCurrentProcess().Handle, 40u, out TokenHandle))
				{
					ODIAnalytics.LogMessage("OpenProcessToken failed", Marshal.GetLastWin32Error(), Constants.InstallStep.InstallDone, "RebootPC", Lumberjack.Severity.Error, 149, "rebootPC", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
					throw new Win32Exception();
				}
				TOKEN_PRIVILEGES NewState = default(TOKEN_PRIVILEGES);
				NewState.PrivilegeCount = 1u;
				NewState.Privileges = new LUID_AND_ATTRIBUTES[1];
				NewState.Privileges[0].Attributes = 2u;
				if (!LookupPrivilegeValue(null, "SeShutdownPrivilege", out NewState.Privileges[0].Luid))
				{
					ODIAnalytics.LogMessage("LookupPrivilegeValue failed", Marshal.GetLastWin32Error(), Constants.InstallStep.InstallDone, "RebootPC", Lumberjack.Severity.Error, 169, "rebootPC", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
					throw new Win32Exception();
				}
				if (!AdjustTokenPrivileges(TokenHandle, DisableAllPrivileges: false, ref NewState, 0u, IntPtr.Zero, IntPtr.Zero))
				{
					ODIAnalytics.LogMessage("AdjustTokenPrivileges failed", Marshal.GetLastWin32Error(), Constants.InstallStep.InstallDone, "RebootPC", Lumberjack.Severity.Error, 187, "rebootPC", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
					throw new Win32Exception();
				}
				if (!ExitWindowsEx(ExitWindows.Reboot, ShutdownReason.MinorHardwareDriver | ShutdownReason.MajorHardware | ShutdownReason.FlagPlanned))
				{
					ODIAnalytics.LogMessage("ExitWindowsEx failed", Marshal.GetLastWin32Error(), Constants.InstallStep.InstallDone, "RebootPC", Lumberjack.Severity.Error, 200, "rebootPC", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
					throw new Win32Exception();
				}
				Lumberjack.Log(Lumberjack.Severity.Info, "PC Reboot started successfully.");
			}
			finally
			{
				if (TokenHandle != IntPtr.Zero)
				{
					CloseHandle(TokenHandle);
				}
			}
		}

		private static bool writeRegistryKey(string versionString)
		{
			try
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "Writing --ODIVersion string to registry: '" + versionString + "'");
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
				{
					using RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\\\Oculus VR, LLC\\\\Oculus", writable: true);
					if (registryKey2 == null)
					{
						ODIAnalytics.LogMessage("Writing ODIVersion to registry failed: OculusKey does not exist", 0, Constants.InstallStep.InstallDriver, "ODIVersionWrite", Lumberjack.Severity.Error, 238, "writeRegistryKey", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
						return false;
					}
					registryKey2.SetValue("DriverVersion", versionString, RegistryValueKind.String);
					Lumberjack.Log(Lumberjack.Severity.Debug, "Wrote --ODIVersion string to registry: '" + versionString + "'");
				}
				return true;
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.InstallDriver, "ODIVersionWrite", Lumberjack.Severity.Error, 258, "writeRegistryKey", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
				return false;
			}
		}

		private static bool removeRegistryKey()
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
				{
					using RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\\\Oculus VR, LLC\\\\Oculus", writable: true);
					if (registryKey2 == null)
					{
						ODIAnalytics.LogMessage("Deleting ODIVersion from registry failed: OculusKey does not exist", 0, Constants.InstallStep.InstallDriver, "ODIVersionDelete", Lumberjack.Severity.Error, 283, "removeRegistryKey", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
						return false;
					}
					registryKey2.DeleteSubKey("DriverVersion", throwOnMissingSubKey: false);
					Lumberjack.Log(Lumberjack.Severity.Debug, "Deleted ODIVersion key'");
				}
				return true;
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.InstallDriver, "ODIVersionDelete", Lumberjack.Severity.Error, 302, "removeRegistryKey", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
				return false;
			}
		}

		public Installer(Driver[] verifyList, Driver[] drivers, int slowDownMillis)
		{
			VerifyList = verifyList;
			Drivers = drivers;
			WinVersion = Mixins.ConvertVersion(WindowsEnvironment.OSVersion);
			WinVersionStr = Mixins.VersionToString(WinVersion);
			SlowDownMillis = slowDownMillis;
		}

		public bool Install()
		{
			if (UnattendedInstall)
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning --unattended installation");
			}
			else
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning interactive installation");
			}
			try
			{
				Smc.SynchronouslyStopService("OVRService");
			}
			catch (Smc.SmcException ex)
			{
				if (ex.InnerException is InvalidOperationException)
				{
					Lumberjack.Log(Lumberjack.Severity.Debug, "Service was already stopped");
				}
				else
				{
					ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.StopService, "", Lumberjack.Severity.Warning, 339, "Install", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
				}
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
			CopyFilesToTemp(Drivers);
			bool flag = true;
			flag &= Mixins.AddCertificate(Path.Combine(Constants.Windows.TempDirectory, "oculus-cmedia.cer"));
			flag &= Mixins.AddCertificate(Path.Combine(Constants.Windows.TempDirectory, "oculus-ev.cer"));
			flag &= Mixins.AddCertificate(Path.Combine(Constants.Windows.TempDirectory, "oculus-llc-2018.cer"));
			Lumberjack.Log(Lumberjack.Severity.Debug, "Did certificates install successfully: " + (flag & Mixins.AddCertificate(Path.Combine(Constants.Windows.TempDirectory, "oculus-llc.cer"))).ToString().ToUpper());
			Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
			bool flag2 = true;
			bool flag3 = false;
			Driver[] drivers = Drivers;
			foreach (Driver driver in drivers)
			{
				bool flag4 = driver.Install();
				flag2 = flag2 && flag4;
				if (!flag4)
				{
					Lumberjack.Log(Lumberjack.Severity.Info, "FAILURE! Failed to install driver: " + driver.ReadableName);
				}
				else
				{
					Lumberjack.Log(Lumberjack.Severity.Info, "SUCCESS! Installed driver: " + driver.ReadableName);
				}
				if (driver.NeedsRebootAfterOperation)
				{
					Lumberjack.Log(Lumberjack.Severity.Warning, "SUCCESS! Reboot required for " + driver.ReadableName);
					flag3 = true;
				}
				Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
			}
			DeleteTemp(Constants.InstallStep.InstallDriver);
			if (flag2)
			{
				ODIAnalytics.LogMessage("Install Complete: Success", 0, Constants.InstallStep.InstallDone, "", Lumberjack.Severity.Info, 382, "Install", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
			}
			else
			{
				ODIAnalytics.LogMessage("Install Complete: An install subtask emitted a warning", 0, Constants.InstallStep.InstallDone, "", Lumberjack.Severity.Warning, 385, "Install", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
			}
			if (flag2)
			{
				string oDIVersionInstallString = ODIVersionInstallString;
				if (oDIVersionInstallString.Length == 0)
				{
					Lumberjack.Log(Lumberjack.Severity.Debug, "No --ODIVersion string provided to be written to registry; skipping.");
				}
				else
				{
					flag2 &= writeRegistryKey(oDIVersionInstallString);
				}
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending installation");
			Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
			Lumberjack.Log(Lumberjack.Severity.Debug, "Starting the service");
			try
			{
				Smc.StartService("OVRService");
			}
			catch (Smc.SmcException ex2)
			{
				ODIAnalytics.LogMessage(ex2.Message + " : " + ex2.GetBaseException().Message, ex2.GetType().Name, Constants.InstallStep.StartService, "", Lumberjack.Severity.Warning, 403, "Install", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
			}
			if (flag3)
			{
				if (!UnattendedInstall)
				{
					try
					{
						Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
						Lumberjack.Log(Lumberjack.Severity.Info, "Reboot was required for one of the drivers, so presenting a modal request to reboot.");
						if (MessageBox.Show(Constants.RebootRequiredText, Constants.RebootRequiredCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) == DialogResult.Yes)
						{
							Lumberjack.Log(Lumberjack.Severity.Debug, "User chose to reboot the PC.");
							rebootPC();
							return flag2;
						}
						Lumberjack.Log(Lumberjack.Severity.Debug, "User chose to not reboot the PC.");
						return flag2;
					}
					catch (Exception ex3)
					{
						ODIAnalytics.LogMessage(ex3.Message + " : " + ex3.GetBaseException().Message, ex3.GetType().Name, Constants.InstallStep.Unknown, "", Lumberjack.Severity.Error, 434, "Install", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
						return flag2;
					}
				}
				Lumberjack.Log(Lumberjack.Severity.Info, "Reboot was required for one of the drivers, but this is an --unattended install, so we will not show the reboot modal dialog.");
			}
			return flag2;
		}

		public bool Uninstall()
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning uninstallation");
			try
			{
				Smc.SynchronouslyStopService("OVRService");
			}
			catch (Exception ex)
			{
				if (ex.InnerException is InvalidOperationException)
				{
					Lumberjack.Log(Lumberjack.Severity.Debug, "Service was already stopped");
				}
				else
				{
					ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, Constants.InstallStep.StopService, "", Lumberjack.Severity.Warning, 460, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
				}
			}
			CopyFilesToTemp(Drivers);
			bool flag = true;
			Driver[] drivers = Drivers;
			foreach (Driver driver in drivers)
			{
				bool flag2 = driver.Uninstall();
				flag = flag && flag2;
				if (!flag2)
				{
					Lumberjack.Log(Lumberjack.Severity.Info, "FAILURE! Failed to uninstall driver: " + driver.ReadableName);
				}
				else
				{
					Lumberjack.Log(Lumberjack.Severity.Info, "SUCCESS! Uninstalled driver: " + driver.ReadableName);
				}
				Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
			}
			DeleteTemp(Constants.InstallStep.UninstallDriver);
			if (flag)
			{
				flag &= removeRegistryKey();
			}
			if (flag)
			{
				ODIAnalytics.LogMessage("DONE SUCCESS", 0, Constants.InstallStep.UninstallDone, "", Lumberjack.Severity.Debug, 492, "Uninstall", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending uninstallation");
			return flag;
		}

		public bool Verify()
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "Beginning verification");
			Console.WriteLine();
			CopyFilesToTemp(VerifyList);
			bool flag = true;
			Driver[] verifyList = VerifyList;
			foreach (Driver driver in verifyList)
			{
				flag &= driver.CheckInstallHealth();
				Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
			}
			DeleteTemp(Constants.InstallStep.VerifyDriver);
			if (flag)
			{
				Lumberjack.Log(Lumberjack.Severity.Info, $"HEALTHY: {flag}");
				ODIAnalytics.LogMessage("DONE SUCCESS", 0, Constants.InstallStep.VerifyDone, "", Lumberjack.Severity.Debug, 512, "Verify", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Ending verification");
			return flag;
		}

		private void CopyFilesToTemp(Driver[] drivers)
		{
			DeleteTemp(Constants.InstallStep.CopyFiles);
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to copy files to temp directory");
			Io.CreateSecureDirectory(Constants.Windows.TempDirectory);
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to copy certs to temp directory");
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "oculus-cmedia.cer"), Resources.oculus_cmedia);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "oculus-ev.cer"), Resources.oculus_ev);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "oculus-llc-2018.cer"), Resources.oculus_llc_2018);
			File.WriteAllBytes(Path.Combine(Constants.Windows.TempDirectory, "oculus-llc.cer"), Resources.oculus_llc);
			Lumberjack.Log(Lumberjack.Severity.Debug, "Copied certs to temp directory");
			foreach (Driver driver in drivers)
			{
				Lumberjack.Log(Lumberjack.Severity.Debug, "About to copy " + driver.ReadableName + " files to temp directory");
				driver.ExtractFiles();
				Lumberjack.Log(Lumberjack.Severity.Debug, "Copied " + driver.ReadableName + " files to temp directory");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Copied files to temp directory");
			Lumberjack.Log(Lumberjack.Severity.Debug, "-------------------------------");
		}

		private void DeleteTemp(Constants.InstallStep installStep)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to delete temp directory");
			try
			{
				Mixins.ReallyDeleteDirectory(Constants.Windows.TempDirectory);
			}
			catch (Exception ex)
			{
				ODIAnalytics.LogMessage(ex.Message, ex.GetType().Name, installStep, "Delete temp directory", Lumberjack.Severity.Warning, 555, "DeleteTemp", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\Installer.cs");
			}
			Lumberjack.Log(Lumberjack.Severity.Debug, "Deleted temp directory");
		}
	}
}
