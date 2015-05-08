using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public partial class frmItemsWindow : Form
	{
		private readonly IGameDataHandler gameData;

		private readonly ILogger logger;

		private readonly Dictionary<int, GameItemData> items;

		private HashSet<short> possessions;

		public frmItemsWindow(IGameDataHandler gameData, ILogger logger)
		{
			this.gameData = gameData;
			this.items = gameData.MmoItems;
			this.logger = logger;

			InitializeComponent();

			foreach (var item in this.items)
			{
				this.listBoxItems.Items.Add(item.Value);
			}
		}

		public void Display(HashSet<short> possessions, IWin32Window owner)
		{
			this.possessions = possessions;

			foreach (var id in possessions)
			{
				var item = this.items[id];
				this.listBoxPossessions.Items.Add(item);
				this.listBoxItems.Items.Remove(item);
			}

			this.ShowDialog(owner);
		}

		private void loadItemsToFields(GameItemData item)
		{
			this.lblName.Text = item.Name;
			this.lblId.Text = item.ItemId.ToString();
			this.lblType.Text = item.ItemType.ToString();
			this.lblRarity.Text = item.Rarity.ToString();
			this.lblSell.Text = item.SellPrice.ToString();
			this.lblBuy.Text = item.BuyoutPrice.ToString();
		}

		private void resetFields()
		{
			var text = "select an item";

			this.lblName.Text = text;
			this.lblId.Text = text;
			this.lblType.Text = text;
			this.lblRarity.Text = text;
			this.lblSell.Text = text;
			this.lblBuy.Text = text;
		}

		private void frmItemsWindow_Load(object sender, EventArgs e)
		{
			if (possessions == null)
			{
				this.logger.Error("Possessive items cannot be null");
				this.Close();
			}

			this.resetFields();
		}

		private void frmItemsWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			foreach (var id in this.possessions)
			{
				this.listBoxItems.Items.Add(this.items[id]);
			}

			this.listBoxPossessions.Items.Clear();
			this.possessions = null;
		}

		private void buttonAddToItems_Click(object sender, EventArgs e)
		{
			if (listBoxItems.SelectedIndex < 0)
			{
				this.logger.Error("Select an item");
				return;
			}

			var item = this.listBoxItems.SelectedItem as GameItemData;
			if (possessions.Contains((short)item.ItemId))
			{
				this.logger.Error("Already has that item");
				return;
			}

			this.possessions.Add((short)item.ItemId);
			this.listBoxPossessions.Items.Add(item);
			this.listBoxItems.Items.Remove(item);
		}

		private void buttonRemoveFromItems_Click(object sender, EventArgs e)
		{
			if (listBoxPossessions.SelectedIndex < 0)
			{
				this.logger.Error("Select an item");
				return;
			}

			var item = this.listBoxPossessions.SelectedItem as GameItemData;
			if (possessions.Remove((short)item.ItemId))
			{
				this.listBoxPossessions.Items.Remove(item);
				this.listBoxItems.Items.Add(item);
			}
		}

		private void listBoxItems_SelectedValueChanged(object sender, EventArgs e)
		{
			if (listBoxItems.SelectedIndex >= 0)
			{
				this.loadItemsToFields(this.listBoxItems.SelectedItem as GameItemData);
			}
		}

		private void listBoxPossessions_SelectedValueChanged(object sender, EventArgs e)
		{
			if (listBoxPossessions.SelectedIndex >= 0)
			{
				this.loadItemsToFields(this.listBoxPossessions.SelectedItem as GameItemData);
			}
		}
	}
}
