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
            splitContainer1 = new SplitContainer();
            btInstance = new Button();
            btBswmd = new Button();
            tbInstance = new TextBox();
            tbBswmd = new TextBox();
            lbInstance = new Label();
            lbBswmd = new Label();
            splitContainer3 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            splitContainer4 = new SplitContainer();
            rtDesc = new RichTextBox();
            tcRight = new TabControl();
            tpConsole = new TabPage();
            rtScript = new RichTextBox();
            tpValid = new TabPage();
            fdMain = new FolderBrowserDialog();
            menuStrip1 = new MenuStrip();
            mnFile = new ToolStripMenuItem();
            mnSave = new ToolStripMenuItem();
            mnReload = new ToolStripMenuItem();
            quitToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer3).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            tcRight.SuspendLayout();
            tpConsole.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Cursor = Cursors.HSplit;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 32);
            splitContainer1.Margin = new Padding(5);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(btInstance);
            splitContainer1.Panel1.Controls.Add(btBswmd);
            splitContainer1.Panel1.Controls.Add(tbInstance);
            splitContainer1.Panel1.Controls.Add(tbBswmd);
            splitContainer1.Panel1.Controls.Add(lbInstance);
            splitContainer1.Panel1.Controls.Add(lbBswmd);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer3);
            splitContainer1.Size = new Size(1848, 1234);
            splitContainer1.SplitterDistance = 100;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 1;
            // 
            // btInstance
            // 
            btInstance.Location = new Point(847, 77);
            btInstance.Margin = new Padding(5);
            btInstance.Name = "btInstance";
            btInstance.Size = new Size(62, 32);
            btInstance.TabIndex = 2;
            btInstance.Text = "...";
            btInstance.UseVisualStyleBackColor = true;
            btInstance.Click += BtInstance_Click;
            // 
            // btBswmd
            // 
            btBswmd.Location = new Point(847, 26);
            btBswmd.Margin = new Padding(5);
            btBswmd.Name = "btBswmd";
            btBswmd.Size = new Size(62, 32);
            btBswmd.TabIndex = 2;
            btBswmd.Text = "...";
            btBswmd.UseVisualStyleBackColor = true;
            btBswmd.Click += BtBswmd_Click;
            // 
            // tbInstance
            // 
            tbInstance.Location = new Point(304, 77);
            tbInstance.Margin = new Padding(5);
            tbInstance.Name = "tbInstance";
            tbInstance.ReadOnly = true;
            tbInstance.Size = new Size(515, 30);
            tbInstance.TabIndex = 1;
            // 
            // tbBswmd
            // 
            tbBswmd.Location = new Point(304, 26);
            tbBswmd.Margin = new Padding(5);
            tbBswmd.Name = "tbBswmd";
            tbBswmd.ReadOnly = true;
            tbBswmd.Size = new Size(515, 30);
            tbBswmd.TabIndex = 1;
            // 
            // lbInstance
            // 
            lbInstance.AutoSize = true;
            lbInstance.Location = new Point(71, 80);
            lbInstance.Margin = new Padding(5, 0, 5, 0);
            lbInstance.Name = "lbInstance";
            lbInstance.Size = new Size(207, 24);
            lbInstance.TabIndex = 0;
            lbInstance.Text = "ECUC instance location";
            // 
            // lbBswmd
            // 
            lbBswmd.AutoSize = true;
            lbBswmd.Location = new Point(71, 31);
            lbBswmd.Margin = new Padding(5, 0, 5, 0);
            lbBswmd.Name = "lbBswmd";
            lbBswmd.Size = new Size(155, 24);
            lbBswmd.TabIndex = 0;
            lbBswmd.Text = "BSWMD location";
            // 
            // splitContainer3
            // 
            splitContainer3.BackColor = SystemColors.HighlightText;
            splitContainer3.Cursor = Cursors.HSplit;
            splitContainer3.Dock = DockStyle.Fill;
            splitContainer3.FixedPanel = FixedPanel.Panel2;
            splitContainer3.Location = new Point(0, 0);
            splitContainer3.Margin = new Padding(5);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(splitContainer4);
            splitContainer3.Size = new Size(1848, 1128);
            splitContainer3.SplitterDistance = 861;
            splitContainer3.SplitterWidth = 6;
            splitContainer3.TabIndex = 1;
            // 
            // splitContainer2
            // 
            splitContainer2.Cursor = Cursors.VSplit;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Margin = new Padding(5);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Size = new Size(1848, 861);
            splitContainer2.SplitterDistance = 539;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            splitContainer4.Cursor = Cursors.VSplit;
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Margin = new Padding(5);
            splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(rtDesc);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(tcRight);
            splitContainer4.Size = new Size(1848, 261);
            splitContainer4.SplitterDistance = 770;
            splitContainer4.SplitterWidth = 5;
            splitContainer4.TabIndex = 0;
            // 
            // rtDesc
            // 
            rtDesc.Dock = DockStyle.Fill;
            rtDesc.Location = new Point(0, 0);
            rtDesc.Margin = new Padding(5);
            rtDesc.Name = "rtDesc";
            rtDesc.Size = new Size(770, 261);
            rtDesc.TabIndex = 0;
            rtDesc.Text = "";
            // 
            // tcRight
            // 
            tcRight.Controls.Add(tpConsole);
            tcRight.Controls.Add(tpValid);
            tcRight.Dock = DockStyle.Fill;
            tcRight.Location = new Point(0, 0);
            tcRight.Margin = new Padding(5);
            tcRight.Name = "tcRight";
            tcRight.SelectedIndex = 0;
            tcRight.Size = new Size(1073, 261);
            tcRight.TabIndex = 1;
            // 
            // tpConsole
            // 
            tpConsole.Controls.Add(rtScript);
            tpConsole.Location = new Point(4, 33);
            tpConsole.Margin = new Padding(5);
            tpConsole.Name = "tpConsole";
            tpConsole.Padding = new Padding(5);
            tpConsole.Size = new Size(1065, 224);
            tpConsole.TabIndex = 0;
            tpConsole.Text = "Console";
            tpConsole.UseVisualStyleBackColor = true;
            // 
            // rtScript
            // 
            rtScript.Dock = DockStyle.Fill;
            rtScript.Location = new Point(5, 5);
            rtScript.Margin = new Padding(5);
            rtScript.Name = "rtScript";
            rtScript.Size = new Size(1055, 214);
            rtScript.TabIndex = 0;
            rtScript.Text = "";
            // 
            // tpValid
            // 
            tpValid.Location = new Point(4, 33);
            tpValid.Margin = new Padding(5);
            tpValid.Name = "tpValid";
            tpValid.Padding = new Padding(5);
            tpValid.Size = new Size(1065, 224);
            tpValid.TabIndex = 1;
            tpValid.Text = "Validation";
            tpValid.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { mnFile });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(10, 2, 0, 2);
            menuStrip1.Size = new Size(1848, 32);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // mnFile
            // 
            mnFile.DropDownItems.AddRange(new ToolStripItem[] { mnSave, mnReload, quitToolStripMenuItem });
            mnFile.Name = "mnFile";
            mnFile.Size = new Size(56, 28);
            mnFile.Text = "File";
            // 
            // mnSave
            // 
            mnSave.Name = "mnSave";
            mnSave.ShortcutKeys = Keys.Control | Keys.S;
            mnSave.Size = new Size(270, 34);
            mnSave.Text = "Save";
            mnSave.Click += MnSave_Click;
            // 
            // mnReload
            // 
            mnReload.Name = "mnReload";
            mnReload.Size = new Size(270, 34);
            mnReload.Text = "Reload";
            mnReload.Click += MnReload_Click;
            // 
            // quitToolStripMenuItem
            // 
            quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            quitToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Q;
            quitToolStripMenuItem.Size = new Size(270, 34);
            quitToolStripMenuItem.Text = "Quit";
            quitToolStripMenuItem.Click += QuitToolStripMenuItem_Click;
            // 
            // WdMain
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1848, 1266);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(5);
            Name = "WdMain";
            Text = "Autosar Configurator V1.2";
            FormClosing += WdMain_FormClosing;
            Load += WdMain_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer3).EndInit();
            splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            tcRight.ResumeLayout(false);
            tpConsole.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private Button btInstance;
        private Button btBswmd;
        private TextBox tbInstance;
        private TextBox tbBswmd;
        private Label lbInstance;
        private Label lbBswmd;
        private FolderBrowserDialog fdMain;
        private SplitContainer splitContainer3;
        private SplitContainer splitContainer4;
        private RichTextBox rtDesc;
        private RichTextBox rtScript;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem mnFile;
        private ToolStripMenuItem mnSave;
        private ToolStripMenuItem mnReload;
        private ToolStripMenuItem quitToolStripMenuItem;
        private TabControl tcRight;
        private TabPage tpConsole;
        private TabPage tpValid;
    }
}
