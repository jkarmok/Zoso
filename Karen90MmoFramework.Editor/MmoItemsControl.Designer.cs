namespace Karen90MmoFramework.Editor
{
	partial class MmoItemsControl
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
			this.treeViewItems = new System.Windows.Forms.TreeView();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.buttonNew = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxTradable = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxMaxStack = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSell = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.labelItemId = new System.Windows.Forms.Label();
			this.textBoxSpellId = new System.Windows.Forms.TextBox();
			this.textBoxBuyout = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxItemName = new System.Windows.Forms.TextBox();
			this.comboBoxItemType = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxRarity = new System.Windows.Forms.ComboBox();
			this.comboBoxUseLimit = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.treeViewItems);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
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
			// treeViewItems
			// 
			this.treeViewItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewItems.Location = new System.Drawing.Point(0, 0);
			this.treeViewItems.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
			this.treeViewItems.Name = "treeViewItems";
			this.treeViewItems.Size = new System.Drawing.Size(192, 624);
			this.treeViewItems.TabIndex = 0;
			this.treeViewItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGameItems_AfterSelect);
			// 
			// buttonCreate
			// 
			this.buttonCreate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonCreate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonCreate.Location = new System.Drawing.Point(473, 573);
			this.buttonCreate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonCreate.Name = "buttonCreate";
			this.buttonCreate.Size = new System.Drawing.Size(128, 37);
			this.buttonCreate.TabIndex = 13;
			this.buttonCreate.Text = "&Create";
			this.buttonCreate.UseVisualStyleBackColor = true;
			this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonDelete.Enabled = false;
			this.buttonDelete.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonDelete.Location = new System.Drawing.Point(205, 573);
			this.buttonDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(128, 37);
			this.buttonDelete.TabIndex = 11;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.buttonUpdate.Enabled = false;
			this.buttonUpdate.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonUpdate.Location = new System.Drawing.Point(339, 573);
			this.buttonUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(128, 37);
			this.buttonUpdate.TabIndex = 12;
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
			this.buttonNew.TabIndex = 10;
			this.buttonNew.Text = "&New";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkBoxTradable);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textBoxMaxStack);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textBoxSell);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.labelItemId);
			this.groupBox1.Controls.Add(this.textBoxSpellId);
			this.groupBox1.Controls.Add(this.textBoxBuyout);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textBoxItemName);
			this.groupBox1.Controls.Add(this.comboBoxItemType);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.comboBoxUseLimit);
			this.groupBox1.Controls.Add(this.comboBoxRarity);
			this.groupBox1.Location = new System.Drawing.Point(25, 18);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(576, 306);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "General Properties";
			// 
			// checkBoxTradable
			// 
			this.checkBoxTradable.AutoSize = true;
			this.checkBoxTradable.Checked = true;
			this.checkBoxTradable.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxTradable.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBoxTradable.Location = new System.Drawing.Point(296, 236);
			this.checkBoxTradable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.checkBoxTradable.Name = "checkBoxTradable";
			this.checkBoxTradable.Size = new System.Drawing.Size(80, 22);
			this.checkBoxTradable.TabIndex = 8;
			this.checkBoxTradable.Text = "Tradable";
			this.checkBoxTradable.UseVisualStyleBackColor = true;
			this.checkBoxTradable.CheckedChanged += new System.EventHandler(this.checkBoxCollectable_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(247, 136);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(69, 18);
			this.label5.TabIndex = 0;
			this.label5.Text = "Sell Price:";
			// 
			// textBoxMaxStack
			// 
			this.textBoxMaxStack.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxMaxStack.Location = new System.Drawing.Point(149, 200);
			this.textBoxMaxStack.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxMaxStack.MaxLength = 2;
			this.textBoxMaxStack.Name = "textBoxMaxStack";
			this.textBoxMaxStack.Size = new System.Drawing.Size(37, 26);
			this.textBoxMaxStack.TabIndex = 6;
			this.textBoxMaxStack.Text = "1";
			this.textBoxMaxStack.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxStack_KeyPress);
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
			// textBoxSell
			// 
			this.textBoxSell.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxSell.Location = new System.Drawing.Point(333, 132);
			this.textBoxSell.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxSell.MaxLength = 4;
			this.textBoxSell.Name = "textBoxSell";
			this.textBoxSell.Size = new System.Drawing.Size(66, 26);
			this.textBoxSell.TabIndex = 4;
			this.textBoxSell.Text = "0";
			this.textBoxSell.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxSell_KeyPress);
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
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(34, 203);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(73, 18);
			this.label7.TabIndex = 0;
			this.label7.Text = "Max Stack:";
			// 
			// labelItemId
			// 
			this.labelItemId.AutoSize = true;
			this.labelItemId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelItemId.Location = new System.Drawing.Point(145, 32);
			this.labelItemId.Name = "labelItemId";
			this.labelItemId.Size = new System.Drawing.Size(15, 18);
			this.labelItemId.TabIndex = 0;
			this.labelItemId.Text = "0";
			// 
			// textBoxSpellId
			// 
			this.textBoxSpellId.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxSpellId.Location = new System.Drawing.Point(148, 234);
			this.textBoxSpellId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxSpellId.MaxLength = 5;
			this.textBoxSpellId.Name = "textBoxSpellId";
			this.textBoxSpellId.Size = new System.Drawing.Size(66, 26);
			this.textBoxSpellId.TabIndex = 7;
			this.textBoxSpellId.Text = "0";
			this.textBoxSpellId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxBuyout_KeyPress);
			// 
			// textBoxBuyout
			// 
			this.textBoxBuyout.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxBuyout.Location = new System.Drawing.Point(149, 132);
			this.textBoxBuyout.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxBuyout.MaxLength = 4;
			this.textBoxBuyout.Name = "textBoxBuyout";
			this.textBoxBuyout.Size = new System.Drawing.Size(66, 26);
			this.textBoxBuyout.TabIndex = 3;
			this.textBoxBuyout.Text = "0";
			this.textBoxBuyout.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxBuyout_KeyPress);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(34, 169);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Rarity:";
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
			// textBoxItemName
			// 
			this.textBoxItemName.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxItemName.Location = new System.Drawing.Point(149, 98);
			this.textBoxItemName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.textBoxItemName.MaxLength = 24;
			this.textBoxItemName.Name = "textBoxItemName";
			this.textBoxItemName.Size = new System.Drawing.Size(250, 26);
			this.textBoxItemName.TabIndex = 2;
			// 
			// comboBoxItemType
			// 
			this.comboBoxItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxItemType.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxItemType.FormattingEnabled = true;
			this.comboBoxItemType.Location = new System.Drawing.Point(149, 64);
			this.comboBoxItemType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxItemType.Name = "comboBoxItemType";
			this.comboBoxItemType.Size = new System.Drawing.Size(147, 26);
			this.comboBoxItemType.TabIndex = 1;
			this.comboBoxItemType.SelectedIndexChanged += new System.EventHandler(this.comboBoxItemType_SelectedIndexChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(34, 237);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(58, 18);
			this.label8.TabIndex = 0;
			this.label8.Text = "Spell Id:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(34, 135);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Buyout Price:";
			// 
			// comboBoxRarity
			// 
			this.comboBoxRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxRarity.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxRarity.FormattingEnabled = true;
			this.comboBoxRarity.Location = new System.Drawing.Point(149, 166);
			this.comboBoxRarity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxRarity.Name = "comboBoxRarity";
			this.comboBoxRarity.Size = new System.Drawing.Size(118, 26);
			this.comboBoxRarity.TabIndex = 5;
			// 
			// comboBoxUseLimit
			// 
			this.comboBoxUseLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxUseLimit.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxUseLimit.FormattingEnabled = true;
			this.comboBoxUseLimit.Location = new System.Drawing.Point(148, 268);
			this.comboBoxUseLimit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.comboBoxUseLimit.Name = "comboBoxUseLimit";
			this.comboBoxUseLimit.Size = new System.Drawing.Size(118, 26);
			this.comboBoxUseLimit.TabIndex = 9;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(33, 271);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(69, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "Use Limit:";
			// 
			// MmoItemsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.splitContainer1);
			this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Name = "MmoItemsControl";
			this.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.Size = new System.Drawing.Size(872, 636);
			this.Load += new System.EventHandler(this.MmoItemsControl_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView treeViewItems;
		private System.Windows.Forms.Button buttonCreate;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxTradable;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textBoxMaxStack;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxSell;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelItemId;
		private System.Windows.Forms.TextBox textBoxBuyout;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxItemName;
		private System.Windows.Forms.ComboBox comboBoxItemType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxRarity;
		private System.Windows.Forms.TextBox textBoxSpellId;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox comboBoxUseLimit;
	}
}
