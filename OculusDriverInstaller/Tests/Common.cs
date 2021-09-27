using System;
using System.IO;

namespace OculusDriverInstaller.Tests
{
	internal class Common
	{
		public static string StagingDir = Path.Combine(Environment.SystemDirectory, Path.Combine("DriverStore", "FileRepository"));

		public static string INFDir => Path.Combine(Environment.GetEnvironmentVariable("windir"), "INF");

		public static bool InDriverStore(string driverName)
		{
			return InDriverStore(new string[1] { driverName });
		}

		public static bool InDriverStore(string[] driverNames)
		{
			bool flag = true;
			foreach (string text in driverNames)
			{
				flag &= Directory.GetDirectories(StagingDir, text + "*").Length != 0;
			}
			return flag;
		}

		private static bool OEMExists(string searchText)
		{
			string[] files = Directory.GetFiles(INFDir, "oem*.inf");
			for (int i = 0; i < files.Length; i++)
			{
				string[] array = File.ReadAllLines(files[i]);
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool InINFDir(string driverName)
		{
			return InINFDir(new string[1] { driverName });
		}

		public static bool InINFDir(string[] driverNames)
		{
			bool flag = true;
			foreach (string searchText in driverNames)
			{
				flag &= OEMExists(searchText);
			}
			return flag;
		}
	}
}
