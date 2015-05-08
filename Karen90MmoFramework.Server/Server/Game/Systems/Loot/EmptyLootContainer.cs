using Karen90MmoFramework.Server.Game.Objects;

namespace Karen90MmoFramework.Server.Game.Systems
{
	public class EmptyLootContainer : ILootContainer
	{
		#region Constants and Fields

		/// <summary>
		/// the empty loot
		/// </summary>
		public static readonly ILoot EmptyLoot = new Loot();
		
		/// <summary>
		/// the loot
		/// </summary>
		private readonly Loot loot;

		#endregion
		
		#region Constructors and Destructors

		/// <summary>
		/// Creates a new instance of the <see cref="RandomLootContainer"/> class.
		/// </summary>
		public EmptyLootContainer()
		{
			this.loot = new Loot();
		}

		#endregion

		#region Implementation of ILootContainer

		/// <summary>
		/// Generates loot for (n)n <see cref="Player"/>.
		/// </summary>
		public void GenerateLootFor(Player player)
		{
		}

		/// <summary>
		/// Tells whether the <see cref="Player"/> has any loot in the container.
		/// </summary>
		public bool HasLootFor(Player player)
		{
			return false;
		}

		/// <summary>
		/// Gets the loot for a certain <see cref="Player"/>.
		/// </summary>
		public ILoot GetLootFor(Player player)
		{
			return this.loot;
		}

		/// <summary>
		/// Removes any generated loot for a certain <see cref="Player"/>.
		/// </summary>
		/// <param name="player"></param>
		public void RemoveLootFor(Player player)
		{
		}

		/// <summary>
		/// Clear all generated loots
		/// </summary>
		public void Clear()
		{
		}

		#endregion

		#region class Loot : ILoot
		class Loot : ILoot
		{
			#region Constants and Fields

			/// <summary>
			/// loot items
			/// </summary>
			private readonly LootItem[] lootItems;

			#endregion

			#region Constructors and Destructors

			/// <summary>
			/// Creates a new <see cref="Loot"/>.
			/// </summary>
			public Loot()
			{
				this.lootItems = new LootItem[0];
			}

			#endregion

			#region Implementation of ILoot

			/// <summary>
			/// Gets the loot items
			/// First index will ALWAYS contain GOLD.
			/// </summary>
			public LootItem[] LootItems
			{
				get
				{
					return lootItems;
				}
			}

			/// <summary>
			/// Determines whether the loot contains a certain item
			/// </summary>
			public bool HasItem(short itemId)
			{
				return false;
			}

			/// <summary>
			/// Collects a <see cref="LootItem"/> from an index and notifies the looter
			/// </summary>
			public void CollectLootItem(int index, Player collector)
			{
			}

			/// <summary>
			/// Collects all the loot item and notifies the looter
			/// </summary>
			public void CollectAll(Player collector)
			{
			}

			#endregion
		}
		#endregion
	}
}
