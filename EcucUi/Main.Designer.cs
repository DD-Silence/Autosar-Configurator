namespace AutosarConfigurator
{
    partial class WdMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btInstance = new System.Windows.Forms.Button();
            this.btBswmd = new System.Windows.Forms.Button();
            this.tbInstance = new System.Windows.Forms.TextBox();
            this.tbBswmd = new System.Windows.Forms.TextBox();
            this.lbInstance = new System.Windows.Forms.Label();
            this.lbBswmd = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.rtDesc = new System.Windows.Forms.RichTextBox();
            this.rtStatus = new System.Windows.Forms.RichTextBox();
            this.fdMain = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnReload = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnScript = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.btInstance);
            this.splitContainer1.Panel1.Controls.Add(this.btBswmd);
            this.splitContainer1.Panel1.Controls.Add(this.tbInstance);
            this.splitContainer1.Panel1.Controls.Add(this.tbBswmd);
            this.splitContainer1.Panel1.Controls.Add(this.lbInstance);
            this.splitContainer1.Panel1.Controls.Add(this.lbBswmd);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(1176, 1008);
            this.splitContainer1.SplitterDistance = 100;
            this.splitContainer1.TabIndex = 1;
            // 
            // btInstance
            // 
            this.btInstance.Location = new System.Drawing.Point(539, 54);
            this.btInstance.Name = "btInstance";
            this.btInstance.Size = new System.Drawing.Size(40, 23);
            this.btInstance.TabIndex = 2;
            this.btInstance.Text = "...";
            this.btInstance.UseVisualStyleBackColor = true;
            this.btInstance.Click += new System.EventHandler(this.BtInstance_Click);
            // 
            // btBswmd
            // 
            this.btBswmd.Location = new System.Drawing.Point(539, 19);
            this.btBswmd.Name = "btBswmd";
            this.btBswmd.Size = new System.Drawing.Size(40, 23);
            this.btBswmd.TabIndex = 2;
            this.btBswmd.Text = "...";
            this.btBswmd.UseVisualStyleBackColor = true;
            this.btBswmd.Click += new System.EventHandler(this.BtBswmd_Click);
            // 
            // tbInstance
            // 
            this.tbInstance.Location = new System.Drawing.Point(194, 54);
            this.tbInstance.Name = "tbInstance";
            this.tbInstance.ReadOnly = true;
            this.tbInstance.Size = new System.Drawing.Size(329, 23);
            this.tbInstance.TabIndex = 1;
            // 
            // tbBswmd
            // 
            this.tbBswmd.Location = new System.Drawing.Point(194, 19);
            this.tbBswmd.Name = "tbBswmd";
            this.tbBswmd.ReadOnly = true;
            this.tbBswmd.Size = new System.Drawing.Size(329, 23);
            this.tbBswmd.TabIndex = 1;
            // 
            // lbInstance
            // 
            this.lbInstance.AutoSize = true;
            this.lbInstance.Location = new System.Drawing.Point(45, 57);
            this.lbInstance.Name = "lbInstance";
            this.lbInstance.Size = new System.Drawing.Size(141, 17);
            this.lbInstance.TabIndex = 0;
            this.lbInstance.Text = "ECUC instance location";
            // 
            // lbBswmd
            // 
            this.lbBswmd.AutoSize = true;
            this.lbBswmd.Location = new System.Drawing.Point(45, 22);
            this.lbBswmd.Name = "lbBswmd";
            this.lbBswmd.Size = new System.Drawing.Size(106, 17);
            this.lbBswmd.TabIndex = 0;
            this.lbBswmd.Text = "BSWMD location";
            // 
            // splitContainer3
            // 
            this.splitContainer3.BackColor = System.Drawing.SystemColors.HighlightText;
            this.splitContainer3.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer3.Size = new System.Drawing.Size(1176, 904);
            this.splitContainer3.SplitterDistance = 645;
            this.splitContainer3.TabIndex = 1;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Size = new System.Drawing.Size(1176, 645);
            this.splitContainer2.SplitterDistance = 343;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.rtDesc);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.rtStatus);
            this.splitContainer4.Size = new System.Drawing.Size(1176, 255);
            this.splitContainer4.SplitterDistance = 490;
            this.splitContainer4.SplitterWidth = 3;
            this.splitContainer4.TabIndex = 0;
            // 
            // rtDesc
            // 
            this.rtDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtDesc.Location = new System.Drawing.Point(0, 0);
            this.rtDesc.Name = "rtDesc";
            this.rtDesc.Size = new System.Drawing.Size(490, 255);
            this.rtDesc.TabIndex = 0;
            this.rtDesc.Text = "";
            // 
            // rtStatus
            // 
            this.rtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtStatus.Location = new System.Drawing.Point(0, 0);
            this.rtStatus.Name = "rtStatus";
            this.rtStatus.Size = new System.Drawing.Size(683, 255);
            this.rtStatus.TabIndex = 0;
            this.rtStatus.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnFile,
            this.mnScript,
            this.generateToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1176, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnFile
            // 
            this.mnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnSave,
            this.mnReload,
            this.quitToolStripMenuItem});
            this.mnFile.Name = "mnFile";
            this.mnFile.Size = new System.Drawing.Size(39, 21);
            this.mnFile.Text = "File";
            // 
            // mnSave
            // 
            this.mnSave.Name = "mnSave";
            this.mnSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnSave.Size = new System.Drawing.Size(147, 22);
            this.mnSave.Text = "Save";
            this.mnSave.Click += new System.EventHandler(this.MnSave_Click);
            // 
            // mnReload
            // 
            this.mnReload.Name = "mnReload";
            this.mnReload.Size = new System.Drawing.Size(147, 22);
            this.mnReload.Text = "Reload";
            this.mnReload.Click += new System.EventHandler(this.MnReload_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.QuitToolStripMenuItem_Click);
            // 
            // mnScript
            // 
            this.mnScript.Name = "mnScript";
            this.mnScript.Size = new System.Drawing.Size(53, 21);
            this.mnScript.Text = "Script";
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(73, 21);
            this.generateToolStripMenuItem.Text = "Generate";
            // 
            // WdMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 1033);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "WdMain";
            this.Text = "Autosar Configurator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WdMain_FormClosing);
            this.Load += new System.EventHandler(this.WdMain_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btInstance;
        private System.Windows.Forms.Button btBswmd;
        private System.Windows.Forms.TextBox tbInstance;
        private System.Windows.Forms.TextBox tbBswmd;
        private System.Windows.Forms.Label lbInstance;
        private System.Windows.Forms.Label lbBswmd;
        private System.Windows.Forms.FolderBrowserDialog fdMain;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.RichTextBox rtDesc;
        private System.Windows.Forms.RichTextBox rtStatus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnFile;
        private System.Windows.Forms.ToolStripMenuItem mnSave;
        private System.Windows.Forms.ToolStripMenuItem mnReload;
        private System.Windows.Forms.ToolStripMenuItem mnScript;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
    }
}
