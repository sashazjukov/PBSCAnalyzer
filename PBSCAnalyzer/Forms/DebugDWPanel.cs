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
            historyList = new ListBox();
            this.Controls.Add(historyList);
            historyList.Dock = DockStyle.Fill;
            historyList.Width = 80;            
            dataTAbleLogManager = new DataTablesLogManager(historyList);
        }

        private ListBox historyList;

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
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
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
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                DateTime ChangedOn = File.GetLastWriteTime(file);
                if (ChangedOn != lastCahnged)
                {
                    lastCahnged = File.GetLastWriteTime(file);
                }
                else { return; }

                var ds = new DataSet();
                DataTable dtsourcfe = null;

                for (int i = 1; i <= NumberOfRetries; ++i)
                {
                    try
                    {
                        Thread.Sleep(100);
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
                    

                    int icontrolIndex=0;
                    string changeTime = File.GetLastWriteTime(file).ToString("mm:ss");
                    this.Text = Path.GetFileName(file) + " " + changeTime;
                    DataTableCollection tables = ds.Tables;

                    if (ds.Tables.Count == 0) {
                        foreach (DataGridView item in ff)
                        {
                            this.Controls.Remove(item);
                        }
                    };

                    dataTAbleLogManager.AddNewEntry(changeTime);                    

                    //var dt = TransposeDataTable(dtsourcfe);
                    foreach (DataTable table in tables)
                    {                        
                        var dt = TransposeDataTable(table);
                        DataGridView dgrv = null;;
                        if (ff.Count > icontrolIndex)
                        {
                            dgrv = ff[icontrolIndex++];
                        }
                        if (dgrv == null)
                        {
                            dgrv = new DataGridView();
                            this.Controls.Add(dgrv);
                            dgrv.Dock = DockStyle.Left;
                            dgrv.Width = (this.Width - 80) / tables.Count;
                            dgrv.DataSource = dt;                                                        
                        }

                        dataTAbleLogManager.AddTable(dt);
                        dataTAbleLogManager.HighlightChangesInLadsTable(dgrv);
                        DatagridUtils.FormatDataGridView(tables.Count, dgrv);
                    }
                }
                ));
                //Debug.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        

        private DataTablesLogManager dataTAbleLogManager;
        
        public DateTime lastCahnged { get; private set; }

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

    public static class DatagridUtils {
        public static void FormatDataGridView(int countTables, DataGridView dgrv)
        {            
            for (int i = 0; i < dgrv.Columns.Count - 1; i++)
            {
                dgrv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                
            }
            dgrv.Columns[dgrv.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            for (int i = 0; i < dgrv.Columns.Count; i++)
            {
                int colw = Math.Min(dgrv.Columns[i].Width,200);
                dgrv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgrv.Columns[i].Width = colw;                
            }
        }
    }

    internal class DataTablesLogManager
    {
        public List<DataTablesLogEntity> dataTablesLog = new List<DataTablesLogEntity>();

        public DataTablesLogManager(ListBox visualList)
        {
            _visualList = visualList;
            _visualList.DisplayMember = "displayValue";
            _visualList.SelectedIndexChanged += _visualList_SelectedIndexChanged;
        }

        private void _visualList_SelectedIndexChanged(object sender, EventArgs e)
        {
           // DataTablesLogEntity logEntity =  (DataTablesLogEntity)_visualList.Items[_visualList.SelectedIndex-1];            
        }

        internal void AddNewEntry(string changeTime)
        {
            DataTablesLogEntity dataTablesLogEntity = new DataTablesLogEntity();
            dataTablesLogEntity.displayValue = changeTime;
            dataTablesLog.Add(dataTablesLogEntity);
            _visualList.Items.Add(dataTablesLogEntity);
        }

        internal void AddTable(DataTable dt)
        {
            DataTablesLogEntity lastInLog = dataTablesLog.Last<DataTablesLogEntity>();
            lastInLog.dataTables.Add(dt);
        }

        Boolean wasHighlighted;
        private ListBox _visualList;

        internal bool HighlightChangesInLadsTable(DataGridView dgrv)
        {
            wasHighlighted = false;
            DataTablesLogEntity lastInLog = dataTablesLog.Last<DataTablesLogEntity>();
            var lastTable = lastInLog.dataTables.Last();
            var index = lastInLog.dataTables.IndexOf(lastTable);
            if (dataTablesLog.Count >= 2)
            {
                var prevInLog = dataTablesLog[dataTablesLog.Count - 2];
                DataTable prevTable = null;
                if (index + 1 <= prevInLog.dataTables.Count)
                {
                    prevTable = prevInLog.dataTables[index];
                } 
                if ((prevTable != null) && (lastTable.Columns.Count == prevTable.Columns.Count) && (lastTable.Rows.Count == prevTable.Rows.Count))
                {
                    for (int j = 0; j <= lastTable.Columns.Count - 1; j++)
                    {
                        for (int k = 0; k <= lastTable.Rows.Count - 1; k++)
                        {
                            if (lastTable.Rows[k][j].ToString() != prevTable.Rows[k][j].ToString())
                            {
                                dgrv.Rows[k].Cells[j].Style.BackColor = Color.Yellow;
                                dgrv.Rows[k].Cells[j].Value = lastTable.Rows[k][j];
                                wasHighlighted = true;
                            }
                            else
                            {
                                dgrv.Rows[k].Cells[j].Style.BackColor = Color.White;
                            }
                        }
                    }
                    if (!wasHighlighted)
                    {
                        RemoveLastLogItem();
                    }
                }
                else {
                    dgrv.DataSource = lastTable;
                    DatagridUtils.FormatDataGridView(lastInLog.dataTables.Count, dgrv);
                    wasHighlighted = true;
                }

            }            
            return wasHighlighted;
        }

        private void RemoveLastLogItem()
        {
            DataTablesLogEntity lastInLog = dataTablesLog.Last<DataTablesLogEntity>();
            dataTablesLog.Remove(lastInLog);
            _visualList.Items.Remove(lastInLog);

        }
    }

    internal class DataTablesLogEntity
    {
        public string displayValue { get; set; }
       public List<DataTable> dataTables = new List<DataTable>();
    }
}
