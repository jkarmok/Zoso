using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public partial class NpcControl : UserControl
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

		/// <summary>
		/// zones
		/// </summary>
		private readonly short[] zones;

		/// <summary>
		/// items window
		/// </summary>
		private readonly frmItemsWindow itemsWindow;

		private HashSet<short> possessiveItems;

		#endregion

		#region Constructor and Destructor

		public NpcControl(IGameDataHandler dataHandler, ILogger logger)
		{
			this.dataHandler = dataHandler;
			this.logger = logger;
			
			this.zones = new short[] { 1, 2 };
			this.itemsWindow = new frmItemsWindow(this.dataHandler, this.logger);
			this.possessiveItems = new HashSet<short>();

			InitializeComponent();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates a new id
		/// </summary>
		public int GenerateNewId()
		{
			for (var i = 10; i < int.MaxValue; i++)
			{
				if (!dataHandler.Npcs.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.labelNpcId.Text = GenerateNewId().ToString(CultureInfo.InvariantCulture);
			this.textBoxNpcName.Text = "Dezmond";
			this.textBoxNpcGroupId.Text = "0";
			this.textBoxCoordX.Text = "0.0";
			this.textBoxCoordY.Text = "0.0";
			this.textBoxCoordZ.Text = "0.0";

			this.possessiveItems.Clear();
			this.listBoxStartQuests.Items.Clear();
			this.listBoxCompleteQuests.Items.Clear();
		}

		#endregion

		#region Local Methods

		private void LoadNpc(NpcData npc)
		{
			this.textBoxNpcName.Text = npc.Name;
			this.labelNpcId.Text = npc.Guid.ToString(CultureInfo.InvariantCulture);
			this.comboBoxNpcType.SelectedIndex = npc.NpcType;
			this.comboBoxNpcAlignment.SelectedIndex = npc.Alignment;
			this.comboBoxSpecies.SelectedIndex = npc.Species;
			this.textBoxNpcGroupId.Text = npc.GroupId.ToString(CultureInfo.InvariantCulture);
			this.textBoxNpcLevel.Text = npc.Level.ToString(CultureInfo.InvariantCulture);
			this.textBoxLootGroupId.Text = npc.LootGroupId.ToString(CultureInfo.InvariantCulture);
			this.comboBoxNpcZone.SelectedIndex = Array.FindIndex(zones, id => id == npc.ZoneId);
			this.textBoxMaxHealth.Text = npc.MaxHealth.ToString(CultureInfo.InvariantCulture);
			this.textBoxMaxPower.Text = npc.MaxMana.ToString(CultureInfo.InvariantCulture);
			var pos = npc.Position;
			this.textBoxCoordX.Text = pos[0].ToString(CultureInfo.InvariantCulture);
			this.textBoxCoordY.Text = pos[1].ToString(CultureInfo.InvariantCulture);
			this.textBoxCoordZ.Text = pos[2].ToString(CultureInfo.InvariantCulture);
			this.txtOrientation.Text = npc.Orientation.ToString(CultureInfo.InvariantCulture);

			if (npc.Items != null)
			{
				this.possessiveItems = new HashSet<short>(npc.Items);
			}

			if (npc.StartQuests != null)
			{
				this.listBoxStartQuests.Items.AddRange(npc.StartQuests.Cast<object>().ToArray());
			}

			if (npc.CompleteQuests != null)
			{
				this.listBoxCompleteQuests.Items.AddRange(npc.CompleteQuests.Cast<object>().ToArray());
			}
		}

		private bool StoreNpc(NpcData npc)
		{
			npc.Name = this.textBoxNpcName.Text;
			npc.Guid = int.Parse(this.labelNpcId.Text);
			npc.NpcType = (byte)this.comboBoxNpcType.SelectedIndex;
			npc.Alignment = (byte)this.comboBoxNpcAlignment.SelectedIndex;
			npc.Species = (byte)this.comboBoxSpecies.SelectedIndex;
			npc.GroupId = short.Parse(this.textBoxNpcGroupId.Text);
			npc.Level = byte.Parse(this.textBoxNpcLevel.Text);
			npc.LootGroupId = short.Parse(textBoxLootGroupId.Text);
			npc.ZoneId = this.zones[this.comboBoxNpcZone.SelectedIndex];
			npc.Position = new[] { float.Parse(this.textBoxCoordX.Text), float.Parse(this.textBoxCoordY.Text), float.Parse(this.textBoxCoordZ.Text) };
			npc.Orientation = float.Parse(this.txtOrientation.Text);
			npc.MaxHealth = int.Parse(this.textBoxMaxHealth.Text);
			npc.MaxMana = int.Parse(this.textBoxMaxPower.Text);

			npc.Items = possessiveItems.Count > 0 ? this.possessiveItems.ToArray() : null;
			npc.StartQuests = listBoxStartQuests.Items.Count > 0 ? this.listBoxStartQuests.Items.Cast<short>().ToArray() : null;
			npc.CompleteQuests = listBoxCompleteQuests.Items.Count > 0 ? this.listBoxCompleteQuests.Items.Cast<short>().ToArray() : null;

			return this.dataHandler.StoreNpc(npc);
		}

		private bool AreFieldsValid(out string errorString)
		{
			errorString = string.Empty;
			if (this.dataHandler.Npcs.ContainsKey(int.Parse(this.labelNpcId.Text)))
			{
				errorString = "Npc ID already exists in the database";
				return false;
			}

			if (this.textBoxNpcName.TextLength <= 3)
			{
				errorString = "Name must be greater than 3 character length";
				return false;
			}
			
			if (int.Parse(this.textBoxNpcGroupId.Text) <= 0)
			{
				errorString = "GroupId must be greater than 0";
				return false;
			}

			if (int.Parse(this.textBoxNpcLevel.Text) <= 0)
			{
				errorString = "Level must be greater than 0";
				return false;
			}

			if (int.Parse(this.textBoxMaxHealth.Text) <= 0)
			{
				errorString = "MaxHealth must be greater than 0";
				return false;
			}

			if (int.Parse(this.textBoxMaxPower.Text) <= 0)
			{
				errorString = "MaxMana must be greater than 0";
				return false;
			}

			return true;
		}

		#endregion

		#region Control Events

		private void NpcControl_Load(object sender, EventArgs e)
		{
			var professions = (NpcType[])Enum.GetValues(typeof(NpcType));
			foreach (var i in professions)
			{
				this.comboBoxNpcType.Items.Add(i);
			}

			var types = (Species[])Enum.GetValues(typeof(Species));
			foreach (var i in types)
			{
				this.comboBoxSpecies.Items.Add(i);
			}

			var aligns = (SocialAlignment[])Enum.GetValues(typeof(SocialAlignment));
			foreach (var i in aligns)
			{
				this.comboBoxNpcAlignment.Items.Add(i);
			}

			foreach (var i in zones)
			{
				this.comboBoxNpcZone.Items.Add(i);
				var nodes = this.treeViewNpcs.Nodes.Add(i.ToString(), i.ToString()).Nodes;

				foreach (var prof in professions)
				{
					nodes.Add(prof.ToString());
				}
			}

			foreach (var i in this.dataHandler.Npcs.Values)
			{
				this.treeViewNpcs.Nodes[i.ZoneId.ToString()].Nodes[(int)i.NpcType].Nodes.Add(i.Guid.ToString(), i.Name);
			}
			
			this.ResetFields();
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			this.ResetFields();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeViewNpcs.SelectedNode.Level > 1)
			{
				var id = int.Parse(treeViewNpcs.SelectedNode.Name);

				NpcData npc;
				if (dataHandler.Npcs.TryGetValue(id, out npc))
				{
					if (MessageBox.Show("Delete Npc " + npc.Name + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						if (dataHandler.RemoveNpc(id))
						{
							this.treeViewNpcs.SelectedNode.Remove();
							this.logger.LogFormat("Npc Deleted. Id = {0} Name = {1} ", npc.Guid, npc.Name);
						}
					}
				}
			}
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (treeViewNpcs.SelectedNode.Level > 1)
			{
				var id = int.Parse(treeViewNpcs.SelectedNode.Name);

				NpcData npc;
				if (dataHandler.Npcs.TryGetValue(id, out npc))
				{
					if (StoreNpc(npc))
					{
						this.treeViewNpcs.SelectedNode.Text = npc.Name;
						this.logger.LogFormat("Npc Updated. Id = {0} Name = {1}", npc.Guid, npc.Name);
					}
				}
			}
		}

		private void buttonCreate_Click(object sender, EventArgs e)
		{
			string errorString;
			if (!AreFieldsValid(out errorString))
			{
				this.logger.Error(errorString);
				return;
			}

			var npc = new NpcData();
			npc.Id = NpcData.GenerateId(int.Parse(labelNpcId.Text));

			if (this.StoreNpc(npc))
			{
				var node = this.treeViewNpcs.Nodes[this.comboBoxNpcZone.SelectedIndex].Nodes[(int)this.comboBoxNpcType.SelectedIndex].Nodes.Add(npc.Guid.ToString(), npc.Name);
				this.logger.LogFormat("New Npc Created. Id = {0} Name = {1} ", npc.Guid, npc.Name);
				this.ResetFields();
			}
		}

		private void buttonPossessions_Click(object sender, EventArgs e)
		{
			this.itemsWindow.Display(this.possessiveItems, this);
		}

		private void treeViewNpcs_AfterSelect(object sender, TreeViewEventArgs e)
		{
			this.possessiveItems.Clear();

			if (e.Node.IsSelected && e.Node.Level > 1)
			{
				var id = int.Parse(e.Node.Name);

				NpcData npc;
				if (dataHandler.Npcs.TryGetValue(id, out npc))
				{
					this.listBoxStartQuests.Items.Clear();
					this.listBoxCompleteQuests.Items.Clear();

					this.LoadNpc(npc);

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

		private void textBoxInt32_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void textBoxFloat_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '.')
			{
				e.Handled = true;
			}
		}

		private void listBoxStartQuests_Enter(object sender, EventArgs e)
		{
			this.listBoxCompleteQuests.SelectedIndex = -1;
		}

		private void listBoxCompleteQuests_Enter(object sender, EventArgs e)
		{
			this.listBoxStartQuests.SelectedIndex = -1;
		}

		private void buttonQuestAdd_Click(object sender, EventArgs e)
		{
			var sQ = short.Parse(textBoxStartQuestId.Text);
			var cQ = short.Parse(textBoxCompleteQuestId.Text);

			if (sQ > 0)
			{
				if(!listBoxStartQuests.Items.Contains(sQ))
					this.listBoxStartQuests.Items.Add(sQ);
			}

			if (cQ > 0)
			{
				if (!listBoxCompleteQuests.Items.Contains(cQ))
					this.listBoxCompleteQuests.Items.Add(cQ);
			}
		}

		private void buttonQuestDelete_Click(object sender, EventArgs e)
		{
			var index1 = this.listBoxCompleteQuests.SelectedIndex;
			if (index1 != -1)
				this.listBoxCompleteQuests.Items.RemoveAt(index1);

			var index2 = this.listBoxStartQuests.SelectedIndex;
			if (index2 != -1)
				this.listBoxStartQuests.Items.RemoveAt(index2);
		}

		#endregion
	}
}
