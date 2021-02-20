using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer.Forms
{
    public partial class SqlResultPanel : DockContent
    {
        public SqlResultPanel()
        {
            InitializeComponent();
            this.Shown += SqlResultPanel_Shown;
        }

        private void SqlResultPanel_Shown(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tb_connectionString.Text))
            {
                tsb_ReadConFromReg_Click(this,null);
            }            
        }

        public SourceContainerDocument SourceContainerDocument { get; set; }
        DataGridView dgrv = null;

        public void ExecuteSql(string getProcessedSqlText)
        {
            //WaitForm wf = new WaitForm();
            try
            {
                
               // wf.Show();
                string queryString = getProcessedSqlText;
                string connectionString = tb_connectionString.Text;

                if (String.IsNullOrEmpty(tb_connectionString.Text))
                {                    
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    //command.Parameters.AddWithValue("@tPatSName", "Your-Parm-Value");
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    try
                    {
                        dgrv = dataGridView1;
                        
                        var dt = new DataTable();
                        dt.Load(reader);
                        if (tsb_transpone.Checked)
                        {
                            dt = DatagridUtils.TransposeDataTable(dt);
                        }

                        if (dgrv == null)
                        {
                            dgrv = new DataGridView();
                            this.Controls.Add(dgrv);
                            dgrv.Dock = DockStyle.Fill;
                            dgrv.Location = new Point(0, 25);
                        }
                        dgrv.DataSource = dt;
                        DatagridUtils.FormatDataGridView(dgrv);
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "SQL Result Panel");
            }
            finally
            {
                //wf.Close();
            }
        }

        private void tsb_ReadConFromReg_Click(object sender, EventArgs e)
        {
            tb_connectionString.Text = @"Server={Servername};Database={Database};User Id={LogId};Password={LogPass}";
            try
            {
                string Servername = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\patrix\patricia\connection", "Servername", null);
                string Database = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\patrix\patricia\connection", "Database", null);
                string LogId = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\patrix\patricia\connection", "LogId", null);
                string LogPass = (string) Registry.GetValue(@"HKEY_CURRENT_USER\Software\patrix\patricia\connection", "LogPass", null);
                if (Servername != null)
                {
                    tb_connectionString.Text = $@"Server={Servername};Database={Database};User Id={LogId};Password={LogPass}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SQL Result Panel");
            }
        }

        private void tb_connectionString_TextChanged(object sender, EventArgs e)
        {

        }

        private void tsb_transpone_Click(object sender, EventArgs e)
        {
            this.SourceContainerDocument.SourceEditorPanel.SourceContainerDocument.ExecuteSql(true);
        }
    }
}
