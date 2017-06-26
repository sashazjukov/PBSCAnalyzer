using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer.Forms
{
    public partial class DebugDWPanel : DockContent
    {
        internal string file;

        public DebugDWPanel()
        {
            InitializeComponent();
        }

        public void CreateFileWatcher()
        {
            try
            {
                this.Text = Path.GetFileName(file);
                string path = Path.GetDirectoryName(file) + '\\';
                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = path;
                /* Watch for changes in LastAccess and LastWrite times, and 
                   the renaming of files or directories. */
                watcher.NotifyFilter = NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime;
                // Only watch text files.
                watcher.Filter = Path.GetFileName(file);

                // Add event handlers.
                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                //watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);

                // Begin watching.
                watcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 500;
        // Define the event handlers.
        private  void OnChanged(object source, FileSystemEventArgs e)
        {
            try { 
            var ds = new DataSet();
                DataTable dtsourcfe = null;

                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        Thread.Sleep(200);
                        ds.ReadXml(file);
                        //dtsourcfe = getDataFromXLS(file);
                        break; // When done we can break loop
                    }
                    catch (IOException ex)
                    {
                        // You may check error code to filter some exceptions, not every error
                        // can be recovered.
                        if (i == NumberOfRetries) // Last one, (re)throw exception and exit
                            throw;

                        Thread.Sleep(DelayOnRetry);
                    }
                }
                                

                // Specify what is done when a file is changed, created, or deleted.
                MainEngine.Instance.MainForm.Invoke(new MethodInvoker(() =>
                {
                    var ff = this.Controls.OfType<DataGridView>().ToList();
                    foreach (DataGridView item in ff)
                    {
                        this.Controls.Remove(item);
                    }

                    //var dt = TransposeDataTable(dtsourcfe);
                    foreach (DataTable table in ds.Tables)
                    {

                        var dt = TransposeDataTable(table);
                        DataGridView dgrv = new DataGridView();
                        this.Controls.Add(dgrv);
                        dgrv.Dock = DockStyle.Left;
                        dgrv.Width = this.Width / ds.Tables.Count;
                        //dataGridView1.DataSource = dt;
                        dgrv.DataSource = dt;
                        this.Text = Path.GetFileName(file) + " " + File.GetLastWriteTime(file).ToString("mm:ss");
                        for (int i = 0; i < dgrv.Columns.Count - 1; i++)
                        {
                            dgrv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        }
                        dgrv.Columns[dgrv.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        for (int i = 0; i < dgrv.Columns.Count; i++)
                        {
                            int colw = dgrv.Columns[i].Width;
                            dgrv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            dgrv.Columns[i].Width = colw;
                        }
                    }
                }
                ));
            Debug.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private DataTable getDataFromXLS(string strFilePath)
        {
            try
            {
                string strConnectionString = "";
                strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                           "Data Source=" + strFilePath + "; Jet OLEDB:Engine Type=5;" + "Extended Properties=Excel 8.0;";
                OleDbConnection cnCSV = new OleDbConnection(strConnectionString);
                cnCSV.Open();
                OleDbCommand cmdSelect = new OleDbCommand(@"SELECT * FROM ["+ Path.GetFileNameWithoutExtension(file) + "$]", cnCSV);
                OleDbDataAdapter daCSV = new OleDbDataAdapter(); daCSV.SelectCommand = cmdSelect;
                DataTable dtCSV = new DataTable();
                daCSV.Fill(dtCSV);
                cnCSV.Close();
                daCSV = null;
                return dtCSV;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            finally
            {
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Debug.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
        }

        /// <summary>
        /// Creates a new DataTable with row count = column count of original table
        ///  and column count = row count of original table. 
        /// </summary>
        /// <param name="dt">Original DataTable to transpose</param>
        /// <returns>A transposed DataTable</returns>
        public DataTable TransposeDataTable(DataTable dt)
        {
            DataTable transposedTable = new DataTable();
            
            DataColumn firstColumn = new DataColumn("Column Name");
            transposedTable.Columns.Add(firstColumn);

            //Add a column for each row in first data table
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataColumn dc = new DataColumn(i.ToString());//dt.Rows[i][0].ToString()
                transposedTable.Columns.Add(dc);
            }
            
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                DataRow dr = transposedTable.NewRow();
                dr[0] = dt.Columns[j].ColumnName;

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    dr[k + 1] = dt.Rows[k][j];
                }

                transposedTable.Rows.Add(dr);
            }
            DataView defaultView = transposedTable.DefaultView;
            defaultView.Sort = "Column Name";
            transposedTable = defaultView.ToTable();
            return transposedTable;
        }
    }
}
