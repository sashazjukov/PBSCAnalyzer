using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using PBSCAnalyzer.Forms;
using PBSCAnalyzer.types;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public partial class SourceContainerDocument : DockContent
    {
        private readonly FindInSourcePanel _findInSourcePanel;
        private readonly ObjectExplorerPanel _objectExplorerPanel;
        public SourceEditorPanel SourceEditorPanel;
        private SourceEditorPanel _sqlPanel;
        private SqlResultPanel _sqlResultPanel;

        public SourceContainerDocument()
        {
            InitializeComponent();
            //this.Closing+=OnClosing;            
            this.FormClosing += SourceContainerDocument_FormClosing; ;            
            TopLevel = true;
            SourceEditorPanel = new SourceEditorPanel();
            _findInSourcePanel = new FindInSourcePanel();
            _objectExplorerPanel = new ObjectExplorerPanel();
            _sqlResultPanel = new SqlResultPanel();

            _findInSourcePanel.SourceEditorPanel = SourceEditorPanel;
            SourceEditorPanel.FindInSourcePanel = _findInSourcePanel;
            SourceEditorPanel.SourceContainerDocument = this;
            SourceEditorPanel.MainSourceHolder = true;
            _objectExplorerPanel.SourceContainerDocument = this;
            _sqlResultPanel.SourceContainerDocument = this;

            SourceEditorPanel.Show(dockPanel1);
            _findInSourcePanel.Show(dockPanel1, DockState.DockBottomAutoHide);
            _sqlResultPanel.Show(dockPanel1, DockState.DockBottomAutoHide);            
            _objectExplorerPanel.Show(dockPanel1, DockState.DockRight);
            this.Activated += SourceContainerDocument_Activated;
        }

        private void SourceContainerDocument_Activated(object sender, EventArgs e)
        {            
            if (SourceEditorPanel != null) SourceEditorPanel.CreateUserSnippetsMenus();
            if (_sqlPanel != null) _sqlPanel.CreateUserSnippetsMenus();
        }

        private void SourceContainerDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            SourceEditorPanel.FileClass.IsOpened = false;
            SourceEditorPanel.FileClass.TextState = ETextState.UnRead;

            if (e.CloseReason != CloseReason.UserClosing) return;
            if (MainEngine.Instance.CloseInBatch == false)
            {
                MainEngine.Instance.OpenedDocumentsPanel.RefreshOpenedDocumentsList();
                if (App.Configuration.SaveOnCloseOpenDocument)
                {
                    MainEngine.Instance.SaveWorkSpace();
                }
            }
            
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            SourceEditorPanel.FileClass.IsOpened = false;
            SourceEditorPanel.FileClass.TextState = ETextState.UnRead;
            
            if (MainEngine.Instance.CloseInBatch == false)
            {                
                MainEngine.Instance.OpenedDocumentsPanel.RefreshOpenedDocumentsList();
                if (App.Configuration.SaveOnCloseOpenDocument)
                {
                    MainEngine.Instance.SaveWorkSpace();
                }
            }
        }

        public SourceEditorPanel SqlPanel { get; set; }
        public FileClass FileClass { get; set; }

        private void ShowSourceEditorPanel()
        {
            MyDockHelper.MakePanelVisible(SourceEditorPanel);
        }

        public void ShowObjectExplorerPanel()
        {
            MyDockHelper.MakePanelVisible(_objectExplorerPanel);
        }

        public void ShowFindPanel()
        {
            MyDockHelper.MakePanelVisible(_findInSourcePanel);
        }

        public void AnalyzeSource()
        {
            if (SourceEditorPanel.fastColoredTextBox1.Lines[0].Contains(".srd"))
            {
                FileClass.IsSql = true;
                CreateSqlPanel();
                _objectExplorerPanel.SetObjectsList(this.FileClass);
            }
            else
            {
               // CreateAnalyzerPanell();
                MainEngine.Instance.AnalayzeFileForTypes(this.FileClass);
                _objectExplorerPanel.SetObjectsList(this.FileClass);
            }
        }

        private void CreateSqlPanel()
        {
            if (_sqlPanel != null) return;
            _sqlPanel = SqlPanel = new SourceEditorPanel();
            SqlPanel.SourceContainerDocument = this;
            SqlPanel.Text = "SQL";
            SqlPanel.SetSourceFileClass(FileClass);
            string sqlText = SourceEditorPanel.fastColoredTextBox1.Text;
            sqlText = sqlText.Replace("~\"", "");
            Match result = Regex.Match(sqlText, @"(?<=retrieve=\"")([^\""\~]\n*)*(?=\""+?)");
            //var result = Regex.Match(sqlText, @"(?<=retrieve=\"")([^\""]\n*)*(?=\""+?)");
            sqlText = result.Value;
            if (string.IsNullOrEmpty(sqlText) || sqlText.ToUpper().Contains("PBSELECT")) { return; }
            SqlPanel.fastColoredTextBox1.Text = sqlText;
            SqlPanel.SetSyntaxSql();
            SqlPanel.Show(dockPanel1);
        }


        private void CreateAnalyzerPanell()
        {
            if (_analyzerPanel != null) return;
            _analyzerPanel = AnalyzerPanel = new SourceEditorPanel();
            AnalyzerPanel.SourceContainerDocument = this;
            AnalyzerPanel.Text = "Analyzer";
            AnalyzerPanel.SetSourceFileClass(FileClass);
            string sqlText = SourceEditorPanel.fastColoredTextBox1.Text;
            //Match result = Regex.Match(sqlText, @"(?<=retrieve=\"")([^\""\~]\n*)*(?=\""+?)");
            ////var result = Regex.Match(sqlText, @"(?<=retrieve=\"")([^\""]\n*)*(?=\""+?)");
            //sqlText = result.Value;
            if (string.IsNullOrEmpty(sqlText)) { return; }
            AnalyzerPanel.fastColoredTextBox1.Text = "test";
            AnalyzerPanel.SetSyntaxSql();
            AnalyzerPanel.Show(dockPanel1,DockState.DockRight);
        }

        public SourceEditorPanel AnalyzerPanel {
            get { return _analyzerPanel; } 
            set { _analyzerPanel = value; }
        }

        private SourceEditorPanel CreateOrSwitchEditFunctionPanel(FilePositionItem filePositionItem)
        {
            SourceEditorPanel panel = null;
            panel = EditFunctionPaels.Where(x => x.FilePositionItem == filePositionItem).FirstOrDefault();

            if (panel == null)
            {
                try
                {
                    panel = new SourceEditorPanel();
                    panel.IsEditFunctionType = true;
                    panel.SourceContainerDocument = this;
                    panel.Text = filePositionItem.Name;
                    panel.FilePositionItem = filePositionItem;
                    panel.SetSourceFileClass(FileClass);
                    string text = SourceEditorPanel.fastColoredTextBox1.GetRange(new Place(0, filePositionItem.LineNumberStart), new Place(0, filePositionItem.LineNumberEnd)).Text;
                    panel.fastColoredTextBox1.Text = text;
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
                if (panel != null) { EditFunctionPaels.Add(panel); }
            }

            if (panel != null) { panel.Show(dockPanel1); }
           return panel;
        }

        public void SetSourceFileClass(FileClass fileClass)
        {
            FileClass = fileClass;
            Text = fileClass.Name;
            ToolTipText = fileClass.FilePath;
            SourceEditorPanel.SetSourceFileClass(FileClass);
            SourceEditorPanel.fastColoredTextBox1.Text = fileClass.Text;
            if (fileClass.IsSql)
            {
                SqlPanel = this.SourceEditorPanel;
            }
            //AnalyzeSource();
        }

        public void SetFindInFileResult(List<SearchInFileLineItem> findResult)
        {
            _findInSourcePanel.SetFindInFileResult(findResult);
        }

        public void ObjecExplorerNodeClicked(TreeNode node)
        {
            int lineNumber = 0;
            var powerBuilderFileType = (node.Tag as PowerBuilderFileType);
            if (powerBuilderFileType != null) { lineNumber = powerBuilderFileType.LineNumberStart; }
            else
            {
                var filePositionItem = (node.Tag as FilePositionItem);
                if (filePositionItem != null && (filePositionItem.ItemType != "sqlArgument"))
                {
                    lineNumber = filePositionItem.LineNumberStart;
                }
            }
            if (lineNumber > 1) { SourceEditorPanel.NavigateToSourceLine(lineNumber, true); }

        }

        public void HighlightObjectInObjectExplorer(int fromLine)
        {
            _objectExplorerPanel.HighlightObjectByLine(fromLine);
        }

        public void FindTextAllInSource(string textToSearch)
        {
            _findInSourcePanel.SetSeachableText(textToSearch, false);
            this.ShowFindPanel();
            SourceEditorPanel.FindTextAll(textToSearch);
        }

        public void SetFont()
        {
            SourceEditorPanel.SetFont();
            _findInSourcePanel.SetFont();
        }

        public void ObjecExplorerNodeDoubleClicked(TreeNode node)
        {
            int lineNumber = 0;
            var powerBuilderFileType = (node.Tag as PowerBuilderFileType);
            if (powerBuilderFileType != null)
            {
                //lineNumber = powerBuilderFileType.LineNumberStart;
            }
            else
            {
                var filePositionItem = (node.Tag as FilePositionItem);
                if (filePositionItem != null)
                {
                    SourceEditorPanel editFunctionPanel = this.CreateOrSwitchEditFunctionPanel(filePositionItem);                    
                }
            }            
        }

        public List<SourceEditorPanel> EditFunctionPaels = new List<SourceEditorPanel>();
        private SourceEditorPanel _analyzerPanel;
        private string _last_ProcessedSqlText;

        public void ReloadDocumentContent()
        {
            MainEngine.Instance.ReloadDocumentContent(this, FileClass);
        }

        public void CopySql()
        {
            if (SqlPanel != null)
            {
                var result = GetProcessedSqlText(SqlPanel.fastColoredTextBox1.Text);
                Clipboard.SetText(result);
            }
        }

        public string GetProcessedSqlText(string result)
        {           
            List<FilePositionItem> list = _objectExplorerPanel.GetFilePositionItems();
            var filePositionItems = list.Where(x => x.ItemType == "sqlArgument").ToList();
            filePositionItems.ForEach((item) =>
            {
                string argValue = (item.NameType == "number" ? item.ArgumentValue : "'" + item.ArgumentValue + "'");
                if (item.ArgumentValue == "null")
                {
                    argValue = "null";
                }

                argValue += "/*:" + item.Name + "*/";
                result = Regex.Replace(result, ":" + item.Name, argValue, RegexOptions.IgnoreCase);
            });
            return result;
        }

        public void ExecuteSql(bool useLastSql = false)
        {
            MyDockHelper.MakePanelVisible(_sqlResultPanel);

            // last sql is for Transponse only
            if (!useLastSql || String.IsNullOrEmpty(_last_ProcessedSqlText))
            {
                string text;
                if (SqlPanel != null)
                {
                    text = string.IsNullOrEmpty(SqlPanel.fastColoredTextBox1.SelectedText) ? SqlPanel.fastColoredTextBox1.Text : SqlPanel.fastColoredTextBox1.SelectedText;
                }
                else
                {
                    text = SourceEditorPanel.fastColoredTextBox1.SelectedText;
                }
                if (string.IsNullOrEmpty(text))
                {
                    MessageBox.Show(this, $@"No SQL query is selected, or not on the SQL tab.", "Execute SQL", MessageBoxButtons.OK);
                    return;
                }
                _last_ProcessedSqlText = GetProcessedSqlText(text);
            }
            FileClass.Name = MarkText(FileClass.Name);
            this.Text = FileClass.Name;
            _sqlResultPanel.ExecuteSql(_last_ProcessedSqlText);
        }

        private string MarkText(string text)
        {
            if (text.LastIndexOf(" ^",StringComparison.InvariantCulture) <= 0)
            {
                return text + " ^";
            }
            return text;
        }

        public void ExecuteSqlSelection()
        {
            ExecuteSql(false);
        }
    }
}