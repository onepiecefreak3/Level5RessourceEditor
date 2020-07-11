namespace Leve5RessourceEditor
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Leve5RessourceEditor.Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.uncheckBtn = new System.Windows.Forms.Button();
            this.pbiList = new System.Windows.Forms.CheckedListBox();
            this.resolutionList = new System.Windows.Forms.ComboBox();
            this.checkBtn = new System.Windows.Forms.Button();
            this.pb = new Cyotek.Windows.Forms.ImageBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtUvLocationY = new System.Windows.Forms.TextBox();
            this.lblUvLocationY = new System.Windows.Forms.Label();
            this.txtUvLocationX = new System.Windows.Forms.TextBox();
            this.lblUvLocationX = new System.Windows.Forms.Label();
            this.txtUvHeight = new System.Windows.Forms.TextBox();
            this.lblUvHeight = new System.Windows.Forms.Label();
            this.txtUvWidth = new System.Windows.Forms.TextBox();
            this.lblUvWidth = new System.Windows.Forms.Label();
            this.txtLocationY = new System.Windows.Forms.TextBox();
            this.lblLocationY = new System.Windows.Forms.Label();
            this.txtLocationX = new System.Windows.Forms.TextBox();
            this.lblLocationX = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.lblHeight = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbGridColor1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsbGridColor2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.clrDialog = new System.Windows.Forms.ColorDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.newMapToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(921, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.newMapToolStripMenuItem.Text = "&New Map";
            this.newMapToolStripMenuItem.Click += new System.EventHandler(this.newMapToolStripMenuItem_Click);
            // 
            // uncheckBtn
            // 
            this.uncheckBtn.Location = new System.Drawing.Point(12, 54);
            this.uncheckBtn.Name = "uncheckBtn";
            this.uncheckBtn.Size = new System.Drawing.Size(120, 23);
            this.uncheckBtn.TabIndex = 4;
            this.uncheckBtn.Text = "Uncheck All";
            this.uncheckBtn.UseVisualStyleBackColor = true;
            this.uncheckBtn.Click += new System.EventHandler(this.UncheckAll_Click);
            // 
            // pbiList
            // 
            this.pbiList.FormattingEnabled = true;
            this.pbiList.Location = new System.Drawing.Point(12, 112);
            this.pbiList.Name = "pbiList";
            this.pbiList.Size = new System.Drawing.Size(120, 394);
            this.pbiList.TabIndex = 5;
            this.pbiList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.pbiList_ItemCheck);
            this.pbiList.SelectedIndexChanged += new System.EventHandler(this.pbiList_SelectedIndexChanged);
            // 
            // resolutionList
            // 
            this.resolutionList.FormattingEnabled = true;
            this.resolutionList.Items.AddRange(new object[] {
            "Bottom Screen",
            "Top Screen"});
            this.resolutionList.Location = new System.Drawing.Point(13, 27);
            this.resolutionList.Name = "resolutionList";
            this.resolutionList.Size = new System.Drawing.Size(119, 21);
            this.resolutionList.Sorted = true;
            this.resolutionList.TabIndex = 8;
            this.resolutionList.Text = "Bottom Screen";
            this.resolutionList.SelectedIndexChanged += new System.EventHandler(this.resolutionList_SelectedIndexChanged);
            // 
            // checkBtn
            // 
            this.checkBtn.Location = new System.Drawing.Point(13, 83);
            this.checkBtn.Name = "checkBtn";
            this.checkBtn.Size = new System.Drawing.Size(119, 23);
            this.checkBtn.TabIndex = 9;
            this.checkBtn.Text = "Check All";
            this.checkBtn.UseVisualStyleBackColor = true;
            this.checkBtn.Click += new System.EventHandler(this.CheckAll_Click);
            // 
            // pb
            // 
            this.pb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb.GridColor = System.Drawing.Color.Green;
            this.pb.GridColorAlternate = System.Drawing.Color.Green;
            this.pb.ImageBorderStyle = Cyotek.Windows.Forms.ImageBoxBorderStyle.FixedSingle;
            this.pb.Location = new System.Drawing.Point(0, 25);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(771, 392);
            this.pb.TabIndex = 10;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(138, 27);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtUvLocationY);
            this.splitContainer1.Panel1.Controls.Add(this.lblUvLocationY);
            this.splitContainer1.Panel1.Controls.Add(this.txtUvLocationX);
            this.splitContainer1.Panel1.Controls.Add(this.lblUvLocationX);
            this.splitContainer1.Panel1.Controls.Add(this.txtUvHeight);
            this.splitContainer1.Panel1.Controls.Add(this.lblUvHeight);
            this.splitContainer1.Panel1.Controls.Add(this.txtUvWidth);
            this.splitContainer1.Panel1.Controls.Add(this.lblUvWidth);
            this.splitContainer1.Panel1.Controls.Add(this.txtLocationY);
            this.splitContainer1.Panel1.Controls.Add(this.lblLocationY);
            this.splitContainer1.Panel1.Controls.Add(this.txtLocationX);
            this.splitContainer1.Panel1.Controls.Add(this.lblLocationX);
            this.splitContainer1.Panel1.Controls.Add(this.txtHeight);
            this.splitContainer1.Panel1.Controls.Add(this.lblHeight);
            this.splitContainer1.Panel1.Controls.Add(this.txtWidth);
            this.splitContainer1.Panel1.Controls.Add(this.lblWidth);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pb);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(771, 479);
            this.splitContainer1.SplitterDistance = 58;
            this.splitContainer1.TabIndex = 11;
            // 
            // txtUvLocationY
            // 
            this.txtUvLocationY.Enabled = false;
            this.txtUvLocationY.Location = new System.Drawing.Point(501, 31);
            this.txtUvLocationY.Name = "txtUvLocationY";
            this.txtUvLocationY.Size = new System.Drawing.Size(100, 20);
            this.txtUvLocationY.TabIndex = 15;
            this.txtUvLocationY.TextChanged += new System.EventHandler(this.txtUvLocationY_TextChanged);
            // 
            // lblUvLocationY
            // 
            this.lblUvLocationY.AutoSize = true;
            this.lblUvLocationY.Location = new System.Drawing.Point(460, 34);
            this.lblUvLocationY.Name = "lblUvLocationY";
            this.lblUvLocationY.Size = new System.Drawing.Size(35, 13);
            this.lblUvLocationY.TabIndex = 14;
            this.lblUvLocationY.Text = "UV Y:";
            // 
            // txtUvLocationX
            // 
            this.txtUvLocationX.Enabled = false;
            this.txtUvLocationX.Location = new System.Drawing.Point(501, 5);
            this.txtUvLocationX.Name = "txtUvLocationX";
            this.txtUvLocationX.Size = new System.Drawing.Size(100, 20);
            this.txtUvLocationX.TabIndex = 13;
            this.txtUvLocationX.TextChanged += new System.EventHandler(this.txtUvLocationX_TextChanged);
            // 
            // lblUvLocationX
            // 
            this.lblUvLocationX.AutoSize = true;
            this.lblUvLocationX.Location = new System.Drawing.Point(460, 8);
            this.lblUvLocationX.Name = "lblUvLocationX";
            this.lblUvLocationX.Size = new System.Drawing.Size(35, 13);
            this.lblUvLocationX.TabIndex = 12;
            this.lblUvLocationX.Text = "UV X:";
            // 
            // txtUvHeight
            // 
            this.txtUvHeight.Enabled = false;
            this.txtUvHeight.Location = new System.Drawing.Point(354, 31);
            this.txtUvHeight.Name = "txtUvHeight";
            this.txtUvHeight.Size = new System.Drawing.Size(100, 20);
            this.txtUvHeight.TabIndex = 11;
            this.txtUvHeight.TextChanged += new System.EventHandler(this.txtUvHeight_TextChanged);
            // 
            // lblUvHeight
            // 
            this.lblUvHeight.AutoSize = true;
            this.lblUvHeight.Location = new System.Drawing.Point(292, 34);
            this.lblUvHeight.Name = "lblUvHeight";
            this.lblUvHeight.Size = new System.Drawing.Size(59, 13);
            this.lblUvHeight.TabIndex = 10;
            this.lblUvHeight.Text = "UV Height:";
            // 
            // txtUvWidth
            // 
            this.txtUvWidth.Enabled = false;
            this.txtUvWidth.Location = new System.Drawing.Point(354, 5);
            this.txtUvWidth.Name = "txtUvWidth";
            this.txtUvWidth.Size = new System.Drawing.Size(100, 20);
            this.txtUvWidth.TabIndex = 9;
            this.txtUvWidth.TextChanged += new System.EventHandler(this.txtUvWidth_TextChanged);
            // 
            // lblUvWidth
            // 
            this.lblUvWidth.AutoSize = true;
            this.lblUvWidth.Location = new System.Drawing.Point(292, 8);
            this.lblUvWidth.Name = "lblUvWidth";
            this.lblUvWidth.Size = new System.Drawing.Size(56, 13);
            this.lblUvWidth.TabIndex = 8;
            this.lblUvWidth.Text = "UV Width:";
            // 
            // txtLocationY
            // 
            this.txtLocationY.Enabled = false;
            this.txtLocationY.Location = new System.Drawing.Point(176, 31);
            this.txtLocationY.Name = "txtLocationY";
            this.txtLocationY.Size = new System.Drawing.Size(100, 20);
            this.txtLocationY.TabIndex = 7;
            this.txtLocationY.TextChanged += new System.EventHandler(this.txtLocationY_TextChanged);
            // 
            // lblLocationY
            // 
            this.lblLocationY.AutoSize = true;
            this.lblLocationY.Location = new System.Drawing.Point(153, 34);
            this.lblLocationY.Name = "lblLocationY";
            this.lblLocationY.Size = new System.Drawing.Size(17, 13);
            this.lblLocationY.TabIndex = 6;
            this.lblLocationY.Text = "Y:";
            // 
            // txtLocationX
            // 
            this.txtLocationX.Enabled = false;
            this.txtLocationX.Location = new System.Drawing.Point(176, 5);
            this.txtLocationX.Name = "txtLocationX";
            this.txtLocationX.Size = new System.Drawing.Size(100, 20);
            this.txtLocationX.TabIndex = 5;
            this.txtLocationX.TextChanged += new System.EventHandler(this.txtLocationX_TextChanged);
            // 
            // lblLocationX
            // 
            this.lblLocationX.AutoSize = true;
            this.lblLocationX.Location = new System.Drawing.Point(153, 8);
            this.lblLocationX.Name = "lblLocationX";
            this.lblLocationX.Size = new System.Drawing.Size(17, 13);
            this.lblLocationX.TabIndex = 4;
            this.lblLocationX.Text = "X:";
            // 
            // txtHeight
            // 
            this.txtHeight.Enabled = false;
            this.txtHeight.Location = new System.Drawing.Point(47, 31);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(100, 20);
            this.txtHeight.TabIndex = 3;
            this.txtHeight.TextChanged += new System.EventHandler(this.txtHeight_TextChanged);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(3, 34);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 2;
            this.lblHeight.Text = "Height:";
            // 
            // txtWidth
            // 
            this.txtWidth.Enabled = false;
            this.txtWidth.Location = new System.Drawing.Point(47, 5);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(100, 20);
            this.txtWidth.TabIndex = 1;
            this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(3, 8);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 0;
            this.lblWidth.Text = "Width:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbGridColor1,
            this.tsbGridColor2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(771, 25);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbGridColor1
            // 
            this.tsbGridColor1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGridColor1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGridColor1.Name = "tsbGridColor1";
            this.tsbGridColor1.Size = new System.Drawing.Size(29, 22);
            this.tsbGridColor1.Text = "Grid Color 1";
            this.tsbGridColor1.Click += new System.EventHandler(this.toolStripDropDownButton1_Click);
            // 
            // tsbGridColor2
            // 
            this.tsbGridColor2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGridColor2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGridColor2.Name = "tsbGridColor2";
            this.tsbGridColor2.Size = new System.Drawing.Size(29, 22);
            this.tsbGridColor2.Text = "Grid Color 2";
            this.tsbGridColor2.Click += new System.EventHandler(this.toolStripDropDownButton2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 517);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.checkBtn);
            this.Controls.Add(this.resolutionList);
            this.Controls.Add(this.pbiList);
            this.Controls.Add(this.uncheckBtn);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Level5RessourceEditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog openFileDlg;
        private System.Windows.Forms.Button uncheckBtn;
        private System.Windows.Forms.CheckedListBox pbiList;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ComboBox resolutionList;
        private System.Windows.Forms.ToolStripMenuItem newMapToolStripMenuItem;
        private System.Windows.Forms.Button checkBtn;
        private Cyotek.Windows.Forms.ImageBox pb;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtUvLocationY;
        private System.Windows.Forms.Label lblUvLocationY;
        private System.Windows.Forms.TextBox txtUvLocationX;
        private System.Windows.Forms.Label lblUvLocationX;
        private System.Windows.Forms.TextBox txtUvHeight;
        private System.Windows.Forms.Label lblUvHeight;
        private System.Windows.Forms.TextBox txtUvWidth;
        private System.Windows.Forms.Label lblUvWidth;
        private System.Windows.Forms.TextBox txtLocationY;
        private System.Windows.Forms.Label lblLocationY;
        private System.Windows.Forms.TextBox txtLocationX;
        private System.Windows.Forms.Label lblLocationX;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton tsbGridColor1;
        private System.Windows.Forms.ToolStripDropDownButton tsbGridColor2;
        private System.Windows.Forms.ColorDialog clrDialog;
    }
}

