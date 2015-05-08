using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public partial class MmoItemsControl : UserControl
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

		#endregion

		#region Constructor and Destructor

		public MmoItemsControl(IGameDataHandler dataHandler, ILogger logger)
		{
			this.dataHandler = dataHandler;
			this.logger = logger;

			InitializeComponent();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates a new item id
		/// </summary>
		public short GenerateNewItemId(MmoItemType itemType)
		{
			for (short i = 10; i <= short.MaxValue; i++)
			{
				if (!dataHandler.MmoItems.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.labelItemId.Text = GenerateNewItemId(0).ToString(CultureInfo.InvariantCulture);
			this.comboBoxItemType.Enabled = true;
			this.comboBoxItemType.SelectedIndex = 0;
			this.textBoxItemName.Text = "Item" + this.labelItemId.Text;
			this.textBoxBuyout.Text = "0";
			this.textBoxSell.Text = "0";
			this.textBoxMaxStack.Text = "0";
			this.checkBoxTradable.Checked = true;

			this.buttonDelete.Enabled = false;
			this.buttonUpdate.Enabled = false;
		}

		/// <summary>
		/// Gets an item color for its rarity
		/// </summary>
		public Color GetItemColor(Rarity rarity)
		{
			switch (rarity)
			{
				case Rarity.Common:
					return Color.White;

				case Rarity.Uncommon:
					return Color.LawnGreen;

				case Rarity.Rare:
					return Color.DeepSkyBlue;

				case Rarity.Superior:
					return Color.Magenta;

				case Rarity.Legendary:
					return Color.Yellow;

				case Rarity.Ancestral:
					return Color.HotPink;

				default:
					return Color.Maroon;
			}
		}

		#endregion

		#region Local Methods

		/// <summary>
		/// Loads item info for display
		/// </summary>
		private void LoadItem(GameItemData item)
		{
			this.labelItemId.Text = item.ItemId.ToString(CultureInfo.InvariantCulture);
			this.comboBoxItemType.SelectedIndex = item.ItemType;
			this.comboBoxItemType.Enabled = false;
			this.textBoxItemName.Text = item.Name;
			this.textBoxBuyout.Text = item.BuyoutPrice.ToString(CultureInfo.InvariantCulture);
			this.textBoxSell.Text = item.SellPrice.ToString(CultureInfo.InvariantCulture);
			this.comboBoxRarity.SelectedIndex = item.Rarity;
			this.textBoxMaxStack.Text = item.MaxStack.ToString(CultureInfo.InvariantCulture);
			this.checkBoxTradable.Checked = item.IsTradable;
			this.textBoxSpellId.Text = item.UseSpellId.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Saves info to the item
		/// </summary>
		private bool StoreItem(GameItemData item)
		{
			item.ItemType = (byte) this.comboBoxItemType.SelectedIndex;
			item.Name = this.textBoxItemName.Text;
			item.BuyoutPrice = int.Parse(this.textBoxBuyout.Text);
			item.SellPrice = int.Parse(this.textBoxSell.Text);
			item.Rarity = (byte) this.comboBoxRarity.SelectedIndex;
			item.MaxStack = int.Parse(this.textBoxMaxStack.Text);
			item.IsTradable = this.checkBoxTradable.Checked;
			item.UseSpellId = short.Parse(this.textBoxSpellId.Text);

			return this.dataHandler.StoreMmoItem(item);
		}

		#endregion

		#region Control Events

		private void MmoItemsControl_Load(object sender, EventArgs e)
		{
			foreach (var itemType in Enum.GetValues(typeof(MmoItemType)))
			{
				treeViewItems.Nodes.Add(((byte) itemType).ToString(CultureInfo.InvariantCulture), itemType.ToString());
				comboBoxItemType.Items.Add(itemType);
			}
			comboBoxItemType.SelectedIndex = 0;

			foreach (var rarity in Enum.GetValues(typeof(Rarity)))
				comboBoxRarity.Items.Add(rarity);
			comboBoxRarity.SelectedIndex = 0;

			foreach (var useLimit in Enum.GetValues(typeof(UseLimit)))
				comboBoxUseLimit.Items.Add(useLimit);
			comboBoxUseLimit.SelectedIndex = 0;

			buttonCreate.NotifyDefault(true);

			var items = this.dataHandler.MmoItems;
			foreach (var item in items.Values)
			{
				var node = this.treeViewItems.Nodes[item.ItemType].Nodes.Add(item.ItemId.ToString(CultureInfo.InvariantCulture), item.Name);
				node.BackColor = this.GetItemColor((Rarity) item.Rarity);
			}

			this.ResetFields();
		}

		private void buttonCreate_Click(object sender, EventArgs e)
		{
			var itemId = short.Parse(this.labelItemId.Text);

			if (this.dataHandler.MmoItems.ContainsKey(itemId))
			{
				MessageBox.Show("Item id already exists in the database", "Item Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			if (int.Parse(textBoxMaxStack.Text) <= 0)
			{
				MessageBox.Show("Max Stack should be greater than 0", "Item Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			var item = new GameItemData {Id = GameItemData.GenerateId(itemId), ItemId = itemId};
			if (this.StoreItem(item))
			{
				var node = this.treeViewItems.Nodes[item.ItemType].Nodes.Add(item.ItemId.ToString(CultureInfo.InvariantCulture), item.Name);
				node.BackColor = this.GetItemColor((Rarity) item.Rarity);

				this.logger.LogFormat("New Item Created. Id = {0} Name = {1} ", item.ItemId, item.Name);
				this.ResetFields();
			}
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeViewItems.SelectedNode.Level > 0)
			{
				var id = int.Parse(treeViewItems.SelectedNode.Name);

				GameItemData item;
				if (dataHandler.MmoItems.TryGetValue(id, out item))
				{
					if (MessageBox.Show("Delete Item " + item.Name + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						if (dataHandler.RemoveMmoItem(id))
						{
							this.treeViewItems.SelectedNode.Remove();
							this.logger.LogFormat("Item Deleted. Id = {0} Name = {1} ", item.ItemId, item.Name);
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
			if (treeViewItems.SelectedNode.Level > 0)
			{
				var id = int.Parse(treeViewItems.SelectedNode.Name);

				GameItemData item;
				if (dataHandler.MmoItems.TryGetValue(id, out item))
				{
					if (StoreItem(item))
					{
						this.treeViewItems.SelectedNode.Text = item.Name;
						this.logger.LogFormat("Item Updated. Id = {0} Name = {1}", item.ItemId, item.Name);
					}
				}
			}
		}

		private void checkBoxCollectable_CheckedChanged(object sender, EventArgs e)
		{
			checkBoxTradable.Font = checkBoxTradable.Checked
				                           ? new Font(checkBoxTradable.Font, FontStyle.Bold)
				                           : new Font(checkBoxTradable.Font, FontStyle.Regular);
		}

		private void comboBoxItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBoxItemType.Enabled)
				this.labelItemId.Text = this.GenerateNewItemId((MmoItemType) comboBoxItemType.SelectedIndex).ToString(CultureInfo.InvariantCulture);
		}

		private void textBoxBuyout_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
				e.Handled = true;
		}

		private void textBoxSell_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
				e.Handled = true;
		}

		private void textBoxMaxStack_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
				e.Handled = true;
		}

		private void treeViewGameItems_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.IsSelected && e.Node.Level > 0)
			{
				var id = int.Parse(e.Node.Name);

				GameItemData item;
				if (dataHandler.MmoItems.TryGetValue(id, out item))
				{
					this.LoadItem(item);

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

		#endregion
	}
}
