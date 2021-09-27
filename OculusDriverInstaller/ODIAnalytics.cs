using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Daybreak.Core;
using Daybreak.Net;

namespace OculusDriverInstaller
{
	public static class ODIAnalytics
	{
		private const long ODIAppId = 1515985728697992L;

		private const string ODIAccessToken = "1515985728697992|0aa9aaa39f0eb6010367bd6dc442f68f";

		private static Analytics _Analytics;

		public static long SetupapidevLogStart;

		public static void Initialise(Networker networker)
		{
			ODIMarauderClient marauderClient = new ODIMarauderClient(networker, 1515985728697992L, "1515985728697992|0aa9aaa39f0eb6010367bd6dc442f68f", Analytics.BuildSubstitutions());
			_Analytics = new Analytics(networker, 1515985728697992L, "1515985728697992|0aa9aaa39f0eb6010367bd6dc442f68f", marauderClient);
		}

		public static void Completion()
		{
			_Analytics.Completion().Wait();
		}

		public static void LogMessage(string msg, int errorCode, Constants.InstallStep installStep, string subInstallStep, Lumberjack.Severity severity, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string filePath = null)
		{
			LogMessageImpl(msg, errorCode.ToString(), installStep, subInstallStep, severity, lineNumber, caller, filePath, writeToDisk: true);
		}

		public static void LogMessage(string msg, string errorCode, Constants.InstallStep installStep, string subInstallStep, Lumberjack.Severity severity, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string filePath = null)
		{
			LogMessageImpl(msg, errorCode, installStep, subInstallStep, severity, lineNumber, caller, filePath, writeToDisk: true);
		}

		public static void LogMessageNetwork(string msg, int errorCode, Constants.InstallStep installStep, string subInstallStep, Lumberjack.Severity severity, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string filePath = null)
		{
			LogMessageImpl(msg, errorCode.ToString(), installStep, subInstallStep, severity, lineNumber, caller, filePath, writeToDisk: false);
		}

		private static void LogMessageImpl(string msg, string errorCode, Constants.InstallStep installStep, string subInstallStep, Lumberjack.Severity severity, int lineNumber, string caller, string filePath, bool writeToDisk)
		{
			_Analytics.LogEvent("oculus_driver_installer_log", () => BuildLogMessageExtra(msg, errorCode, installStep, subInstallStep, severity, lineNumber, caller, filePath, writeToDisk));
		}

		private static void LogMessageImpl(string msg, string errorCode, Constants.InstallStep installStep, string subInstallStep, Lumberjack.Severity severity, int lineNumber, string caller, string filePath)
		{
			_Analytics.LogEvent("oculus_driver_installer_log", () => BuildLogMessageExtra(msg, errorCode, installStep, subInstallStep, severity, lineNumber, caller, filePath, writeToDisk: true));
		}

		private static Dictionary<string, dynamic> BuildLogMessageExtra(string msg, string errorCode, Constants.InstallStep installStep, string subInstallStep, Lumberjack.Severity severity, int lineNumber, string caller, string filePath, bool writeToDisk)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				["install_step"] = installStep.ToString(),
				["odi_version"] = Constants.ODIVersionString
			};
			if (installStep != Constants.InstallStep.VerifyDone && installStep != Constants.InstallStep.InstallDone && installStep != Constants.InstallStep.UninstallDone)
			{
				dictionary["message"] = msg;
				dictionary["error_code"] = errorCode;
				dictionary["sub_install_step"] = subInstallStep.ToString();
				dictionary["line_number"] = lineNumber.ToString();
				dictionary["caller"] = caller;
				dictionary["file_path"] = filePath;
				dictionary["severity"] = severity.ToString();
			}
			_Analytics.AddMachineAnalyticsInfoToExtra(dictionary);
			string text = null;
			string text2 = null;
			if (severity == Lumberjack.Severity.Error || severity == Lumberjack.Severity.Fatal)
			{
				try
				{
					FileInfo fileInfo = new FileInfo(Constants.Windows.DriverLog);
					byte[] array = new byte[Math.Min(Convert.ToInt64(Math.Pow(2.0, 20.0)), fileInfo.Length - SetupapidevLogStart)];
					using (FileStream fileStream = File.OpenRead(Constants.Windows.DriverLog))
					{
						fileStream.Position = SetupapidevLogStart;
						fileStream.Read(array, 0, array.Length);
						SetupapidevLogStart = fileStream.Length;
					}
					dictionary["setupapidev.log"] = Encoding.Default.GetString(array);
				}
				catch (Exception ex)
				{
					dictionary["setupapidev.log_error_type"] = ex.GetType();
					dictionary["setupapidev.log_error_msg"] = ex.Message;
					text = ex.GetType().ToString();
					text2 = ex.Message;
				}
			}
			if (writeToDisk)
			{
				if (text != null && text2 != null)
				{
					Lumberjack.Log(severity, msg + "\n                        Error Code: " + errorCode + "\n                        Install Step: " + installStep.ToString() + "\n                        Sub Install Step: " + subInstallStep + "\n                        Severity: " + severity.ToString() + "\n                        Line Number: " + lineNumber + "\n                        Caller: " + caller + "\n                        ODI Version: " + Constants.ODIVersionString + "\n                        File Path: " + filePath + "\n                        SetupAPI Error Type: " + text + "\n                        SetupAPI Error Msg: " + text2);
				}
				else
				{
					Lumberjack.Log(severity, msg + "\n                        Error Code: " + errorCode + "\n                        Install Step: " + installStep.ToString() + "\n                        Sub Install Step: " + subInstallStep + "\n                        Severity: " + severity.ToString() + "\n                        Line Number: " + lineNumber + "\n                        Caller: " + caller + "\n                        ODI Version: " + Constants.ODIVersionString + "\n                        File Path: " + filePath);
				}
			}
			return dictionary;
		}
	}
}
