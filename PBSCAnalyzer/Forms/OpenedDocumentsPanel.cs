using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public partial class OpenedDocumentsPanel : DockContent
    {
        private ESearchIn _eSearchIn;

        public OpenedDocumentsPanel()
        {
            InitializeComponent();                        
        }

        public SolutionTreeDisplayType SolutionTreeDisplayType { get; set; }

        public void SetStatusText(string text)
        {
            toolStripStatusLabel1.Text = text;
            statusStrip1.Refresh();
        }


        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                MainEngine.Instance.SolutionItemClicked(treeView1.SelectedNode);
            }

            if (e.KeyCode == Keys.Escape) { e.SuppressKeyPress = true; }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        public void RefreshOpenedDocumentsList()
        {
            if (!MainEngine.Instance.IsLoadingWorkspase)
            {
                _eSearchIn = ESearchIn.OpenedDocuments;
                MainEngine.Instance.FilterSolutionTree("", _eSearchIn, treeView1);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            RefreshOpenedDocumentsList();
        }
    }
}