using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public partial class SolutionTree : DockContent
    {
        private ESearchIn _eSearchIn;

        public SolutionTree()
        {
            InitializeComponent();
            //treeView1.Font = SourceFileStylesClass.SourceEditorFont;
            SetStatusText("Ready.");
        }

        public SolutionTreeDisplayType SolutionTreeDisplayType { get; set; }

        private void FindInSolutionTextBox_TextChanged(object sender, EventArgs e)
        {
            FindInSolutionTextBox.BackColor = Color.LightGray;
        }

        private void FilterSolutionTree(ESearchIn eSearchIn)
        {
            MainEngine tempQualifier = MainEngine.Instance;
            MainEngine.Instance.FilterSolutionTree(FindInSolutionTextBox.Text, eSearchIn, tempQualifier.SolutionTree.treeView1);
        }

        public void SetStatusText(string text)
        {
            toolStripStatusLabel1.Text = text;
            statusStrip1.Refresh();
        }

        private void FindInSolutionTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down) { MainEngine.Instance.SetFocusToTreeView(); }
            if (e.KeyCode == Keys.Enter) { FindInFileNames(); }
            if (e.KeyCode == Keys.F12) { FindInAllFiles(); }
            if (e.KeyCode == Keys.Escape) { ShowOpenedDocuments(); }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                MainEngine.Instance.SolutionItemClicked(treeView1.SelectedNode);
            }
            if (e.KeyCode == Keys.Right) { FindInSolutionTextBox.Focus(); }
            if (e.KeyCode == Keys.Left) { FindInSolutionTextBox.Focus(); }
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                FindInSolutionTextBox.Text = "";
                MainEngine.Instance.ResetSolutionFilter();
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void findInSource_toolStripButton_Click(object sender, EventArgs e)
        {
            FindInAllFiles();
        }

        private void FindInAllFiles()
        {
            AddToHistoryList(FindInSolutionTextBox.Text);
            _eSearchIn = ESearchIn.FileText;
            FilterSolutionTree(_eSearchIn);
            FindInSolutionTextBox.BackColor = Color.Bisque;
        }

        private void OpenedFiles_toolStripButton_Click(object sender, EventArgs e)
        {
            ShowOpenedDocuments();
        }

        private void ShowOpenedDocuments()
        {
            FindInSolutionTextBox.Text = "";
            //SolutionTreeDisplayType = SolutionTreeDisplayType.OpenedDocuments;
            _eSearchIn = ESearchIn.OpenedDocuments;
            FindInSolutionTextBox.BackColor = Color.White;
            FilterSolutionTree(_eSearchIn);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        public void SetSearchProgress(int value)
        {
            if (value == 0) { FindProgressBar.Visible = true; }
            FindProgressBar.Value = value;
            toolStrip1.Refresh();
            if (value == 100)
            {
                FindProgressBar.Value = 0;
                FindProgressBar.Visible = false;
                toolStrip1.Refresh();
            }
        }

        public void SetAndFindFile(string selectedText)
        {
            FindInSolutionTextBox.Text = selectedText;
            FindInFileNames();
        }

        private void FindInFileNames()
        {
            AddToHistoryList(FindInSolutionTextBox.Text);
            _eSearchIn = ESearchIn.FileName;
            FilterSolutionTree(_eSearchIn);
            FindInSolutionTextBox.BackColor = Color.White;
        }

        private void AddToHistoryList(string searchedText)
        {
            var currentWorkSpace = MainEngine.Instance.GetCurrentWorkSpace();
            if (false == currentWorkSpace.SolutionSearchHistoryList.Contains(searchedText) && false == string.IsNullOrEmpty(searchedText))
            {
                currentWorkSpace.SolutionSearchHistoryList.Add(searchedText);
                FindInSolutionTextBox.Items.Add(searchedText);
            }
        }

        public void SetSearchHistoryListBox()
        {
            var currentWorkSpace = MainEngine.Instance.GetCurrentWorkSpace();
            currentWorkSpace.SolutionSearchHistoryList.Clear();
            currentWorkSpace.SolutionSearchHistoryList.ForEach(e => { FindInSolutionTextBox.Items.Add(e); });
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FindInFileNames();
        }

        public void SetAndFindInAllFiles(string selectedText)
        {
            FindInSolutionTextBox.Text = selectedText;
            FindInAllFiles();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void tsb_export_search_result_Click(object sender, EventArgs e)
        {
            var newNote =  MainEngine.Instance.OpenNewNote(true);
            var reslt = new StringBuilder();
            FillNotePanelFromTree( treeView1.SelectedNode.Nodes, reslt);
            newNote.SourceEditorPanel.fastColoredTextBox1.Text = reslt.ToString();
        }

        private void FillNotePanelFromTree(TreeNodeCollection treeView1Nodes, StringBuilder reslt)
        {
            foreach (TreeNode treeView1Node in treeView1Nodes)
            {
                string outline = new String(' ', treeView1Node.Level);
                string txt = outline  + treeView1Node.Text;
                reslt.AppendLine(txt);
                if (treeView1Node.Nodes.Count != 0)
                {
                    FillNotePanelFromTree(treeView1Node.Nodes, reslt);
                }
            }
        }
    }
}