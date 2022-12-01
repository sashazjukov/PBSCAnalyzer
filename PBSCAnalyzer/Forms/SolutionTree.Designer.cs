namespace PBSCAnalyzer
{
    partial class SolutionTree
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SolutionTree));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.findInSource_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.OpenedFiles_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.tsb_export_search_result = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.FindProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.FindInSolutionTextBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.treeView1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.FindInSolutionTextBox, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(234, 651);
            this.tableLayoutPanel1.TabIndex = 7;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(3, 75);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            treeNode2.ImageIndex = -2;
            treeNode2.Name = "Node0";
            treeNode2.Text = "Node0";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(228, 548);
            this.treeView1.TabIndex = 8;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Silver;
            this.imageList1.Images.SetKeyName(0, "Folder.png");
            this.imageList1.Images.SetKeyName(1, "27.png");
            this.imageList1.Images.SetKeyName(2, "40.png");
            this.imageList1.Images.SetKeyName(3, "37.png");
            this.imageList1.Images.SetKeyName(4, "25.png");
            this.imageList1.Images.SetKeyName(5, "38.png");
            this.imageList1.Images.SetKeyName(6, "16.png");
            this.imageList1.Images.SetKeyName(7, "5.png");
            this.imageList1.Images.SetKeyName(8, "Application_Design.png");
            this.imageList1.Images.SetKeyName(9, "Sum.png");
            this.imageList1.Images.SetKeyName(10, "26.png");
            this.imageList1.Images.SetKeyName(11, "70.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.findInSource_toolStripButton,
            this.toolStripButton1,
            this.OpenedFiles_toolStripButton,
            this.toolStripSeparator1,
            this.toolStripButton3,
            this.toolStripButton4,
            this.tsb_export_search_result});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(234, 38);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Silver;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(34, 33);
            this.toolStripButton2.Text = "Find in file names (Enter)";
            this.toolStripButton2.ToolTipText = "Find in file names using RegEx (Enter)";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // findInSource_toolStripButton
            // 
            this.findInSource_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findInSource_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("findInSource_toolStripButton.Image")));
            this.findInSource_toolStripButton.ImageTransparentColor = System.Drawing.Color.Silver;
            this.findInSource_toolStripButton.Name = "findInSource_toolStripButton";
            this.findInSource_toolStripButton.Size = new System.Drawing.Size(34, 33);
            this.findInSource_toolStripButton.Text = "Find in each file (F12)";
            this.findInSource_toolStripButton.ToolTipText = "Find in each file RegEx (F12)";
            this.findInSource_toolStripButton.Click += new System.EventHandler(this.findInSource_toolStripButton_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 38);
            // 
            // OpenedFiles_toolStripButton
            // 
            this.OpenedFiles_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenedFiles_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenedFiles_toolStripButton.Image")));
            this.OpenedFiles_toolStripButton.ImageTransparentColor = System.Drawing.Color.Silver;
            this.OpenedFiles_toolStripButton.Name = "OpenedFiles_toolStripButton";
            this.OpenedFiles_toolStripButton.Size = new System.Drawing.Size(34, 33);
            this.OpenedFiles_toolStripButton.Text = "Show opened files";
            this.OpenedFiles_toolStripButton.ToolTipText = "Show opened files";
            this.OpenedFiles_toolStripButton.Click += new System.EventHandler(this.OpenedFiles_toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 38);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Silver;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(34, 33);
            this.toolStripButton3.Text = "Epand all nodes";
            this.toolStripButton3.ToolTipText = "Epand all nodes";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Silver;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(34, 33);
            this.toolStripButton4.Text = "Collapse all nodes";
            this.toolStripButton4.ToolTipText = "Collapse all nodes";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // tsb_export_search_result
            // 
            this.tsb_export_search_result.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsb_export_search_result.Image = ((System.Drawing.Image)(resources.GetObject("tsb_export_search_result.Image")));
            this.tsb_export_search_result.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_export_search_result.Name = "tsb_export_search_result";
            this.tsb_export_search_result.Size = new System.Drawing.Size(34, 33);
            this.tsb_export_search_result.Text = "Export Search Result in a new note tab";
            this.tsb_export_search_result.Click += new System.EventHandler(this.tsb_export_search_result_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.FindProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 626);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(234, 25);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(64, 18);
            this.toolStripStatusLabel1.Text = "Ready.";
            // 
            // FindProgressBar
            // 
            this.FindProgressBar.Name = "FindProgressBar";
            this.FindProgressBar.Size = new System.Drawing.Size(100, 17);
            this.FindProgressBar.ToolTipText = "Find Progress";
            this.FindProgressBar.Visible = false;
            // 
            // FindInSolutionTextBox
            // 
            this.FindInSolutionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FindInSolutionTextBox.FormattingEnabled = true;
            this.FindInSolutionTextBox.Location = new System.Drawing.Point(3, 41);
            this.FindInSolutionTextBox.MaxDropDownItems = 30;
            this.FindInSolutionTextBox.Name = "FindInSolutionTextBox";
            this.FindInSolutionTextBox.Size = new System.Drawing.Size(228, 28);
            this.FindInSolutionTextBox.TabIndex = 12;
            this.FindInSolutionTextBox.TextChanged += new System.EventHandler(this.FindInSolutionTextBox_TextChanged);
            this.FindInSolutionTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FindInSolutionTextBox_KeyUp);
            // 
            // SolutionTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 651);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SolutionTree";
            this.Text = "Solution Tree / Filter";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton findInSource_toolStripButton;
        private System.Windows.Forms.ToolStripButton OpenedFiles_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar FindProgressBar;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ComboBox FindInSolutionTextBox;
        private System.Windows.Forms.ToolStripButton tsb_export_search_result;
    }
}