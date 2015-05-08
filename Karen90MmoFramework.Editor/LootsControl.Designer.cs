namespace Karen90MmoFramework.Editor
{
	partial class LootsControl
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
			this.treeViewLoots = new System.Windows.Forms.TreeView();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.buttonNew = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.buttonItemAdd = new System.Windows.Forms.Button();
			this.buttonItemDelete = new System.Windows.Forms.Button();
			this.listBoxLootItems = new System.Windows.Forms.ListBox();
			this.textBoxItemMaxCount = new System.Windows.Forms.TextBox();
			this.textBoxItemDropChance = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxItemId = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textBoxItemMinCount = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxMaxGold = new System.Windows.Forms.TextBox();
			this.labelLootGroupId = new System.Windows.Forms.Label();
			this.textBoxGoldChance = new System.Windows.Forms.TextBox();
			this.textBoxMinGold = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.treeViewLoots);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.buttonCreate);
			this.splitContainer1.Panel2.Controls.Add(this.buttonDelete);
			this.splitContainer1.Panel2.Controls.Add(this.buttonUpdate);
			this.splitContainer1.Panel2.Controls.Add(this.buttonNew);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.splitContainer1.Panel2MinSize = 540;
			this.splitContainer1.Size = new System.Drawing.Size(862, 628);
			this.splitContainer1.SplitterDistance = 196;
			this.splitContainer1.SplitterWidth = 7;
			this.splitContainer1.TabIndex = 1;
			this.splitContainer1.TabStop = false;
			// 
			// treeViewLoots
			// 
			this.treeViewLoots.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewLoots.Location = new System.Drawing.Point(0, 0);
			this.treeViewLoots.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.treeViewLoots.Name = "treeViewLoots";
			this.treeViewLoots.Size = new System.Drawing.Size(192, 624);
			this.treeViewLoots.TabIndex = 1;
			this.treeViewLoots.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewLoots_AfterSelect);
			// 
			// buttonCreate
			// 
			this.buttonCreate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonCreate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCreate.Location = new System.Drawing.Point(472, 573);
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
			this.buttonDelete.Location = new System.Drawing.Point(202, 573);
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
			this.buttonUpdate.Location = new System.Drawing.Point(336, 573);
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
			this.buttonNew.Location = new System.Drawing.Point(21, 573);
			this.buttonNew.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(128, 37);
			this.buttonNew.TabIndex = 13;
			this.buttonNew.Text = "&New";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.buttonItemAdd);
			this.groupBox2.Controls.Add(this.buttonItemDelete);
			this.groupBox2.Controls.Add(this.listBoxLootItems);
			this.groupBox2.Controls.Add(this.textBoxItemMaxCount);
			this.groupBox2.Controls.Add(this.textBoxItemDropChance);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.textBoxItemId);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.textBoxItemMinCount);
			this.groupBox2.Location = new System.Drawing.Point(25, 161);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox2.Size = new System.Drawing.Size(553, 259);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Loot Items";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(294, 93);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 18);
			this.label7.TabIndex = 0;
			this.label7.Text = "Max Count:";
			// 
			// buttonItemAdd
			// 
			this.buttonItemAdd.Location = new System.Drawing.Point(297, 216);
			this.buttonItemAdd.Name = "buttonItemAdd";
			this.buttonItemAdd.Size = new System.Drawing.Size(175, 34);
			this.buttonItemAdd.TabIndex = 1;
			this.buttonItemAdd.Text = "Add";
			this.buttonItemAdd.UseVisualStyleBackColor = true;
			this.buttonItemAdd.Click += new System.EventHandler(this.buttonItemAdd_Click);
			// 
			// buttonItemDelete
			// 
			this.buttonItemDelete.Location = new System.Drawing.Point(98, 216);
			this.buttonItemDelete.Name = "buttonItemDelete";
			this.buttonItemDelete.Size = new System.Drawing.Size(164, 34);
			this.buttonItemDelete.TabIndex = 1;
			this.buttonItemDelete.Text = "Delete";
			this.buttonItemDelete.UseVisualStyleBackColor = true;
			this.buttonItemDelete.Click += new System.EventHandler(this.buttonItemDelete_Click);
			// 
			// listBoxLootItems
			// 
			this.listBoxLootItems.FormattingEnabled = true;
			this.listBoxLootItems.ItemHeight = 18;
			this.listBoxLootItems.Location = new System.Drawing.Point(98, 26);
			this.listBoxLootItems.Name = "listBoxLootItems";
			this.listBoxLootItems.Size = new System.Drawing.Size(164, 184);
			this.listBoxLootItems.TabIndex = 0;
			// 
			// textBoxItemMaxCount
			// 
			this.textBoxItemMaxCount.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxItemMaxCount.Location = new System.Drawing.Point(408, 90);
			this.textBoxItemMaxCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxItemMaxCount.MaxLength = 3;
			this.textBoxItemMaxCount.Name = "textBoxItemMaxCount";
			this.textBoxItemMaxCount.Size = new System.Drawing.Size(37, 26);
			this.textBoxItemMaxCount.TabIndex = 5;
			this.textBoxItemMaxCount.Text = "0";
			this.textBoxItemMaxCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumeric_KeyPress);
			// 
			// textBoxItemDropChance
			// 
			this.textBoxItemDropChance.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxItemDropChance.Location = new System.Drawing.Point(408, 124);
			this.textBoxItemDropChance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxItemDropChance.MaxLength = 5;
			this.textBoxItemDropChance.Name = "textBoxItemDropChance";
			this.textBoxItemDropChance.Size = new System.Drawing.Size(56, 26);
			this.textBoxItemDropChance.TabIndex = 4;
			this.textBoxItemDropChance.Text = "0";
			this.textBoxItemDropChance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxDecimal_KeyPress);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(294, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "Item Id:";
			// 
			// textBoxItemId
			// 
			this.textBoxItemId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxItemId.Location = new System.Drawing.Point(408, 22);
			this.textBoxItemId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxItemId.MaxLength = 5;
			this.textBoxItemId.Name = "textBoxItemId";
			this.textBoxItemId.Size = new System.Drawing.Size(64, 26);
			this.textBoxItemId.TabIndex = 4;
			this.textBoxItemId.Text = "0";
			this.textBoxItemId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumeric_KeyPress);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(294, 127);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(90, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "Drop Chance:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(294, 59);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(76, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Min Count:";
			// 
			// textBoxItemMinCount
			// 
			this.textBoxItemMinCount.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxItemMinCount.Location = new System.Drawing.Point(408, 56);
			this.textBoxItemMinCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxItemMinCount.MaxLength = 3;
			this.textBoxItemMinCount.Name = "textBoxItemMinCount";
			this.textBoxItemMinCount.Size = new System.Drawing.Size(37, 26);
			this.textBoxItemMinCount.TabIndex = 4;
			this.textBoxItemMinCount.Text = "0";
			this.textBoxItemMinCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumeric_KeyPress);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textBoxMaxGold);
			this.groupBox1.Controls.Add(this.labelLootGroupId);
			this.groupBox1.Controls.Add(this.textBoxGoldChance);
			this.groupBox1.Controls.Add(this.textBoxMinGold);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(25, 18);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(553, 135);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group Properties";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(248, 62);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(70, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Max Gold:";
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
			// textBoxMaxGold
			// 
			this.textBoxMaxGold.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxMaxGold.Location = new System.Drawing.Point(334, 58);
			this.textBoxMaxGold.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxMaxGold.MaxLength = 5;
			this.textBoxMaxGold.Name = "textBoxMaxGold";
			this.textBoxMaxGold.Size = new System.Drawing.Size(66, 26);
			this.textBoxMaxGold.TabIndex = 5;
			this.textBoxMaxGold.Text = "0";
			this.textBoxMaxGold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumeric_KeyPress);
			// 
			// labelLootGroupId
			// 
			this.labelLootGroupId.AutoSize = true;
			this.labelLootGroupId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelLootGroupId.Location = new System.Drawing.Point(145, 32);
			this.labelLootGroupId.Name = "labelLootGroupId";
			this.labelLootGroupId.Size = new System.Drawing.Size(15, 18);
			this.labelLootGroupId.TabIndex = 0;
			this.labelLootGroupId.Text = "0";
			// 
			// textBoxGoldChance
			// 
			this.textBoxGoldChance.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxGoldChance.Location = new System.Drawing.Point(148, 92);
			this.textBoxGoldChance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxGoldChance.MaxLength = 5;
			this.textBoxGoldChance.Name = "textBoxGoldChance";
			this.textBoxGoldChance.Size = new System.Drawing.Size(66, 26);
			this.textBoxGoldChance.TabIndex = 4;
			this.textBoxGoldChance.Text = "0";
			this.textBoxGoldChance.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxDecimal_KeyPress);
			// 
			// textBoxMinGold
			// 
			this.textBoxMinGold.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxMinGold.Location = new System.Drawing.Point(148, 58);
			this.textBoxMinGold.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxMinGold.MaxLength = 5;
			this.textBoxMinGold.Name = "textBoxMinGold";
			this.textBoxMinGold.Size = new System.Drawing.Size(66, 26);
			this.textBoxMinGold.TabIndex = 4;
			this.textBoxMinGold.Text = "0";
			this.textBoxMinGold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumeric_KeyPress);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(34, 95);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Gold Chance:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(34, 62);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Min Gold:";
			// 
			// LootsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "LootsControl";
			this.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Size = new System.Drawing.Size(872, 636);
			this.Load += new System.EventHandler(this.MmoItemsControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeViewLoots;
		private System.Windows.Forms.Button buttonCreate;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxMaxGold;
		private System.Windows.Forms.Label labelLootGroupId;
		private System.Windows.Forms.TextBox textBoxMinGold;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxGoldChance;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListBox listBoxLootItems;
		private System.Windows.Forms.Button buttonItemDelete;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxItemId;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBoxItemMaxCount;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textBoxItemMinCount;
		private System.Windows.Forms.TextBox textBoxItemDropChance;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button buttonItemAdd;
	}
}
