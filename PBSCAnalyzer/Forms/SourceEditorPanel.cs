using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using PBSCAnalyzer.types;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public partial class SourceEditorPanel : DockContent
    {
        //styles        
        private DateTime lastNavigatedDateTime = DateTime.Now;
        public FileClass FileClass;

        public SourceEditorPanel()
        {
            InitializeComponent();
//            _fastColoredTextBox.Anchor = AnchorStyles.Bottom & AnchorStyles.Left & AnchorStyles.Right & AnchorStyles.Top;
//            _fastColoredTextBox.Dock = DockStyle.Fill;
            fastColoredTextBox1.Language= Language.Custom;
            fastColoredTextBox1.TextChangedDelayed += OnTextChangedDelayed;
            fastColoredTextBox1.VisibleRangeChangedDelayed += FastColoredTextBoxOnVisibleRangeChangedDelayed;
            fastColoredTextBox1.VisibleRangeChanged += FastColoredTextBoxOnVisibleRangeChanged;
            fastColoredTextBox1.SelectionChanged += FastColoredTextBoxOnSelectionChanged;
            fastColoredTextBox1.SelectionChangedDelayed += FastColoredTextBox1OnSelectionChangedDelayed;
            fastColoredTextBox1.HighlightingRangeType = HighlightingRangeType.VisibleRange;
            fastColoredTextBox1.CurrentLineColor = Color.FromArgb(50, 150, 150, 150);
            //fastColoredTextBox1.BackColor = Color.FromArgb(39, 238, 238, 238);
            fastColoredTextBox1.CaretBlinking = true;
            fastColoredTextBox1.ShowFoldingLines = true;
            
            //User Commands
            App.UserCommandsConfiguration.UserCommands.ForEach(x =>
                                                   {
                                                       var toolStripItem = this.toolStrip1.Items.Add(x.ComandCaption);
                                                       toolStripItem.Tag = x;                                                       
                                                       toolStripItem.Click+=UserComnadToolStripItemOnClick;
                                                   });

            this.toolStrip1.Items.Add(new ToolStripSeparator());

            // User Text Snippets
            CreateUserSnippetsMenus();
            
            //fastColoredTextBox1.Styles.
            //            fastColoredTextBox1.Font = new Font("Microsoft Sans Serif", 10f,FontStyle.Regular);

            this.SetFont();
            
            //fastColoredTextBox1.Font = SourceFileStylesClass.SourceEditorFont;
            
            //            fastColoredTextBox1.Font = new Font("Consolas", 9.25f, FontStyle.Regular);
            //            fastColoredTextBox1.Font = new Font("Consolas", 10f, FontStyle.Regular);
            //            fastColoredTextBox1.Font = new Font("Arial", 10f, FontStyle.Regular);
            //            fastColoredTextBox1.Font = new Font("Source Code Pro-Regular", 10f, FontStyle.Regular);
            //fastColoredTextBox1.Font = new Font(FontFamily.GenericMonospace, 10);
            //fastColoredTextBox1.ShowFoldingLines = true;            
            //fastColoredTextBox1.DelayedEventsInterval = 100;
        }        

        List<ToolStripDropDownButton> tempTollstripItems = new List<ToolStripDropDownButton>();
        private int _totalUsersnippets = -1;

        public void CreateUserSnippetsMenus()
        {
            if (App.TotalUsersnippets == _totalUsersnippets) return;

            _totalUsersnippets = 0;
            if (tempTollstripItems.Count > 0)
            {
                tempTollstripItems.ForEach(x =>
                {
                    x.HideDropDown();
                    toolStrip1.Items.Remove(x);
                });
                tempTollstripItems.Clear();
            }

            App.UserSnippetConfiguration.UserSnippets.ForEach(x =>
            {
                ToolStripDropDownButton ts = new ToolStripDropDownButton();
                ts.AutoToolTip = true;
                ts.Text = x.Caption;
                ts.DisplayStyle = ToolStripItemDisplayStyle.Text;

                tempTollstripItems.Add(ts);
                var toolStripItem = this.toolStrip1.Items.Add(ts);
                _totalUsersnippets++;
                ProcessToolStripSubItems(ts, x);
            });
            App.TotalUsersnippets = _totalUsersnippets;
        }

        private void ProcessToolStripSubItems(ToolStripDropDownButton ts, UserTextSnippet x)
        {
            x.SubSnippets.ForEach(n =>
                {
                    if (n.IsMenu)
                    {
                        ToolStripDropDownButton subts = new ToolStripDropDownButton();
                        subts.AutoToolTip = true;
                        subts.Text = n.Caption;
                        subts.Tag = n;
                        subts.MouseUp += ToolStripItem_MouseUp;
                        ts.DropDownItems.Add(subts);
                        n.ParentUserTextSnippet = x;
                        ProcessToolStripSubItems(subts, n);
                    }
                    else
                    {
                        var toolStripItem = ts.DropDownItems.Add(n.Caption.Length == 0 ? n.Text : n.Caption);
                        toolStripItem.Tag = n;
                        toolStripItem.ToolTipText = n.Text;
                        //toolStripItem.Click += TextSnippetToolStripItemOnClick;
                        toolStripItem.MouseEnter += ToolStripItem_MouseEnter;
                        toolStripItem.MouseUp += ToolStripItem_MouseUp;
                        n.ParentUserTextSnippet = x;
                    }
                    _totalUsersnippets++;
                }
            );
            var toolStripItemAdd = ts.DropDownItems.Add("{Add}");
            toolStripItemAdd.Tag = x;
            toolStripItemAdd.ToolTipText = "Left Click - Add selected/(from clipboard) text as a new snippet!" +
                                           "\r\n" + "Right click - Add a new sub menu with a caption = selected/(from clipboard) text" +
                                           "\r\n" + "If text = '-' then insert a separator";            
            toolStripItemAdd.MouseUp += ToolStripItemAdd_MouseUp;
            toolStripItemAdd.MouseEnter += ToolStripItemAdd_MouseEnter;
        }

        private void ToolStripItemAdd_MouseUp(object sender, MouseEventArgs e)
        {
            var toolStripItem = sender as ToolStripItem;
            UserTextSnippet ddButtons = toolStripItem.Tag as UserTextSnippet;

            UserTextSnippet textSnippet = new UserTextSnippet();
            string text = fastColoredTextBox1.SelectedText;

            if (string.IsNullOrEmpty(text))
            {
                string ClipboardText = System.Windows.Forms.Clipboard.GetText();
                if (string.IsNullOrEmpty(ClipboardText))
                {
                    return;
                }
                else
                {
                    text = ClipboardText;
                }
            }

            textSnippet.Caption = text;
            
            textSnippet.ParentUserTextSnippet = ddButtons;

            if ((e.Button & MouseButtons.Right) != 0)
            {
                textSnippet.IsMenu = true;
                textSnippet.Text = "";
            }
            else
            {                              
                textSnippet.IsMenu = false;
                textSnippet.Text = text;
            }

            ddButtons.SubSnippets.Add(textSnippet);
            _totalUsersnippets = 0;
            CreateUserSnippetsMenus();
        }

        private void ToolStripItem_MouseUp(object sender, MouseEventArgs e)
        {
            var snd = sender as ToolStripDropDownButton;
            if (snd != null)
            {

            }

            if ((e.Button & MouseButtons.Right) != 0)
            {
                var toolStripItem = sender as ToolStripItem;
                UserTextSnippet userCommand = toolStripItem.Tag as UserTextSnippet;
                if (userCommand != null)
                {
                    if (MessageBox.Show(this, $@"Remove User snippet?", "Remove User snippet?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {

                        userCommand.ParentUserTextSnippet.SubSnippets.Remove(userCommand);                        
                    }
                    _totalUsersnippets = 0;
                    CreateUserSnippetsMenus();
                }                
            }
            else
            {
                var toolStripItem = sender as ToolStripItem;
                UserTextSnippet userCommand = toolStripItem.Tag as UserTextSnippet;

                if (!string.IsNullOrEmpty(userCommand.Text) && userCommand.Text != "-")
                {

                    string ClipboardText = System.Windows.Forms.Clipboard.GetText();
                    if (string.IsNullOrEmpty(ClipboardText))
                    {
                        ClipboardText = "";
                    }

                    string result = userCommand.Text.Replace("{1}", ClipboardText);
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        System.Windows.Forms.Clipboard.SetText(result);
                    }
                    else
                    {
                        fastColoredTextBox1.InsertText(result); //"\r\n"
                    }
                }
            }

        }       

        private void ToolStripItemAdd_MouseEnter(object sender, EventArgs e)
        {
            
        }

        private void ToolStripItem_MouseEnter(object sender, EventArgs e)
        {
            var toolStripItem = sender as ToolStripItem;
            UserTextSnippet userCommand = toolStripItem.Tag as UserTextSnippet;

            string ClipboardText = System.Windows.Forms.Clipboard.GetText();
            if (string.IsNullOrEmpty(ClipboardText))
            {
                ClipboardText = "";
            }

            string result = userCommand.Text.Replace("{1}", ClipboardText);
            toolStripItem.ToolTipText = result;
        }
       

        private void UserComnadToolStripItemOnClick(object sender, EventArgs eventArgs)
        {
            var toolStripItem = sender as ToolStripItem;
            var userCommand = toolStripItem.Tag as UserCommand;
            var pathWithoutName = Path.GetFullPath(this.FileClass.FilePath);
            ExecuteUserCommand(userCommand,pathWithoutName, this.FileClass.FilePath, this.FileClass.FileName);
        }

        private void ExecuteUserCommand(UserCommand userCommand, string pathWithoutName, string filePath, string fileName)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                var comandScript = "/C " + userCommand.ComandScript;
                comandScript = comandScript.Replace("%FileName%", fileName);
                comandScript = comandScript.Replace("%FilePath%", pathWithoutName);
                comandScript = comandScript.Replace("%FilePathName%", filePath);
                comandScript = comandScript.Replace("%LineNum%", (fastColoredTextBox1.Selection.Start.iLine + 1).ToString());
                startInfo.Arguments = comandScript;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error running user command: " + userCommand.ComandCaption);
            }
        }

        private void FastColoredTextBox1OnSelectionChangedDelayed(object sender, EventArgs eventArgs)
        {
            HighlightSelected(sender);

            if (_isdisableHighlightingObjectOnce == true)
            {
                _isdisableHighlightingObjectOnce = false;
                return;
            }
            if (IsSqlSyntax == false) { SourceContainerDocument.HighlightObjectInObjectExplorer(fastColoredTextBox1.Selection.FromLine); }
        }

        private bool _isdisableHighlightingObjectOnce;

        public FindInSourcePanel FindInSourcePanel { get; set; }

        private void FastColoredTextBoxOnVisibleRangeChanged(object sender, EventArgs e)
        {
           // SetSourceRules(sender);
        }

        private void OnTextChangedDelayed(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            
            if (MainSourceHolder)
            {
                FileClass.Text = fastColoredTextBox1.Text;
                FileClass.TextState = ETextState.Changed;
            }
            if (!IsDisableAnalization)
            {
                AnalyzeTypesInSource();               
            }
            IsSqlSyntax = IsSqlSyntax ? IsSqlSyntax : FileClass.FilePath.Contains(".sql") || FileClass.IsSql;
            HighlightEngine.SetSourceRules(sender, IsSqlSyntax, FileClass);
            if (IsSqlSyntax)
            {
                IsDisableAnalization = true;
            }

        }

        public bool IsDisableAnalization = false;

        private void AnalyzeTypesInSource()
        {
            if (IsEditFunctionType) return;
            SourceContainerDocument.AnalyzeSource();
        }

        public void NavigateToSourceLine(int lineNum, bool lb_top = false, bool lb_syncObjectExplorer = true)
        {
            this.Show();
            _isdisableHighlightingObjectOnce = true;
            fastColoredTextBox1.BeginUpdate();            
            fastColoredTextBox1.Navigate(lineNum);
            if (lb_top)
            {
                if (!IsSqlSyntax)
                {
                    fastColoredTextBox1.Navigate(fastColoredTextBox1.VisibleRange.End.iLine);
                    fastColoredTextBox1.Selection.Start = new Place(1, fastColoredTextBox1.VisibleRange.Start.iLine + 1);
                }
            }
            if (lb_syncObjectExplorer && IsSqlSyntax == false) { SourceContainerDocument.HighlightObjectInObjectExplorer(fastColoredTextBox1.Selection.FromLine); }
            fastColoredTextBox1.EndUpdate();
        }

        private void FastColoredTextBoxOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            //HighlightSelected(sender);
        }

        private void HighlightSelected(object sender)
        {
            var tb = sender as FastColoredTextBox;
            //remember last visit time
            if (tb.Selection.IsEmpty && tb.Selection.Start.iLine < tb.LinesCount)
            {
                if (lastNavigatedDateTime != tb[tb.Selection.Start.iLine].LastVisit)
                {
                    tb[tb.Selection.Start.iLine].LastVisit = DateTime.Now;
                    lastNavigatedDateTime = tb[tb.Selection.Start.iLine].LastVisit;
                }
            }

            //highlight same words
            Style wordsUnderCursorStyle;

            string text;
            if (!tb.Selection.IsEmpty)
            {
                wordsUnderCursorStyle = SourceFileStylesClass.sameWordsStyle;
                //user selected diapason
                text = tb.Selection.Text;
            }
            else
            {
                wordsUnderCursorStyle = SourceFileStylesClass.SameWordsStyle;
                //get fragment around caret
                Range fragment = tb.Selection.GetFragment(@"\w");
                text = fragment.Text;
            }
            var range = tb.Range;
            range.ClearStyle(wordsUnderCursorStyle);
            if (text.Length == 0) { return; }
            //highlight same words      
            try
            {
                Range[] ranges = range.GetRanges("\\b" + text + "\\b",RegexOptions.IgnoreCase).ToArray();
                if (ranges.Length > 1) { foreach (Range r in ranges) { r.SetStyle(wordsUnderCursorStyle); } }
            }
            catch (Exception) {
            }
            //Range activeeventFunctionRange = new Range(fastColoredTextBox1,);
            //range.SetStyle(SourceFileStylesClass.SqlCoulmnsBrown, @"[\n](?<range>event)\b(?<range>\.*?)[\n](?<range>end event)\b", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //@"[\n](?<range>event)\b", @"[\n](?<range>end event)\b"
        }

        private void FastColoredTextBoxOnVisibleRangeChangedDelayed(object sender, EventArgs eventArgs)
        {
            // SetSourceRules(sender);
        }

        public void FindTextAll(string textToSearch)
        {
            try
            {
                if (string.IsNullOrEmpty(textToSearch)) { return; }
                var searchInFileCriteria = new SearchInFileCriteria(textToSearch);
                var searchInFileLineItems = new List<SearchInFileLineItem>();
                List<int> findLines = fastColoredTextBox1.FindLines(textToSearch, RegexOptions.IgnoreCase |RegexOptions.Singleline);
                foreach (int findLine in findLines)
                {
                    var searchInFileLineItem = new SearchInFileLineItem();
                    searchInFileLineItem.TextLine = fastColoredTextBox1.Lines[findLine].Trim();
                    searchInFileLineItem.LineNum = findLine;
                    searchInFileLineItem.SearchInFileCriteria = searchInFileCriteria;
                    searchInFileLineItems.Add(searchInFileLineItem);
                }
                FindInSourcePanel.SetFindInFileResult(searchInFileLineItems);
            }
            catch (Exception ex)
            {
                
            }
        }

        public void SetSyntaxSql()
        {
            //fastColoredTextBox1.SyntaxHighlighter.SQLSyntaxHighlight(fastColoredTextBox1.Range);
            //fastColoredTextBox1.Language = Language.SQL;
            IsSqlSyntax = true;            
        }

        public bool IsSqlSyntax { get; set; }
        public SourceContainerDocument SourceContainerDocument { get; set; }
        public bool IsEditFunctionType { get; set; }
        public string EditFunctionName { get; set; }
        public FilePositionItem FilePositionItem { get; set; }
        public bool MainSourceHolder { get; set; }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {            
            SourceContainerDocument.FindTextAllInSource(fastColoredTextBox1.SelectedText);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MainEngine.Instance.FindFileName(fastColoredTextBox1.SelectedText);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            WordWrap_toolStripButton.Checked = !WordWrap_toolStripButton.Checked;
            fastColoredTextBox1.WordWrap = WordWrap_toolStripButton.Checked;
        }

        public void SetSourceFileClass(FileClass fileClass)
        {
            FileClass = fileClass;           
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            MainEngine.Instance.SelectedTextFindInAllFiles(fastColoredTextBox1.SelectedText);
        }

        public void SetFont()
        {
            fastColoredTextBox1.Font = App.Configuration.SourceEditorFont;            
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SourceContainerDocument.ReloadDocumentContent();
        }

        private void CopySql_Button_Click(object sender, EventArgs e)
        {
            SourceContainerDocument.CopySql();
        }

        public void tsb_ExecuteSql_Click(object sender, EventArgs e)
        {
            SourceContainerDocument.ExecuteSql();
        }

        private void fastColoredTextBox1_KeyPressed(object sender, KeyPressEventArgs e)
        {
           
        }

        private void fastColoredTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {            
        }

        private void fastColoredTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                SourceContainerDocument.ExecuteSqlSelection();
                e.Handled = true;
            }
            if ((e.Shift && e.Control)) {
                if (e.KeyCode == Keys.S)
                {
                    if (fastColoredTextBox1.SelectedText.Length > 0)
                    {
                        var start = fastColoredTextBox1.SelectionStart;
                        string res = "Select * FROM " + fastColoredTextBox1.SelectedText;
                        fastColoredTextBox1.SelectedText = res;
                        fastColoredTextBox1.SelectionStart = start;
                        fastColoredTextBox1.SelectionLength = res.Length;
                        //SourceContainerDocument.ExecuteSqlSelection();
                        e.Handled = true;
                    }
                }
                if (e.KeyCode == Keys.C)
                {
                    if (fastColoredTextBox1.SelectedText.Length > 0)
                    {
                        var start = fastColoredTextBox1.SelectionStart;
                        string res = "Select COUNT(*) FROM " + fastColoredTextBox1.SelectedText;
                        fastColoredTextBox1.SelectedText = res;
                        fastColoredTextBox1.SelectionStart = start;
                        fastColoredTextBox1.SelectionLength = res.Length;
                        //SourceContainerDocument.ExecuteSqlSelection();
                        e.Handled = true;
                    }
                }
            }
        }

                      
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            PoorMansTSqlFormatterLib.SqlFormattingManager fullFormatter = new PoorMansTSqlFormatterLib.SqlFormattingManager(new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter(new PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatterOptions {ExpandCommaLists = true }));
            var isSelected = fastColoredTextBox1.SelectedText.Length > 0;
            var selectedText = !isSelected ? fastColoredTextBox1.Text : fastColoredTextBox1.SelectedText;
            var result = fullFormatter.Format(selectedText);
            if (isSelected)
            {
                fastColoredTextBox1.SelectedText = result;
            }
            else
            {
                fastColoredTextBox1.Text = result;

            }
        }

        private void forPBCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = GetSelectedOrText();
            string[] lines = text.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );
            string result = "ls_sql = \" \" + &"+Environment.NewLine;
            foreach(var line in lines)
            {
                result += "\" " + line + " \" + &" + Environment.NewLine;
            }
            //result = result+"+\"\"";
            Clipboard.SetText(result);
        }

        private string GetSelectedOrText()
        {
            var isSelected = fastColoredTextBox1.SelectedText.Length > 0;
            var selectedText = !isSelected ? fastColoredTextBox1.Text : fastColoredTextBox1.SelectedText;
            
            return selectedText;
        }

        private void toolStripButton_copyPath_Click(object sender, EventArgs e)
        {
            
            Clipboard.SetText(FileClass.FilePath);
        }

        private void toolStripButton_count_Click(object sender, EventArgs e)
        {

        }
    }
}