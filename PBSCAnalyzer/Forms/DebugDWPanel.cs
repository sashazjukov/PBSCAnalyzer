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
            historyList.Dock = DockStyle.Right;
            historyList.Width = 80;            
            dataTAbleLogManager = new DataTablesLogManager(historyList, ShowDataGridViews);
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
                    List<DataTable> tables = ConvertToTables(ds);
                    ShowDataGridViews(tables, false);
                }
                ));
                //Debug.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowDataGridViews(List<DataTable> tables, Boolean isFromHistory)
        {
            int icontrolIndex = 0;
            string changeTime = "history";
            if (!isFromHistory)
            {
                changeTime = File.GetLastWriteTime(file).ToString("mm:ss");
                this.Text = Path.GetFileName(file) + " " + changeTime;
            }
            dataTAbleLogManager.AddNewEntry(changeTime);


            var ff = this.Controls.OfType<DataGridView>().ToList();

            if (tables.Count == 0)
            {
                foreach (DataGridView item in ff)
                {
                    this.Controls.Remove(item);
                }
            };

            //var dt = TransposeDataTable(dtsourcfe);
            foreach (DataTable table in tables)
            {
                DataTable dt = table;
                if (!isFromHistory)
                {
                    dt = TransposeDataTable(table);
                }
                DataGridView dgrv = null; ;
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

                // if (!isFromHistory)
                {
                    dataTAbleLogManager.AddTable(dt);
                }
                dataTAbleLogManager.HighlightChangesInLadsTable(dgrv, isFromHistory);

                DatagridUtils.FormatDataGridView(tables.Count, dgrv);
            }
            if (!isFromHistory)
            {
                dataTAbleLogManager.RemoveLastLogIfNotHighlighted();
            }
        }

        private static List<DataTable> ConvertToTables(DataSet ds)
        {
            List<DataTable> tables = new List<DataTable>();
            foreach (DataTable item in ds.Tables)
            {
                tables.Add(item);
            }

            return tables;
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

        public DataTablesLogManager(ListBox visualList, Action<List<DataTable>, bool> showDataGridViews) : this(visualList)
        {
            this.showDataGridViews = showDataGridViews;
        }

        private void _visualList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_visualList.SelectedIndex >= 0 && _visualList.Items.Count > 0)
            {
                DataTablesLogEntity logEntity = (DataTablesLogEntity)_visualList.Items[_visualList.SelectedIndex];
                indexOfPrevLogEntity = _visualList.SelectedIndex - 1;
                //if (indexOfPrevLogEntity < 0) indexOfPrevLogEntity = 0;
                showDataGridViews.Invoke(logEntity.dataTables, true);
                RemoveLastLogItem();
            }
        }

        internal void AddNewEntry(string changeTime)
        {
            DataTablesLogEntity dataTablesLogEntity = new DataTablesLogEntity();
            dataTablesLogEntity.displayValue = changeTime;
            dataTablesLog.Add(dataTablesLogEntity);
            _visualList.Items.Add(dataTablesLogEntity);
            wasHighlighted = false;
        }

        internal void AddTable(DataTable dt)
        {
            DataTablesLogEntity lastInLog = dataTablesLog.Last<DataTablesLogEntity>();
            lastInLog.dataTables.Add(dt);
        }

        Boolean wasHighlighted;
        private ListBox _visualList;
        private Action<List<DataTable>, bool> showDataGridViews;

        int indexOfPrevLogEntity = 0;

        internal bool HighlightChangesInLadsTable(DataGridView dgrv, Boolean isFromHistory)
        {
            int indexOfChanges = -1;
            DataTablesLogEntity lastInLog;
            if (indexOfChanges >= 0) {
                
            } else {
                lastInLog = dataTablesLog.Last<DataTablesLogEntity>();
                indexOfChanges = dataTablesLog.IndexOf(lastInLog);
            }
            lastInLog = dataTablesLog[indexOfChanges];
            var lastTable = lastInLog.dataTables.Last();

            var index = lastInLog.dataTables.IndexOf(lastTable);
            if (dataTablesLog.Count >= 2)
            {
                if (!isFromHistory)
                {
                    indexOfPrevLogEntity = dataTablesLog.Count - 2;
                }

                DataTable prevTable = null;
                if (indexOfPrevLogEntity >= 0)
                {
                    var prevInLog = dataTablesLog[indexOfPrevLogEntity];
                    if (index + 1 <= prevInLog.dataTables.Count)
                    {
                        prevTable = prevInLog.dataTables[index];
                    }
                }
                else {

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
                }
                else {
                    dgrv.DataSource = lastTable;
                    DatagridUtils.FormatDataGridView(lastInLog.dataTables.Count, dgrv);
                    wasHighlighted = true;
                }

            }else
            {
                wasHighlighted = true;
            }
            return wasHighlighted;
        }

        public void RemoveLastLogIfNotHighlighted()
        {
            if (!wasHighlighted)
            {
                RemoveLastLogItem();
            }
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
