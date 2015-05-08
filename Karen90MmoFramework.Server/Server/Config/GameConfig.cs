using System;
using System.Xml.Serialization;
using System.IO;

// ReSharper disable InconsistentNaming
namespace Karen90MmoFramework.Server.Config
{
	[Serializable]
	[XmlRoot("configuration")]
	public class GameConfig
	{
		/// <summary>
		/// Configuration of the world
		/// </summary>
		[XmlElement("world")]
		public WorldConfig world { get; set; }

		/// <summary>
		/// Configuration of the physics engine
		/// </summary>
		[XmlElement("physics")]
		public PhysicsConfig physics { get; set; }

		/// <summary>
		/// Initializes a new settings instance
		/// </summary>
		/// <returns></returns>
		public static GameConfig Initialize(string applicationDirectory)
		{
			var serializer = new XmlSerializer(typeof(GameConfig));
			var file = new FileInfo(Path.Combine(applicationDirectory, "GameConfig.config"));

			if (!file.Exists)
				throw new FileNotFoundException("Config file not found");

			GameConfig settings;
			using (var reader = new StreamReader(file.ToString()))
			{
				settings = (GameConfig)serializer.Deserialize(reader);
				reader.Close();
			}

			return settings;
		}
	}

	[Serializable]
	[XmlRoot("world")]
	public class WorldConfig
	{
		/// <summary>
		/// Configuration of all zones
		/// </summary>
		[XmlElement("zone")]
		public ZoneConfig[] zones { get; set; }

		/// <summary>
		/// Tries to retrieve a <see cref="ZoneConfig"/> by its id
		/// </summary>
		public bool TryGetZoneConfig(int zoneId, out ZoneConfig zoneConfig)
		{
			zoneConfig = null;
			return this.zones != null && (zoneConfig = Array.Find(this.zones, z => z.id == zoneId)) != null;
		}

		[Serializable]
		[XmlRoot("zone")]
		public class ZoneConfig
		{
			/// <summary>
			/// Id of the zone
			/// </summary>
			[XmlAttribute("id")]
			public int id { get; set; }

			/// <summary>
			/// Name of the zone
			/// </summary>
			[XmlAttribute("name")]
			public string name { get; set; }

			/// <summary>
			/// xMin
			/// </summary>
			[XmlAttribute("xMin")]
			public int xMin { get; set; }

			/// <summary>
			/// zMin
			/// </summary>
			[XmlAttribute("zMin")]
			public int zMin { get; set; }

			/// <summary>
			/// xMax
			/// </summary>
			[XmlAttribute("xMax")]
			public int xMax { get; set; }

			/// <summary>
			/// zMax
			/// </summary>
			[XmlAttribute("zMax")]
			public int zMax { get; set; }

			/// <summary>
			/// Tile Dimension
			/// </summary>
			[XmlAttribute("tileSize")]
			public int tileSize { get; set; }

			/// <summary>
			/// Terrain Data File
			/// </summary>
			[XmlAttribute("terrainDataFile")]
			public string terrainDataFile { get; set; }

			/// <summary>
			/// Collider Data File
			/// </summary>
			[XmlAttribute("colliderDataFile")]
			public string colliderDataFile { get; set; }
		}
	}

	[Serializable]
	[XmlRoot("Physics")]
	public class PhysicsConfig
	{
		/// <summary>
		/// Key
		/// </summary>
		[XmlElement("engine")]
		public EngineConfig engine { get; set; }

		[Serializable]
		[XmlRoot("engine")]
		public class EngineConfig
		{
			/// <summary>
			/// The engine type to use
			/// </summary>
			[XmlAttribute("type")]
			public string type { get; set; }

			/// <summary>
			/// The engine type to use
			/// </summary>
			[XmlAttribute("multithreaded")]
			public string multithreaded { get; set; }

			/// <summary>
			/// The engine user object
			/// </summary>
			[XmlAttribute("userObject")]
			public string userObject { get; set; }
		}
	}
}
