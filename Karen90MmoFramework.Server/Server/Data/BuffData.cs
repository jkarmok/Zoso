namespace Karen90MmoFramework.Server.Data
{
	public class BuffData : AuraData
	{
		public byte BuffType { get; set; }
		public byte? Attribute { get; set; }
		public byte? Vital { get; set; }
		public int Amount { get; set; }
		public short? Frequency { get; set; }
		public bool Beneficial { get; set; }
	}
}
