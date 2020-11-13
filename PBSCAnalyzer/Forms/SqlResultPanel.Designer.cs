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
            this.tb_connectionString = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsb_transpone = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tsb_ReadConFromReg = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_connectionString
            // 
            this.tb_connectionString.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tb_connectionString.Location = new System.Drawing.Point(0, 336);
            this.tb_connectionString.Name = "tb_connectionString";
            this.tb_connectionString.Size = new System.Drawing.Size(519, 20);
            this.tb_connectionString.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsb_transpone,
            this.tsb_ReadConFromReg});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(519, 25);
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
            this.tsb_transpone.Size = new System.Drawing.Size(66, 22);
            this.tsb_transpone.Text = "Transpone";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(519, 311);
            this.dataGridView1.TabIndex = 2;
            // 
            // tsb_ReadConFromReg
            // 
            this.tsb_ReadConFromReg.Image = ((System.Drawing.Image)(resources.GetObject("tsb_ReadConFromReg.Image")));
            this.tsb_ReadConFromReg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsb_ReadConFromReg.Name = "tsb_ReadConFromReg";
            this.tsb_ReadConFromReg.Size = new System.Drawing.Size(130, 22);
            this.tsb_ReadConFromReg.Text = "Read Con from Reg";
            this.tsb_ReadConFromReg.Click += new System.EventHandler(this.tsb_ReadConFromReg_Click);
            // 
            // SqlResultPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 356);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tb_connectionString);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "SqlResultPanel";
            this.Text = "SqlResultPanel";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_connectionString;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsb_transpone;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripButton tsb_ReadConFromReg;
    }
}