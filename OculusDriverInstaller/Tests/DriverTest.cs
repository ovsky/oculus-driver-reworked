using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace OculusDriverInstaller.Tests
{
	[TestFixture]
	internal class DriverTest
	{
		private Dictionary<string, string> commandLineNamesToInstalledNames = new Dictionary<string, string>
		{
			{ "dk2", "riftsensor" },
			{ "monitor", "riftdisplay" },
			{ "tracker", "ocusbvid" },
			{ "gamepad", "oculus_vigembus" },
			{ "audio", "oculus119b" },
			{ "riftsaudio", "riftsaudio" },
			{ "riftssensor", "riftssensor" },
			{ "riftsusb", "riftsusb" },
			{ "remoteaudio", "remoteaudio" },
			{ "androidusb", "androidusb" }
		};

		private static string[] GenArgs(string installType, string[] drivers)
		{
			List<string> list = new List<string>();
			list.Add("--installType");
			list.Add(installType);
			list.Add("--driver");
			foreach (string item in drivers)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		private static void AssertStatus(string[] installed, string[] uninstalled)
		{
			string[] array = installed;
			foreach (string driverName in array)
			{
				Assert.IsTrue(Common.InINFDir(driverName) && Common.InDriverStore(driverName));
			}
			array = uninstalled;
			foreach (string driverName2 in array)
			{
				Assert.IsTrue(!Common.InINFDir(driverName2) && !Common.InDriverStore(driverName2));
			}
		}

		[SetUp]
		public void Init()
		{
			EntryPoint.Main(new string[0]);
		}

		[Test]
		public void TestSingleDrivers()
		{
			string[] array = commandLineNamesToInstalledNames.Values.ToArray();
			AssertStatus(array, new string[0]);
			List<string> list = new List<string>(array);
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, string> commandLineNamesToInstalledName in commandLineNamesToInstalledNames)
			{
				Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("uninstall", new string[1] { commandLineNamesToInstalledName.Key })));
				list2.Add(commandLineNamesToInstalledName.Value);
				list.Remove(commandLineNamesToInstalledName.Value);
				AssertStatus(list.ToArray(), list2.ToArray());
			}
			AssertStatus(new string[0], array);
			AssertStatus(new string[0], array);
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>(array);
			foreach (KeyValuePair<string, string> commandLineNamesToInstalledName2 in commandLineNamesToInstalledNames)
			{
				Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("install", new string[1] { commandLineNamesToInstalledName2.Key })));
				list3.Add(commandLineNamesToInstalledName2.Value);
				list4.Remove(commandLineNamesToInstalledName2.Value);
				AssertStatus(list3.ToArray(), list4.ToArray());
			}
			AssertStatus(array, new string[0]);
		}

		[Test]
		public void TestTwoDrivers()
		{
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("uninstall", new string[2] { "dk2", "monitor" })));
			AssertStatus(new string[3] { "oculus_vigembus", "oculus119b", "ocusbvid" }, new string[2] { "riftsensor", "riftdisplay" });
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("install", new string[2] { "dk2", "monitor" })));
			AssertStatus(new string[5] { "riftsensor", "riftdisplay", "oculus_vigembus", "oculus119b", "ocusbvid" }, new string[0]);
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("uninstall", new string[2] { "tracker", "gamepad" })));
			AssertStatus(new string[3] { "riftsensor", "riftdisplay", "oculus119b" }, new string[2] { "ocusbvid", "oculus_vigembus" });
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("install", new string[2] { "tracker", "gamepad" })));
			AssertStatus(new string[5] { "riftsensor", "riftdisplay", "oculus_vigembus", "oculus119b", "ocusbvid" }, new string[0]);
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("uninstall", new string[2] { "gamepad", "audio" })));
			AssertStatus(new string[3] { "riftsensor", "riftdisplay", "ocusbvid" }, new string[2] { "oculus_vigembus", "oculus119b" });
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("install", new string[2] { "gamepad", "audio" })));
			AssertStatus(new string[5] { "riftsensor", "riftdisplay", "oculus_vigembus", "oculus119b", "ocusbvid" }, new string[0]);
		}

		[Test]
		public void TestAllDrivers()
		{
			string[] drivers = commandLineNamesToInstalledNames.Keys.ToArray();
			string[] array = commandLineNamesToInstalledNames.Values.ToArray();
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("uninstall", drivers)));
			AssertStatus(new string[0], array);
			Assert.AreEqual((object)0, (object)EntryPoint.Main(GenArgs("install", drivers)));
			AssertStatus(array, new string[0]);
			Assert.AreEqual((object)0, (object)EntryPoint.Main(new string[2] { "--installType", "uninstall" }));
			AssertStatus(new string[0], array);
			Assert.AreEqual((object)0, (object)EntryPoint.Main(new string[0]));
			AssertStatus(array, new string[0]);
		}
	}
}
