using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public abstract class CharacterData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public string Name { get; set; }
		public int Guid { get; set; }
		public byte Race { get; set; }
		public byte Species { get; set; }
		public byte Level { get; set; }
		public float[] Position { get; set; }
		public float Orientation { get; set; }
		public short ZoneId { get; set; }
	}
}
