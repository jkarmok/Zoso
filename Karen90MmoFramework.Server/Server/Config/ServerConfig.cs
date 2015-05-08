using System;
using System.Xml.Serialization;
using System.IO;

namespace Karen90MmoFramework.Server.Config
{
	[Serializable]
	[XmlRoot("configuration")]
	public class ServerConfig
	{
		/// <summary>
		/// Connection info about the master server
		/// </summary>
		[XmlElement("master")]
		public MasterInfo MasterServer { get; set; }

		/// <summary>
		/// Connection info about the current sub server
		/// </summary>
		[XmlElement("server")]
		public SubServerInfo SubServer { get; set; }
		
		/// <summary>
		/// Initializes a new settings instance
		/// </summary>
		/// <returns></returns>
		public static ServerConfig Initialize(string applicationDirectory)
		{
			var serializer = new XmlSerializer(typeof(ServerConfig));
			var file = new FileInfo(Path.Combine(applicationDirectory, "ServerConfig.config"));

			if (!file.Exists)
			{
				throw new FileNotFoundException(string.Format("Config file not found at {0}", file.FullName));
			}

			ServerConfig settings;
			using(var reader = new StreamReader(file.ToString()))
			{
				settings = (ServerConfig)serializer.Deserialize(reader);
				reader.Close();
			}
			
			return settings;
		}
	}

	[Serializable]
	public class MasterInfo
	{
		/// <summary>
		/// Name of the photon application
		/// </summary>
		[XmlAttribute("appName")]
		public string MasterAppName { get; set; }

		/// <summary>
		/// IP of the master server
		/// </summary>
		[XmlAttribute("ip")]
		public string IP { get; set; }

		/// <summary>
		/// Port the master server listens for sub servers
		/// </summary>
		[XmlAttribute("serverPort")]
		public int ServerPort { get; set; }

		/// <summary>
		/// Port the master server listens for sub servers
		/// </summary>
		[XmlAttribute("clientPort")]
		public int ClientPort { get; set; }
	}

	[Serializable]
	public class SubServerInfo
	{
		/// <summary>
		/// Public ip of the current sub server
		/// </summary>
		[XmlAttribute("ip")]
		public string PublicIP { get; set; }

		/// <summary>
		/// sub server id
		/// </summary>
		[XmlAttribute("subServerId")]
		public byte SubServerId { get; set; }

		/// <summary>
		/// Udp Port the clients can connect to
		/// </summary>
		[XmlAttribute("udpPort")]
		public int UdpPort { get; set; }

		/// <summary>
		/// Tcp Port the other sub servers can can connect to
		/// </summary>
		[XmlAttribute("tcpPort")]
		public int TcpPort { get; set; }
	}
}
