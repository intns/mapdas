namespace mapdas
{
  partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.DebugLog = new System.Windows.Forms.RichTextBox();
			this.MapTree = new System.Windows.Forms.TreeView();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.commonFSCleanerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// DebugLog
			// 
			this.DebugLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.DebugLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.DebugLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.DebugLog.ForeColor = System.Drawing.Color.White;
			this.DebugLog.Location = new System.Drawing.Point(0, 627);
			this.DebugLog.Name = "DebugLog";
			this.DebugLog.ReadOnly = true;
			this.DebugLog.Size = new System.Drawing.Size(680, 130);
			this.DebugLog.TabIndex = 0;
			this.DebugLog.Text = "";
			// 
			// MapTree
			// 
			this.MapTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.MapTree.Dock = System.Windows.Forms.DockStyle.Top;
			this.MapTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.MapTree.ForeColor = System.Drawing.Color.White;
			this.MapTree.Location = new System.Drawing.Point(0, 24);
			this.MapTree.Name = "MapTree";
			this.MapTree.Size = new System.Drawing.Size(680, 601);
			this.MapTree.TabIndex = 1;
			this.MapTree.Click += new System.EventHandler(this.MapTree_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(680, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
			this.fileToolStripMenuItem.Text = "Open Map";
			this.fileToolStripMenuItem.Click += new System.EventHandler(this.FileToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commonFSCleanerToolStripMenuItem});
			this.toolsToolStripMenuItem.ForeColor = System.Drawing.SystemColors.Control;
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// commonFSCleanerToolStripMenuItem
			// 
			this.commonFSCleanerToolStripMenuItem.Name = "commonFSCleanerToolStripMenuItem";
			this.commonFSCleanerToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
			this.commonFSCleanerToolStripMenuItem.Text = "Common FS Cleaner";
			this.commonFSCleanerToolStripMenuItem.Click += new System.EventHandler(this.commonFSCleanerToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(680, 757);
			this.Controls.Add(this.MapTree);
			this.Controls.Add(this.DebugLog);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Form1";
			this.Text = "mapdas";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox DebugLog;
    private System.Windows.Forms.TreeView MapTree;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem commonFSCleanerToolStripMenuItem;
	}
}

