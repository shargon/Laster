namespace Laster.Core.Forms
{
    partial class FScriptForm
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
            this.tEdit = new ScintillaNET.Scintilla();
            this.tError = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tEdit
            // 
            this.tEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tEdit.CaretLineVisible = true;
            this.tEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tEdit.Lexer = ScintillaNET.Lexer.Cpp;
            this.tEdit.Location = new System.Drawing.Point(0, 36);
            this.tEdit.MultipleSelection = true;
            this.tEdit.Name = "tEdit";
            this.tEdit.Size = new System.Drawing.Size(835, 444);
            this.tEdit.TabIndex = 0;
            this.tEdit.UseTabs = false;
            this.tEdit.TextChanged += new System.EventHandler(this.scintilla_TextChanged);
            // 
            // tError
            // 
            this.tError.BackColor = System.Drawing.SystemColors.Info;
            this.tError.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tError.Location = new System.Drawing.Point(0, 480);
            this.tError.Multiline = true;
            this.tError.Name = "tError";
            this.tError.ReadOnly = true;
            this.tError.Size = new System.Drawing.Size(835, 52);
            this.tError.TabIndex = 1;
            this.tError.Visible = false;
            this.tError.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tError_MouseDoubleClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(835, 36);
            this.toolStrip1.TabIndex = 2;
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Laster.Core.Res.compile;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(36, 33);
            this.toolStripButton2.Text = "Compile (F1/F5)";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 36);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Laster.Core.Res.check;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 33);
            this.toolStripButton1.Text = "Acept (F3)";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::Laster.Core.Res.close;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(36, 33);
            this.toolStripButton3.Text = "Cancel (Esc)";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // FScriptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 532);
            this.CloseOnEscape = false;
            this.Controls.Add(this.tEdit);
            this.Controls.Add(this.tError);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FScriptForm";
            this.ShowIcon = false;
            this.Text = "Script Form";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FScriptForm_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScintillaNET.Scintilla tEdit;
        private System.Windows.Forms.TextBox tError;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}