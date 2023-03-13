using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PBSCAnalyzer.Forms;
using PBSCAnalyzer.types;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public class MainEngine
    {
        private static MainEngine _instance;
        private bool IsSoultionFiltered;

        public List<PowerBuilderFileType> PowerBuilderTypes = new List<PowerBuilderFileType>();
        private List<RnsCountClass> _rnCounts;
                
        private int _currentProgress;
        private int _maxProgress;
        private int _totalFiind;
        private WaitForm _waitForm;

        public MainEngine()
        {
            GeneralTreeNodeCollection = new List<TreeNode>();
        }

        public static MainEngine Instance
        {
            get
            {
                if (_instance == null) { _instance = new MainEngine(); }
                return _instance;
            }
        }

        public SolutionTree SolutionTree { get; set; }
        public DockPanel MainDockPanel { get; set; }
        public Form1 MainForm { get; set; }
        public List<TreeNode> GeneralTreeNodeCollection { get; set; }
        public OpenedDocumentsPanel OpenedDocumentsPanel { get; set; }

        public void SolutionItemClicked(TreeNode node)
        {
            if (node.Tag is FileClass)
            {
                var fileClass = node.Tag as FileClass;
                OpenOrSwitchFile(fileClass);
                
                //Searched in file result in file nodes
                if (node.Nodes.Count > 0)
                {
                    var sourceContainerDocument = GetActiveSourceContainerDocument();
                    sourceContainerDocument.ShowFindPanel();
                    List<SearchInFileLineItem> findResult = NodesToList(node.Nodes).Select(s => s.Tag as SearchInFileLineItem).ToList();
                    sourceContainerDocument.SetFindInFileResult(findResult);
                }
            }
            if (node.Tag is SearchInFileLineItem)
            {
                var fileClass = node.Parent.Tag as FileClass;
                var searchInFileLineItem = node.Tag as SearchInFileLineItem;
                OpenOrSwitchFile(fileClass);
//                var sourceContainerDocument = GetActiveSourceContainerDocument();
//                sourceContainerDocument.ShowFindPanel();
//                sourceContainerDocument.SetFindInFileResult(findResult);
                NavigateToLineInSource(searchInFileLineItem);
            }
            
        }

        public SourceContainerDocument GetActiveSourceContainerDocument()
        {
            SourceContainerDocument sourceContainerDocument = MainForm.ActiveMdiChild as SourceContainerDocument;
            return sourceContainerDocument;
        }

        private IEnumerable<TreeNode> NodesToList(TreeNodeCollection nodes)
        {            
            foreach (TreeNode node in nodes) { yield return node; }
        }

        private void NavigateToLineInSource(SearchInFileLineItem searchInFileLineItem)
        {
            var doc = MainForm.ActiveMdiChild as SourceContainerDocument;
            doc.SourceEditorPanel.NavigateToSourceLine(searchInFileLineItem.LineNum);
        }

        private void OpenOrSwitchFile(FileClass fileClass, bool reloadFileContent = false)
        {
            if (fileClass != null)
            {
                SourceContainerDocument newDocument = null;
                
                DockPane orDefault = MainDockPanel.Panes.FirstOrDefault(x => x.Appearance == DockPane.AppearanceStyle.Document);
                if (orDefault != null)
                {
                    IDockContent firstOrDefault = orDefault.Contents.FirstOrDefault(s => (s is SourceContainerDocument) && ((SourceContainerDocument) s).FileClass.FilePath == fileClass.FilePath);
                    if (firstOrDefault != null) { newDocument = firstOrDefault as SourceContainerDocument; }
                }
                if (newDocument == null)
                {
                    if (fileClass.TextState == ETextState.UnRead)
                    {
                        fileClass.Text = ReadSourceFile(fileClass.FilePath);
                        fileClass.TextState = ETextState.ReadNotChanged;
                    }
                    fileClass.IsOpened = true;
                    newDocument = CreateNewDocument(fileClass);
                    OpenedDocumentsPanel.RefreshOpenedDocumentsList();
                    if (App.Configuration.SaveOnCloseOpenDocument)
                    {
                        if (MainEngine.Instance.IsLoadingWorkspase == false)
                        {
                            SaveWorkSpace();
                        }
                    }
                }
                else
                {
                    if (reloadFileContent)
                    {            
                        ReloadDocumentContent(newDocument,fileClass);
                    }
                }
                if (reloadFileContent == false) { ShowPanel(newDocument); }
                newDocument.SourceEditorPanel.fastColoredTextBox1.Focus();
            }
        }

        public void ReloadDocumentContent(SourceContainerDocument sourceContainerDocument, FileClass fileClass)
        {
            fileClass.Text = ReadSourceFile(fileClass.FilePath);
            fileClass.TextState = ETextState.ReadNotChanged;
            fileClass.IsOpened = true;
            sourceContainerDocument.SetSourceFileClass(fileClass);
        }

        public void ShowPanel(DockContent dummyDoc)
        {
                  dummyDoc.Show(MainDockPanel); 
        }

        private SourceContainerDocument CreateNewDocument(FileClass fileClass)
        {
            var sourceContainerDocument = new SourceContainerDocument();            
            sourceContainerDocument.SetSourceFileClass(fileClass);
            return sourceContainerDocument;
        }

        private string ReadSourceFile(string filePath)
        {
            var result = new StringBuilder();
            const Int32 BufferSize = 128;
            string fileName = filePath;
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null) { result.Append(line + "\r\n"); }
                }
            }
            return result.ToString();
        }

        public void FilterSolutionTree(string text, ESearchIn eSearchIn, TreeView treeView)
        {
            if (text.Length <= 2 && eSearchIn != ESearchIn.OpenedDocuments)
            {
                if (IsSoultionFiltered) { ResetSolutionFilter(); }
                return;
            }
            string lower = text.ToLower();
            IsSoultionFiltered = true;
            treeView.BeginUpdate();
            TreeNodeCollection treeNodeCollection = treeView.Nodes;
            treeNodeCollection.Clear();
            _totalFiind = 0;
            _currentProgress = 1;
            _maxProgress = 0;

            // GeneralTreeNodeCollection.ForEach(x => _maxProgress += x.Nodes.Count);
            SetSolutionTreeStatusText("Searching...");
            SetSearchProgress(0);

            var searchInFileCriteria = new SearchInFileCriteria(text);

            foreach (TreeNode treeNode in GeneralTreeNodeCollection)
            {

                var clonedNode = (TreeNode)treeNode.Clone();

                //treeNodeCollection.Add(clonedNode);
                //TreeNode parentfolderNode = clonedNode;

                ProcessFindInNodes(clonedNode, clonedNode.Nodes, eSearchIn, lower, text, searchInFileCriteria, null);

                SetSearchProgress((int)(((float)(_currentProgress) / _maxProgress) * 100));
                treeNodeCollection.Add(clonedNode);
            }

            SetSolutionTreeStatusText("Ready.");
            if (eSearchIn == ESearchIn.OpenedDocuments)
            {
                OpenedDocumentsPanel.SetStatusText($"Total: {_totalFiind}");
                //SetSolutionTreeStatusText($"Opened {_totalFiind} files");
            }
            else
            { if (eSearchIn == ESearchIn.FileName) { SetSolutionTreeStatusText($"{_totalFiind} file names match '{text}'"); } }
            { if (eSearchIn == ESearchIn.FileText) { SetSolutionTreeStatusText($"{_totalFiind} files containing '{text}'"); } }



            if (treeView.Nodes.Count > 0)
            {
                treeView.SelectedNode = treeView.Nodes[0];
            }
            treeView.EndUpdate();
        }

        private void ProcessFindInNodes(TreeNode folderNode, TreeNodeCollection treeNodeCollection, ESearchIn eSearchIn, string lower, string text, SearchInFileCriteria searchInFileCriteria, List<TreeNode> toRemoveParentFolder)
        {
            {
                //_currentProgress++;
                var toRemove = new List<TreeNode>();
                foreach (TreeNode fileNode in folderNode.Nodes)
                {
                    var fileClass = (fileNode.Tag as FileClass);
                    if (fileClass == null) { ProcessFindInNodes(fileNode, fileNode.Nodes, eSearchIn, lower, text, searchInFileCriteria, toRemove); }
                    else
                    {
                        
                        fileNode.Nodes.Clear();
                        bool contains = false;
                        switch (eSearchIn)
                        {
                            case ESearchIn.FileName:
                                //contains = (fileClass.Name ?? String.Empty).ToLower().Contains(lower);
                                Match match2 = Regex.Match((fileClass.Name ?? String.Empty), text, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                                contains = match2.Success;
                                if (!contains) { toRemove.Add(fileNode); }
                                else
                                { _totalFiind++; }
                                break;
                            case ESearchIn.FileText:
                                if (fileClass.TextState == ETextState.UnRead)
                                {
                                    fileClass.Text = ReadSourceFile(fileClass.FilePath);
                                    fileClass.TextState = ETextState.ReadNotChanged;
                                }
                                string source = fileClass.Text ?? String.Empty;
                                string[] result = Regex.Split(source, "\r\n");
                                int lineNum = 0;
                                int totalLinesFind = 0;
                                foreach (string line in result)
                                {
                                    Match match = Regex.Match(line, text, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
                                    //if (line.ToLower().Contains(text.ToLower()))
                                    if (match.Success)
                                    {
                                        string lineText = line.Trim();
                                        var node = new TreeNode() {Name = line, ImageIndex = 7, SelectedImageIndex = 7, Text = $"{lineText}", Tag = new SearchInFileLineItem() {LineNum = lineNum, TextLine = lineText, SearchInFileCriteria = searchInFileCriteria}};
                                        node.NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
                                        fileNode.Nodes.Add(node);
                                        totalLinesFind++;
                                        contains = true;
                                    }
                                    lineNum++;
                                }

                                if (!contains) { toRemove.Add(fileNode); }
                                else
                                {
                                    fileNode.Text = $"{fileClass.FileName} ({totalLinesFind})";
                                    SetFileNodeFontForSearch(fileNode);
                                    _totalFiind++;
                                }
                                break;
                            case ESearchIn.OpenedDocuments:
                                contains = fileClass.IsOpened;
                                if (!contains) { toRemove.Add(fileNode); }
                                else
                                { _totalFiind++; }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("eSearchIn");
                        }
                    }
                }
                if (folderNode != null) {
                    folderNode.Expand();
                    foreach (TreeNode node in toRemove) { folderNode.Nodes.Remove(node); }
                    if (folderNode.Nodes.Count == 0) { if (toRemoveParentFolder != null) { toRemoveParentFolder.Add(folderNode); } }
                }
            }
        }

        private void SetSolutionTreeStatusText(string s)
        {
            SolutionTree.SetStatusText(s);
        }

        private void SetSearchProgress(int value)
        {
            return;
            if (value > 100) value = 100;
            SolutionTree.SetSearchProgress(value);
        }

        private static void SetFileNodeFontForSearch(TreeNode fileNode)
        {
            fileNode.NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular);
        }

        public void ResetSolutionFilter()
        {
            SetSolutionTree(GeneralTreeNodeCollection);
            IsSoultionFiltered = false;
        }

        public void LoadSourceFolder(string selectedPath)
        {            
            var parentNode = new TreeNode();
            var match = Regex.Match(selectedPath, @"[\\]+([^\\]+)[\\]?$"); // get last dir name
            //var match = Regex.Match(selectedPath, @"([^\\]+$)"); // get last dir name
            parentNode.Text = match.Groups[1].Value.ToUpper();
            parentNode.ToolTipText = selectedPath;            
            parentNode.ImageIndex = 0;            
            parentNode.SelectedImageIndex = 0;
            parentNode.BackColor = Color.FromArgb(194, 235, 235, 255);
            parentNode.Tag = selectedPath;

            GeneralTreeNodeCollection.Add(parentNode);
            AddFolderItems(parentNode,selectedPath);
            SetSolutionTree(GeneralTreeNodeCollection);
        }

        private void AddFolderItems(TreeNode parentNode, string selectedPath)
        {
            string[] directories = Directory.GetDirectories(selectedPath);
            // Exclude paths starting from '.'
            directories = directories.Where(x => !x.Contains("\\.")).ToArray();

            foreach (string directory in directories)
            {
                var match = Regex.Match(directory, @"([^\\]+$)");

                var directoryNode = new TreeNode();
                directoryNode.BackColor = Color.FromArgb(163, 254, 252, 225);
                directoryNode.Text = match.Value;
                directoryNode.ToolTipText = directory;
                directoryNode.ImageIndex = 2;
                directoryNode.SelectedImageIndex = 2;

                parentNode.Nodes.Add(directoryNode);

                AddFolderItems(directoryNode, directory);
            }

            var files = Directory.GetFiles(selectedPath, "*.sr*").ToList();
            files.AddRange(Directory.GetFiles(selectedPath, "*.sql"));
            files.AddRange(Directory.GetFiles(selectedPath, "*.cs"));

            foreach (string file in files)
             {
                var fileNode = new TreeNode();
                fileNode.Text = Path.GetFileNameWithoutExtension(file);
                string extension = Path.GetExtension(file);
                if (extension == ".srd") { fileNode.ImageIndex = 1; }
                else if (extension == ".srw") { fileNode.ImageIndex = 4; }
                else if (extension == ".sru") { fileNode.ImageIndex = 5; }
                else if (extension == ".srm") { fileNode.ImageIndex = 10; }
                else if (extension == ".srs") { fileNode.ImageIndex = 11; }
                else { fileNode.ImageIndex = 6; }
                fileNode.SelectedImageIndex = fileNode.ImageIndex;
                fileNode.ForeColor = Color.Black;
                fileNode.ToolTipText = file;
                    
                var fileClass = new FileClass();
                fileClass.Name = fileNode.Text;
                fileClass.FilePath = file;
                fileClass.FileName = fileNode.Text;
                fileNode.Tag = fileClass;
                parentNode.Nodes.Add(fileNode);
             }                           
        }

//        public void LoadSourceFolder(string selectedPath)
//        {
//            var parentNode = new TreeNode();
//            var match = Regex.Match(selectedPath, @"[\\]+(\w+)[\\]$"); // get last dir name
//            parentNode.Text = match.Groups[1].Value.ToUpper();
//            parentNode.ToolTipText = selectedPath;
//            parentNode.ImageIndex = 0;
//            parentNode.SelectedImageIndex = 0;
//            GeneralTreeNodeCollection.Add(parentNode);
//            parentNode.BackColor = Color.FromArgb(194, 235, 235, 255);
//            //            parentNode.NodeFont = new Font();
//            string[] directories = Directory.GetDirectories(selectedPath);
//
//            foreach (string directory in directories)
//            {
//                var diretoryNode = new TreeNode();
//                diretoryNode.BackColor = Color.FromArgb(163, 254, 252, 225);
//                match = Regex.Match(directory, @"([^\\]+$)");
//                diretoryNode.Text = match.Value;
//                diretoryNode.ToolTipText = directory;
//                diretoryNode.ImageIndex = 2;
//                diretoryNode.SelectedImageIndex = 2;
//                parentNode.Nodes.Add(diretoryNode);
//                string[] files = Directory.GetFiles(directory, "*.sr*");
//                foreach (string file in files)
//                {
//                    var fileNode = new TreeNode();
//                    fileNode.Text = Path.GetFileNameWithoutExtension(file);
//                    string extension = Path.GetExtension(file);
//                    if (extension == ".srd") { fileNode.ImageIndex = 1; }
//                    else if (extension == ".srw") { fileNode.ImageIndex = 4; }
//                    else if (extension == ".sru") { fileNode.ImageIndex = 5; }
//                    else if (extension == ".srm") { fileNode.ImageIndex = 10; }
//                    else if (extension == ".srs") { fileNode.ImageIndex = 11; }
//                    else { fileNode.ImageIndex = 6; }
//                    fileNode.SelectedImageIndex = fileNode.ImageIndex;
//                    fileNode.ForeColor = Color.Black;
//                    fileNode.ToolTipText = file;
//
//                    var fileClass = new FileClass();
//                    fileClass.Name = fileNode.Text;
//                    fileClass.FilePath = file;
//                    fileClass.FileName = fileNode.Text;
//                    fileNode.Tag = fileClass;
//                    diretoryNode.Nodes.Add(fileNode);
//                }
//            }
//            SetSolutionTree(GeneralTreeNodeCollection);
//        }

        public void SetSolutionTree(IEnumerable<TreeNode> generalTreeNodeCollection)
        {
            SolutionTree.treeView1.BeginUpdate();
            SolutionTree.treeView1.Nodes.Clear();
            foreach (TreeNode treeNode in generalTreeNodeCollection) { SolutionTree.treeView1.Nodes.Add(treeNode); }
            foreach (TreeNode node in SolutionTree.treeView1.Nodes) { node.Expand(); }
            SolutionTree.treeView1.EndUpdate();
        }

        public void SetFocusToTreeView()
        {
            SolutionTree.treeView1.Focus();
        }

        public void FindFileName(string selectedText)
        {
//            SourceContainerDocument sourceDocument = MainForm.ActiveMdiChild as SourceContainerDocument;
//            if (sourceDocument != null) {          
//                string selectedText = sourceDocument.SourceEditorPanel.fastColoredTextBox1.SelectedText;
            MyDockHelper.MakePanelVisible(SolutionTree);
            if (!string.IsNullOrEmpty(selectedText)) { SolutionTree.SetAndFindFile(selectedText); }
//            }
        }

//        public void SelectedTextFindInCurrentSource()
//        {
//            SourceContainerDocument sourceDocument = MainForm.ActiveMdiChild as SourceContainerDocument;
//            if (sourceDocument != null)
//            {
//                string selectedText = sourceDocument.SourceEditorPanel.fastColoredTextBox1.SelectedText;
//                if (!string.IsNullOrEmpty(selectedText)) { sourceDocument.SourceEditorPanel.FindInSourcePanel.SetSeachableText(selectedText,true); }
//            }
//        }

         private int _noteCount = 0;
        public SourceContainerDocument OpenNewNote(bool isSql)
        {
            SourceContainerDocument newDocument;
            FileClass fileClass = new FileClass()
                                  {
                                      FileName = "SQL Note",
                                      FilePath = "Notes",
                                      TextState = ETextState.ReadNotChanged,                                      
                                      Name = $"Note [{++_noteCount}]",
                                      IsSql = isSql
            };
            newDocument = CreateNewDocument(fileClass); 
            ShowPanel(newDocument);
            newDocument.SourceEditorPanel.fastColoredTextBox1.Focus();
            return newDocument;
        }

        public void AnalayzeFileForTypes(FileClass fileClass)
        {
            string text = fileClass.Text;
            try
            {
                //return;
//                var regex = new Regex(@"^event\s*(?<eventname>\w+)\s*(\((?<params>.*?)\))?;.*", RegexOptions.Multiline);
//                MatchCollection matchCollection = regex.Matches(text);
                //var test = Regex.Matches(text, @"(?<=event)\s*(?<eventname>\w+)\s*\((?<params>.+?)\);(.+?)(?=end event)", RegexOptions.Multiline);
                
                FillRnCounts(text);

                Match matchEndFarword = Regex.Match(text, "end forward", RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
                int indexEndFarword = matchEndFarword.Groups[0].Index;

                string typeListRegex = @"(?si)[\n](global\s+type|type)+\s+(?<typename>\w+)\s+from\s+(?<inferitfrom>\w+)(\s+within\s+\w+)?(?<instatncevars>.*?)(?<endtype>end type)";

                MatchCollection matchCollection = Regex.Matches(text, typeListRegex, RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
                // Find Types
                PowerBuilderFileType fileType = null;
                fileClass.PowerBuilderFileTypes = new List<PowerBuilderFileType>();
                foreach (Match matchType in matchCollection)
                {
                    if (indexEndFarword > matchType.Groups[1].Index) continue;

                    if (fileType != null)
                    {
                        fileType.IndexEnd = matchType.Groups[1].Index;
                        fileType.LineNumberEnd = GetTextLineByCharIndex(fileType.IndexEnd);
                    }
                    //New type
                    fileType = new PowerBuilderFileType();
                    fileType.Name = matchType.Groups["typename"].Value;
                    fileType.InheritFrom = matchType.Groups["inferitfrom"].Value;                    
                    fileType.IndexInstanceVarStart = matchType.Groups[3].Index;
                    fileType.IndexStart = matchType.Groups[1].Index;
                    fileType.IndexInstanceVarEnd = matchType.Groups["endtype"].Index;

                    fileType.LineNumberStart = GetTextLineByCharIndex(fileType.IndexStart);
                    fileType.LineNumberInstanceVarsEnd = GetTextLineByCharIndex(fileType.IndexInstanceVarEnd);

                    fileClass.PowerBuilderFileTypes.Add(fileType);

                    //Find Instance Vars
                    if (matchType.Groups[3].Length > 5)
                    {
                        //
                    }
                }

                string eventListRegex = @"(?si)([\n]event|^event)(.*?)(?<eventname>\w+)\s*(\((?<params>.*?)\))?;(?<body>.*?)(?<endevent>([\n]e|^e)nd event)";
                foreach (var powerBuilderFileType in fileClass.PowerBuilderFileTypes)
                {
                    int length = powerBuilderFileType.IndexEnd - powerBuilderFileType.IndexInstanceVarEnd;
                    if (length < 0) { length = text.Length - powerBuilderFileType.IndexInstanceVarEnd;}
                    var subText = text.Substring(powerBuilderFileType.IndexInstanceVarEnd, length);
                    matchCollection = Regex.Matches(subText, eventListRegex, RegexOptions.ExplicitCapture,TimeSpan.FromSeconds(5));
                    foreach (Match eventMatch in matchCollection)
                    {
                        var filePositionItem = new FilePositionItem();
                        filePositionItem.Name = eventMatch.Groups[1].Value;
                        filePositionItem.ItemType = "event";
                        filePositionItem.ItemParameters = eventMatch.Groups[2].Value;
                        filePositionItem.IndexItemStart = eventMatch.Groups[1].Index + powerBuilderFileType.IndexInstanceVarEnd;
                        filePositionItem.IndexItemEnd = eventMatch.Groups[4].Index + powerBuilderFileType.IndexInstanceVarEnd;
                        filePositionItem.IsHasCode = eventMatch.Groups[3].Length > 5;
                        filePositionItem.LineNumberStart = GetTextLineByCharIndex(filePositionItem.IndexItemStart);
                        filePositionItem.LineNumberEnd = GetTextLineByCharIndex(filePositionItem.IndexItemEnd);
                        powerBuilderFileType.Events.Add(filePositionItem);
                    }
                }

                
                if (fileClass.PowerBuilderFileTypes.Count > 0)
                {
                    string functionListRegex = @"(?si)([\n](public|private|protected)(\sfunction)?|^(public|private|protected)(\sfunction)?)\s*(?<functype>\w+)\s*(?<functname>\w+)\s*(\((?<params>[^\)]*?)\));+(?<body>.*?)(?<endfunct>end(\sfunction|\ssubroutine))";
//                    string functionListRegex = @"(?si)([\n](public|private)(\sfunction)?|^(public|private)(\sfunction)?)\s*(?<functype>\w+)\s*(?<functname>\w+)\s*(\((?<params>[^)]*?)\));+(?<body>.*?)(?<endfunct>end(\sfunction|\ssubroutine))";
                    //var powerBuilderFileType = fileClass.PowerBuilderFileTypes.FirstOrDefault();
                    foreach (var powerBuilderFileType in fileClass.PowerBuilderFileTypes)
                    {
                        int length = powerBuilderFileType.IndexEnd - powerBuilderFileType.IndexInstanceVarEnd;
                        if (length < 0) { length = text.Length - powerBuilderFileType.IndexInstanceVarEnd; }
                        var subText = text.Substring(powerBuilderFileType.IndexInstanceVarEnd, length);
                        matchCollection = Regex.Matches(subText, functionListRegex, RegexOptions.ExplicitCapture, TimeSpan.FromSeconds(5));
                        foreach (Match eventMatch in matchCollection)
                        {
                            var filePositionItem = new FilePositionItem();
                            filePositionItem.Name = eventMatch.Groups[2].Value;
                            filePositionItem.ReturnType = eventMatch.Groups[1].Value;
                            filePositionItem.ItemType = "function";
                            filePositionItem.ItemParameters = eventMatch.Groups[3].Value;
                            filePositionItem.IndexItemStart = eventMatch.Groups[2].Index + powerBuilderFileType.IndexInstanceVarEnd;
                            filePositionItem.IndexItemEnd = eventMatch.Groups[5].Index + powerBuilderFileType.IndexInstanceVarEnd;
                            filePositionItem.IsHasCode = eventMatch.Groups[4].Length > 5;
                            filePositionItem.LineNumberStart = GetTextLineByCharIndex(filePositionItem.IndexItemStart);
                            filePositionItem.LineNumberEnd = GetTextLineByCharIndex(filePositionItem.IndexItemEnd);
                            powerBuilderFileType.Functions.Add(filePositionItem);
                        }
                    }
                }

                _rnCounts = null;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        public void FillRnCounts(string text)
        {
            int rnsCount = 0;
            _rnCounts = new List<RnsCountClass>();
            RnsCountClass rnsCountClass = null;
            int count = text.Count();
            for (int index = 0; index < count; index++)
            {
                char ch = text[index];
                if (ch == '\r')
                {
                    rnsCount++;
                    if (rnsCountClass != null) { rnsCountClass.IndexEnd = index; }
                    rnsCountClass = new RnsCountClass();
                    rnsCountClass.rnCount = rnsCount;
                    rnsCountClass.IndexStart = index;
                    _rnCounts.Add(rnsCountClass);
                }
            }
            if (rnsCountClass != null) { rnsCountClass.IndexEnd = count; }
        }

        public int GetTextLineByCharIndex(int indexStart)
        {
            var firstOrDefault = _rnCounts.Where(x => x.IndexEnd > indexStart && x.IndexStart < indexStart).FirstOrDefault();
            if (firstOrDefault != null) { return firstOrDefault.rnCount; }
            return 0;
        }

        public void SelectedTextFindInAllFiles(string selectedText)
        {                
            if (!string.IsNullOrEmpty(selectedText))
            {
                MyDockHelper.MakePanelVisible(SolutionTree);
                SolutionTree.SetAndFindInAllFiles(selectedText);
            }
        }

        public void RemoveSourcePathes()
        {
            ClearSolutionTree();
            var workSpaceItem = GetCurrentWorkSpace();
            workSpaceItem.SourceFloders.Clear();            
        }

        private void ClearSolutionTree()
        {
            GeneralTreeNodeCollection.Clear();
            SetSolutionTree(GeneralTreeNodeCollection);
        }

        public WorkSpaceItem GetCurrentWorkSpace()
        {
            return GetWorkspace(App.Configuration.CurrentWorkSpaceName);
        }

        public void SaveWorkSpace()
        {
            RemeberOpenedDocumentsList();
            App.Save();
        }

        public void LoadWorkSpace(string currentWorkSpaceName, bool reloadOpenedDocumetns = false)
        {
            IsLoadingWorkspase = true;
            if (reloadOpenedDocumetns == false) { CloseAllDocuments(); }
            App.Configuration.CurrentWorkSpaceName = currentWorkSpaceName;
            //ShowWaitPanel(true);
            //Action a = () =>
            {
                ClearSolutionTree(); 
                var workSpaceItem = GetWorkspace(currentWorkSpaceName);
                           foreach (var item in workSpaceItem.SourceFloders) { LoadSourceFolder(item); }
                       };
            //a.BeginInvoke(ar => {/* ShowWaitPanel(false); */}, null);            
            SolutionTree.SetSearchHistoryListBox();
            LoadDocumentsListFromWorkspace(reloadOpenedDocumetns);
            SetMainFormCaption();
            IsLoadingWorkspase = false;
            OpenedDocumentsPanel.RefreshOpenedDocumentsList();
            RefershOpenedDocumentsPanelCaption();
        }

        private void RefershOpenedDocumentsPanelCaption()
        {
            OpenedDocumentsPanel.Text = string.Format("Opened Documents [{0}]", App.Configuration.CurrentWorkSpaceName);
        }

        public bool IsLoadingWorkspase { get; set; }

        public void SetMainFormCaption()
        {
            MainForm.Text = $@"PBSC Analyzer, v.{Application.ProductVersion}, [{App.Configuration.CurrentWorkSpaceName}]";
        }

        private void ShowWaitPanel(bool b)
        {            
            _waitForm = _waitForm ?? new WaitForm();
            if (b)
            {
                if (_waitForm.Visible) return;
                _waitForm.Show(MainForm);                
            }
            else {_waitForm.Hide();}
        }

        private WorkSpaceItem GetWorkspace(string currentWorkSpaceName)
        {
            WorkSpaceItem workSpaceItem = App.Configuration.WorkSpaces.Where(x => x.Name == currentWorkSpaceName).FirstOrDefault();
            if (workSpaceItem == null)
            {
                workSpaceItem = CreateWorkspace(currentWorkSpaceName);
            }
            return workSpaceItem;
        }

        public WorkSpaceItem CreateWorkspace(string currentWorkSpaceName)
        {
            App.Configuration.CurrentWorkSpaceName = currentWorkSpaceName; 
            var workSpaceItem = new WorkSpaceItem() {Name = currentWorkSpaceName};
            App.Configuration.WorkSpaces.Add(workSpaceItem);
            return workSpaceItem;
        }

        public WorkSpaceItem CloneWorkspace(string newName)
        {
            WorkSpaceItem spaceItem = GetWorkspace(App.Configuration.CurrentWorkSpaceName);
            App.Configuration.CurrentWorkSpaceName = newName;
            var workSpaceItem = ObjectCopier.Clone<WorkSpaceItem>(spaceItem);
            App.Configuration.WorkSpaces.Add(workSpaceItem);
            workSpaceItem.Name = newName;

            return workSpaceItem;
        }

        public void AddSourceFolderToWorkspace(string selectedPath)
        {
            var workSpaceItem = GetWorkspace(App.Configuration.CurrentWorkSpaceName);
            if (!workSpaceItem.SourceFloders.Contains(selectedPath)) { workSpaceItem.SourceFloders.Add(selectedPath); }
            LoadSourceFolder(selectedPath);
        }

        public void ReloadAllDocuments()
        {
            //            ForeachNodeFileClass(@class => true,SolutionTree.treeView1.Nodes, node =>
            //                                                                              {
            //                                                                                  (node.Tag as FileClass)
            //                                                                              })

            Instance.SaveWorkSpace();
            Instance.LoadWorkSpace(App.Configuration.CurrentWorkSpaceName,true);           
        }

        public void CloseAllDocuments()
        {
            this.CloseInBatch = true;
            for (int index = MainDockPanel.Contents.Count - 1; index >= 0; index--)
            {
                if (MainDockPanel.Contents[index] is SourceContainerDocument)
                {
                    IDockContent content = (IDockContent) MainDockPanel.Contents[index];
                    content.DockHandler.Close();
                }
            }
            this.CloseInBatch = false;
            MainEngine.Instance.OpenedDocumentsPanel.RefreshOpenedDocumentsList();
        }

        public bool CloseInBatch { get; set; }

        public void RemeberOpenedDocumentsList()
        {
            var workSpaceItem = GetWorkspace(App.Configuration.CurrentWorkSpaceName);
            workSpaceItem.FileClasses.Clear();
            foreach (var dockContent in MainDockPanel.Contents)
            {
                if (dockContent is SourceContainerDocument)
                {
                    SourceContainerDocument content = (SourceContainerDocument) dockContent;
                    workSpaceItem.FileClasses.Add(new FileClassInWorkspace() {FilePath = content.FileClass.FilePath, Name = content.FileClass.Name});
                }
            }
        }
        
        public void LoadDocumentsListFromWorkspace(bool reloadFileContent = false)
        {
            var workSpaceItem = GetWorkspace(App.Configuration.CurrentWorkSpaceName);
            foreach (FileClassInWorkspace fileClassInWorkspace in workSpaceItem.FileClasses)
            {
                var findNodeByFileClass = FindNodeByFileClass(x =>x.FilePath == fileClassInWorkspace.FilePath, SolutionTree.treeView1.Nodes);
                if (findNodeByFileClass != null) { OpenOrSwitchFile(findNodeByFileClass.Tag as FileClass, reloadFileContent); }
            }
        }

        private TreeNode FindNodeByFileClass(Func<FileClass,bool> patern, TreeNodeCollection treeNodeCollection)
        {
            TreeNode result = null;
            foreach (TreeNode node in treeNodeCollection)
            {
                if (node.Nodes.Count > 0)
                {
                    result = FindNodeByFileClass(patern, node.Nodes);
                }
                if (result != null) { return result;}
                if (node.Tag is FileClass)
                {
                    var fileClass = (node.Tag as FileClass);
                    if (patern.Invoke(fileClass)) return node;
                }
            }
            return null;
        }

        private TreeNode ForeachNodeFileClass(Func<FileClass, bool> patern, TreeNodeCollection treeNodeCollection, Action<TreeNode> nodeAction )
        {
            TreeNode result = null;
            foreach (TreeNode node in treeNodeCollection)
            {
                if (node.Nodes.Count > 0)
                {
                    result = FindNodeByFileClass(patern, node.Nodes);
                }
                if (result != null) { return result; }
                if (node.Tag is FileClass)
                {
                    var fileClass = (node.Tag as FileClass);
                    if (patern.Invoke(fileClass))
                    {
                        nodeAction.Invoke(node);
                    }
                }
            }
            return null;
        }

        public void ApplyFonts()
        {
            ForeachOpenedDocument<SourceContainerDocument>(document => document.SetFont());            
        }

        private void ForeachOpenedDocument<T>(Action<T> action)
        {
            foreach (var dockContent in MainDockPanel.Contents)
            {
                if (dockContent is T)
                {
                    T content = (T)dockContent;
                    action.Invoke(content);
                }
            }
        }

        public void RemoveSelectedSourcePath()
        {            
            if (SolutionTree.treeView1.SelectedNode != null)
            {
                var workSpaceItem = GetWorkspace(App.Configuration.CurrentWorkSpaceName);

                workSpaceItem.SourceFloders.Remove((string) SolutionTree.treeView1.SelectedNode.Tag);

                GeneralTreeNodeCollection.Remove(SolutionTree.treeView1.SelectedNode);
                SolutionTree.treeView1.Nodes.Remove(SolutionTree.treeView1.SelectedNode);
            }
        }
    }

    public class RnsCountClass
    {
        public int rnCount;
        public int IndexStart { get; set; }
        public int IndexEnd { get; set; }
    }

    public class SearchInFileCriteria
    {
        private readonly string _searchedText;

        public SearchInFileCriteria(string searchedText)
        {
            _searchedText = searchedText;
        }

        public string searchedText
        {
            get { return _searchedText; }
        }
    }

    public class SearchInFileLineItem
    {
        public int LineNum { get; set; }
        public string TextLine { get; set; }
        public SearchInFileCriteria SearchInFileCriteria { get; set; }
    }

    public enum ESearchIn
    {
        FileName,
        FileText,
        OpenedDocuments
    }

    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}