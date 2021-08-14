using System.Windows.Forms;

namespace mapdas
{
  partial class DisasmView
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
      this.ppcView = new SyncTextBox(ScrollBars.Vertical);
      this.hexView = new SyncTextBox(ScrollBars.None);

      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisasmView));
      this.funcIdentityView = new System.Windows.Forms.TextBox();
      this.SuspendLayout();

      // 
      // ppcView
      // 
      this.ppcView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
      this.ppcView.Buddy = this.hexView;
      this.ppcView.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ppcView.ForeColor = System.Drawing.SystemColors.Control;
      this.ppcView.Location = new System.Drawing.Point(89, 49);
      this.ppcView.Multiline = true;
      this.ppcView.Name = "ppcView";
      this.ppcView.ReadOnly = true;
      this.ppcView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.ppcView.Size = new System.Drawing.Size(865, 600);
      this.ppcView.TabIndex = 0;
      this.ppcView.WordWrap = false;
      // 
      // hexView
      // 
      this.hexView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
      this.hexView.Buddy = this.ppcView;
      this.hexView.Font = new System.Drawing.Font("Lucida Console", 9F);
      this.hexView.ForeColor = System.Drawing.SystemColors.Control;
      this.hexView.Location = new System.Drawing.Point(12, 49);
      this.hexView.Multiline = true;
      this.hexView.Name = "hexView";
      this.hexView.ReadOnly = true;
      this.hexView.Size = new System.Drawing.Size(78, 600);
      this.hexView.TabIndex = 1;
      this.hexView.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.hexView.WordWrap = false;
      // 
      // funcIdentityView
      // 
      this.funcIdentityView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
      this.funcIdentityView.Font = new System.Drawing.Font("Lucida Console", 15F);
      this.funcIdentityView.ForeColor = System.Drawing.SystemColors.Control;
      this.funcIdentityView.Location = new System.Drawing.Point(12, 12);
      this.funcIdentityView.Name = "funcIdentityView";
      this.funcIdentityView.ReadOnly = true;
      this.funcIdentityView.Size = new System.Drawing.Size(942, 27);
      this.funcIdentityView.TabIndex = 2;
      this.funcIdentityView.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.funcIdentityView.WordWrap = false;
      // 
      // DisasmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
      this.ClientSize = new System.Drawing.Size(966, 661);
      this.Controls.Add(this.funcIdentityView);
      this.Controls.Add(this.hexView);
      this.Controls.Add(this.ppcView);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "DisasmView";
      this.Text = "Disassembly View";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private SyncTextBox ppcView;
    private SyncTextBox hexView;
    private System.Windows.Forms.TextBox funcIdentityView;
  }
}