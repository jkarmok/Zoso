using System;
using Karen90MmoFramework.Server.Config;

namespace Karen90MmoTests
{
	public class XmlDemo : IDemo
	{
		public void Run()
		{
			var config = GameConfig.Initialize(AppDomain.CurrentDomain.BaseDirectory);
			WorldConfig.ZoneConfig zoneInfo;
			if (config.world.TryGetZoneConfig(1, out zoneInfo))
				Console.WriteLine(zoneInfo.name);
		}
	}
}
