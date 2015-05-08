using System;
using System.Drawing;
using System.Windows.Forms;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;

namespace Karen90MmoFramework.Editor
{
	public partial class AuraControl : UserControl
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

		private Stats[] stats;
		private Vitals[] vitalTypes;

		#endregion

		#region Constructor and Destructor

		public AuraControl(IGameDataHandler dataHandler, ILogger logger)
		{
			this.dataHandler = dataHandler;
			this.logger = logger;

			InitializeComponent();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Generates a new aura id
		/// </summary>
		public short GenerateNewAuraId()
		{
			for (short i = 10; i < short.MaxValue; i++)
			{
				if (!this.dataHandler.Auras.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.labelAuraId.Text = this.GenerateNewAuraId().ToString();
			this.textBoxAuraName.Text = "Aura_" + this.labelAuraId.Text;
			this.textBoxAuraFamilyId.Text = "0";
			this.textBoxAuraDuration.Text = "0";
			this.textBoxAmount.Text = "0";
			this.textBoxFrequency.Text = "0";

			this.buttonDelete.Enabled = false;
			this.buttonUpdate.Enabled = false;
		}

		#endregion

		#region Local Methods

		/// <summary>
		/// Loads aura info for display
		/// </summary>
		private void LoadAura(AuraData aura)
		{
			this.labelAuraId.Text = aura.AuraId.ToString();
			this.comboBoxAuraType.SelectedIndex = (int)aura.AuraType;
			this.textBoxAuraName.Text = aura.Name;
			this.textBoxAuraLevel.Text = aura.Level.ToString();

			if (checkBoxExpirable.Checked = aura.Duration.HasValue)
			{
				this.textBoxAuraDuration.Text = aura.Duration.Value.ToString();
			}

			var buff = aura as BuffData;
			if (buff != null)
			{
				this.groupBoxBuff.Visible = true;
				this.comboBoxBuffType.SelectedIndex = (int)buff.BuffType;
				this.comboBoxBuffTypeVal1.SelectedIndex = buff.Vital.HasValue ? (int)buff.Vital.Value : (int)buff.Attribute.Value;
				this.radioButtonBeneficial.Checked = buff.Beneficial;
				this.textBoxFrequency.Text = buff.Frequency.HasValue ? buff.Frequency.Value.ToString() : "0";
				this.textBoxAmount.Text = buff.Amount.ToString();
			}
		}

		/// <summary>
		/// Saves info to the aura
		/// </summary>
		private bool StoreAura(AuraData aura)
		{
			aura.Name = this.textBoxAuraName.Text;
			aura.Level = byte.Parse(this.textBoxAuraLevel.Text);
			aura.FamilyId = short.Parse(this.textBoxAuraFamilyId.Text);
			aura.Duration = this.checkBoxExpirable.Checked ? short.Parse(this.textBoxAuraDuration.Text) : (short?)null;

			if (aura.Level < 1)
			{
				logger.Error("Level must be greater than 0");
				return false;
			}

			if (aura.FamilyId < 1)
			{
				logger.Error("FamilyId must be greater than 0");
				return false;
			}

			if (aura.Duration.HasValue && aura.Duration.Value < 1)
			{
				logger.Error("Duration must be greater than 0");
				return false;
			}

			switch (aura.AuraType)
			{
				case (byte) AuraType.Buff:
					{
						var buff = aura as BuffData;
						buff.BuffType = (byte)this.comboBoxBuffType.SelectedIndex;
						buff.Beneficial = this.radioButtonBeneficial.Checked;
						buff.Amount = int.Parse(this.textBoxAmount.Text);

						switch (buff.BuffType)
						{
							case (byte) BuffType.AttributeFixed:
								{
									buff.Attribute = (byte) this.comboBoxBuffTypeVal1.SelectedIndex;
								}
								break;

							case (byte) BuffType.VitalOvertime:
								{
									buff.Vital = (byte) this.comboBoxBuffTypeVal1.SelectedIndex;
									buff.Frequency = short.Parse(this.textBoxFrequency.Text);

									if (buff.Frequency < 1)
									{
										logger.Error("Frequency must be greater than 0");
										return false;
									}
								}
								break;
						}
					}
					break;

				default:
					{
						logger.Error(string.Format("Aura type {0} is not handled", aura.AuraType));
					}
					break;
			}

			return this.dataHandler.StoreAura(aura);
		}

		#endregion

		#region Control Events

		private void AuraControl_Load(object sender, EventArgs e)
		{
			var auraTypes = (AuraType[])Enum.GetValues(typeof(AuraType));

			foreach (var item in auraTypes)
			{
				this.treeViewAuras.Nodes.Add(((int)item).ToString(), item.ToString());
				this.comboBoxAuraType.Items.Add(item);
			}

			this.comboBoxAuraType.SelectedIndex = 0;

			this.stats = (Stats[])Enum.GetValues(typeof(Stats));
			this.vitalTypes = (Vitals[])Enum.GetValues(typeof(Vitals));

			var buffTypes = (BuffType[])Enum.GetValues(typeof(BuffType));

			foreach (var item in buffTypes)
			{
				this.comboBoxBuffType.Items.Add(item);
			}

			this.comboBoxBuffType.SelectedIndex = 0;

			var auras = this.dataHandler.Auras;
			foreach (var aura in auras.Values)
			{
				var node = this.treeViewAuras.Nodes[(int)aura.AuraType].Nodes.Add(aura.AuraId.ToString(), string.Format("{0} - Lvl {1}", aura.Name, aura.Level));
			}

			this.buttonCreate.NotifyDefault(true);
			this.ResetFields();
		}

		private void LoadComboBoxBuffTypeVal1(Type val)
		{
			this.comboBoxBuffTypeVal1.Items.Clear();

			if (val == typeof(Stats))
			{
				foreach (var item in stats)
				{
					this.comboBoxBuffTypeVal1.Items.Add(item);
				}
			}
			else
			{
				foreach (var item in vitalTypes)
				{
					this.comboBoxBuffTypeVal1.Items.Add(item);
				}
			}

			this.comboBoxBuffTypeVal1.SelectedIndex = 0;
		}

		private void buttonCreate_Click(object sender, EventArgs e)
		{
			short auraId = short.Parse(this.labelAuraId.Text);

			if (this.dataHandler.Auras.ContainsKey(auraId))
			{
				MessageBox.Show("Auraid already exists in the database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			AuraData aura;
			var type = (byte) this.comboBoxAuraType.SelectedIndex;
			switch (type)
			{
				case (byte) AuraType.Buff:
					aura = new BuffData();
					break;

				default:
					aura = new AuraData();
					break;
			}

			aura.Id = AuraData.GenerateId(auraId);
			aura.AuraId = auraId;
			aura.AuraType = type;
			
			if (this.StoreAura(aura))
			{
				var node = this.treeViewAuras.Nodes[(int)aura.AuraType].Nodes.Add(aura.AuraId.ToString(), string.Format("{0} - Lvl {1}", aura.Name, aura.Level));

				this.logger.LogFormat("New Aura Created. Id = {0} Name = {1}", auraId, aura.Name);
				this.ResetFields();
			}
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			if (treeViewAuras.SelectedNode.Level > 0)
			{
				short auraId = short.Parse(treeViewAuras.SelectedNode.Name);

				AuraData aura;
				if (dataHandler.Auras.TryGetValue(auraId, out aura))
				{
					if (MessageBox.Show("Delete Aura " + aura.Name + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
					{
						if (dataHandler.RemoveAura(auraId))
						{
							this.treeViewAuras.SelectedNode.Remove();
							this.logger.LogFormat("Aura Deleted. Id = {0} Name = {1} ", auraId, aura.Name);
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
			if (treeViewAuras.SelectedNode.Level > 0)
			{
				short auraId = short.Parse(treeViewAuras.SelectedNode.Name);

				AuraData aura;
				if (dataHandler.Auras.TryGetValue(auraId, out aura))
				{
					if (StoreAura(aura))
					{
						this.logger.LogFormat("Item Updated. Id = {0} Name = {1}", auraId, aura.Name);
					}
				}
			}
		}

		private void textBoxAuraLevel_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void textBoxAuraFamilyId_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void textBoxAuraDuration_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		private void textBoxAmount_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b' && e.KeyChar != '-')
			{
				e.Handled = true;
			}
		}

		private void checkBoxExpirable_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxExpirable.Checked)
			{
				checkBoxExpirable.Font = new Font(checkBoxExpirable.Font, FontStyle.Bold);
				this.textBoxAuraDuration.Enabled = true;
			}
			else
			{
				checkBoxExpirable.Font = new Font(checkBoxExpirable.Font, FontStyle.Regular);
				this.textBoxAuraDuration.Enabled = false;
			}
		}

		private void comboBoxAuraType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch ((AuraType)comboBoxAuraType.SelectedIndex)
			{
				case AuraType.Buff:
					{
						this.groupBoxBuff.Visible = true;
					}
					break;

				case AuraType.Special:
					{
						this.groupBoxBuff.Visible = false;
					}
					break;
			}
		}

		private void comboBoxBuffType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch ((BuffType)comboBoxBuffType.SelectedIndex)
			{
				case BuffType.AttributeFixed:
					{
						this.LoadComboBoxBuffTypeVal1(typeof(Stats));
						this.labelBuffTypeVal1.Text = "Attribute:";
						this.panelFrequency.Visible = false;
					}
					break;

				case BuffType.VitalOvertime:
					{
						this.LoadComboBoxBuffTypeVal1(typeof(Vitals));
						this.labelBuffTypeVal1.Text = "Vital:";
						this.panelFrequency.Visible = true;
					}
					break;
			}
		}

		private void treeViewAuras_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.IsSelected && e.Node.Level > 0)
			{
				var id = short.Parse(e.Node.Name);

				AuraData aura;
				if (dataHandler.Auras.TryGetValue(id, out aura))
				{
					this.LoadAura(aura);

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
