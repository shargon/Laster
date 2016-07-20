using Laster.Controls;

namespace Laster
{
    partial class FEditTopology
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FEditTopology));
            this.Splitter = new System.Windows.Forms.Splitter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.generateExeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pGrid = new System.Windows.Forms.Panel();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.cmItems = new System.Windows.Forms.ComboBox();
            this.tPaintPlay = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.pItems = new Laster.Controls.UCPanelDoubleBufffer();
            this.pError = new System.Windows.Forms.Panel();
            this.rError = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.pGrid.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pError.SuspendLayout();
            this.SuspendLayout();
            // 
            // Splitter
            // 
            this.Splitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.Splitter.Location = new System.Drawing.Point(691, 24);
            this.Splitter.Name = "Splitter";
            this.Splitter.Size = new System.Drawing.Size(3, 537);
            this.Splitter.TabIndex = 3;
            this.Splitter.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.inputToolStripMenuItem,
            this.processToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItem2,
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.toolStripMenuItem1,
            this.generateExeToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Image = global::Laster.Res.play;
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Image = global::Laster.Res.stop;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Visible = false;
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(138, 6);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::Laster.Res.new_file;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Image = global::Laster.Res.save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Image = global::Laster.Res.save;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.saveAsToolStripMenuItem.Text = "Save as";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Image = global::Laster.Res.load;
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 6);
            // 
            // generateExeToolStripMenuItem
            // 
            this.generateExeToolStripMenuItem.Enabled = false;
            this.generateExeToolStripMenuItem.Image = global::Laster.Res.exe;
            this.generateExeToolStripMenuItem.Name = "generateExeToolStripMenuItem";
            this.generateExeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.generateExeToolStripMenuItem.Text = "Generate exe";
            this.generateExeToolStripMenuItem.Click += new System.EventHandler(this.generateExeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(138, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::Laster.Res.exit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // inputToolStripMenuItem
            // 
            this.inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            this.inputToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.inputToolStripMenuItem.Text = "Add Inputs";
            // 
            // processToolStripMenuItem
            // 
            this.processToolStripMenuItem.Name = "processToolStripMenuItem";
            this.processToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.processToolStripMenuItem.Text = "Add Process";
            // 
            // pGrid
            // 
            this.pGrid.Controls.Add(this.propertyGrid1);
            this.pGrid.Controls.Add(this.cmItems);
            this.pGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.pGrid.Location = new System.Drawing.Point(694, 24);
            this.pGrid.Name = "pGrid";
            this.pGrid.Size = new System.Drawing.Size(290, 537);
            this.pGrid.TabIndex = 6;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 21);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(290, 516);
            this.propertyGrid1.TabIndex = 7;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // cmItems
            // 
            this.cmItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmItems.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmItems.FormattingEnabled = true;
            this.cmItems.Location = new System.Drawing.Point(0, 0);
            this.cmItems.Name = "cmItems";
            this.cmItems.Size = new System.Drawing.Size(290, 21);
            this.cmItems.TabIndex = 6;
            this.cmItems.SelectedIndexChanged += new System.EventHandler(this.cmItems_SelectedIndexChanged);
            this.cmItems.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmItems_Format);
            // 
            // tPaintPlay
            // 
            this.tPaintPlay.Tick += new System.EventHandler(this.tPaintPlay_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pItems);
            this.panel1.Controls.Add(this.pError);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(694, 537);
            this.panel1.TabIndex = 0;
            // 
            // pItems
            // 
            this.pItems.AutoScroll = true;
            this.pItems.BackColor = System.Drawing.Color.White;
            this.pItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pItems.Location = new System.Drawing.Point(0, 0);
            this.pItems.Margin = new System.Windows.Forms.Padding(15);
            this.pItems.Name = "pItems";
            this.pItems.Size = new System.Drawing.Size(694, 385);
            this.pItems.TabIndex = 1;
            this.pItems.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.pItems_ControlAdded);
            this.pItems.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.pItems_ControlAdded);
            this.pItems.Paint += new System.Windows.Forms.PaintEventHandler(this.pItems_Paint);
            this.pItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pItems_MouseDown);
            this.pItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pItems_MouseMove);
            // 
            // pError
            // 
            this.pError.BackColor = System.Drawing.Color.Red;
            this.pError.Controls.Add(this.rError);
            this.pError.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pError.Location = new System.Drawing.Point(0, 385);
            this.pError.Name = "pError";
            this.pError.Size = new System.Drawing.Size(694, 152);
            this.pError.TabIndex = 0;
            this.pError.Visible = false;
            // 
            // rError
            // 
            this.rError.BackColor = System.Drawing.Color.Red;
            this.rError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rError.ForeColor = System.Drawing.Color.White;
            this.rError.Location = new System.Drawing.Point(0, 0);
            this.rError.Name = "rError";
            this.rError.Size = new System.Drawing.Size(694, 152);
            this.rError.TabIndex = 2;
            this.rError.Text = "";
            this.rError.DoubleClick += new System.EventHandler(this.rError_DoubleClick);
            // 
            // FEditTopology
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.Splitter);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pGrid);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FEditTopology";
            this.Text = "Laster - Edit Topology";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FEditTopology_FormClosed);
            this.Load += new System.EventHandler(this.FEditTopology_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FEditTopology_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pGrid.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pError.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem inputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processToolStripMenuItem;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ComboBox cmItems;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Splitter Splitter;
        public System.Windows.Forms.Panel pGrid;
        private UCPanelDoubleBufffer pItems;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.Timer tPaintPlay;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pError;
        private System.Windows.Forms.RichTextBox rError;
        private System.Windows.Forms.ToolStripMenuItem generateExeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    }
}