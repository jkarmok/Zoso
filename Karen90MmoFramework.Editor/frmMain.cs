using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Karen90MmoFramework.Server.Data;
using Raven.Client.Indexes;

using Karen90MmoFramework.Database;

namespace Karen90MmoFramework.Editor
{
	public partial class frmMain : Form, ILogger, IGameDataHandler
	{
		/// <summary>
		/// world db
		/// </summary>
		private readonly DatabaseFactory worldDb;

		/// <summary>
		/// item db
		/// </summary>
		private readonly DatabaseFactory itemDb;

		/// <summary>
		/// items
		/// </summary>
		private readonly Dictionary<int, GameItemData> mmoItems;

		/// <summary>
		/// auras
		/// </summary>
		private readonly Dictionary<short, AuraData> auras;

		/// <summary>
		/// spells
		/// </summary>
		private readonly Dictionary<short, SpellData> spells;

		/// <summary>
		/// quests
		/// </summary>
		private readonly Dictionary<short, QuestData> quests;

		/// <summary>
		/// npcs
		/// </summary>
		private readonly Dictionary<int, NpcData> npcs;

		/// <summary>
		/// gos
		/// </summary>
		private readonly Dictionary<int, GameObjectData> gameObjects;

		/// <summary>
		/// loot group
		/// </summary>
		private readonly Dictionary<short, LootGroupData> lootGroups;

		public frmMain()
		{
			this.worldDb = new DatabaseFactory("WorldDB");
			this.itemDb = new DatabaseFactory("ItemDB");

			this.mmoItems = new Dictionary<int, GameItemData>();
			this.npcs = new Dictionary<int, NpcData>();
			this.gameObjects = new Dictionary<int, GameObjectData>();
			this.auras = new Dictionary<short, AuraData>();
			this.spells = new Dictionary<short, SpellData>();
			this.quests = new Dictionary<short, QuestData>();
			this.lootGroups = new Dictionary<short, LootGroupData>();

			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.worldDb.Initialize();
			this.itemDb.Initialize();

			//itemDb.CreateIndex<EntityByAuraId>();
			
			using (var session = worldDb.OpenSession())
			{
				var npcs = (from npc in session.Query<NpcData>() select npc).ToArray();
				foreach (var npc in npcs)
				{
					this.npcs.Add(npc.Guid, npc);
				}

				var gos = (from go in session.Query<GameObjectData>() select go).ToArray();
				foreach (var go in gos)
				{
					this.gameObjects.Add(go.Guid, go);
				}

				var quests = (from quest in session.Query<QuestData>() select quest).ToArray();
				foreach (var quest in quests)
				{
					this.quests.Add(quest.QuestId, quest);
				}

				var lootGroups = (from ltGrp in session.Query<LootGroupData>() select ltGrp).ToArray();
				foreach (var lootGroup in lootGroups)
				{
					this.lootGroups.Add(lootGroup.GroupId, lootGroup);
				}
			}

			using (var session = itemDb.OpenSession())
			{
				var items = (from item in session.Query<GameItemData>() select item).ToArray<GameItemData>();
				foreach (var item in items)
				{
					this.mmoItems.Add(item.ItemId, item);
				}

				var auras = (from aura in session.Query<AuraData>("EntityByAuraId")
				             select aura).ToArray<AuraData>();

				foreach (var aura in auras)
				{
					this.auras.Add(aura.AuraId, aura);
				}

				var spells = (from spell in session.Query<SpellData>()
				              select spell).ToArray<SpellData>();

				foreach (var spell in spells)
				{
					this.spells.Add(spell.SpellId, spell);
				}
			}

			// Custom Controls
			this.mmoItemsControl = new MmoItemsControl(this, this) {Dock = DockStyle.Fill};
			this.tabGameItem.Controls.Add(mmoItemsControl);

			this.npcControl = new NpcControl(this, this) {Dock = DockStyle.Fill};
			this.tabNpc.Controls.Add(npcControl);

			this.auraControl = new AuraControl(this, this) {Dock = DockStyle.Fill};
			this.tabAura.Controls.Add(auraControl);

			this.spellControl = new SpellControls(this, this) {Dock = DockStyle.Fill};
			this.tabSpell.Controls.Add(spellControl);

			this.questControl = new QuestControls(this, this) {Dock = DockStyle.Fill};
			this.tabQuest.Controls.Add(questControl);

			this.lootControl = new LootsControl(this, this) {Dock = DockStyle.Fill};
			this.tabLootGroup.Controls.Add(lootControl);

			this.goControl = new GameObjectControl(this, this) {Dock = DockStyle.Fill};
			this.tabGameObject.Controls.Add(goControl);
		}

		private void Form1_Disposed(object sender, System.EventArgs e)
		{
			this.worldDb.Dispose();
			this.itemDb.Dispose();
		}

		#region ILogger Implementation

		/// <summary>
		/// Sends a log
		/// </summary>
		public void Log(string content)
		{
			this.labelStatus.Text = content;
		}

		/// <summary>
		/// Sends a log as a format string
		/// </summary>
		public void LogFormat(string format, params object[] arguments)
		{
			this.labelStatus.Text = string.Format(format, arguments);
		}

		/// <summary>
		/// Displays an error message
		/// </summary>
		public void Error(string errorDescription)
		{
			MessageBox.Show(errorDescription, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}

		#endregion

		#region IGameData Handler Implementation

		/// <summary>
		/// Quests
		/// </summary>
		public Dictionary<short, QuestData> Quests
		{
			get
			{
				return this.quests;
			}
		}

		/// <summary>
		/// Spells
		/// </summary>
		public Dictionary<short, SpellData> Spells
		{
			get
			{
				return this.spells;
			}
		}

		/// <summary>
		/// Auras
		/// </summary>
		public Dictionary<short, AuraData> Auras
		{
			get
			{
				return this.auras;
			}
		}

		/// <summary>
		/// Items
		/// </summary>
		public Dictionary<int, GameItemData> MmoItems
		{
			get
			{
				return this.mmoItems;
			}
		}

		/// <summary>
		/// Npcs
		/// </summary>
		public Dictionary<int, NpcData> Npcs
		{
			get
			{
				return this.npcs;
			}
		}

		/// <summary>
		/// Game Objects
		/// </summary>
		public Dictionary<int, GameObjectData> GameObjects
		{
			get
			{
				return this.gameObjects;
			}
		}

		/// <summary>
		/// Loot Groups
		/// </summary>
		public Dictionary<short, LootGroupData> LootGroups
		{
			get
			{
				return this.lootGroups;
			}
		}

		public bool StoreMmoItem(GameItemData mmoItem)
		{
			if (mmoItems.ContainsKey(mmoItem.ItemId))
			{
				this.mmoItems[mmoItem.ItemId] = mmoItem;
			}
			else
			{
				this.mmoItems.Add(mmoItem.ItemId, mmoItem);
			}

			this.itemDb.Store(mmoItem);
			
			return true;
		}

		public bool RemoveMmoItem(int itemId)
		{
			GameItemData item;
			if (mmoItems.TryGetValue(itemId, out item))
			{
				itemDb.Delete(item.Id);

				return this.mmoItems.Remove(item.ItemId);
			}

			return false;
		}

		public bool StoreNpc(NpcData npc)
		{
			var id = npc.Guid;
			if (npcs.ContainsKey(id))
			{
				this.npcs[id] = npc;
			}
			else
			{
				this.npcs.Add(id, npc);
			}

			this.worldDb.Store(npc);

			return true;
		}

		public bool RemoveNpc(int npcId)
		{
			NpcData npc;
			if (npcs.TryGetValue(npcId, out npc))
			{
				itemDb.Delete(npc.Id);

				return this.npcs.Remove(npcId);
			}

			return false;
		}

		public bool StoreGO(GameObjectData go)
		{
			var id = go.Guid;
			if (gameObjects.ContainsKey(id))
			{
				this.gameObjects[id] = go;
			}
			else
			{
				this.gameObjects.Add(id, go);
			}

			this.worldDb.Store(go);

			return true;
		}

		public bool RemoveGO(int goId)
		{
			GameObjectData go;
			if (gameObjects.TryGetValue(goId, out go))
			{
				itemDb.Delete(go.Id);

				return this.npcs.Remove(goId);
			}

			return false;
		}

		public bool StoreAura(AuraData aura)
		{
			if (auras.ContainsKey(aura.AuraId))
			{
				this.auras[aura.AuraId] = aura;
			}
			else
			{
				this.auras.Add(aura.AuraId, aura);
			}

			this.itemDb.Store(aura);

			return true;
		}

		public bool RemoveAura(short auraId)
		{
			AuraData aura;
			if (auras.TryGetValue(auraId, out aura))
			{
				itemDb.Delete(aura.Id);

				return this.auras.Remove(aura.AuraId);
			}

			return false;
		}

		public bool StoreSpell(SpellData spell)
		{
			if (spells.ContainsKey(spell.SpellId))
			{
				this.spells[spell.SpellId] = spell;
			}
			else
			{
				this.spells.Add(spell.SpellId, spell);
			}

			this.itemDb.Store(spell);

			return true;
		}

		public bool RemoveSpell(short spellId)
		{
			SpellData spell;
			if (spells.TryGetValue(spellId, out spell))
			{
				itemDb.Delete(spell.Id);

				return this.spells.Remove(spell.SpellId);
			}

			return false;
		}

		public bool StoreQuest(QuestData quest)
		{
			if (quests.ContainsKey(quest.QuestId))
			{
				this.quests[quest.QuestId] = quest;
			}
			else
			{
				this.quests.Add(quest.QuestId, quest);
			}

			this.worldDb.Store(quest);

			return true;
		}

		public bool RemoveQuest(short questId)
		{
			QuestData quest;
			if (quests.TryGetValue(questId, out quest))
			{
				itemDb.Delete(quest.Id);

				return this.quests.Remove(quest.QuestId);
			}

			return false;
		}

		public bool StoreLootGroup(LootGroupData lootGroup)
		{
			if (lootGroups.ContainsKey(lootGroup.GroupId))
			{
				this.lootGroups[lootGroup.GroupId] = lootGroup;
			}
			else
			{
				this.lootGroups.Add(lootGroup.GroupId, lootGroup);
			}

			this.worldDb.Store(lootGroup);

			return true;
		}

		public bool RemoveLootGroup(short groupId)
		{
			LootGroupData lootGroup;
			if (lootGroups.TryGetValue(groupId, out lootGroup))
			{
				itemDb.Delete(lootGroup.Id);

				return this.quests.Remove(lootGroup.GroupId);
			}

			return false;
		}

		#endregion
	}

	public class EntityByAuraId : AbstractMultiMapIndexCreationTask
	{
		public EntityByAuraId()
		{
			AddMap<AuraData>(auras => from a in auras
										 select new { a.AuraId });

			AddMap<BuffData>(buffs => from b in buffs
									select new { b.AuraId });
		}
	}
}
