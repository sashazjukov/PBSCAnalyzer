namespace PBSCAnalyzer.Forms
{
    partial class SqlResultPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlResultPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tb_connectionString = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_transpone = new System.Windows.Forms.ToolStripButton();
            this.tsb_ReadConFromReg = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_rows = new System.Windows.Forms.TabPage();
            this.tabPage_messages = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox_sql_messages = new System.Windows.Forms.TextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_statistics_time = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_statistics_IO = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_rows.SuspendLayout();
            this.tabPage_messages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_connectionString
            // 
            this.tb_connectionString.Location = new System.Drawing.Point(101, 5);
            this.tb_connectionString.Name = "tb_connectionString";
            this.tb_connectionString.Size = new System.Drawing.Size(933, 20);
            this.tb_connectionString.TabIndex = 0;
            this.tb_connectionString.TextChanged += new System.EventHandler(this.tb_connectionString_TextChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_transpone,
            this.tsb_ReadConFromReg,
            this.toolStripSeparator1,
            this.toolStripButton_statistics_time,
            this.toolStripButton_statistics_IO,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(905, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsb_transpone
            // 
            this.tsb_transpone.CheckOnClick = true;
            this.tsb_transpone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsb_transpone.Image = ((System.Drawing.Image)(resources.GetObject("tsb_transpone.Image")));
            this.tsb_transpone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_transpone.Name = "tsb_transpone";
            this.tsb_transpone.Size = new System.Drawing.Size(65, 22);
            this.tsb_transpone.Text = "Transpone";
            this.tsb_transpone.ToolTipText = "Transpone query result";
            this.tsb_transpone.Click += new System.EventHandler(this.tsb_transpone_Click);
            // 
            // tsb_ReadConFromReg
            // 
            this.tsb_ReadConFromReg.Image = ((System.Drawing.Image)(resources.GetObject("tsb_ReadConFromReg.Image")));
            this.tsb_ReadConFromReg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_ReadConFromReg.Name = "tsb_ReadConFromReg";
            this.tsb_ReadConFromReg.Size = new System.Drawing.Size(141, 22);
            this.tsb_ReadConFromReg.Text = "Get connection string";
            this.tsb_ReadConFromReg.ToolTipText = "Get connection string";
            this.tsb_ReadConFromReg.Click += new System.EventHandler(this.tsb_ReadConFromReg_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tb_connectionString);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 503);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(905, 33);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Connection string:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_rows);
            this.tabControl1.Controls.Add(this.tabPage_messages);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(905, 478);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage_rows
            // 
            this.tabPage_rows.Controls.Add(this.dataGridView1);
            this.tabPage_rows.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage_rows.Location = new System.Drawing.Point(4, 22);
            this.tabPage_rows.Name = "tabPage_rows";
            this.tabPage_rows.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_rows.Size = new System.Drawing.Size(897, 279);
            this.tabPage_rows.TabIndex = 0;
            this.tabPage_rows.Text = "Rows";
            this.tabPage_rows.UseVisualStyleBackColor = true;
            // 
            // tabPage_messages
            // 
            this.tabPage_messages.Controls.Add(this.textBox_sql_messages);
            this.tabPage_messages.Location = new System.Drawing.Point(4, 22);
            this.tabPage_messages.Name = "tabPage_messages";
            this.tabPage_messages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_messages.Size = new System.Drawing.Size(897, 452);
            this.tabPage_messages.TabIndex = 1;
            this.tabPage_messages.Text = "Messages";
            this.tabPage_messages.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle7.NullValue = "{NULL}";
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.RowTemplate.DefaultCellStyle.NullValue = "{NULL}";
            this.dataGridView1.Size = new System.Drawing.Size(891, 273);
            this.dataGridView1.TabIndex = 3;
            // 
            // textBox_sql_messages
            // 
            this.textBox_sql_messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_sql_messages.Location = new System.Drawing.Point(3, 3);
            this.textBox_sql_messages.Multiline = true;
            this.textBox_sql_messages.Name = "textBox_sql_messages";
            this.textBox_sql_messages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_sql_messages.Size = new System.Drawing.Size(891, 446);
            this.textBox_sql_messages.TabIndex = 0;
            this.textBox_sql_messages.TextChanged += new System.EventHandler(this.textBox_sql_messages_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton_statistics_time
            // 
            this.toolStripButton_statistics_time.Checked = true;
            this.toolStripButton_statistics_time.CheckOnClick = true;
            this.toolStripButton_statistics_time.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButton_statistics_time.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_statistics_time.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_statistics_time.Image")));
            this.toolStripButton_statistics_time.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_statistics_time.Name = "toolStripButton_statistics_time";
            this.toolStripButton_statistics_time.Size = new System.Drawing.Size(37, 22);
            this.toolStripButton_statistics_time.Text = "Time";
            this.toolStripButton_statistics_time.ToolTipText = "Enable Statistics Time";
            // 
            // toolStripButton_statistics_IO
            // 
            this.toolStripButton_statistics_IO.CheckOnClick = true;
            this.toolStripButton_statistics_IO.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_statistics_IO.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_statistics_IO.Image")));
            this.toolStripButton_statistics_IO.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_statistics_IO.Name = "toolStripButton_statistics_IO";
            this.toolStripButton_statistics_IO.Size = new System.Drawing.Size(28, 22);
            this.toolStripButton_statistics_IO.Text = "I/O";
            this.toolStripButton_statistics_IO.ToolTipText = "Enable Statistics I/O";
            this.toolStripButton_statistics_IO.Click += new System.EventHandler(this.toolStripButton_statistics_IO_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // SqlResultPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 536);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "SqlResultPanel";
            this.Text = "Sql Output";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage_rows.ResumeLayout(false);
            this.tabPage_messages.ResumeLayout(false);
            this.tabPage_messages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_connectionString;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_transpone;
        private System.Windows.Forms.ToolStripButton tsb_ReadConFromReg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_rows;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPage_messages;
        private System.Windows.Forms.TextBox textBox_sql_messages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_statistics_time;
        private System.Windows.Forms.ToolStripButton toolStripButton_statistics_IO;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}