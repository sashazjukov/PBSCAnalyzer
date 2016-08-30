using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public partial class FindInSourcePanel : DockContent
    {
        
        private string _searchedText;

        public FindInSourcePanel()
        {
            InitializeComponent();        
            SearchResult_fctb.VisibleRangeChanged+=SearchResultFctbOnVisibleRangeChanged;            
            SearchResult_fctb.TextChangedDelayed+=SearchResultFctbOnTextChangedDelayed;
            SearchResult_fctb.CurrentLineColor = Color.FromArgb(102, 63, 208, 86);
            SearchResult_fctb.SelectionChanged+=SearchResultFctbOnSelectionChanged;
            SetFont();
        }

        public void SetFont()
        {
            SearchResult_fctb.Font = App.Configuration.SourceEditorFont;
        }

        private void SearchResultFctbOnSelectionChanged(object sender, EventArgs eventArgs)
        {
              var tb = sender as FastColoredTextBox;
            //remember last visit time
            if (tb.Selection.IsEmpty && tb.Selection.Start.iLine < tb.LinesCount)
            {
                SourceEditorPanel.fastColoredTextBox1.Navigate(tb.Selection.Start.iLine);
                var lineText = SearchResult_fctb.Lines[tb.Selection.Start.iLine];
                Match result = Regex.Match(lineText, @"([0-9]+)");
                if (!string.IsNullOrEmpty(result.Value)) { SourceEditorPanel.fastColoredTextBox1.Navigate(Convert.ToInt32(result.Value)); }

//                if (lastNavigatedDateTime != tb[tb.Selection.Start.iLine].LastVisit)
//                {
//                    tb[tb.Selection.Start.iLine].LastVisit = DateTime.Now;
//                    lastNavigatedDateTime = tb[tb.Selection.Start.iLine].LastVisit;
//                }
            }
        }

        public SourceEditorPanel SourceEditorPanel { get; set; }

        private void SearchResultFctbOnTextChangedDelayed(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            HighlightEngine.SetSourceRules(sender, false, SourceEditorPanel.FileClass);
            HighlightEngine.SetSourceRulesHighlightRegex(sender,_searchedText);
        }

        private void SearchResultFctbOnVisibleRangeChanged(object sender, EventArgs eventArgs)
        {
            
        }

        public void SetSeachableText(string searchedText, bool initiateSearch = false)
        {
            if (searchedText != _searchedText)
            {
                _searchedText = searchedText;
                comboBox1.Text = searchedText;
                AddToHistoryList(searchedText);
            }
            if (initiateSearch)
            {
                SearchInSource(searchedText);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {                
                SearchInSource(comboBox1.Text);
            }
            if (e.KeyCode == Keys.F12)
            {
                try {
                var reslt = new StringBuilder();
                _searchedText = comboBox1.Text;
                if (false == comboBox1.Items.Contains(_searchedText)) { comboBox1.Items.Add(_searchedText); }
                string text = SourceEditorPanel.fastColoredTextBox1.Text;
                MatchCollection matchCollection = Regex.Matches(text,_searchedText,RegexOptions.ExplicitCapture,TimeSpan.FromSeconds(5));
                reslt.Append(_searchedText + "\r\n");
                    reslt.Append("Total groups: "+matchCollection.Count+"\r\n");
                foreach (Match item in matchCollection)
                {
                    if (item.Success)
                    {
                        List<Group> resGroup = new List<Group>();
                        foreach (Group @group in item.Groups)
                        {
                            resGroup.Add(group);
                        }
                        foreach (Group group in resGroup.OrderBy(x=>x.Index))
                        {
                            string format = $"{@group.Index} - {@group.Value}\r\n";
                            reslt.Append(format);
                        }
                        reslt.Append("------------------------------------------------------------------------" + "\r\n");
                    }
                }
                SearchResult_fctb.Text = reslt.ToString();
                }
                catch (Exception ex) {
                    SearchResult_fctb.Text = ex.ToString();

                }
            }
        }

        private List<string> searchHistoryList = new List<string>();

        private void SearchInSource(string searchedText)
        {
            _searchedText = searchedText;
            AddToHistoryList(_searchedText);

            SourceEditorPanel.FindTextAll(_searchedText);
        }

        private void AddToHistoryList(string searchedText)
        {
            if (false == searchHistoryList.Contains(searchedText) && false == string.IsNullOrEmpty(searchedText))
            {
                 searchHistoryList.Add(searchedText);
                comboBox1.Items.Add(searchedText);
                listBox1.Items.Add(searchedText);
            }
        }

        public void SetFindInFileResult(List<SearchInFileLineItem> findResult)
        {
            SearchResult_fctb.Text = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            findResult.ForEach(s => stringBuilder.Append($"{s.LineNum} - {s.TextLine}\t\n"));
            SearchResult_fctb.Text = stringBuilder.ToString();
            var searchInFileLineItem = findResult.FirstOrDefault();
            if (searchInFileLineItem != null) { SetSeachableText(searchInFileLineItem.SearchInFileCriteria.searchedText); }
        }

        private void SearchResult_fctb_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            SearchInSource((string) listBox1.SelectedItem);
        }
    }
}
