using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Server.Data
{
	public class SpellData : IDataObject
	{
		/// <summary>
		/// Gets or sets the Id
		/// </summary>
		public string Id { get; set; }

		public short SpellId { get; set; }
		public string Name { get; set; }
		public int Flags { get; set; }

		public byte School { get; set; }
		public byte RequiredTargetType { get; set; }
		public byte AffectionMethod { get; set; }
		public short RequiredWeaponType { get; set; }
		public byte TargetSelectionMethod { get; set; }

		public int[] Effects { get; set; }
		public int[] EffectBaseValues { get; set; }

		public float Cooldown { get; set; }
		public bool AffectedByGCD { get; set; }
		public bool IsProc { get; set; }
		public bool TriggersGCD { get; set; }

		public byte PowerType { get; set; }
		public int PowerCost { get; set; }

		public float CastTime { get; set; }
		public int MinCastRadius { get; set; }
		public int MaxCastRadius { get; set; }

		/// <summary>
		/// Generates an id for this datamember
		/// </summary>
		public static string GenerateId(short spellId)
		{
			return "SPELL/" + spellId;
		}
	}
}
