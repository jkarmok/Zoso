namespace Karen90MmoFramework.Editor
{
	partial class GameObjectControl
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
			this.treeViewGOs = new System.Windows.Forms.TreeView();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label20 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.btnQuestDelete = new System.Windows.Forms.Button();
			this.btnQuestAdd = new System.Windows.Forms.Button();
			this.lstCompleteQuests = new System.Windows.Forms.ListBox();
			this.txtCompleteQuestId = new System.Windows.Forms.TextBox();
			this.lstStartQuests = new System.Windows.Forms.ListBox();
			this.txtStartQuestId = new System.Windows.Forms.TextBox();
			this.btnCreate = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.btnNew = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtOrientation = new System.Windows.Forms.TextBox();
			this.lblId = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.txtLootGrpId = new System.Windows.Forms.TextBox();
			this.txtCoordZ = new System.Windows.Forms.TextBox();
			this.txtCoordY = new System.Windows.Forms.TextBox();
			this.txtCoordX = new System.Windows.Forms.TextBox();
			this.txtGroupId = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cboZone = new System.Windows.Forms.ComboBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.cboGOType = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox3.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.treeViewGOs);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
			this.splitContainer1.Panel2.Controls.Add(this.btnCreate);
			this.splitContainer1.Panel2.Controls.Add(this.btnDelete);
			this.splitContainer1.Panel2.Controls.Add(this.btnUpdate);
			this.splitContainer1.Panel2.Controls.Add(this.btnNew);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.splitContainer1.Panel2MinSize = 540;
			this.splitContainer1.Size = new System.Drawing.Size(862, 628);
			this.splitContainer1.SplitterDistance = 196;
			this.splitContainer1.SplitterWidth = 7;
			this.splitContainer1.TabIndex = 2;
			this.splitContainer1.TabStop = false;
			// 
			// treeViewGOs
			// 
			this.treeViewGOs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewGOs.Location = new System.Drawing.Point(0, 0);
			this.treeViewGOs.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.treeViewGOs.Name = "treeViewGOs";
			this.treeViewGOs.Size = new System.Drawing.Size(192, 624);
			this.treeViewGOs.TabIndex = 0;
			this.treeViewGOs.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewNpcs_AfterSelect);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label20);
			this.groupBox3.Controls.Add(this.label19);
			this.groupBox3.Controls.Add(this.btnQuestDelete);
			this.groupBox3.Controls.Add(this.btnQuestAdd);
			this.groupBox3.Controls.Add(this.lstCompleteQuests);
			this.groupBox3.Controls.Add(this.txtCompleteQuestId);
			this.groupBox3.Controls.Add(this.lstStartQuests);
			this.groupBox3.Controls.Add(this.txtStartQuestId);
			this.groupBox3.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(25, 270);
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
			// btnQuestDelete
			// 
			this.btnQuestDelete.Location = new System.Drawing.Point(112, 85);
			this.btnQuestDelete.Name = "btnQuestDelete";
			this.btnQuestDelete.Size = new System.Drawing.Size(58, 94);
			this.btnQuestDelete.TabIndex = 17;
			this.btnQuestDelete.Text = "Delete";
			this.btnQuestDelete.UseVisualStyleBackColor = true;
			this.btnQuestDelete.Click += new System.EventHandler(this.buttonQuestDelete_Click);
			// 
			// btnQuestAdd
			// 
			this.btnQuestAdd.Location = new System.Drawing.Point(112, 52);
			this.btnQuestAdd.Name = "btnQuestAdd";
			this.btnQuestAdd.Size = new System.Drawing.Size(58, 26);
			this.btnQuestAdd.TabIndex = 17;
			this.btnQuestAdd.Text = "Add";
			this.btnQuestAdd.UseVisualStyleBackColor = true;
			this.btnQuestAdd.Click += new System.EventHandler(this.buttonQuestAdd_Click);
			// 
			// lstCompleteQuests
			// 
			this.lstCompleteQuests.FormattingEnabled = true;
			this.lstCompleteQuests.ItemHeight = 18;
			this.lstCompleteQuests.Location = new System.Drawing.Point(185, 85);
			this.lstCompleteQuests.Name = "lstCompleteQuests";
			this.lstCompleteQuests.Size = new System.Drawing.Size(80, 94);
			this.lstCompleteQuests.TabIndex = 16;
			this.lstCompleteQuests.Enter += new System.EventHandler(this.listBoxCompleteQuests_Enter);
			// 
			// txtCompleteQuestId
			// 
			this.txtCompleteQuestId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtCompleteQuestId.Location = new System.Drawing.Point(185, 52);
			this.txtCompleteQuestId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCompleteQuestId.MaxLength = 6;
			this.txtCompleteQuestId.Name = "txtCompleteQuestId";
			this.txtCompleteQuestId.Size = new System.Drawing.Size(80, 26);
			this.txtCompleteQuestId.TabIndex = 15;
			this.txtCompleteQuestId.Text = "0";
			this.txtCompleteQuestId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// lstStartQuests
			// 
			this.lstStartQuests.FormattingEnabled = true;
			this.lstStartQuests.ItemHeight = 18;
			this.lstStartQuests.Location = new System.Drawing.Point(17, 85);
			this.lstStartQuests.Name = "lstStartQuests";
			this.lstStartQuests.Size = new System.Drawing.Size(80, 94);
			this.lstStartQuests.TabIndex = 16;
			this.lstStartQuests.Enter += new System.EventHandler(this.listBoxStartQuests_Enter);
			// 
			// txtStartQuestId
			// 
			this.txtStartQuestId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtStartQuestId.Location = new System.Drawing.Point(17, 52);
			this.txtStartQuestId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtStartQuestId.MaxLength = 6;
			this.txtStartQuestId.Name = "txtStartQuestId";
			this.txtStartQuestId.Size = new System.Drawing.Size(80, 26);
			this.txtStartQuestId.TabIndex = 15;
			this.txtStartQuestId.Text = "0";
			this.txtStartQuestId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// btnCreate
			// 
			this.btnCreate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnCreate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCreate.Location = new System.Drawing.Point(472, 573);
			this.btnCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.Size = new System.Drawing.Size(128, 37);
			this.btnCreate.TabIndex = 20;
			this.btnCreate.Text = "&Create";
			this.btnCreate.UseVisualStyleBackColor = true;
			this.btnCreate.Click += new System.EventHandler(this.buttonCreate_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnDelete.Enabled = false;
			this.btnDelete.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnDelete.Location = new System.Drawing.Point(202, 573);
			this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(128, 37);
			this.btnDelete.TabIndex = 18;
			this.btnDelete.Text = "&Delete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// btnUpdate
			// 
			this.btnUpdate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnUpdate.Enabled = false;
			this.btnUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnUpdate.Location = new System.Drawing.Point(336, 573);
			this.btnUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(128, 37);
			this.btnUpdate.TabIndex = 19;
			this.btnUpdate.Text = "&Update";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// btnNew
			// 
			this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnNew.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnNew.Location = new System.Drawing.Point(21, 573);
			this.btnNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(128, 37);
			this.btnNew.TabIndex = 17;
			this.btnNew.Text = "&New";
			this.btnNew.UseVisualStyleBackColor = true;
			this.btnNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.txtOrientation);
			this.groupBox1.Controls.Add(this.lblId);
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.txtLootGrpId);
			this.groupBox1.Controls.Add(this.txtCoordZ);
			this.groupBox1.Controls.Add(this.txtCoordY);
			this.groupBox1.Controls.Add(this.txtCoordX);
			this.groupBox1.Controls.Add(this.txtGroupId);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label17);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.cboZone);
			this.groupBox1.Controls.Add(this.txtName);
			this.groupBox1.Controls.Add(this.cboGOType);
			this.groupBox1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(25, 18);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(583, 245);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "General Properties";
			// 
			// txtOrientation
			// 
			this.txtOrientation.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtOrientation.Location = new System.Drawing.Point(333, 163);
			this.txtOrientation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtOrientation.MaxLength = 0;
			this.txtOrientation.Name = "txtOrientation";
			this.txtOrientation.Size = new System.Drawing.Size(67, 26);
			this.txtOrientation.TabIndex = 17;
			this.txtOrientation.Text = "0.0";
			// 
			// lblId
			// 
			this.lblId.BackColor = System.Drawing.SystemColors.Control;
			this.lblId.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lblId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblId.Location = new System.Drawing.Point(148, 32);
			this.lblId.Name = "lblId";
			this.lblId.ReadOnly = true;
			this.lblId.Size = new System.Drawing.Size(148, 19);
			this.lblId.TabIndex = 16;
			this.lblId.Text = "#id#";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(34, 169);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(95, 18);
			this.label18.TabIndex = 0;
			this.label18.Text = "Loot Group Id:";
			// 
			// txtLootGrpId
			// 
			this.txtLootGrpId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLootGrpId.Location = new System.Drawing.Point(148, 166);
			this.txtLootGrpId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtLootGrpId.MaxLength = 6;
			this.txtLootGrpId.Name = "txtLootGrpId";
			this.txtLootGrpId.Size = new System.Drawing.Size(51, 26);
			this.txtLootGrpId.TabIndex = 15;
			this.txtLootGrpId.Text = "0";
			this.txtLootGrpId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
			// 
			// txtCoordZ
			// 
			this.txtCoordZ.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtCoordZ.Location = new System.Drawing.Point(510, 200);
			this.txtCoordZ.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCoordZ.MaxLength = 0;
			this.txtCoordZ.Name = "txtCoordZ";
			this.txtCoordZ.Size = new System.Drawing.Size(67, 26);
			this.txtCoordZ.TabIndex = 12;
			this.txtCoordZ.Text = "0.0";
			this.txtCoordZ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// txtCoordY
			// 
			this.txtCoordY.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtCoordY.Location = new System.Drawing.Point(415, 200);
			this.txtCoordY.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCoordY.MaxLength = 0;
			this.txtCoordY.Name = "txtCoordY";
			this.txtCoordY.Size = new System.Drawing.Size(67, 26);
			this.txtCoordY.TabIndex = 11;
			this.txtCoordY.Text = "0.0";
			this.txtCoordY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// txtCoordX
			// 
			this.txtCoordX.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtCoordX.Location = new System.Drawing.Point(320, 200);
			this.txtCoordX.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCoordX.MaxLength = 0;
			this.txtCoordX.Name = "txtCoordX";
			this.txtCoordX.Size = new System.Drawing.Size(67, 26);
			this.txtCoordX.TabIndex = 10;
			this.txtCoordX.Text = "0.0";
			this.txtCoordX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxFloat_KeyPress);
			// 
			// txtGroupId
			// 
			this.txtGroupId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtGroupId.Location = new System.Drawing.Point(148, 132);
			this.txtGroupId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtGroupId.MaxLength = 4;
			this.txtGroupId.Name = "txtGroupId";
			this.txtGroupId.Size = new System.Drawing.Size(42, 26);
			this.txtGroupId.TabIndex = 4;
			this.txtGroupId.Text = "1";
			this.txtGroupId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInt32_KeyPress);
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
			this.label2.Size = new System.Drawing.Size(63, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "GO Type:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(244, 166);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(83, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Orientation:";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.Location = new System.Drawing.Point(488, 203);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(15, 18);
			this.label17.TabIndex = 0;
			this.label17.Text = "Z";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(393, 203);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(16, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "Y";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(298, 203);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(16, 18);
			this.label10.TabIndex = 0;
			this.label10.Text = "X";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(244, 203);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Coord: ";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(34, 203);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(43, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "Zone:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(34, 135);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(68, 18);
			this.label11.TabIndex = 0;
			this.label11.Text = "Family Id:";
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
			// cboZone
			// 
			this.cboZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboZone.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboZone.FormattingEnabled = true;
			this.cboZone.Location = new System.Drawing.Point(148, 200);
			this.cboZone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboZone.Name = "cboZone";
			this.cboZone.Size = new System.Drawing.Size(69, 26);
			this.cboZone.TabIndex = 9;
			// 
			// txtName
			// 
			this.txtName.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtName.Location = new System.Drawing.Point(148, 98);
			this.txtName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtName.MaxLength = 16;
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(188, 26);
			this.txtName.TabIndex = 1;
			// 
			// cboGOType
			// 
			this.cboGOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboGOType.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboGOType.FormattingEnabled = true;
			this.cboGOType.Location = new System.Drawing.Point(149, 64);
			this.cboGOType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboGOType.Name = "cboGOType";
			this.cboGOType.Size = new System.Drawing.Size(147, 26);
			this.cboGOType.TabIndex = 0;
			// 
			// GameObjectControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "GameObjectControl";
			this.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Size = new System.Drawing.Size(872, 636);
			this.Load += new System.EventHandler(this.GameObjectControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeViewGOs;
		private System.Windows.Forms.Button btnCreate;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.ComboBox cboGOType;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtCoordX;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox txtGroupId;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cboZone;
		private System.Windows.Forms.TextBox txtCoordZ;
		private System.Windows.Forms.TextBox txtCoordY;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox txtLootGrpId;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox lstStartQuests;
		private System.Windows.Forms.TextBox txtStartQuestId;
		private System.Windows.Forms.ListBox lstCompleteQuests;
		private System.Windows.Forms.TextBox txtCompleteQuestId;
		private System.Windows.Forms.Button btnQuestDelete;
		private System.Windows.Forms.Button btnQuestAdd;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox lblId;
		private System.Windows.Forms.TextBox txtOrientation;
	}
}
