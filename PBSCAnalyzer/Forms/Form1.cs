using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PBSCAnalyzer;
using PBSCAnalyzer.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
   
    public partial class Form1 : Form
    {
        private readonly SolutionTree _solutionTree;
        private WorkspaceListPanel _workspaceListPanel;
        private OpenedDocumentsPanel _openedDocumentsPanel;

        public Form1()
        {
            InitializeComponent();
            var a = App.Configuration.WorkSpaces;
            this.Closing+=OnClosing;
            
            TopLevel = true;
            this.Text = "PBSC Analyzer";            
            _solutionTree = new SolutionTree();
            _openedDocumentsPanel = new OpenedDocumentsPanel();
            _solutionTree.Show(MainDockPanel, DockState.DockLeft);
            _solutionTree.treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            _openedDocumentsPanel.treeView1.NodeMouseClick += treeView1_NodeMouseClick;

            _workspaceListPanel = new WorkspaceListPanel();
            //_workspaceListPanel.Show(_solutionTree.FloatPane, DockAlignment.Bottom,20);
            _workspaceListPanel.Show(MainDockPanel, DockState.DockLeftAutoHide);
            _openedDocumentsPanel.Show(MainDockPanel, DockState.DockLeftAutoHide);
            _workspaceListPanel.SetWorkspaceList();

            MainEngine.Instance.SolutionTree = _solutionTree;
            MainEngine.Instance.OpenedDocumentsPanel = _openedDocumentsPanel;
            MainEngine.Instance.MainDockPanel = MainDockPanel;
            MainEngine.Instance.MainForm = this;
            MainEngine.Instance.LoadWorkSpace(App.Configuration.CurrentWorkSpaceName);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            MainEngine.Instance.CloseInBatch = true;            
            MainEngine.Instance.SaveWorkSpace();
            cancelEventArgs.Cancel = false;            
        }

        private void selectPBWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            return;
            var openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "PBW|*.PBW";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                const Int32 BufferSize = 128;
                string fileName = openFileDialog.FileName;
                using (FileStream fileStream = File.OpenRead(fileName))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                    {
                        String line;
                        while ((line = streamReader.ReadLine()) != null) { Debug.WriteLine(line); }
                    }
                }
            }
        }

        private void selectSourceDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath =@"\Work\Patrix\source\";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                MainEngine.Instance.AddSourceFolderToWorkspace(selectedPath);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.X > 40) { MainEngine.Instance.SolutionItemClicked(e.Node); }
        }
        
        private void dockPanel1_ActiveContentChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MainEngine.Instance.OpenNewNote(true);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SourceContainerDocument activeSourceContainerDocument = MainEngine.Instance.GetActiveSourceContainerDocument();
            activeSourceContainerDocument.SourceEditorPanel.fastColoredTextBox1.Font = new Font("Consolas", 9.75f, FontStyle.Regular);
                //            fastColoredTextBox1.Font = new Font("Consolas", 9.25f, FontStyle.Regular);
//            fastColoredTextBox1.Font = new Font("Consolas", 10f, FontStyle.Regular);
//            fastColoredTextBox1.Font = new Font("Arial", 10f, FontStyle.Regular);
//            fastColoredTextBox1.Font = new Font("Source Code Pro-Regular", 10f, FontStyle.Regular);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SourceContainerDocument activeSourceContainerDocument = MainEngine.Instance.GetActiveSourceContainerDocument();
            activeSourceContainerDocument.SourceEditorPanel.fastColoredTextBox1.Font = new Font("Arial", 10f, FontStyle.Regular);
        }

        private void clearSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Remove all source pathes?", "Are You Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MainEngine.Instance.RemoveSourcePathes();
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //if (MessageBox.Show(this, "Close current Workspace and create a new one?", "Are You Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                InputMessageForm inputMessageForm = new InputMessageForm();
                if (inputMessageForm.ShowDialog() == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(inputMessageForm.textBox1.Text)) { return;}
                    MainEngine.Instance.SaveWorkSpace();
                    if (inputMessageForm.checkBox1.Checked == false)
                    {
                        MainEngine.Instance.RemoveSourcePathes();
                        MainEngine.Instance.CreateWorkspace(inputMessageForm.textBox1.Text);
                    }
                    else
                    { MainEngine.Instance.CloneWorkspace(inputMessageForm.textBox1.Text); }
                    if (inputMessageForm.checkBox2.Checked == true) MainEngine.Instance.CloseAllDocuments();
                    MainEngine.Instance.SaveWorkSpace();                    
                    _workspaceListPanel.SetWorkspaceList();
                    MainEngine.Instance.SetMainFormCaption();
                }
            }
        }
      
        private void reloadOpenedFilesToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Reload opened documents?", "Reload Documetns", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                MainEngine.Instance.ReloadAllDocuments();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
             MainEngine.Instance.SaveWorkSpace();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Close all documents?", "Close Documents", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                MainEngine.Instance.CloseAllDocuments();
                MainEngine.Instance.OpenedDocumentsPanel.RefreshOpenedDocumentsList();
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fontDialog = new FontDialog();
            fontDialog.Font = App.Configuration.SourceEditorFont;
            fontDialog.FixedPitchOnly = true;
            fontDialog.ShowApply = true;
            fontDialog.Apply+=FontDialogOnApply;
            var dialogResult = fontDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                FontDialogOnApply(fontDialog, null);
//                App.Configuration.SourceEditorFont = fontDialog.Font;
//                MainEngine.Instance.ApplyFonts();
            }            
        }

        private void FontDialogOnApply(object sender, EventArgs eventArgs)
        {
            App.Configuration.SourceEditorFont = (sender as FontDialog).Font;
            MainEngine.Instance.ApplyFonts();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
             AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            string datawindowName = Clipboard.GetText();
            if (string.IsNullOrEmpty(datawindowName))
            {
                MessageBox.Show("Clipboard is empty!", "Error!");
                return;
            }
            //            if (Regex.Match(datawindowName, @"^\b[a-zA-Z0-9_]+\b$").Success == false)
            //            {
            //                MessageBox.Show("Should be one word!","Error!");
            //                return;
            //            }
            string pathToDw = @"c:\development\AZU\debubpb\" + datawindowName + ".xml";
            string dwTemplate = datawindowName + ".saveas(\"" + pathToDw + "\", xml!, true)";

            //string pathToDw = @"c:\development\AZU\debubpb\" + datawindowName + ".xls";
            //string dwTemplate = datawindowName + ".saveas(\"" + pathToDw + "\", Excel!, true)";
            string result = dwTemplate;
            Clipboard.SetText(result);
            ShowDebugDWPanel(pathToDw);
            //MessageBox.Show(""+result,"-= Success! =-");
        }

        private DebugDWPanel _panel;
        private DebugDWContainer debugContainer;
        private void ShowDebugDWPanel(string file)
        {
            if (debugContainer == null)
            {
                debugContainer = new DebugDWContainer();
            }

            debugContainer.ShowOrSwitchPanel(file);
            MainEngine.Instance.ShowPanel(debugContainer);
                       
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            MainEngine.Instance.SaveWorkSpace();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string datawindowName = Clipboard.GetText();
            if (string.IsNullOrEmpty(datawindowName))
            {
                MessageBox.Show("Clipboard is empty!", "Error!");
                return;
            }
            //            if (Regex.Match(datawindowName, @"^\b[a-zA-Z0-9_]+\b$").Success == false)
            //            {
            //                MessageBox.Show("Should be one word!","Error!");
            //                return;
            //            }
            //string pathToDw = @"c:\development\AZU\debubpb\" + datawindowName + ".xml";
            //string dwTemplate = datawindowName + ".saveas(\"" + pathToDw + "\", xml!, true)";

            string pathToDw = @"c:\development\AZU\debubpb\" + datawindowName + ".xls";
            string dwTemplate = datawindowName + ".saveas(\"" + pathToDw + "\", Excel!, true)";
            string result = dwTemplate;
            Clipboard.SetText(result);
            //ShowDebugDWPanel(pathToDw);
            MessageBox.Show("" + result, "-= Success! =-");
        }
    }
}