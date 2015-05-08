using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Karen90MmoFramework.Game;
using Karen90MmoFramework.Server.Data;
using Karen90MmoFramework.Server.Game.Systems;

namespace Karen90MmoFramework.Editor
{
	public partial class SpellControls : UserControl
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

		private SpellSchools[] schools;
		private SpellTargetTypes[] targets;
		private WeaponTypes[] weapons;
		private SpellFlags[] flags;
		private SpellAffectionMethod[] affectionMethods;

		#endregion

		#region Constructors and Destructors

		public SpellControls(IGameDataHandler dataHandler, ILogger logger)
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
				if (!dataHandler.Spells.ContainsKey(i))
					return i;
			}

			throw new ArithmeticException("Max Id limit reached");
		}

		/// <summary>
		/// Resets all the fields
		/// </summary>
		public void ResetFields()
		{
			this.labelSpellId.Text = this.GenerateNewId().ToString();
			this.textBoxSpellName.Text = "Spell_" + this.labelSpellId.Text;

			ClearChecked(this.checkedListSchools);
			ClearChecked(this.checkedListTargets);
			ClearChecked(this.checkedListWeapons);
			ClearChecked(this.checkedListFlags);

			this.cboAffectionMethod.SelectedIndex = 0;

			this.checkBoxEffect1.Checked = false;
			this.checkBoxEffect2.Checked = false;
			this.checkBoxEffect3.Checked = false;

			this.comboBoxEffect1.SelectedIndex = 0;
			this.comboBoxEffect2.SelectedIndex = 0;
			this.comboBoxEffect3.SelectedIndex = 0;

			this.textBoxValue1.Text = "0";
			this.textBoxValue2.Text = "0";
			this.textBoxValue3.Text = "0";

			this.panelEffect1.Visible = false;
			this.panelEffect2.Visible = false;
			this.panelEffect3.Visible = false;

			this.buttonDelete.Enabled = false;
			this.buttonUpdate.Enabled = false;
		}

		#endregion

		#region Local Methods
		
		private void LoadSpell(SpellData spell)
		{
			this.labelSpellId.Text = spell.SpellId.ToString();
			this.textBoxSpellName.Text = spell.Name;
			this.comboBoxTargetMethod.SelectedIndex = (int)spell.TargetSelectionMethod;
			this.cboAffectionMethod.SelectedIndex = (int) spell.AffectionMethod;
			this.comboBoxPowerType.SelectedIndex = (int)spell.PowerType;
			this.textBoxPowerCost.Text = spell.PowerCost.ToString();
			this.textBoxCooldown.Text = spell.Cooldown.ToString();
			this.textBoxCastTime.Text = spell.CastTime.ToString();
			this.textBoxMinDis.Text = spell.MinCastRadius.ToString();
			this.textBoxMaxDis.Text = spell.MaxCastRadius.ToString();
			this.checkBoxAffByGCD.Checked = spell.AffectedByGCD;
			this.checkBoxTriggerGCD.Checked = spell.TriggersGCD;
			this.checkBoxProc.Checked = spell.IsProc;

			for (var i = 0; i < schools.Length; i++)
			{
				var school = (byte) schools[i];
				if ((spell.School & school) == school)
				{
					this.checkedListSchools.SetItemChecked(i, true);
				}
			}

			for (var i = 0; i < targets.Length; i++)
			{
				var targetType = (byte) targets[i];
				if ((spell.RequiredTargetType & targetType) == targetType)
				{
					this.checkedListTargets.SetItemChecked(i, true);
				}
			}

			for (var i = 0; i < weapons.Length; i++)
			{
				var weaponType = (short) weapons[i];
				if ((spell.RequiredWeaponType & weaponType) == weaponType)
				{
					this.checkedListWeapons.SetItemChecked(i, true);
				}
			}

			for (int i = 0; i < flags.Length; i++)
			{
				var spellFlags = (int) flags[i];
				if ((spell.Flags & spellFlags) == spellFlags)
				{
					this.checkedListFlags.SetItemChecked(i, true);
				}
			}

			if (spell.Effects != null)
			{
				var length = spell.Effects.Length;
				if (length > 0)
				{
					this.comboBoxEffect1.SelectedIndex = (int)spell.Effects[0];
					this.textBoxValue1.Text = spell.EffectBaseValues[0].ToString();
				}

				this.checkBoxEffect1.Checked = length > 0;

				if (length > 1)
				{
					this.comboBoxEffect2.SelectedIndex = (int)spell.Effects[1];
					this.textBoxValue2.Text = spell.EffectBaseValues[1].ToString();
				}

				this.checkBoxEffect2.Checked = length > 1;

				if (length > 2)
				{
					this.checkBoxEffect3.Checked = true;
					this.comboBoxEffect3.SelectedIndex = (int)spell.Effects[2];
					this.textBoxValue3.Text = spell.EffectBaseValues[2].ToString();
				}

				this.checkBoxEffect3.Checked = length > 2;
			}
		}

		private bool StoreSpell(SpellData spell)
		{
			spell.Name = this.textBoxSpellName.Text;
			spell.TargetSelectionMethod = (byte) this.comboBoxTargetMethod.SelectedIndex;
			spell.AffectionMethod = (byte) this.cboAffectionMethod.SelectedIndex;
			spell.PowerType = (byte) this.comboBoxPowerType.SelectedIndex;
			spell.PowerCost = int.Parse(this.textBoxPowerCost.Text);
			spell.Cooldown = float.Parse(this.textBoxCooldown.Text);
			spell.CastTime = float.Parse(this.textBoxCastTime.Text);
			spell.MinCastRadius = int.Parse(this.textBoxMinDis.Text);
			spell.MaxCastRadius = int.Parse(this.textBoxMaxDis.Text);
			spell.AffectedByGCD = this.checkBoxAffByGCD.Checked;
			spell.IsProc = this.checkBoxProc.Checked;
			spell.TriggersGCD = this.checkBoxTriggerGCD.Checked;

			var spellSchool = SpellSchools.None;
			for (var i = 0; i < schools.Length; i++)
			{
				if (checkedListSchools.GetItemChecked(i))
				{
					spellSchool |= this.schools[i];
				}
			}

			spell.School =  (byte) spellSchool;

			var spellTargets = SpellTargetTypes.None;
			for (var i = 0; i < targets.Length; i++)
			{
				if (checkedListTargets.GetItemChecked(i))
				{
					spellTargets |= this.targets[i];
				}
			}

			spell.RequiredTargetType = (byte) spellTargets;

			var spellWeapons = WeaponTypes.None;
			for (var i = 0; i < weapons.Length; i++)
			{
				if (checkedListWeapons.GetItemChecked(i))
				{
					spellWeapons |= this.weapons[i];
				}
			}

			spell.RequiredWeaponType = (short) spellWeapons;

			var spellFlags = SpellFlags.FLAG_NONE;
			for (var i = 0; i < flags.Length; i++)
			{
				if (checkedListFlags.GetItemChecked(i))
				{
					spellFlags |= this.flags[i];
				}
			}

			spell.Flags = (int) spellFlags;

			var fx = new List<int>();
			var fxVal = new List<int>();

			if (checkBoxEffect1.Checked)
			{
				fx.Add(comboBoxEffect1.SelectedIndex);
				fxVal.Add(int.Parse(textBoxValue1.Text));
			}

			if (checkBoxEffect2.Checked)
			{
				fx.Add(comboBoxEffect2.SelectedIndex);
				fxVal.Add(int.Parse(textBoxValue2.Text));
			}

			if (checkBoxEffect3.Checked)
			{
				fx.Add(comboBoxEffect3.SelectedIndex);
				fxVal.Add(int.Parse(textBoxValue3.Text));
			}

			if (fx.Count > 0)
			{
				spell.Effects = fx.ToArray();
				spell.EffectBaseValues = fxVal.ToArray();
			}

			if (spell.MinCastRadius >= spell.MaxCastRadius)
			{
				logger.Error("Min Distance must be less than Max Distance");
				return false;
			}

			return this.dataHandler.StoreSpell(spell);
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
			var targetSelectionMethods = (SpellTargetSelectionMethods[])Enum.GetValues(typeof(SpellTargetSelectionMethods));
			foreach (var item in targetSelectionMethods)
			{
				this.comboBoxTargetMethod.Items.Add(item);
			}

			this.comboBoxTargetMethod.SelectedIndex = 0;

			var powerTypes = (Vitals[])Enum.GetValues(typeof(Vitals));
			foreach (var item in powerTypes)
			{
				this.comboBoxPowerType.Items.Add(item);
			}

			this.comboBoxPowerType.SelectedIndex = 0;

			var tAffects = (SpellAffectionMethod[])Enum.GetValues(typeof(SpellAffectionMethod));
			foreach (var affect in tAffects)
			{
				this.cboAffectionMethod.Items.Add(affect);
			}

			this.cboAffectionMethod.SelectedIndex = 0;

			var tSchools = (SpellSchools[])Enum.GetValues(typeof(SpellSchools));
			var lSchools = new List<SpellSchools>();
			foreach (var school in tSchools)
			{
				var val = (int) school;
				var n = Math.Log(val, 2);

				if (val > 0 && (n - (int) n) == 0)
				{
					lSchools.Add(school);
					this.checkedListSchools.Items.Add(school);
				}
			}

			this.schools = lSchools.ToArray();

			var tTargets = (SpellTargetTypes[])Enum.GetValues(typeof(SpellTargetTypes));
			var lTargets = new List<SpellTargetTypes>();
			foreach (var target in tTargets)
			{
				var val = (int)target;
				var n = Math.Log(val, 2);

				if (val > 0 && (n - (int)n) == 0)
				{
					lTargets.Add(target);
					this.checkedListTargets.Items.Add(target);
				}
			}

			this.targets = lTargets.ToArray();

			var tWeapons = (WeaponTypes[])Enum.GetValues(typeof(WeaponTypes));
			var lWeapons = new List<WeaponTypes>();
			foreach (var weapon in tWeapons)
			{
				var val = (int)weapon;
				var n = Math.Log(val, 2);

				if (val > 0 && (n - (int)n) == 0)
				{
					lWeapons.Add(weapon);
					this.checkedListWeapons.Items.Add(weapon);
				}
			}

			this.weapons = lWeapons.ToArray();

			var tFlags = (SpellFlags[])Enum.GetValues(typeof(SpellFlags));
			var lFlags = new List<SpellFlags>();
			foreach (var flags in tFlags)
			{
				var val = (int)flags;
				var n = Math.Log(val, 2);

				if (val > 0 && (n - (int)n) == 0)
				{
					lFlags.Add(flags);
					this.checkedListFlags.Items.Add(flags);
				}
			}

			this.flags = lFlags.ToArray();

			var effects = (SpellEffects[])Enum.GetValues(typeof(SpellEffects));
			foreach (var item in effects)
			{
				this.comboBoxEffect1.Items.Add(item);
				this.comboBoxEffect2.Items.Add(item);
				this.comboBoxEffect3.Items.Add(item);
			}

			this.comboBoxEffect1.SelectedIndex = 0;
			this.comboBoxEffect2.SelectedIndex = 0;
			this.comboBoxEffect3.SelectedIndex = 0;

			var spells = this.dataHandler.Spells;
			foreach (var spell in spells.Values)
			{
				var node = this.treeViewSpells.Nodes.Add(spell.SpellId.ToString(), spell.Name);
			}

			this.buttonCreate.NotifyDefault(true);
			this.ResetFields();
		}

		private void buttonCreate_Click(object sender, EventArgs e)
		{
			short spellId = short.Parse(this.labelSpellId.Text);

			if (this.dataHandler.Spells.ContainsKey(spellId))
			{
				MessageBox.Show("SpellId already exists in the database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			var spell = new SpellData
				{
					Id = SpellData.GenerateId(spellId),
					SpellId = spellId,
				};

			if (this.StoreSpell(spell))
			{
				var node = this.treeViewSpells.Nodes.Add(spell.SpellId.ToString(), spell.Name);

				this.logger.LogFormat("New Spell Created. Id = {0} Name = {1}", spellId, spell.Name);
				this.ResetFields();
			}
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			var spellId = short.Parse(treeViewSpells.SelectedNode.Name);

			SpellData spell;
			if (dataHandler.Spells.TryGetValue(spellId, out spell))
			{
				if (MessageBox.Show("Delete Spell " + spell.Name + "?", "Delete Item", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					if (dataHandler.RemoveSpell(spellId))
					{
						this.treeViewSpells.SelectedNode.Remove();
						this.logger.LogFormat("Spell Deleted. Id = {0} Name = {1} ", spellId, spell.Name);
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
			var spellId = short.Parse(treeViewSpells.SelectedNode.Name);

			SpellData spell;
			if (dataHandler.Spells.TryGetValue(spellId, out spell))
			{
				if (StoreSpell(spell))
				{
					this.logger.LogFormat("Spell Updated. Id = {0} Name = {1}", spellId, spell.Name);
				}
			}
		}

		private void treeViewAuras_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.IsSelected)
			{
				var id = short.Parse(e.Node.Name);

				SpellData spell;
				if (dataHandler.Spells.TryGetValue(id, out spell))
				{
					this.ResetFields();
					this.LoadSpell(spell);

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

		private void textBoxAuraLevel_KeyPress(object sender, KeyPressEventArgs e)
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

		private void checkBoxEffect1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxEffect1.Checked)
			{
				this.checkBoxEffect1.Font = new Font(checkBoxEffect1.Font, FontStyle.Bold);
				this.panelEffect1.Visible = true;

			}
			else
			{
				this.checkBoxEffect1.Font = new Font(checkBoxEffect1.Font, FontStyle.Regular);
				this.panelEffect1.Visible = false;
			}
		}

		private void checkBoxEffect2_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxEffect2.Checked)
			{
				this.checkBoxEffect2.Font = new Font(checkBoxEffect2.Font, FontStyle.Bold);
				this.panelEffect2.Visible = true;

			}
			else
			{
				this.checkBoxEffect2.Font = new Font(checkBoxEffect2.Font, FontStyle.Regular);
				this.panelEffect2.Visible = false;
			}
		}

		private void checkBoxEffect3_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxEffect3.Checked)
			{
				this.checkBoxEffect3.Font = new Font(checkBoxEffect3.Font, FontStyle.Bold);
				this.panelEffect3.Visible = true;

			}
			else
			{
				this.checkBoxEffect3.Font = new Font(checkBoxEffect3.Font, FontStyle.Regular);
				this.panelEffect3.Visible = false;
			}
		}

		private void checkBoxTriggerGCD_CheckedChanged(object sender, EventArgs e)
		{
			this.checkBoxTriggerGCD.Font = checkBoxTriggerGCD.Checked
				                               ? new Font(checkBoxTriggerGCD.Font, FontStyle.Bold)
				                               : new Font(checkBoxTriggerGCD.Font, FontStyle.Regular);
		}

		private void checkBoxAffByGCD_CheckedChanged(object sender, EventArgs e)
		{
			this.checkBoxAffByGCD.Font = checkBoxAffByGCD.Checked
				                             ? new Font(checkBoxAffByGCD.Font, FontStyle.Bold)
				                             : new Font(checkBoxAffByGCD.Font, FontStyle.Regular);
		}

		private void checkBoxProc_CheckedChanged(object sender, EventArgs e)
		{
			this.checkBoxProc.Font = checkBoxProc.Checked
				                         ? new Font(checkBoxProc.Font, FontStyle.Bold)
				                         : new Font(checkBoxProc.Font, FontStyle.Regular);
		}

		private void checkedListSchools_Leave(object sender, EventArgs e)
		{
			this.checkedListSchools.ClearSelected();
		}

		private void checkedListTargets_Leave(object sender, EventArgs e)
		{
			this.checkedListTargets.ClearSelected();
		}

		private void checkedListWeapons_Leave(object sender, EventArgs e)
		{
			this.checkedListWeapons.ClearSelected();
		}

		private void checkedListFlags_Leave(object sender, EventArgs e)
		{
			this.checkedListFlags.ClearSelected();
		}

		#endregion
	}
}
