using System;
using System.Data;
using System.Windows.Forms;

namespace PBSCAnalyzer.Forms
{
    public static class DatagridUtils {
        public static void FormatDataGridView(DataGridView dgrv)
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

        public static DataTable TransposeDataTable(DataTable dt)
        {
            DataTable transposedTable = new DataTable();

            DataColumn firstColumn = new DataColumn("Column Name");
            transposedTable.Columns.Add(firstColumn);

            //Add a column for each row in first data table
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataColumn dc = new DataColumn((i+1).ToString());//dt.Rows[i][0].ToString()
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