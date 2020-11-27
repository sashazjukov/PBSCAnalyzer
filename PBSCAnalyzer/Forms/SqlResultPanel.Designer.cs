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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tb_connectionString = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_transpone = new System.Windows.Forms.ToolStripButton();
            this.tsb_ReadConFromReg = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
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
            this.tsb_ReadConFromReg});
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
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle2.NullValue = "{NULL}";
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.RowTemplate.DefaultCellStyle.NullValue = "{NULL}";
            this.dataGridView1.Size = new System.Drawing.Size(905, 305);
            this.dataGridView1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tb_connectionString);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 330);
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
            // SqlResultPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 363);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "SqlResultPanel";
            this.Text = "Sql Output";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_connectionString;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_transpone;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripButton tsb_ReadConFromReg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}