using System;
using System.Linq;
using System.Collections.Generic;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Quantum;
using Karen90MmoFramework.Database;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Items;
using Karen90MmoFramework.Server.Game.Objects;

/*
 * NOTE: A negative spell Id represents the GCD Spell Id
 */

namespace Karen90MmoFramework.Server.Game.Systems
{
	public class SpellManager : ISpellController, IDisposable, IDataFieldObject<SpellsData>
	{
		#region Constants and Fields

		/// <summary>
		/// Global Cooldown
		/// </summary>
		public const float GCD = 1.0f;

		/// <summary>
		/// spell collection
		/// </summary>
		private readonly Dictionary<short, Spell> spells;

		/// <summary>
		/// usable item counts
		/// </summary>
		private readonly Dictionary<short, int> usableItemCounts;

		/// <summary>
		/// spell update events
		/// </summary>
		private readonly Dictionary<Spell, IDisposable> updateEvents;

		/// <summary>
		/// the owner
		/// </summary>
		private readonly Player owner;

		/// <summary>
		/// in gcd?
		/// </summary>
		private bool inGcd;

		/// <summary>
		/// gcd subscription
		/// </summary>
		private IDisposable gcdSubscription;

		private short currentSpellId;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the value indicating whether the GCD has been activated or not. While in GCD no spells can be casted except the spells which don't get affected by GCD.
		/// </summary>
		public bool InGCD
		{
			get
			{
				return this.inGcd;
			}
		}

		/// <summary>
		/// Gets the value indicating whether a spell is being casted or not
		/// </summary>
		public bool IsCasting
		{
			get
			{
				return this.currentSpellId != 0;
			}
		}

		/// <summary>
		/// Called when a spell is added
		/// </summary>
		public event Action<Spell> OnSpellAdded;

		/// <summary>
		/// Called when a spell is removed
		/// </summary>
		public event Action<Spell> OnSpellRemoved;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///	Creates an instance of <see cref="SpellManager"/> from <see cref="SpellsData"/>.
		/// </summary>
		///<param name="spellsInfo"> </param>
		///<param name="owner"> The owner. </param>
		public SpellManager(SpellsData spellsInfo, Player owner)
		{
			this.updateEvents = new Dictionary<Spell, IDisposable>();
			this.owner = owner;

			// loading spells
			var spellIds = spellsInfo.ClientSpells;
			this.spells = new Dictionary<short, Spell>(spellIds.Length);

			foreach (var id in spellIds)
			{
				SpellData spellInfo;
				if (owner.CurrentZone.World.ItemCache.TryGetSpellData(id, out spellInfo))
				{
					var spell = new Spell(spellInfo, this.owner);
					this.AddSpellInternal(spell);
				}
			}
			
			// loading useable items
			var usableItems = spellsInfo.UsableItems;
			this.usableItemCounts = new Dictionary<short, int>(usableItems.Length);
			
			foreach (var id in usableItems)
			{
				GameItemData itemInfo;
				if (false == owner.CurrentZone.World.ItemCache.TryGetGameItemData(id, out itemInfo))
				{
					continue;
				}

				var count = this.owner.Inventory.GetItemCount(id);
				if (count > 0)
				{
					SpellData spellInfo;
					if (owner.CurrentZone.World.ItemCache.TryGetSpellData(itemInfo.UseSpellId, out spellInfo))
					{
						var spell = new Spell(spellInfo, this.owner);
						this.AddSpellInternal(spell);
						this.usableItemCounts.Add(id, count);
					}
				}
			}

			// loading the use spell
			SpellData useSpellInfo;
			if (owner.CurrentZone.World.ItemCache.TryGetSpellData(ServerGameSettings.USE_SPELL_ID, out useSpellInfo))
			{
				var spell = new Spell(useSpellInfo, this.owner);
				this.AddSpellInternal(spell);
			}
		}

		private SpellManager()
		{
			this.spells = new Dictionary<short, Spell>();
			this.usableItemCounts = new Dictionary<short, int>();
			this.updateEvents = new Dictionary<Spell, IDisposable>();
		}

		~SpellManager()
		{
			this.Dispose(false);
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (gcdSubscription != null)
					this.gcdSubscription.Dispose();

				foreach (var subscriptions in this.updateEvents)
				{
					subscriptions.Value.Dispose();
				}

				this.updateEvents.Clear();
			}
		}

		#endregion

		#region IDatafield Implementation

		/// <summary>
		/// Converts this object to a datafield
		/// </summary>
		public SpellsData ToDataField()
		{
			var size = Mathf.Clamp(this.spells.Count - this.usableItemCounts.Count - 1, 0, short.MaxValue - 1);
			var spellColl = new short[size];
			int count = 0;

			if (this.spells.Count > 0)
			{
				foreach (var spell in this.spells.Values)
				{
					if ((spell.Flags & SpellFlags.FLAG_ITEM) == SpellFlags.FLAG_ITEM)
						continue;

					if (spell.Id == ServerGameSettings.USE_SPELL_ID)
						continue;

					spellColl[count++] = spell.Id;
				}
			}

			return new SpellsData
				{
					ClientSpells = spellColl,
					UsableItems = this.usableItemCounts.Keys.ToArray(),
				};
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds a spell and initializes it with the owner as the caster.
		/// </summary>
		/// <param name="spellId"> spell id to be added. </param>
		public void AddSpell(short spellId)
		{
			SpellData spellData;
			if (owner.CurrentZone.World.ItemCache.TryGetSpellData(spellId, out spellData))
			{
				var spell = new Spell(spellData, this.owner);
				if (AddSpellInternal(spell))
				{
					if (OnSpellAdded != null)
						this.OnSpellAdded(spell);
				}
			}
		}

		/// <summary>
		/// Removes a spell with an Id.
		/// </summary>
		/// <param name="id"> Id of the spell to be removed. </param>
		public void RemoveSpell(short id)
		{
			Spell spell;
			if(spells.TryGetValue(id, out spell))
			{
				this.spells.Remove(id);
				if (OnSpellRemoved != null)
					this.OnSpellRemoved(spell);
			}
		}

		/// <summary>
		/// Casts a spell
		/// </summary>
		public CastResults CastSpell(short id, MmoObject target)
		{
			if (IsCasting)
				return CastResults.AlreadyCasting;

			Spell spell;
			if (spells.TryGetValue(id, out spell))
			{
				if (spell.AffectedByGCD && this.inGcd)
					return CastResults.InGcd;

				var result = spell.Cast(target);
				if (result == CastResults.Ok)
				{
					if (spell.TriggersGCD)
					{
						this.inGcd = true;
						this.owner.BeginClientCooldown(-1); // SpellId of -1 = GCD

						this.gcdSubscription = this.owner.CurrentZone.PrimaryFiber.Schedule(() =>
							{
								this.inGcd = false;
								this.owner.EndClientCooldown(-1); // SpellId of -1 = GCD
							}
						, (long) (GCD * 1000)); // activates gcd
					}
				}

				return result;
			}
			
			return CastResults.SpellNotFound;
		}

		/// <summary>
		/// Stops the current casting
		/// </summary>
		public void StopCasting()
		{
			if (IsCasting)
			{
				Spell spell;
				if (spells.TryGetValue(this.currentSpellId, out spell))
				{
					spell.InteruptCast();
				}
			}
		}

		/// <summary>
		/// Casts the use spell on an <see cref="MmoObject"/>.
		/// </summary>
		/// <param name="mmoObject"></param>
		public void CastUse(MmoObject mmoObject)
		{
			this.CastSpell(ServerGameSettings.USE_SPELL_ID, mmoObject);
		}

		/// <summary>
		/// Stops casting use spell
		/// </summary>
		public void StopUse()
		{
			if (currentSpellId == ServerGameSettings.USE_SPELL_ID)
				this.StopCasting();
		}

		/// <summary>
		/// Add usable items
		/// </summary>
		public void AddUsableItem(IMmoItem item, int count)
		{
			int localCount;
			if (false == usableItemCounts.TryGetValue(item.Id, out localCount))
			{
				SpellData spellInfo;
				if (owner.CurrentZone.World.ItemCache.TryGetSpellData(item.SpellId, out spellInfo))
				{
					var spell = new Spell(spellInfo, this.owner);
					this.AddSpellInternal(spell);
					this.usableItemCounts.Add(item.Id, count);
				}
			}
			else
			{
				this.usableItemCounts[item.Id] = localCount + count;
			}
		}

		/// <summary>
		/// Remove usable items
		/// </summary>
		public void RemoveUsableItem(IMmoItem item, int count)
		{
			int localCount;
			if (usableItemCounts.TryGetValue(item.Id, out localCount))
			{
				if (localCount <= count)
				{
					this.usableItemCounts.Remove(item.Id);
					this.spells.Remove(item.SpellId);
				}
				else
				{
					this.usableItemCounts[item.Id] = localCount - count;
				}
			}
		}

		/// <summary>
		/// Adds a spell update event
		/// </summary>
		/// <param name="spell"></param>
		void ISpellController.AddSpellUpdateEvent(Spell spell)
		{
			var updateSubscription = this.owner.CreateUpdateEvent(ms => spell.Update(ms / 1000f), 100L);
			this.updateEvents.Add(spell, updateSubscription);
		}

		/// <summary>
		/// Removes a spell update event
		/// </summary>
		/// <param name="spell"></param>
		void ISpellController.RemoveSpellUpdateEvent(Spell spell)
		{
			lock (updateEvents)
			{
				IDisposable subscription;
				if (updateEvents.TryGetValue(spell, out subscription))
				{
					subscription.Dispose();
					updateEvents.Remove(spell);
				}
			}
		}

		/// <summary>
		/// Called from a <see cref="Spell"/> when it is started casting.
		/// </summary>
		void ISpellController.OnBeginCasting(Spell spell)
		{
			this.currentSpellId = spell.Id;
			this.owner.BeginClientCast(spell.Id);
		}

		/// <summary>
		/// Called by a <see cref="Spell"/> when it is finished casting.
		/// </summary>
		void ISpellController.OnEndCasting(Spell spell, bool isInterupt)
		{
			this.currentSpellId = 0;
			if (isInterupt)
			{
				this.owner.EndClientCast();
			}
		}

		/// <summary>
		/// Called by a <see cref="Spell"/> when its cooldown has started.
		/// </summary>
		/// <param name="spell"></param>
		void ISpellController.OnBeginCooldown(Spell spell)
		{
			this.owner.BeginClientCooldown(spell.Id);
		}

		/// <summary>
		/// Called by a <see cref="Spell"/> when its cooldown has ended.
		/// </summary>
		/// <param name="spell"></param>
		void ISpellController.OnEndCooldown(Spell spell)
		{
			this.owner.EndClientCooldown(spell.Id);
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Creates a new empty spell manager
		/// </summary>
		public static SpellManager CreateNew()
		{
			return new SpellManager();
		}

		#endregion

		#region Local Methods

		private bool AddSpellInternal(Spell spell)
		{
			if (!spells.ContainsKey(spell.Id))
			{
				this.spells.Add(spell.Id, spell);
				spell.Initialize(this);

				return true;
			}

			return false;
		}

		#endregion
	}
}
