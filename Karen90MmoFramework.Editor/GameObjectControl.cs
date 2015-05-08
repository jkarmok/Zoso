using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public partial class GameObjectControl : UserControl
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

		/// <summary>
		/// zones
		/// </summary>
		private readonly int[] zones;

		#endregion

		#region Constructor and Destructor

		public GameObjectControl(IGameDataHandler dataHandler, ILogger logger)
		{
			this.dataHandler = dataHandler;
			this.logger = logger;
			
			this.zones = new[] { 1, 2 };

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
				if (!dataHandler.GameObjects.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.lblId.Text = GenerateNewId().ToString(CultureInfo.InvariantCulture);
			this.txtName.Text = @"One Third Leomonade";
			this.txtGroupId.Text = "0";
			this.txtOrientation.Text = "0.0";
			this.txtCoordX.Text = "0.0";
			this.txtCoordY.Text = "0.0";
			this.txtCoordZ.Text = "0.0";

			this.lstStartQuests.Items.Clear();
			this.lstCompleteQuests.Items.Clear();
		}

		#endregion

		#region Local Methods

		private void LoadGameObject(GameObjectData go)
		{
			this.txtName.Text = go.Name;
			this.lblId.Text = go.Guid.ToString(CultureInfo.InvariantCulture);
			this.cboGOType.SelectedIndex = go.GOType;
			this.txtGroupId.Text = go.GroupId.ToString(CultureInfo.InvariantCulture);
			this.txtLootGrpId.Text = go.LootGroupId.ToString(CultureInfo.InvariantCulture);
			this.cboZone.SelectedIndex = Array.FindIndex(zones, id => id == go.ZoneId);
			this.txtOrientation.Text = go.Orientation.ToString(CultureInfo.InvariantCulture);
			var pos = go.Position;
			this.txtCoordX.Text = pos[0].ToString(CultureInfo.InvariantCulture);
			this.txtCoordY.Text = pos[1].ToString(CultureInfo.InvariantCulture);
			this.txtCoordZ.Text = pos[2].ToString(CultureInfo.InvariantCulture);

			if (go.StartQuests != null)
			{
				this.lstStartQuests.Items.AddRange(go.StartQuests.Cast<object>().ToArray());
			}

			if (go.CompleteQuests != null)
			{
				this.lstCompleteQuests.Items.AddRange(go.CompleteQuests.Cast<object>().ToArray());
			}
		}

		private bool StoreGameObject(GameObjectData go)
		{
			go.Name = this.txtName.Text;
			go.Guid = int.Parse(this.lblId.Text);

			var goType = (GameObjectType) this.cboGOType.SelectedIndex;
			go.GOType = (byte) goType;

			go.GroupId = short.Parse(txtGroupId.Text);
			go.LootGroupId = short.Parse(txtLootGrpId.Text);
			go.ZoneId = this.zones[this.cboZone.SelectedIndex];
			go.Orientation = float.Parse(txtOrientation.Text);
			go.Position = new[] { float.Parse(this.txtCoordX.Text), float.Parse(this.txtCoordY.Text), float.Parse(this.txtCoordZ.Text) };

			go.StartQuests = (lstStartQuests.Items.Count > 0) ? this.lstStartQuests.Items.Cast<short>().ToArray() : null;
			go.CompleteQuests = (lstCompleteQuests.Items.Count > 0) ? this.lstCompleteQuests.Items.Cast<short>().ToArray() : null;
			
			return this.dataHandler.StoreGO(go);
		}

		private bool AreFieldsValid(out string errorString)
		{
			errorString = string.Empty;
			if (this.dataHandler.GameObjects.ContainsKey(int.Parse(this.lblId.Text)))
			{
				errorString = "Game Object ID already exists in the database";
				return false;
			}

			if (this.txtName.TextLength <= 3)
			{
				errorString = "Name must be greater than 3 character length";
				return false;
			}

			return true;
		}

		#endregion

		#region Control Events

		private void GameObjectControl_Load(object sender, EventArgs e)
		{
			var goTypes = (GameObjectType[]) Enum.GetValues(typeof (GameObjectType));
			foreach (var i in goTypes)
			{
				this.cboGOType.Items.Add(i);
			}

			foreach (var i in zones)
			{
				this.cboZone.Items.Add(i);
				var nodes = this.treeViewGOs.Nodes.Add(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture)).Nodes;

				foreach (var prof in goTypes)
				{
					nodes.Add(prof.ToString());
				}
			}

			foreach (var i in this.dataHandler.GameObjects.Values)
			{
				this.treeViewGOs.Nodes[i.ZoneId.ToString(CultureInfo.InvariantCulture)].Nodes[i.GOType].Nodes.Add(i.Guid.ToString(CultureInfo.InvariantCulture), i.Name);
			}

			this.ResetFields();
		}

		private void buttonNew_Click(object sender, EventArgs e)
		{
			this.ResetFields();
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeViewGOs.SelectedNode.Level > 1)
			{
				var id = int.Parse(treeViewGOs.SelectedNode.Name);

				GameObjectData go;
				if (dataHandler.GameObjects.TryGetValue(id, out go))
				{
					if (MessageBox.Show("Delete Game Object " + go.Name + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						if (dataHandler.RemoveGO(id))
						{
							this.treeViewGOs.SelectedNode.Remove();
							this.logger.LogFormat("Game Object Deleted. Id = {0} Name = {1} ", go.Guid, go.Name);
						}
					}
				}
			}
		}

		private void buttonUpdate_Click(object sender, EventArgs e)
		{
			if (treeViewGOs.SelectedNode.Level > 1)
			{
				var id = int.Parse(treeViewGOs.SelectedNode.Name);

				GameObjectData go;
				if (dataHandler.GameObjects.TryGetValue(id, out go))
				{
					if (StoreGameObject(go))
					{
						this.treeViewGOs.SelectedNode.Text = go.Name;
						this.logger.LogFormat("Game Object Updated. Id = {0} Name = {1}", go.Guid, go.Name);
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

			var go = new GameObjectData {Id = GameObjectData.GenerateId(short.Parse(this.lblId.Text))};

			if (this.StoreGameObject(go))
			{
				this.treeViewGOs.Nodes[this.cboZone.SelectedIndex].Nodes[this.cboGOType.SelectedIndex].Nodes.Add(go.Guid.ToString(), go.Name);
				this.logger.LogFormat("New Game Object Created. Id = {0} Name = {1} ", go.Guid, go.Name);
				this.ResetFields();
			}
		}

		private void treeViewNpcs_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.IsSelected && e.Node.Level > 1)
			{
				var id = int.Parse(e.Node.Name);

				GameObjectData go;
				if (dataHandler.GameObjects.TryGetValue(id, out go))
				{
					this.lstStartQuests.Items.Clear();
					this.lstCompleteQuests.Items.Clear();

					this.LoadGameObject(go);

					this.btnDelete.Enabled = true;
					this.btnUpdate.Enabled = true;
				}
			}
			else
			{
				this.btnDelete.Enabled = false;
				this.btnUpdate.Enabled = false;
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
			this.lstCompleteQuests.SelectedIndex = -1;
		}

		private void listBoxCompleteQuests_Enter(object sender, EventArgs e)
		{
			this.lstStartQuests.SelectedIndex = -1;
		}

		private void buttonQuestAdd_Click(object sender, EventArgs e)
		{
			var sQ = short.Parse(txtStartQuestId.Text);
			var cQ = short.Parse(txtCompleteQuestId.Text);

			if (sQ > 0)
			{
				if(!lstStartQuests.Items.Contains(sQ))
					this.lstStartQuests.Items.Add(sQ);
			}

			if (cQ > 0)
			{
				if (!lstCompleteQuests.Items.Contains(cQ))
					this.lstCompleteQuests.Items.Add(cQ);
			}
		}

		private void buttonQuestDelete_Click(object sender, EventArgs e)
		{
			var index1 = this.lstCompleteQuests.SelectedIndex;
			if (index1 != -1)
				this.lstCompleteQuests.Items.RemoveAt(index1);

			var index2 = this.lstStartQuests.SelectedIndex;
			if (index2 != -1)
				this.lstStartQuests.Items.RemoveAt(index2);
		}

		#endregion
	}
}
