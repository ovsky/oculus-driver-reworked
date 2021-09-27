using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Daybreak.Core;
using Daybreak.Net;

namespace OculusDriverInstaller
{
	internal class EntryPoint
	{
		private enum Mode
		{
			Install,
			Uninstall,
			Verify
		}

		public static int Main(string[] args)
		{
			Constants.ODIVersionString = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
			Lumberjack.Initialise(Logging.CreateLogFile, "OculusDriverInstaller_" + $"{DateTime.Now:yyyy-MM-dd_HH.mm.ss}" + ".log", Constants.ODIVersionString);
			bool flag = false;
			try
			{
				if (!new Mutex(initiallyOwned: false, "Global\\OculusDriverInstallerRun").WaitOne(0, exitContext: false))
				{
					Lumberjack.Log(Lumberjack.Severity.Warning, "Couldn't get execution mutex");
					return -3;
				}
				Lumberjack.Log(Lumberjack.Severity.Debug, "Grabbed execution mutex");
				CertificatePinner.Initialise();
				ODIAnalytics.Initialise(new Networker());
				flag = true;
				long setupapidevLogStart = 0L;
				try
				{
					Constants.Windows.INFPath = Path.Combine(Environment.GetEnvironmentVariable("windir"), "INF");
					Constants.Windows.DriverLog = Path.Combine(Constants.Windows.INFPath, "setupapi.dev.log");
					Constants.Windows.DriverStagingPath = Path.Combine(Environment.SystemDirectory, Path.Combine("DriverStore", "FileRepository"));
					Constants.Windows.DriverBinaryPath = Path.Combine(Environment.SystemDirectory, "drivers");
					Constants.Windows.TempDirectory = Path.Combine(Path.GetTempPath(), "OculusDriverInstaller");
					setupapidevLogStart = new FileInfo(Constants.Windows.DriverLog).Length;
				}
				catch (Exception ex)
				{
					Lumberjack.Log(Lumberjack.Severity.Error, "Exception thrown while grabbing SetupApiDevLog: " + ex.Message + "\n" + ex.StackTrace);
					ODIAnalytics.LogMessage(ex.Message + "\n" + ex.StackTrace, ex.GetType().Name, Constants.InstallStep.Unknown, "Exception thrown while grabbing SetupApiDevLog", Lumberjack.Severity.Fatal, 66, "Main", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\EntryPoint.cs");
				}
				ODIAnalytics.SetupapidevLogStart = setupapidevLogStart;
			}
			catch (Exception ex2)
			{
				Lumberjack.Log(Lumberjack.Severity.Error, "Exception thrown during pre-init that wasn't caught locally: " + ex2.Message + "\n" + ex2.StackTrace);
				if (flag)
				{
					ODIAnalytics.LogMessage(ex2.Message + "\n" + ex2.StackTrace, ex2.GetType().Name, Constants.InstallStep.Unknown, "Exception thrown during pre-init that wasn't caught locally", Lumberjack.Severity.Fatal, 83, "Main", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\EntryPoint.cs");
					ODIAnalytics.Completion();
				}
				return -2;
			}
			bool flag2 = false;
			try
			{
				flag2 = Run(args);
			}
			catch (Exception ex3)
			{
				Lumberjack.Log(Lumberjack.Severity.Error, "Exception thrown that wasn't caught locally: " + ex3.Message + "\n" + ex3.StackTrace);
				ODIAnalytics.LogMessage(ex3.Message + "\n" + ex3.StackTrace, ex3.GetType().Name, Constants.InstallStep.Unknown, "Exception thrown that wasn't caught locally", Lumberjack.Severity.Fatal, 106, "Main", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\EntryPoint.cs");
				ODIAnalytics.Completion();
				return -2;
			}
			ODIAnalytics.Completion();
			if (!flag2)
			{
				return -1;
			}
			return 0;
		}

		private static bool Run(string[] args)
		{
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to parse args");
			int slowDown;
			Driver[] verifyList;
			Driver[] drivers;
			Type[] types = null;
			Mode mode = parseArgs(args, out slowDown, out verifyList, out drivers);
			Lumberjack.Log(Lumberjack.Severity.Debug, "Parsed args: `" + string.Join("`, `", args) + "`");
			if ((mode == Mode.Install || mode == Mode.Uninstall) && drivers.Length == 0)
			{
				types = new Type[10]
				{
					typeof(Monitor),
					typeof(DK2Sensor),
					typeof(Tracker),
					typeof(GamepadEmulation),
					typeof(Audio),
					typeof(RiftSAudio),
					typeof(RiftSSensor),
					typeof(RiftSUSB),
					typeof(RemoteAudio),
					typeof(AndroidUSB)
				};
				for (int i = 0; i < types.Length; i++)
                {
					try
                    {
						drivers[i] = Activator.CreateInstance(types[i]) as Driver;
                    }
					catch(Exception e)
                    {
						Console.WriteLine(e);
                    }
                }
			}

			Installer @object = new Installer(verifyList, drivers, slowDown);
			Func<bool> func = mode switch
			{
				Mode.Uninstall => @object.Uninstall, 
				Mode.Verify => @object.Verify, 
				_ => @object.Install, 
			};
			Lumberjack.Log(Lumberjack.Severity.Debug, "About to invoke installer");
			bool result = func();
			Lumberjack.Log(Lumberjack.Severity.Debug, "\nstatus: " + result);
			ODIAnalytics.Completion();
			return result;
		}

		private static Mode parseArgs(string[] args, out int slowDown, out Driver[] verifyList, out Driver[] drivers)
		{
			slowDown = 0;
			List<Driver> list = new List<Driver>();
			List<Driver> list2 = new List<Driver>();
			Mode result = Mode.Install;
			int end = 0;
			while (end < args.Length)
			{
				switch (args[end])
				{
				case "--slow":
					if (end + 1 < args.Length)
					{
						slowDown = Convert.ToInt32(args[end + 1]);
					}
					end += 2;
					break;
				case "--installType":
					if (end + 1 < args.Length)
					{
						result = ((args[end + 1] == "uninstall") ? Mode.Uninstall : Mode.Install);
					}
					end += 2;
					break;
				case "--ODIVersion":
					if (end + 1 < args.Length)
					{
						Installer.ODIVersionInstallString = args[end + 1];
						Lumberjack.Log(Lumberjack.Severity.Debug, "Accepting --ODIVersion string that will be written to registry: '" + Installer.ODIVersionInstallString + "'");
					}
					end += 2;
					break;
				case "--mode":
					if (end + 1 < args.Length && args[end + 1] == "unattended")
					{
						Installer.UnattendedInstall = true;
					}
					end += 2;
					break;
				case "--verify":
					result = Mode.Verify;
					list = ExtractDevices(args, end + 1, out end);
					break;
				case "--driver":
					list2 = ExtractDevices(args, end + 1, out end);
					break;
				case "/unattended":
				case "--unattended":
					Installer.UnattendedInstall = true;
					break;
				default:
					Console.WriteLine("Unrecognized command line input");
					ODIAnalytics.LogMessage("Unrecognized command line input", -1, Constants.InstallStep.ParseArgs, "cmd: " + args[end] + ". args: " + string.Join(" ", args), Lumberjack.Severity.Warning, 220, "parseArgs", "D:\\ovrsource-null-hg\\arvr\\projects\\oculus_pc_infra\\Support\\OculusDriverInstaller\\OculusDriverInstaller\\EntryPoint.cs");
					end++;
					break;
				}
			}
			verifyList = list.ToArray();
			drivers = list2.ToArray();
			return result;
		}

		private static List<Driver> ExtractDevices(string[] args, int start, out int end)
		{
			List<Driver> list = new List<Driver>();
			int i;
			for (i = start; i < args.Length && args[i].IndexOf("--") != 0; i++)
			{
				switch (args[i])
				{
				case "monitor":
					list.Add(new Monitor());
					break;
				case "dk2":
					list.Add(new DK2Sensor());
					break;
				case "tracker":
					list.Add(new Tracker());
					break;
				case "gamepad":
					list.Add(new GamepadEmulation());
					break;
				case "audio":
					list.Add(new Audio());
					break;
				case "riftsaudio":
					list.Add(new RiftSAudio());
					break;
				case "riftssensor":
					list.Add(new RiftSSensor());
					break;
				case "riftsusb":
					list.Add(new RiftSUSB());
					break;
				case "remoteaudio":
					list.Add(new RemoteAudio());
					break;
				case "androidusb":
					list.Add(new AndroidUSB());
					break;
				}
			}
			end = i;
			return list;
		}
	}
}
