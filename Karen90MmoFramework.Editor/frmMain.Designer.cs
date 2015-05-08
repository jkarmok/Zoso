namespace Karen90MmoFramework.Editor
{
	partial class frmMain
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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabAura = new System.Windows.Forms.TabPage();
			this.tabNpc = new System.Windows.Forms.TabPage();
			this.tabGameItem = new System.Windows.Forms.TabPage();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabSpell = new System.Windows.Forms.TabPage();
			this.tabQuest = new System.Windows.Forms.TabPage();
			this.tabLootGroup = new System.Windows.Forms.TabPage();
			this.tabGameObject = new System.Windows.Forms.TabPage();
			this.statusStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 702);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
			this.statusStrip1.Size = new System.Drawing.Size(915, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 2;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// labelStatus
			// 
			this.labelStatus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(186, 17);
			this.labelStatus.Text = "Karen90 Database Editor Started";
			// 
			// tabAura
			// 
			this.tabAura.Location = new System.Drawing.Point(4, 27);
			this.tabAura.Name = "tabAura";
			this.tabAura.Padding = new System.Windows.Forms.Padding(3);
			this.tabAura.Size = new System.Drawing.Size(893, 644);
			this.tabAura.TabIndex = 3;
			this.tabAura.Text = "Auras";
			this.tabAura.UseVisualStyleBackColor = true;
			// 
			// tabNpc
			// 
			this.tabNpc.Location = new System.Drawing.Point(4, 27);
			this.tabNpc.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tabNpc.Name = "tabNpc";
			this.tabNpc.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tabNpc.Size = new System.Drawing.Size(893, 644);
			this.tabNpc.TabIndex = 2;
			this.tabNpc.Text = "Npcs";
			this.tabNpc.UseVisualStyleBackColor = true;
			// 
			// tabGameItem
			// 
			this.tabGameItem.Location = new System.Drawing.Point(4, 27);
			this.tabGameItem.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tabGameItem.Name = "tabGameItem";
			this.tabGameItem.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tabGameItem.Size = new System.Drawing.Size(907, 667);
			this.tabGameItem.TabIndex = 0;
			this.tabGameItem.Text = "Mmo Items";
			this.tabGameItem.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabGameItem);
			this.tabControl1.Controls.Add(this.tabNpc);
			this.tabControl1.Controls.Add(this.tabAura);
			this.tabControl1.Controls.Add(this.tabSpell);
			this.tabControl1.Controls.Add(this.tabQuest);
			this.tabControl1.Controls.Add(this.tabLootGroup);
			this.tabControl1.Controls.Add(this.tabGameObject);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(915, 698);
			this.tabControl1.TabIndex = 0;
			// 
			// tabSpell
			// 
			this.tabSpell.Location = new System.Drawing.Point(4, 27);
			this.tabSpell.Name = "tabSpell";
			this.tabSpell.Size = new System.Drawing.Size(893, 644);
			this.tabSpell.TabIndex = 4;
			this.tabSpell.Text = "Spells";
			this.tabSpell.UseVisualStyleBackColor = true;
			// 
			// tabQuest
			// 
			this.tabQuest.Location = new System.Drawing.Point(4, 27);
			this.tabQuest.Name = "tabQuest";
			this.tabQuest.Padding = new System.Windows.Forms.Padding(3);
			this.tabQuest.Size = new System.Drawing.Size(893, 644);
			this.tabQuest.TabIndex = 5;
			this.tabQuest.Text = "Quests";
			this.tabQuest.UseVisualStyleBackColor = true;
			// 
			// tabLootGroup
			// 
			this.tabLootGroup.Location = new System.Drawing.Point(4, 27);
			this.tabLootGroup.Name = "tabLootGroup";
			this.tabLootGroup.Padding = new System.Windows.Forms.Padding(3);
			this.tabLootGroup.Size = new System.Drawing.Size(893, 644);
			this.tabLootGroup.TabIndex = 6;
			this.tabLootGroup.Text = "Loot Groups";
			this.tabLootGroup.UseVisualStyleBackColor = true;
			// 
			// tabGameObject
			// 
			this.tabGameObject.Location = new System.Drawing.Point(4, 27);
			this.tabGameObject.Name = "tabGameObject";
			this.tabGameObject.Padding = new System.Windows.Forms.Padding(3);
			this.tabGameObject.Size = new System.Drawing.Size(893, 644);
			this.tabGameObject.TabIndex = 7;
			this.tabGameObject.Text = "Game Objects";
			this.tabGameObject.UseVisualStyleBackColor = true;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(915, 724);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.tabControl1);
			this.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.Text = "Karen90 Database Editor";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Disposed += new System.EventHandler(this.Form1_Disposed);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel labelStatus;

		private MmoItemsControl mmoItemsControl;
		private NpcControl npcControl;
		private AuraControl auraControl;
		private SpellControls spellControl;
		private QuestControls questControl;
		private LootsControl lootControl;
		private GameObjectControl goControl;
		private System.Windows.Forms.TabPage tabAura;
		private System.Windows.Forms.TabPage tabNpc;
		private System.Windows.Forms.TabPage tabGameItem;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabSpell;
		private System.Windows.Forms.TabPage tabQuest;
		private System.Windows.Forms.TabPage tabLootGroup;
		private System.Windows.Forms.TabPage tabGameObject;
	}
}

