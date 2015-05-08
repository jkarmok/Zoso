using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Editor
{
	public partial class QuestControls : UserControl
	{
		#region Constants and Fields

		/// <summary>
		/// logger
		/// </summary>
		private readonly ILogger logger;

		/// <summary>
		/// parent form
		/// </summary>
		private readonly IGameDataHandler dataHandler;

		private QuestFlags[] flags;

		#endregion

		#region Constructors and Destructors

		public QuestControls(IGameDataHandler dataHandler, ILogger logger)
		{
			this.dataHandler = dataHandler;
			this.logger = logger;

			InitializeComponent();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates a new id
		/// </summary>
		public short GenerateNewId()
		{
			for (short i = 10; i < short.MaxValue; i++)
			{
				if (!this.dataHandler.Quests.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.labelId.Text = this.GenerateNewId().ToString();
			this.textBoxName.Text = "Quest_" + this.labelId.Text;

			ClearChecked(checkedListFlags);

			this.listBoxReqItems.Items.Clear();
			this.listBoxReqNpcs.Items.Clear();
			this.listBoxConversation.Items.Clear();
			this.listBoxRewItems.Items.Clear();
			this.listBoxRewOptItems.Items.Clear();
			this.listBoxRewSpells.Items.Clear();

			var zero = "0";
			this.textBoxReqPlayers.Text = zero;
			this.textBoxRewXp.Text = zero;
			this.textBoxRewGold.Text = zero;
			this.textBoxRewRenown.Text = zero;
			this.textBoxNextQuest.Text = zero;
			this.textBoxPrevQuest.Text = zero;
		}

		#endregion

		#region Local Methods

		private void LoadQuest(QuestData quest)
		{
			this.ResetFields();

			this.labelId.Text = quest.QuestId.ToString();
			this.textBoxName.Text = quest.Name;
			this.textBoxLevel.Text = quest.Level.ToString();

			for (var i = 1; i < flags.Length; i++)
			{
				var questFlags = quest.Flags;
				if (questFlags.HasFlag(flags[i]))
				{
					this.checkedListFlags.SetItemChecked(i - 1, true);
				}
			}

			this.listBoxReqItems.Items.AddRange(quest.ItemRequirements.Cast<object>().ToArray());
			this.listBoxReqNpcs.Items.AddRange(quest.NpcRequirements.Cast<object>().ToArray());
			this.listBoxConversation.Items.AddRange(quest.QuestConversations.Cast<object>().ToArray());
			this.textBoxReqPlayers.Text = quest.PlayerRequirements.ToString(CultureInfo.InvariantCulture);
			this.textBoxPrevQuest.Text = quest.PrevQuest.ToString(CultureInfo.InvariantCulture);
			this.textBoxNextQuest.Text = quest.NextQuest.ToString(CultureInfo.InvariantCulture);

			this.listBoxRewItems.Items.AddRange(quest.RewardItems.Cast<object>().ToArray());
			this.listBoxRewSpells.Items.AddRange(quest.RewardSpells.Cast<object>().ToArray());
			this.listBoxRewOptItems.Items.AddRange(quest.RewardOptionalItems.Cast<object>().ToArray());
			this.textBoxRewXp.Text = quest.RewardXp.ToString(CultureInfo.InvariantCulture);
			this.textBoxRewGold.Text = quest.RewardMoney.ToString(CultureInfo.InvariantCulture);
			this.textBoxRewRenown.Text = quest.RewardRenown.ToString(CultureInfo.InvariantCulture);
		}

		private bool StoreQuest(QuestData quest)
		{
			var questFlags = QuestFlags.QUEST_FLAG_NONE;
			for (var i = 0; i < this.checkedListFlags.Items.Count; i++)
			{
				if (checkedListFlags.GetItemChecked(i))
				{
					questFlags |= this.flags[i + 1];
				}
			}

			quest.Flags = questFlags;
			quest.PrevQuest = short.Parse(textBoxPrevQuest.Text);
			quest.NextQuest = short.Parse(textBoxNextQuest.Text);

			quest.ItemRequirements = this.listBoxReqItems.Items.Cast<ItemRequirement>().ToArray();
			quest.NpcRequirements = this.listBoxReqNpcs.Items.Cast<NpcRequirement>().ToArray();
			quest.QuestConversations = this.listBoxConversation.Items.Cast<ConversationStructure>().ToArray();
			quest.PlayerRequirements = byte.Parse(textBoxReqPlayers.Text);

			quest.RewardItems = this.listBoxRewItems.Items.Cast<ItemStructure>().ToArray();
			quest.RewardOptionalItems = this.listBoxRewOptItems.Items.Cast<ItemStructure>().ToArray();
			quest.RewardSpells = this.listBoxRewSpells.Items.Cast<short>().ToArray();
			quest.RewardMoney = int.Parse(textBoxRewGold.Text);
			quest.RewardXp = int.Parse(textBoxRewXp.Text);
			quest.RewardRenown = int.Parse(textBoxRewRenown.Text);

			return this.dataHandler.StoreQuest(quest);
		}

		private static void ClearChecked(CheckedListBox list)
		{
			for (var i = 0; i < list.Items.Count; i++)
			{
				list.SetItemChecked(i, false);
			}
		}

		#endregion

		#region Control Events

		private void AuraControl_Load(object sender, EventArgs e)
		{
			this.flags = (QuestFlags[]) Enum.GetValues(typeof (QuestFlags));
			foreach (var flag in this.flags)
			{
				if (flag > QuestFlags.QUEST_FLAG_NONE)
					this.checkedListFlags.Items.Add(flag);
			}

			foreach (var quest in this.dataHandler.Quests.Values)
			{
				var level = quest.Level.ToString(CultureInfo.InvariantCulture);
				if (!treeViewQuests.Nodes.ContainsKey(level))
				{
					treeViewQuests.Nodes.Add(level, level);
				}

				treeViewQuests.Nodes[level].Nodes.Add(quest.QuestId.ToString(), quest.Name);
			}
			
			this.buttonCreate.NotifyDefault(true);
			this.ResetFields();
		}

		private void buttonCreate_Click(object sender, EventArgs e)
		{
			var questId = short.Parse(this.labelId.Text);

			if (dataHandler.Quests.ContainsKey(questId))
			{
				this.logger.Error("QuestId already exists in the database");
				return;
			}

			if (checkedListFlags.CheckedItems.Count == 0)
			{
				this.logger.Error("Must check atleast 1 flag");
				return;
			}

			var quest = new QuestData
				{
					Id = QuestData.GenerateId(questId),
					QuestId = questId,
					Name = this.textBoxName.Text,
					Level = short.Parse(textBoxLevel.Text)
				};

			if (this.StoreQuest(quest))
			{
				var level = quest.Level.ToString(CultureInfo.InvariantCulture);
				if (!treeViewQuests.Nodes.ContainsKey(level))
				{
					this.treeViewQuests.Nodes.Add(level, level);
				}

				this.treeViewQuests.Nodes[level].Nodes.Add(quest.QuestId.ToString(), quest.Name);

				this.logger.LogFormat("New Quest Created. Id = {0} Name = {1}", questId, quest.Name);
				this.ResetFields();
			}
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeViewQuests.SelectedNode.Level > 0)
			{
				var questId = short.Parse(treeViewQuests.SelectedNode.Name);

				QuestData quest;
				if (dataHandler.Quests.TryGetValue(questId, out quest))
				{
					if (MessageBox.Show("Delete Quest " + quest.Name + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						if (dataHandler.RemoveQuest(questId))
						{
							this.treeViewQuests.SelectedNode.Remove();
							this.logger.LogFormat("Quest Deleted. Id = {0} Name = {1} ", questId, quest.Name);
						}
					}
				}
			}
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			this.ResetFields();
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (treeViewQuests.SelectedNode.Level > 0)
			{
				var questId = short.Parse(treeViewQuests.SelectedNode.Name);

				QuestData quest;
				if (dataHandler.Quests.TryGetValue(questId, out quest))
				{
					if (StoreQuest(quest))
					{
						this.logger.LogFormat("Quest Updated. Id = {0} Name = {1}", questId, quest.Name);
					}
				}
			}
		}

		private void treeViewAuras_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.IsSelected && e.Node.Level > 0)
			{
				var id = short.Parse(e.Node.Name);

				QuestData quest;
				if (dataHandler.Quests.TryGetValue(id, out quest))
				{
					this.LoadQuest(quest);

					this.buttonDelete.Enabled = true;
					this.buttonUpdate.Enabled = true;
				}
			}
			else
			{
				this.buttonDelete.Enabled = false;
				this.buttonUpdate.Enabled = false;
			}
		}

		private void textBoxNumeric_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void buttonReqItemAdd_Click(object sender, EventArgs e)
		{
			var itemId = short.Parse(textBoxReqItemId.Text);
			if (itemId <= 0)
			{
				logger.Error("ItemId must be greater than 0");
				return;
			}

			var count = byte.Parse(textBoxReqItemCount.Text);
			if (count <= 0)
			{
				logger.Error("Count must be greater than 0");
				return;
			}

			var itemRequirement = new ItemRequirement(itemId, count);
			if (listBoxReqItems.Items.Contains(itemRequirement))
			{
				logger.Error("ItemId already added");
				return;
			}

			this.listBoxReqItems.Items.Add(itemRequirement);
		}

		private void buttonReqItemDelete_Click(object sender, EventArgs e)
		{
			var index = this.listBoxReqItems.SelectedIndex;
			if (index < 0)
			{
				logger.Error("Need a selection");
				return;
			}

			this.listBoxReqItems.Items.RemoveAt(index);
		}

		private void buttonReqNpcAdd_Click(object sender, EventArgs e)
		{
			var npcId = int.Parse(textBoxReqNpcId.Text);
			if (npcId <= 0)
			{
				logger.Error("NpcId must be greater than 0");
				return;
			}

			var count = byte.Parse(textBoxReqNpcCount.Text);
			if (count <= 0)
			{
				logger.Error("Count must be greater than 0");
				return;
			}

			var npcRequirement = new NpcRequirement(npcId, count);
			if (listBoxReqNpcs.Items.Contains(npcRequirement))
			{
				logger.Error("NpcId already added");
				return;
			}

			this.listBoxReqNpcs.Items.Add(npcRequirement);
		}

		private void buttonReqNpcDelete_Click(object sender, EventArgs e)
		{
			var index = this.listBoxReqNpcs.SelectedIndex;
			if (index < 0)
			{
				logger.Error("Need a selection");
				return;
			}

			this.listBoxReqNpcs.Items.RemoveAt(index);
		}

		private void buttonRewSpellAdd_Click(object sender, EventArgs e)
		{
			var spellId = short.Parse(textBoxRewSpellId.Text);
			if (spellId <= 0)
			{
				logger.Error("SpellId must be greater than 0");
				return;
			}

			if (listBoxRewSpells.Items.Contains(spellId))
			{
				logger.Error("SpellId already added");
				return;
			}

			this.listBoxRewSpells.Items.Add(spellId);
		}

		private void buttonRewSpellDelete_Click(object sender, EventArgs e)
		{
			var index = this.listBoxRewSpells.SelectedIndex;
			if (index < 0)
			{
				logger.Error("Need a selection");
				return;
			}

			this.listBoxRewSpells.Items.RemoveAt(index);
		}

		private void buttonRewItemAdd_Click(object sender, EventArgs e)
		{
			var itemId = short.Parse(textBoxRewItemId.Text);
			if (itemId <= 0)
			{
				logger.Error("ItemId must be greater than 0");
				return;
			}

			var count = byte.Parse(textBoxRewItemCount.Text);
			if (count <= 0)
			{
				logger.Error("Count must be greater than 0");
				return;
			}

			var itemStructure = new ItemStructure(itemId, count);
			if (listBoxRewItems.Items.Contains(itemStructure))
			{
				logger.Error("ItemId already added");
				return;
			}

			this.listBoxRewItems.Items.Add(itemStructure);
		}

		private void buttonRewItemDelete_Click(object sender, EventArgs e)
		{
			var index = this.listBoxRewItems.SelectedIndex;
			if (index < 0)
			{
				logger.Error("Need a selection");
				return;
			}

			this.listBoxRewItems.Items.RemoveAt(index);
		}

		private void buttonRewOptItemAdd_Click(object sender, EventArgs e)
		{
			var itemId = short.Parse(textBoxRewOptItemId.Text);
			if (itemId <= 0)
			{
				logger.Error("ItemId must be greater than 0");
				return;
			}

			var count = byte.Parse(textBoxRewOptItemCount.Text);
			if (count <= 0)
			{
				logger.Error("Count must be greater than 0");
				return;
			}

			var itemStructure = new ItemStructure(itemId, count);
			if (listBoxRewOptItems.Items.Contains(itemStructure))
			{
				logger.Error("ItemId already added");
				return;
			}

			this.listBoxRewOptItems.Items.Add(itemStructure);
		}

		private void buttonRewOptItemDelete_Click(object sender, EventArgs e)
		{
			var index = this.listBoxRewOptItems.SelectedIndex;
			if (index < 0)
			{
				logger.Error("Need a selection");
				return;
			}

			this.listBoxRewOptItems.Items.RemoveAt(index);
		}

		private void buttonConversationAdd_Click(object sender, EventArgs e)
		{
			var npcId = int.Parse(textBoxConvNpcId.Text);
			if (npcId <= 0)
			{
				logger.Error("NpcId must be greater than 0");
				return;
			}

			var convId = short.Parse(textBoxConversationId.Text);
			if (convId <= 0)
			{
				logger.Error("Count must be greater than 0");
				return;
			}

			var conversationStructure = new ConversationStructure(npcId, convId);
			if (listBoxConversation.Items.Contains(conversationStructure))
			{
				logger.Error("NpcId already added");
				return;
			}

			this.listBoxConversation.Items.Add(conversationStructure);
		}

		private void buttonConversationDelete_Click(object sender, EventArgs e)
		{
			var index = this.listBoxConversation.SelectedIndex;
			if (index < 0)
			{
				logger.Error("Need a selection");
				return;
			}

			this.listBoxConversation.Items.RemoveAt(index);
		}

		#endregion
	}
}
