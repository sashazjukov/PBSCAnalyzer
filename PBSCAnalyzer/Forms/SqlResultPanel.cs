using System;
using System.Collections;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace PBSCAnalyzer.Forms
{
    public partial class SqlResultPanel : DockContent
    {
        public SqlResultPanel()
        {
            InitializeComponent();
            this.Shown += SqlResultPanel_Shown;
            dataGridView1.DataError += DataGridView1_DataError;
        }

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
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
                textBox_sql_messages.AppendText("------------------------");
                textBox_sql_messages.AppendText(Environment.NewLine);

                // wf.Show();
                string queryString = getProcessedSqlText;
                string connectionString = tb_connectionString.Text;

                if (String.IsNullOrEmpty(tb_connectionString.Text))
                {
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    string statistics = "";
                    if (toolStripButton_statistics_time.Checked)
                    { statistics += "set statistics time on;"; }

                    if (toolStripButton_statistics_IO.Checked)
                    { statistics += "set statistics io on;"; }

                    queryString = statistics + queryString;

                    SqlCommand command = new SqlCommand(queryString, connection);
                    //command.Parameters.AddWithValue("@tPatSName", "Your-Parm-Value");
                    connection.FireInfoMessageEventOnUserErrors = true;
                    connection.InfoMessage += delegate (object obj, SqlInfoMessageEventArgs err)
                    {
                        var ab = err.Message;
                        textBox_sql_messages.AppendText(ab);
                        textBox_sql_messages.AppendText(Environment.NewLine);
                    };

                    //connection.StatisticsEnabled = true;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    IDictionary stats = connection.RetrieveStatistics();
                    foreach (DictionaryEntry item in stats)
                    {
                        //list all the availables keys and values
                        //textBox_sql_messages.AppendText(", " + item.Key + " :   " + item.Value);                       
                    }
                    // textBox_sql_messages.AppendText(Environment.NewLine);

                    try
                    {
                        dgrv = dataGridView1;
                        dgrv.SuspendLayout();

                        var dt = new DataTable();
                        dt.Load(reader);

                        tabPage_rows.Text = "Rows (" + dt.Rows.Count + ")";

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
                        dgrv.ResumeLayout();
                        //dgrv.PerformLayout();
                        tabControl1.SelectTab(0);
                        SourceContainerDocument.SourceEditorPanel.fastColoredTextBox1.Focus();

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
                //MessageBox.Show(e.Message, "SQL Result Panel");
                tabControl1.SelectTab(1);
                SourceContainerDocument.SourceEditorPanel.fastColoredTextBox1.Focus();
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
            // reexecutuate last sql
            this.SourceContainerDocument.SourceEditorPanel.SourceContainerDocument.ExecuteSql(true);
        }

        private void toolStripButton_statistics_IO_Click(object sender, EventArgs e)
        {
            // reexecutuate last sql when button is checked
            if (toolStripButton_statistics_IO.Checked)
            {
                // show tab first, it then can be changed if there are errors
                tabControl1.SelectTab(1);
                // reexcute sql and represent ressult in transponse mode
                this.SourceContainerDocument.SourceEditorPanel.SourceContainerDocument.ExecuteSql(true);                
            }
        }

        private void textBox_sql_messages_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
