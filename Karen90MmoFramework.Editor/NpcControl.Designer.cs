namespace Karen90MmoFramework.Editor
{
	partial class NpcControl
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
			this.treeViewNpcs = new System.Windows.Forms.TreeView();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label20 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.buttonQuestDelete = new System.Windows.Forms.Button();
			this.buttonQuestAdd = new System.Windows.Forms.Button();
			this.listBoxCompleteQuests = new System.Windows.Forms.ListBox();
			this.textBoxCompleteQuestId = new System.Windows.Forms.TextBox();
			this.listBoxStartQuests = new System.Windows.Forms.ListBox();
			this.textBoxStartQuestId = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textBoxMaxPower = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textBoxMaxHealth = new System.Windows.Forms.TextBox();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.buttonNew = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelNpcId = new System.Windows.Forms.TextBox();
			this.buttonPossessions = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.txtOrientation = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textBoxLootGroupId = new System.Windows.Forms.TextBox();
			this.textBoxCoordZ = new System.Windows.Forms.TextBox();
			this.textBoxCoordY = new System.Windows.Forms.TextBox();
			this.textBoxCoordX = new System.Windows.Forms.TextBox();
			this.textBoxNpcGroupId = new System.Windows.Forms.TextBox();
			this.textBoxNpcLevel = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxNpcZone = new System.Windows.Forms.ComboBox();
			this.comboBoxNpcAlignment = new System.Windows.Forms.ComboBox();
			this.textBoxNpcName = new System.Windows.Forms.TextBox();
			this.comboBoxSpecies = new System.Windows.Forms.ComboBox();
			this.comboBoxNpcType = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.treeViewNpcs);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
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
			this.splitContainer1.TabIndex = 2;
			this.splitContainer1.TabStop = false;
			// 
			// treeViewNpcs
			// 
			this.treeViewNpcs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewNpcs.Location = new System.Drawing.Point(0, 0);
			this.treeViewNpcs.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.treeViewNpcs.Name = "treeViewNpcs";
			this.treeViewNpcs.Size = new System.Drawing.Size(192, 624);
			this.treeViewNpcs.TabIndex = 0;
			this.treeViewNpcs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewNpcs_AfterSelect);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label20);
			this.groupBox3.Controls.Add(this.label19);
			this.groupBox3.Controls.Add(this.buttonQuestDelete);
			this.groupBox3.Controls.Add(this.buttonQuestAdd);
			this.groupBox3.Controls.Add(this.listBoxCompleteQuests);
			this.groupBox3.Controls.Add(this.textBoxCompleteQuestId);
			this.groupBox3.Controls.Add(this.listBoxStartQuests);
			this.groupBox3.Controls.Add(this.textBoxStartQuestId);
			this.groupBox3.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(337, 363);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(284, 194);
			this.groupBox3.TabIndex = 15;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Quests";
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(182, 30);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(79, 18);
			this.label20.TabIndex = 18;
			this.label20.Text = "Completes:";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(14, 30);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(47, 18);
			this.label19.TabIndex = 18;
			this.label19.Text = "Starts:";
			// 
			// buttonQuestDelete
			// 
			this.buttonQuestDelete.Location = new System.Drawing.Point(112, 85);
			this.buttonQuestDelete.Name = "buttonQuestDelete";
			this.buttonQuestDelete.Size = new System.Drawing.Size(58, 94);
			this.buttonQuestDelete.TabIndex = 17;
			this.buttonQuestDelete.Text = "Delete";
			this.buttonQuestDelete.UseVisualStyleBackColor = true;
			this.buttonQuestDelete.Click += new System.EventHandler(this.buttonQuestDelete_Click);
			// 
			// buttonQuestAdd
			// 
			this.buttonQuestAdd.Location = new System.Drawing.Point(112, 52);
			this.buttonQuestAdd.Name = "buttonQuestAdd";
			this.buttonQuestAdd.Size = new System.Drawing.Size(58, 26);
			this.buttonQuestAdd.TabIndex = 17;
			this.buttonQuestAdd.Text = "Add";
			this.buttonQuestAdd.UseVisualStyleBackColor = true;
			this.buttonQuestAdd.Click += new System.EventHandler(this.buttonQuestAdd_Click);
			// 
			// listBoxCompleteQuests
			// 
			this.listBoxCompleteQuests.FormattingEnabled = true;
			this.listBoxCompleteQuests.ItemHeight = 18;
			this.listBoxCompleteQuests.Location = new System.Drawing.Point(185, 85);
			this.listBoxCompleteQuests.Name = "listBoxCompleteQuests";
			this.listBoxCompleteQuests.Size = new System.Drawing.Size(80, 94);
			this.listBoxCompleteQuests.TabIndex = 16;
			this.listBoxCompleteQuests.Enter += new System.EventHandler(this.listBoxCompleteQuests_Enter);
			// 
			// textBoxCompleteQuestId
			// 
			this.textBoxCompleteQuestId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxCompleteQuestId.Location = new System.Drawing.Point(185, 52);
			this.textBoxCompleteQuestId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxCompleteQuestId.MaxLength = 6;
			this.textBoxCompleteQuestId.Name = "textBoxCompleteQuestId";
			this.textBoxCompleteQuestId.Size = new System.Drawing.Size(80, 26);
			this.textBoxCompleteQuestId.TabIndex = 15;
			this.textBoxCompleteQuestId.Text = "0";
			this.textBoxCompleteQuestId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// listBoxStartQuests
			// 
			this.listBoxStartQuests.FormattingEnabled = true;
			this.listBoxStartQuests.ItemHeight = 18;
			this.listBoxStartQuests.Location = new System.Drawing.Point(17, 85);
			this.listBoxStartQuests.Name = "listBoxStartQuests";
			this.listBoxStartQuests.Size = new System.Drawing.Size(80, 94);
			this.listBoxStartQuests.TabIndex = 16;
			this.listBoxStartQuests.Enter += new System.EventHandler(this.listBoxStartQuests_Enter);
			// 
			// textBoxStartQuestId
			// 
			this.textBoxStartQuestId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxStartQuestId.Location = new System.Drawing.Point(17, 52);
			this.textBoxStartQuestId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxStartQuestId.MaxLength = 6;
			this.textBoxStartQuestId.Name = "textBoxStartQuestId";
			this.textBoxStartQuestId.Size = new System.Drawing.Size(80, 26);
			this.textBoxStartQuestId.TabIndex = 15;
			this.textBoxStartQuestId.Text = "0";
			this.textBoxStartQuestId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label16);
			this.groupBox2.Controls.Add(this.textBoxMaxPower);
			this.groupBox2.Controls.Add(this.label15);
			this.groupBox2.Controls.Add(this.textBoxMaxHealth);
			this.groupBox2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox2.Location = new System.Drawing.Point(36, 363);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(284, 194);
			this.groupBox2.TabIndex = 15;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Stats";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(34, 64);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(81, 18);
			this.label16.TabIndex = 0;
			this.label16.Text = "Max Power:";
			// 
			// textBoxMaxPower
			// 
			this.textBoxMaxPower.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxMaxPower.Location = new System.Drawing.Point(149, 61);
			this.textBoxMaxPower.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxMaxPower.MaxLength = 6;
			this.textBoxMaxPower.Name = "textBoxMaxPower";
			this.textBoxMaxPower.Size = new System.Drawing.Size(100, 26);
			this.textBoxMaxPower.TabIndex = 16;
			this.textBoxMaxPower.Text = "100";
			this.textBoxMaxPower.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(34, 30);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(82, 18);
			this.label15.TabIndex = 0;
			this.label15.Text = "Max Health:";
			// 
			// textBoxMaxHealth
			// 
			this.textBoxMaxHealth.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxMaxHealth.Location = new System.Drawing.Point(149, 27);
			this.textBoxMaxHealth.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxMaxHealth.MaxLength = 6;
			this.textBoxMaxHealth.Name = "textBoxMaxHealth";
			this.textBoxMaxHealth.Size = new System.Drawing.Size(100, 26);
			this.textBoxMaxHealth.TabIndex = 15;
			this.textBoxMaxHealth.Text = "100";
			this.textBoxMaxHealth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// buttonCreate
			// 
			this.buttonCreate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonCreate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCreate.Location = new System.Drawing.Point(481, 573);
			this.buttonCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonCreate.Name = "buttonCreate";
			this.buttonCreate.Size = new System.Drawing.Size(128, 37);
			this.buttonCreate.TabIndex = 20;
			this.buttonCreate.Text = "&Create";
			this.buttonCreate.UseVisualStyleBackColor = true;
			this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonDelete.Location = new System.Drawing.Point(199, 573);
			this.buttonDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(128, 37);
			this.buttonDelete.TabIndex = 18;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonUpdate.Enabled = false;
			this.buttonUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonUpdate.Location = new System.Drawing.Point(333, 573);
			this.buttonUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(128, 37);
			this.buttonUpdate.TabIndex = 19;
			this.buttonUpdate.Text = "&Update";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// buttonNew
			// 
			this.buttonNew.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonNew.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonNew.Location = new System.Drawing.Point(24, 573);
			this.buttonNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(128, 37);
			this.buttonNew.TabIndex = 17;
			this.buttonNew.Text = "&New";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.labelNpcId);
			this.groupBox1.Controls.Add(this.buttonPossessions);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.txtOrientation);
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.textBoxLootGroupId);
			this.groupBox1.Controls.Add(this.textBoxCoordZ);
			this.groupBox1.Controls.Add(this.textBoxCoordY);
			this.groupBox1.Controls.Add(this.textBoxCoordX);
			this.groupBox1.Controls.Add(this.textBoxNpcGroupId);
			this.groupBox1.Controls.Add(this.textBoxNpcLevel);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label17);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.label12);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.comboBoxNpcZone);
			this.groupBox1.Controls.Add(this.comboBoxNpcAlignment);
			this.groupBox1.Controls.Add(this.textBoxNpcName);
			this.groupBox1.Controls.Add(this.comboBoxSpecies);
			this.groupBox1.Controls.Add(this.comboBoxNpcType);
			this.groupBox1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(25, 18);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(584, 338);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "General Properties";
			// 
			// labelNpcId
			// 
			this.labelNpcId.BackColor = System.Drawing.SystemColors.Control;
			this.labelNpcId.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.labelNpcId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNpcId.Location = new System.Drawing.Point(148, 32);
			this.labelNpcId.Name = "labelNpcId";
			this.labelNpcId.ReadOnly = true;
			this.labelNpcId.Size = new System.Drawing.Size(148, 19);
			this.labelNpcId.TabIndex = 16;
			this.labelNpcId.Text = "#id#";
			// 
			// buttonPossessions
			// 
			this.buttonPossessions.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonPossessions.Location = new System.Drawing.Point(456, 307);
			this.buttonPossessions.Name = "buttonPossessions";
			this.buttonPossessions.Size = new System.Drawing.Size(122, 24);
			this.buttonPossessions.TabIndex = 13;
			this.buttonPossessions.Text = "Possessions";
			this.buttonPossessions.UseVisualStyleBackColor = true;
			this.buttonPossessions.Click += new System.EventHandler(this.buttonPossessions_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(411, 241);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(83, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Orientation:";
			// 
			// txtOrientation
			// 
			this.txtOrientation.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtOrientation.Location = new System.Drawing.Point(500, 238);
			this.txtOrientation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtOrientation.MaxLength = 6;
			this.txtOrientation.Name = "txtOrientation";
			this.txtOrientation.Size = new System.Drawing.Size(51, 26);
			this.txtOrientation.TabIndex = 15;
			this.txtOrientation.Text = "0.0";
			this.txtOrientation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(215, 170);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(95, 18);
			this.label18.TabIndex = 0;
			this.label18.Text = "Loot Group Id:";
			// 
			// textBoxLootGroupId
			// 
			this.textBoxLootGroupId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxLootGroupId.Location = new System.Drawing.Point(338, 167);
			this.textBoxLootGroupId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxLootGroupId.MaxLength = 6;
			this.textBoxLootGroupId.Name = "textBoxLootGroupId";
			this.textBoxLootGroupId.Size = new System.Drawing.Size(51, 26);
			this.textBoxLootGroupId.TabIndex = 15;
			this.textBoxLootGroupId.Text = "0";
			this.textBoxLootGroupId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// textBoxCoordZ
			// 
			this.textBoxCoordZ.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxCoordZ.Location = new System.Drawing.Point(338, 238);
			this.textBoxCoordZ.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxCoordZ.MaxLength = 0;
			this.textBoxCoordZ.Name = "textBoxCoordZ";
			this.textBoxCoordZ.Size = new System.Drawing.Size(67, 26);
			this.textBoxCoordZ.TabIndex = 12;
			this.textBoxCoordZ.Text = "0.0";
			this.textBoxCoordZ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// textBoxCoordY
			// 
			this.textBoxCoordY.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxCoordY.Location = new System.Drawing.Point(243, 238);
			this.textBoxCoordY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxCoordY.MaxLength = 0;
			this.textBoxCoordY.Name = "textBoxCoordY";
			this.textBoxCoordY.Size = new System.Drawing.Size(67, 26);
			this.textBoxCoordY.TabIndex = 11;
			this.textBoxCoordY.Text = "0.0";
			this.textBoxCoordY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// textBoxCoordX
			// 
			this.textBoxCoordX.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxCoordX.Location = new System.Drawing.Point(148, 238);
			this.textBoxCoordX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxCoordX.MaxLength = 0;
			this.textBoxCoordX.Name = "textBoxCoordX";
			this.textBoxCoordX.Size = new System.Drawing.Size(67, 26);
			this.textBoxCoordX.TabIndex = 10;
			this.textBoxCoordX.Text = "0.0";
			this.textBoxCoordX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// textBoxNpcGroupId
			// 
			this.textBoxNpcGroupId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxNpcGroupId.Location = new System.Drawing.Point(149, 167);
			this.textBoxNpcGroupId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxNpcGroupId.MaxLength = 4;
			this.textBoxNpcGroupId.Name = "textBoxNpcGroupId";
			this.textBoxNpcGroupId.Size = new System.Drawing.Size(42, 26);
			this.textBoxNpcGroupId.TabIndex = 4;
			this.textBoxNpcGroupId.Text = "1";
			this.textBoxNpcGroupId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// textBoxNpcLevel
			// 
			this.textBoxNpcLevel.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxNpcLevel.Location = new System.Drawing.Point(149, 133);
			this.textBoxNpcLevel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxNpcLevel.MaxLength = 3;
			this.textBoxNpcLevel.Name = "textBoxNpcLevel";
			this.textBoxNpcLevel.Size = new System.Drawing.Size(31, 26);
			this.textBoxNpcLevel.TabIndex = 3;
			this.textBoxNpcLevel.Text = "1";
			this.textBoxNpcLevel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
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
			this.label2.Size = new System.Drawing.Size(68, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Npc Type:";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.Location = new System.Drawing.Point(316, 241);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(15, 18);
			this.label17.TabIndex = 0;
			this.label17.Text = "Z";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(221, 241);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(16, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "Y";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(126, 241);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(16, 18);
			this.label10.TabIndex = 0;
			this.label10.Text = "X";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(35, 241);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Coord: ";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(35, 275);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(43, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "Zone:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(35, 170);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(68, 18);
			this.label11.TabIndex = 0;
			this.label11.Text = "Family Id:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(35, 136);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(45, 18);
			this.label7.TabIndex = 0;
			this.label7.Text = "Level:";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(304, 204);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(77, 18);
			this.label13.TabIndex = 0;
			this.label13.Text = "Alignment:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(35, 204);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(55, 18);
			this.label12.TabIndex = 0;
			this.label12.Text = "Species";
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
			// comboBoxNpcZone
			// 
			this.comboBoxNpcZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNpcZone.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxNpcZone.FormattingEnabled = true;
			this.comboBoxNpcZone.Location = new System.Drawing.Point(149, 272);
			this.comboBoxNpcZone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxNpcZone.Name = "comboBoxNpcZone";
			this.comboBoxNpcZone.Size = new System.Drawing.Size(69, 26);
			this.comboBoxNpcZone.TabIndex = 9;
			// 
			// comboBoxNpcAlignment
			// 
			this.comboBoxNpcAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNpcAlignment.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxNpcAlignment.FormattingEnabled = true;
			this.comboBoxNpcAlignment.Location = new System.Drawing.Point(397, 201);
			this.comboBoxNpcAlignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxNpcAlignment.Name = "comboBoxNpcAlignment";
			this.comboBoxNpcAlignment.Size = new System.Drawing.Size(101, 26);
			this.comboBoxNpcAlignment.TabIndex = 8;
			// 
			// textBoxNpcName
			// 
			this.textBoxNpcName.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxNpcName.Location = new System.Drawing.Point(148, 98);
			this.textBoxNpcName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxNpcName.MaxLength = 16;
			this.textBoxNpcName.Name = "textBoxNpcName";
			this.textBoxNpcName.Size = new System.Drawing.Size(188, 26);
			this.textBoxNpcName.TabIndex = 1;
			// 
			// comboBoxSpecies
			// 
			this.comboBoxSpecies.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSpecies.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxSpecies.FormattingEnabled = true;
			this.comboBoxSpecies.Location = new System.Drawing.Point(149, 201);
			this.comboBoxSpecies.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxSpecies.Name = "comboBoxSpecies";
			this.comboBoxSpecies.Size = new System.Drawing.Size(118, 26);
			this.comboBoxSpecies.TabIndex = 7;
			// 
			// comboBoxNpcType
			// 
			this.comboBoxNpcType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNpcType.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxNpcType.FormattingEnabled = true;
			this.comboBoxNpcType.Location = new System.Drawing.Point(149, 64);
			this.comboBoxNpcType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxNpcType.Name = "comboBoxNpcType";
			this.comboBoxNpcType.Size = new System.Drawing.Size(147, 26);
			this.comboBoxNpcType.TabIndex = 0;
			// 
			// NpcControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "NpcControl";
			this.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Size = new System.Drawing.Size(872, 636);
			this.Load += new System.EventHandler(this.NpcControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeViewNpcs;
		private System.Windows.Forms.Button buttonCreate;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxNpcLevel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxNpcName;
		private System.Windows.Forms.ComboBox comboBoxNpcType;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textBoxCoordX;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBoxNpcGroupId;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.ComboBox comboBoxSpecies;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox comboBoxNpcAlignment;
		private System.Windows.Forms.Button buttonPossessions;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textBoxMaxHealth;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textBoxMaxPower;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxNpcZone;
		private System.Windows.Forms.TextBox textBoxCoordZ;
		private System.Windows.Forms.TextBox textBoxCoordY;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox textBoxLootGroupId;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox listBoxStartQuests;
		private System.Windows.Forms.TextBox textBoxStartQuestId;
		private System.Windows.Forms.ListBox listBoxCompleteQuests;
		private System.Windows.Forms.TextBox textBoxCompleteQuestId;
		private System.Windows.Forms.Button buttonQuestDelete;
		private System.Windows.Forms.Button buttonQuestAdd;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox labelNpcId;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtOrientation;
	}
}
