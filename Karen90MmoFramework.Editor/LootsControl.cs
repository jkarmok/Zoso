using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public partial class LootsControl : UserControl
	{
		#region Members

		/// <summary>
		/// logger
		/// </summary>
		private readonly ILogger logger;

		/// <summary>
		/// parent form
		/// </summary>
		private readonly IGameDataHandler dataHandler;

		#endregion

		#region Constructor and Destructor

		public LootsControl(IGameDataHandler dataHandler, ILogger logger)
		{
			this.dataHandler = dataHandler;
			this.logger = logger;

			InitializeComponent();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates a new group id
		/// </summary>
		public short GenerateGroupId()
		{
			for (short i = 10; i < short.MaxValue; i++)
			{
				if (!this.dataHandler.LootGroups.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.labelLootGroupId.Text = this.GenerateGroupId().ToString();
			this.listBoxLootItems.Items.Clear();

			this.buttonDelete.Enabled = false;
			this.buttonUpdate.Enabled = false;
		}

		#endregion

		#region Local Methods

		/// <summary>
		/// Loads item info for display
		/// </summary>
		private void loadItem(LootGroupData item)
		{
			this.labelLootGroupId.Text = item.GroupId.ToString();
			this.textBoxMinGold.Text = item.MinGold.ToString();
			this.textBoxMaxGold.Text = item.MaxGold.ToString();
			this.textBoxGoldChance.Text = item.GoldChance.ToString();
			this.listBoxLootItems.Items.AddRange(item.LootItems.Cast<object>().ToArray());
		}

		/// <summary>
		/// Saves info to the item
		/// </summary>
		private bool storeItem(LootGroupData item)
		{
			item.MinGold = byte.Parse(textBoxMinGold.Text);
			item.MaxGold = byte.Parse(textBoxMaxGold.Text);
			item.GoldChance = float.Parse(textBoxGoldChance.Text);

			item.LootItems = listBoxLootItems.Items.Cast<LootItemData>().ToArray();

			return this.dataHandler.StoreLootGroup(item);
		}

		#endregion

		#region Control Events

		private void MmoItemsControl_Load(object sender, EventArgs e)
		{
			foreach (var lootGroup in this.dataHandler.LootGroups.Values)
			{
				var id = lootGroup.GroupId.ToString(CultureInfo.InvariantCulture);
				if (!treeViewLoots.Nodes.ContainsKey(id))
					treeViewLoots.Nodes.Add(id, id);
			}

			this.buttonCreate.NotifyDefault(true);
			this.ResetFields();
		}

		private void buttonCreate_Click(object sender, EventArgs e)
		{
			var groupId = short.Parse(this.labelLootGroupId.Text);

			if (dataHandler.LootGroups.ContainsKey(groupId))
			{
				this.logger.Error("GroupId already exists in the database");
				return;
			}

			var lootGroup = new LootGroupData
			{
				Id = LootGroupData.GenerateId(groupId),
				GroupId = groupId,
			};

			if (storeItem(lootGroup))
			{
				var idToString = groupId.ToString();
				if (!treeViewLoots.Nodes.ContainsKey(idToString))
				{
					this.treeViewLoots.Nodes.Add(idToString, idToString);
					this.logger.LogFormat("New Loot Group Created. Id = {0}", lootGroup.GroupId);
					this.ResetFields();
				}
			}
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeViewLoots.SelectedNode.Level > 1)
			{
				var id = short.Parse(treeViewLoots.SelectedNode.Name);

				LootGroupData lootGroup;
				if (dataHandler.LootGroups.TryGetValue(id, out lootGroup))
				{
					if (MessageBox.Show("Delete LootGroup " + lootGroup.GroupId + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						if (dataHandler.RemoveLootGroup(id))
						{
							this.treeViewLoots.SelectedNode.Remove();
							this.logger.LogFormat("LootGroup Deleted. Id = {0} ", lootGroup.GroupId);
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
			var id = short.Parse(treeViewLoots.SelectedNode.Name);

			LootGroupData lootGroup;
			if (dataHandler.LootGroups.TryGetValue(id, out lootGroup))
			{
				if (storeItem(lootGroup))
				{
					this.logger.LogFormat("LootGroup Updated. Id = {0}", lootGroup.GroupId);
				}
			}
		}

		private void textBoxNumeric_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void textBoxDecimal_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '.')
			{
				e.Handled = true;
			}
		}

		private void buttonItemDelete_Click(object sender, EventArgs e)
		{
			this.listBoxLootItems.Items.RemoveAt(listBoxLootItems.SelectedIndex);
		}

		private void buttonItemAdd_Click(object sender, EventArgs e)
		{
			short itemId;
			if (short.TryParse(textBoxItemId.Text, out itemId) == false || itemId < 1)
			{
				this.logger.Error("Invalid ItemId");
				return;
			}

			byte min;
			if (byte.TryParse(textBoxItemMinCount.Text, out min) == false || min == 0)
			{
				this.logger.Error("Invalid Min Count");
				return;
			}

			byte max;
			if (byte.TryParse(textBoxItemMaxCount.Text, out max) == false || max == 0)
			{
				this.logger.Error("Invalid Max Count");
				return;
			}

			if (max < min)
			{
				this.logger.Error("Min must be less than Max");
				return;
			}

			float chance;
			if (float.TryParse(textBoxItemDropChance.Text, out chance) == false || chance <= 0f || chance > 100f)
			{
				this.logger.Error("Invalid Drop Chance");
				return;
			}

			var item = new LootItemData
			{
				ItemId = itemId,
				MinCount = min,
				MaxCount = max,
				DropChance = chance,
			};
			
			if (listBoxLootItems.Items.Contains(item))
			{
				this.logger.Error("Loot item already added");
				return;
			}

			this.listBoxLootItems.Items.Add(item);
		}

		private void treeViewLoots_AfterSelect(object sender, TreeViewEventArgs e)
		{
			var id = short.Parse(e.Node.Name);

			LootGroupData lootGroup;
			if (dataHandler.LootGroups.TryGetValue(id, out lootGroup))
			{
				this.listBoxLootItems.Items.Clear();
				this.loadItem(lootGroup);

				this.buttonDelete.Enabled = true;
				this.buttonUpdate.Enabled = true;
			}
			else
			{
				this.buttonDelete.Enabled = true;
				this.buttonUpdate.Enabled = true;
			}
		}

		#endregion
	}
}
