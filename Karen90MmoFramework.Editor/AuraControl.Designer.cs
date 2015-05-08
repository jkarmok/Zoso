namespace Karen90MmoFramework.Editor
{
	partial class AuraControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeViewAuras = new System.Windows.Forms.TreeView();
			this.groupBoxBuff = new System.Windows.Forms.GroupBox();
			this.panelFrequency = new System.Windows.Forms.Panel();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textBoxFrequency = new System.Windows.Forms.TextBox();
			this.textBoxAmount = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.radioButtonNonBeneficial = new System.Windows.Forms.RadioButton();
			this.radioButtonBeneficial = new System.Windows.Forms.RadioButton();
			this.comboBoxBuffTypeVal1 = new System.Windows.Forms.ComboBox();
			this.comboBoxBuffType = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.labelBuffTypeVal1 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.buttonNew = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBoxAuraDuration = new System.Windows.Forms.TextBox();
			this.checkBoxExpirable = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelAuraId = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxAuraFamilyId = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxAuraLevel = new System.Windows.Forms.TextBox();
			this.textBoxAuraName = new System.Windows.Forms.TextBox();
			this.comboBoxAuraType = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBoxBuff.SuspendLayout();
			this.panelFrequency.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(5, 4);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeViewAuras);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.groupBoxBuff);
			this.splitContainer1.Panel2.Controls.Add(this.buttonCreate);
			this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
			this.splitContainer1.Panel2.Controls.Add(this.buttonUpdate);
			this.splitContainer1.Panel2.Controls.Add(this.buttonNew);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.splitContainer1.Panel2MinSize = 540;
			this.splitContainer1.Size = new System.Drawing.Size(862, 628);
			this.splitContainer1.SplitterDistance = 196;
			this.splitContainer1.SplitterWidth = 7;
			this.splitContainer1.TabIndex = 1;
			this.splitContainer1.TabStop = false;
			// 
			// treeViewAuras
			// 
			this.treeViewAuras.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewAuras.Location = new System.Drawing.Point(0, 0);
			this.treeViewAuras.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.treeViewAuras.Name = "treeViewAuras";
			this.treeViewAuras.Size = new System.Drawing.Size(192, 624);
			this.treeViewAuras.TabIndex = 0;
			this.treeViewAuras.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAuras_AfterSelect);
			// 
			// groupBoxBuff
			// 
			this.groupBoxBuff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxBuff.Controls.Add(this.panelFrequency);
			this.groupBoxBuff.Controls.Add(this.textBoxAmount);
			this.groupBoxBuff.Controls.Add(this.textBox1);
			this.groupBoxBuff.Controls.Add(this.radioButtonNonBeneficial);
			this.groupBoxBuff.Controls.Add(this.radioButtonBeneficial);
			this.groupBoxBuff.Controls.Add(this.comboBoxBuffTypeVal1);
			this.groupBoxBuff.Controls.Add(this.comboBoxBuffType);
			this.groupBoxBuff.Controls.Add(this.label10);
			this.groupBoxBuff.Controls.Add(this.labelBuffTypeVal1);
			this.groupBoxBuff.Controls.Add(this.label9);
			this.groupBoxBuff.Controls.Add(this.label8);
			this.groupBoxBuff.Location = new System.Drawing.Point(25, 268);
			this.groupBoxBuff.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxBuff.Name = "groupBoxBuff";
			this.groupBoxBuff.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBoxBuff.Size = new System.Drawing.Size(559, 146);
			this.groupBoxBuff.TabIndex = 11;
			this.groupBoxBuff.TabStop = false;
			this.groupBoxBuff.Text = "Buff";
			// 
			// panelFrequency
			// 
			this.panelFrequency.Controls.Add(this.label12);
			this.panelFrequency.Controls.Add(this.label11);
			this.panelFrequency.Controls.Add(this.textBoxFrequency);
			this.panelFrequency.Location = new System.Drawing.Point(358, 60);
			this.panelFrequency.Name = "panelFrequency";
			this.panelFrequency.Size = new System.Drawing.Size(201, 43);
			this.panelFrequency.TabIndex = 17;
			this.panelFrequency.Visible = false;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(120, 12);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(58, 18);
			this.label12.TabIndex = 10;
			this.label12.Text = "seconds";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(3, 12);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(73, 18);
			this.label11.TabIndex = 0;
			this.label11.Text = "Frequency";
			// 
			// textBoxFrequency
			// 
			this.textBoxFrequency.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxFrequency.Location = new System.Drawing.Point(82, 9);
			this.textBoxFrequency.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxFrequency.MaxLength = 4;
			this.textBoxFrequency.Name = "textBoxFrequency";
			this.textBoxFrequency.Size = new System.Drawing.Size(32, 26);
			this.textBoxFrequency.TabIndex = 11;
			this.textBoxFrequency.Text = "0";
			this.textBoxFrequency.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAuraDuration_KeyPress);
			// 
			// textBoxAmount
			// 
			this.textBoxAmount.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxAmount.Location = new System.Drawing.Point(149, 103);
			this.textBoxAmount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxAmount.MaxLength = 4;
			this.textBoxAmount.Name = "textBoxAmount";
			this.textBoxAmount.Size = new System.Drawing.Size(60, 26);
			this.textBoxAmount.TabIndex = 12;
			this.textBoxAmount.Text = "0";
			this.textBoxAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAmount_KeyPress);
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.Location = new System.Drawing.Point(149, 104);
			this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBox1.MaxLength = 4;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(60, 26);
			this.textBox1.TabIndex = 9;
			this.textBox1.Text = "0";
			this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAuraDuration_KeyPress);
			// 
			// radioButtonNonBeneficial
			// 
			this.radioButtonNonBeneficial.AutoSize = true;
			this.radioButtonNonBeneficial.Location = new System.Drawing.Point(414, 36);
			this.radioButtonNonBeneficial.Name = "radioButtonNonBeneficial";
			this.radioButtonNonBeneficial.Size = new System.Drawing.Size(118, 22);
			this.radioButtonNonBeneficial.TabIndex = 9;
			this.radioButtonNonBeneficial.Text = "Non-Beneficial";
			this.radioButtonNonBeneficial.UseVisualStyleBackColor = true;
			// 
			// radioButtonBeneficial
			// 
			this.radioButtonBeneficial.AutoSize = true;
			this.radioButtonBeneficial.Checked = true;
			this.radioButtonBeneficial.Location = new System.Drawing.Point(316, 36);
			this.radioButtonBeneficial.Name = "radioButtonBeneficial";
			this.radioButtonBeneficial.Size = new System.Drawing.Size(83, 22);
			this.radioButtonBeneficial.TabIndex = 8;
			this.radioButtonBeneficial.TabStop = true;
			this.radioButtonBeneficial.Text = "Benefical";
			this.radioButtonBeneficial.UseVisualStyleBackColor = true;
			// 
			// comboBoxBuffTypeVal1
			// 
			this.comboBoxBuffTypeVal1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBuffTypeVal1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxBuffTypeVal1.FormattingEnabled = true;
			this.comboBoxBuffTypeVal1.Location = new System.Drawing.Point(148, 69);
			this.comboBoxBuffTypeVal1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxBuffTypeVal1.Name = "comboBoxBuffTypeVal1";
			this.comboBoxBuffTypeVal1.Size = new System.Drawing.Size(204, 26);
			this.comboBoxBuffTypeVal1.TabIndex = 10;
			// 
			// comboBoxBuffType
			// 
			this.comboBoxBuffType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBuffType.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxBuffType.FormattingEnabled = true;
			this.comboBoxBuffType.Location = new System.Drawing.Point(148, 35);
			this.comboBoxBuffType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxBuffType.Name = "comboBoxBuffType";
			this.comboBoxBuffType.Size = new System.Drawing.Size(126, 26);
			this.comboBoxBuffType.TabIndex = 7;
			this.comboBoxBuffType.SelectedIndexChanged += new System.EventHandler(this.comboBoxBuffType_SelectedIndexChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(34, 106);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(62, 18);
			this.label10.TabIndex = 0;
			this.label10.Text = "Amount:";
			// 
			// labelBuffTypeVal1
			// 
			this.labelBuffTypeVal1.AutoSize = true;
			this.labelBuffTypeVal1.Location = new System.Drawing.Point(34, 72);
			this.labelBuffTypeVal1.Name = "labelBuffTypeVal1";
			this.labelBuffTypeVal1.Size = new System.Drawing.Size(69, 18);
			this.labelBuffTypeVal1.TabIndex = 0;
			this.labelBuffTypeVal1.Text = "Attribute:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(34, 107);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(65, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "Duration:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(34, 38);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(69, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "Buff Type:";
			// 
			// buttonCreate
			// 
			this.buttonCreate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonCreate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCreate.Location = new System.Drawing.Point(471, 573);
			this.buttonCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonCreate.Name = "buttonCreate";
			this.buttonCreate.Size = new System.Drawing.Size(128, 37);
			this.buttonCreate.TabIndex = 16;
			this.buttonCreate.Text = "&Create";
			this.buttonCreate.UseVisualStyleBackColor = true;
			this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonDelete.Location = new System.Drawing.Point(201, 573);
			this.buttonDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(128, 37);
			this.buttonDelete.TabIndex = 14;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonUpdate.Enabled = false;
			this.buttonUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonUpdate.Location = new System.Drawing.Point(335, 573);
			this.buttonUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(128, 37);
			this.buttonUpdate.TabIndex = 15;
			this.buttonUpdate.Text = "&Update";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// buttonNew
			// 
			this.buttonNew.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonNew.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonNew.Location = new System.Drawing.Point(20, 573);
			this.buttonNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(128, 37);
			this.buttonNew.TabIndex = 13;
			this.buttonNew.Text = "&New";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textBoxAuraDuration);
			this.groupBox1.Controls.Add(this.checkBoxExpirable);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.labelAuraId);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textBoxAuraFamilyId);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBoxAuraLevel);
			this.groupBox1.Controls.Add(this.textBoxAuraName);
			this.groupBox1.Controls.Add(this.comboBoxAuraType);
			this.groupBox1.Location = new System.Drawing.Point(25, 18);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(559, 242);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "General";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(215, 203);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(58, 18);
			this.label7.TabIndex = 10;
			this.label7.Text = "seconds";
			// 
			// textBoxAuraDuration
			// 
			this.textBoxAuraDuration.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxAuraDuration.Location = new System.Drawing.Point(149, 200);
			this.textBoxAuraDuration.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxAuraDuration.MaxLength = 4;
			this.textBoxAuraDuration.Name = "textBoxAuraDuration";
			this.textBoxAuraDuration.Size = new System.Drawing.Size(60, 26);
			this.textBoxAuraDuration.TabIndex = 5;
			this.textBoxAuraDuration.Text = "0";
			this.textBoxAuraDuration.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAuraDuration_KeyPress);
			// 
			// checkBoxExpirable
			// 
			this.checkBoxExpirable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxExpirable.AutoSize = true;
			this.checkBoxExpirable.Checked = true;
			this.checkBoxExpirable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxExpirable.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBoxExpirable.Location = new System.Drawing.Point(299, 202);
			this.checkBoxExpirable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.checkBoxExpirable.Name = "checkBoxExpirable";
			this.checkBoxExpirable.Size = new System.Drawing.Size(85, 22);
			this.checkBoxExpirable.TabIndex = 6;
			this.checkBoxExpirable.Text = "Expirable";
			this.checkBoxExpirable.UseVisualStyleBackColor = true;
			this.checkBoxExpirable.CheckedChanged += new System.EventHandler(this.checkBoxExpirable_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(34, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(24, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Id:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(34, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Type:";
			// 
			// labelAuraId
			// 
			this.labelAuraId.AutoSize = true;
			this.labelAuraId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAuraId.Location = new System.Drawing.Point(145, 32);
			this.labelAuraId.Name = "labelAuraId";
			this.labelAuraId.Size = new System.Drawing.Size(15, 18);
			this.labelAuraId.TabIndex = 0;
			this.labelAuraId.Text = "0";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(34, 203);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(65, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Duration:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(34, 169);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(68, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Family Id:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(34, 135);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Level:";
			// 
			// textBoxAuraFamilyId
			// 
			this.textBoxAuraFamilyId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxAuraFamilyId.Location = new System.Drawing.Point(149, 166);
			this.textBoxAuraFamilyId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxAuraFamilyId.MaxLength = 4;
			this.textBoxAuraFamilyId.Name = "textBoxAuraFamilyId";
			this.textBoxAuraFamilyId.Size = new System.Drawing.Size(60, 26);
			this.textBoxAuraFamilyId.TabIndex = 4;
			this.textBoxAuraFamilyId.Text = "0";
			this.textBoxAuraFamilyId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAuraFamilyId_KeyPress);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(34, 101);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "Name:";
			// 
			// textBoxAuraLevel
			// 
			this.textBoxAuraLevel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxAuraLevel.Location = new System.Drawing.Point(149, 132);
			this.textBoxAuraLevel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxAuraLevel.MaxLength = 2;
			this.textBoxAuraLevel.Name = "textBoxAuraLevel";
			this.textBoxAuraLevel.Size = new System.Drawing.Size(30, 26);
			this.textBoxAuraLevel.TabIndex = 3;
			this.textBoxAuraLevel.Text = "1";
			this.textBoxAuraLevel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxAuraLevel_KeyPress);
			// 
			// textBoxAuraName
			// 
			this.textBoxAuraName.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxAuraName.Location = new System.Drawing.Point(149, 98);
			this.textBoxAuraName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxAuraName.MaxLength = 24;
			this.textBoxAuraName.Name = "textBoxAuraName";
			this.textBoxAuraName.Size = new System.Drawing.Size(250, 26);
			this.textBoxAuraName.TabIndex = 2;
			// 
			// comboBoxAuraType
			// 
			this.comboBoxAuraType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAuraType.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxAuraType.FormattingEnabled = true;
			this.comboBoxAuraType.Location = new System.Drawing.Point(149, 64);
			this.comboBoxAuraType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxAuraType.Name = "comboBoxAuraType";
			this.comboBoxAuraType.Size = new System.Drawing.Size(108, 26);
			this.comboBoxAuraType.TabIndex = 1;
			this.comboBoxAuraType.SelectedIndexChanged += new System.EventHandler(this.comboBoxAuraType_SelectedIndexChanged);
			// 
			// AuraControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "AuraControl";
			this.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Size = new System.Drawing.Size(872, 636);
			this.Load += new System.EventHandler(this.AuraControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBoxBuff.ResumeLayout(false);
			this.groupBoxBuff.PerformLayout();
			this.panelFrequency.ResumeLayout(false);
			this.panelFrequency.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeViewAuras;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxExpirable;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelAuraId;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxAuraName;
		private System.Windows.Forms.ComboBox comboBoxAuraType;
		private System.Windows.Forms.GroupBox groupBoxBuff;
		private System.Windows.Forms.ComboBox comboBoxBuffType;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboBoxBuffTypeVal1;
		private System.Windows.Forms.Label labelBuffTypeVal1;
		private System.Windows.Forms.RadioButton radioButtonNonBeneficial;
		private System.Windows.Forms.RadioButton radioButtonBeneficial;
		private System.Windows.Forms.Button buttonCreate;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.TextBox textBoxAuraDuration;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxAuraLevel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxAuraFamilyId;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textBoxFrequency;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Panel panelFrequency;
		private System.Windows.Forms.TextBox textBoxAmount;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
	}
}
