namespace Karen90MmoFramework.Editor
{
	partial class frmItemsWindow
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listBoxPossessions = new System.Windows.Forms.ListBox();
			this.listBoxItems = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblBuy = new System.Windows.Forms.Label();
			this.lblRarity = new System.Windows.Forms.Label();
			this.lblId = new System.Windows.Forms.Label();
			this.lblSell = new System.Windows.Forms.Label();
			this.lblType = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonAddToItems = new System.Windows.Forms.Button();
			this.buttonRemoveFromItems = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBoxPossessions
			// 
			this.listBoxPossessions.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxPossessions.FormattingEnabled = true;
			this.listBoxPossessions.ItemHeight = 15;
			this.listBoxPossessions.Location = new System.Drawing.Point(10, 32);
			this.listBoxPossessions.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.listBoxPossessions.Name = "listBoxPossessions";
			this.listBoxPossessions.Size = new System.Drawing.Size(197, 214);
			this.listBoxPossessions.TabIndex = 0;
			this.listBoxPossessions.SelectedValueChanged += new System.EventHandler(this.listBoxPossessions_SelectedValueChanged);
			// 
			// listBoxItems
			// 
			this.listBoxItems.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxItems.FormattingEnabled = true;
			this.listBoxItems.ItemHeight = 15;
			this.listBoxItems.Location = new System.Drawing.Point(276, 32);
			this.listBoxItems.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.listBoxItems.Name = "listBoxItems";
			this.listBoxItems.Size = new System.Drawing.Size(197, 214);
			this.listBoxItems.TabIndex = 0;
			this.listBoxItems.SelectedValueChanged += new System.EventHandler(this.listBoxItems_SelectedValueChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.lblBuy);
			this.groupBox1.Controls.Add(this.lblRarity);
			this.groupBox1.Controls.Add(this.lblId);
			this.groupBox1.Controls.Add(this.lblSell);
			this.groupBox1.Controls.Add(this.lblType);
			this.groupBox1.Controls.Add(this.lblName);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(10, 267);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBox1.Size = new System.Drawing.Size(463, 137);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Item Info";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(262, 31);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(29, 19);
			this.label6.TabIndex = 0;
			this.label6.Text = "Id: ";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(262, 96);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(37, 19);
			this.label8.TabIndex = 0;
			this.label8.Text = "Buy:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(17, 96);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 19);
			this.label7.TabIndex = 0;
			this.label7.Text = "Sell:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(262, 63);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(51, 19);
			this.label5.TabIndex = 0;
			this.label5.Text = "Rarity:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(17, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 19);
			this.label4.TabIndex = 0;
			this.label4.Text = "Type: ";
			// 
			// lblBuy
			// 
			this.lblBuy.AutoSize = true;
			this.lblBuy.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblBuy.Location = new System.Drawing.Point(337, 96);
			this.lblBuy.Name = "lblBuy";
			this.lblBuy.Size = new System.Drawing.Size(101, 19);
			this.lblBuy.TabIndex = 0;
			this.lblBuy.Text = "select an item";
			// 
			// lblRarity
			// 
			this.lblRarity.AutoSize = true;
			this.lblRarity.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblRarity.Location = new System.Drawing.Point(337, 63);
			this.lblRarity.Name = "lblRarity";
			this.lblRarity.Size = new System.Drawing.Size(101, 19);
			this.lblRarity.TabIndex = 0;
			this.lblRarity.Text = "select an item";
			// 
			// lblId
			// 
			this.lblId.AutoSize = true;
			this.lblId.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblId.Location = new System.Drawing.Point(337, 31);
			this.lblId.Name = "lblId";
			this.lblId.Size = new System.Drawing.Size(101, 19);
			this.lblId.TabIndex = 0;
			this.lblId.Text = "select an item";
			// 
			// lblSell
			// 
			this.lblSell.AutoSize = true;
			this.lblSell.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblSell.Location = new System.Drawing.Point(96, 96);
			this.lblSell.Name = "lblSell";
			this.lblSell.Size = new System.Drawing.Size(101, 19);
			this.lblSell.TabIndex = 0;
			this.lblSell.Text = "select an item";
			// 
			// lblType
			// 
			this.lblType.AutoSize = true;
			this.lblType.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblType.Location = new System.Drawing.Point(96, 63);
			this.lblType.Name = "lblType";
			this.lblType.Size = new System.Drawing.Size(101, 19);
			this.lblType.TabIndex = 0;
			this.lblType.Text = "select an item";
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.lblName.Location = new System.Drawing.Point(96, 31);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(101, 19);
			this.lblName.TabIndex = 0;
			this.lblName.Text = "select an item";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(55, 19);
			this.label3.TabIndex = 0;
			this.label3.Text = "Name: ";
			// 
			// buttonAddToItems
			// 
			this.buttonAddToItems.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAddToItems.Location = new System.Drawing.Point(213, 32);
			this.buttonAddToItems.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.buttonAddToItems.Name = "buttonAddToItems";
			this.buttonAddToItems.Size = new System.Drawing.Size(56, 101);
			this.buttonAddToItems.TabIndex = 2;
			this.buttonAddToItems.Text = "<";
			this.buttonAddToItems.UseVisualStyleBackColor = true;
			this.buttonAddToItems.Click += new System.EventHandler(this.buttonAddToItems_Click);
			// 
			// buttonRemoveFromItems
			// 
			this.buttonRemoveFromItems.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonRemoveFromItems.Location = new System.Drawing.Point(213, 145);
			this.buttonRemoveFromItems.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.buttonRemoveFromItems.Name = "buttonRemoveFromItems";
			this.buttonRemoveFromItems.Size = new System.Drawing.Size(56, 101);
			this.buttonRemoveFromItems.TabIndex = 2;
			this.buttonRemoveFromItems.Text = ">";
			this.buttonRemoveFromItems.UseVisualStyleBackColor = true;
			this.buttonRemoveFromItems.Click += new System.EventHandler(this.buttonRemoveFromItems_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(6, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 19);
			this.label1.TabIndex = 3;
			this.label1.Text = "Possessive Items:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(272, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 19);
			this.label2.TabIndex = 3;
			this.label2.Text = "Available Items:";
			// 
			// frmItemsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(486, 416);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonRemoveFromItems);
			this.Controls.Add(this.buttonAddToItems);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.listBoxItems);
			this.Controls.Add(this.listBoxPossessions);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.Name = "frmItemsWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Items Window";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmItemsWindow_FormClosed);
			this.Load += new System.EventHandler(this.frmItemsWindow_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListBox listBoxPossessions;
		private System.Windows.Forms.ListBox listBoxItems;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonAddToItems;
		private System.Windows.Forms.Button buttonRemoveFromItems;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lblBuy;
		private System.Windows.Forms.Label lblRarity;
		private System.Windows.Forms.Label lblId;
		private System.Windows.Forms.Label lblSell;
		private System.Windows.Forms.Label lblType;
		private System.Windows.Forms.Label lblName;
	}
}