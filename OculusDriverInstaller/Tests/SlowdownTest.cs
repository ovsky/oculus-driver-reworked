using System.Collections.Generic;
using NUnit.Framework;

namespace OculusDriverInstaller.Tests
{
	[TestFixture]
	internal class SlowdownTest
	{
		private const int slowDownMilli = 1000;

		private static string[] GenArgs(string installType, string[] drivers, int slowDown)
		{
			List<string> list = new List<string>();
			list.Add("--installType");
			list.Add(installType);
			list.Add("--driver");
			foreach (string item in drivers)
			{
				list.Add(item);
			}
			list.Add("--slow");
			list.Add(slowDown.ToString());
			return list.ToArray();
		}
	}
}
