using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer.Forms
{
    public partial class WorkspaceListPanel : DockContent
    {
        public WorkspaceListPanel()
        {
            InitializeComponent();
            listBox1.DisplayMember = "Name";
        }

        public void SetWorkspaceList()
        {
            listBox1.Items.Clear();
            App.Configuration.WorkSpaces.OrderBy(x=>x.Name).ToList().ForEach(e => { listBox1.Items.Add(e); });            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //if (MessageBox.Show("Open workspace?"))
            MainEngine.Instance.SaveWorkSpace();
            WorkSpaceItem workSpaceItem = listBox1.SelectedItem as WorkSpaceItem;
            if (workSpaceItem != null) { MainEngine.Instance.LoadWorkSpace(workSpaceItem.Name); }
        }
    }
}
