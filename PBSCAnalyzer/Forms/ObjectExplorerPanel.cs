using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using PBSCAnalyzer.Forms;
using PBSCAnalyzer.types;
using WeifenLuo.WinFormsUI.Docking;

namespace PBSCAnalyzer
{
    public partial class ObjectExplorerPanel : DockContent
    {
        public ObjectExplorerPanel()
        {
            InitializeComponent();
            //treeView1.Font = SourceFileStylesClass.SourceEditorFont;
        }

        public SourceContainerDocument SourceContainerDocument { get; set; }

        public void SetObjectsList(FileClass fileClass)
        {
            try
            {                
                treeView1.BeginUpdate();

                //DataWindow's objects
                if (fileClass.Text.Contains(".srd"))
                {
                    treeView1.Nodes.Clear();
                    
                    MainEngine.Instance.FillRnCounts(fileClass.Text);
                    AddNodeByRegex(fileClass, "Arguments ({0})", @"\,?\(\""(?<name>(\w+))\""\,\s*(?<type>\w+)\)", ":{0}=? : {1}", 11,"sqlArgument");
                    AddNodeByRegex(fileClass, "SQL Columns ({0})", @"(column\=\().*?name\=(?<name>\w+).*?dbname\=\""(?<dbname>(\w|\.)+)\"".*\)", "{0} : {1}", 9,"sqlColumns");
                    AddNodeByRegex(fileClass, "Columns ({0})", @"(?<=column\().*?name\=(?<name>\w+).*\)", "{0}", 12,"pbColumns");
                    AddNodeByRegex(fileClass, "Reports ({0})", @"(?<=report\().*?dataobject\=\""(?<dataobject>(\w)+)\"".*?name\=(?<name>\w+).*?\)", "{1} : {0}", 12,"reports");
                    AddNodeByRegex(fileClass, "Calculated ({0})", @"(?<=compute\().*?expression\=\""(?<expr>.*?)\"".*?name\=(?<name>\w+).*\)", "{1} : {0}", 12,"calculated");
                    AddNodeByRegex(fileClass, "Texts ({0})", @"(?<=text\().*?text\=\""(?<expr>.*?)\"".*?name\=(?<name>\w+).*\)", "\"{0}\" : {1}", 15,"text");
                    //treeView1.ExpandAll();
                }
                else if (fileClass.IsSql == false)
                {
                    treeView1.Nodes.Clear();
                    foreach (PowerBuilderFileType powerBuilderFileType in fileClass.PowerBuilderFileTypes.OrderBy(x=>x.InheritFrom).ThenBy(x=>x.Name))
                    {
                        MainEngine.Instance.FillRnCounts(fileClass.Text);

                        var typeNode = new TreeNode();
                        if (powerBuilderFileType.IsMainType) { typeNode.Expand(); }
                        FilePositionItem textVar = GetVarValueFromType(powerBuilderFileType,"Text", fileClass, @"string\s*(text|title)\s*\=\s*\""(?<dataobj>[^""]+)""");
                        var name = string.IsNullOrEmpty(textVar?.Name) ? "" : "  '" + textVar?.Name + "'  ";
                        typeNode.Text = $"{powerBuilderFileType.Name}{name}: {powerBuilderFileType.InheritFrom}";
                        typeNode.Tag = powerBuilderFileType;
                        //typeNode.NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
                        if (typeNode.Text.ToLower().StartsWith("w")) { typeNode.ImageIndex = 0; }
                        if (typeNode.Text.ToLower().StartsWith("d")) { typeNode.ImageIndex = 7; }
                        if (typeNode.Text.ToLower().StartsWith("u")) { typeNode.ImageIndex = 2; }
                        if (typeNode.Text.ToLower().StartsWith("s")) { typeNode.ImageIndex = 1; }
                        if (typeNode.Text.ToLower().StartsWith("cb")) { typeNode.ImageIndex = 13; }
                        typeNode.SelectedImageIndex = typeNode.ImageIndex;

                        //DataObjects
                        if (powerBuilderFileType.Name.ToLower().StartsWith("d"))
                        {
                            string initialDataObject = "";
                            var dataObjNode = new TreeNode();
                            dataObjNode.Text = "Dataobject";
                            dataObjNode.ImageIndex = 7;
                            dataObjNode.SelectedImageIndex = dataObjNode.ImageIndex;
                            typeNode.Nodes.Add(dataObjNode);

                            string textInstanceRange = fileClass.Text.Substring(powerBuilderFileType.IndexStart, powerBuilderFileType.IndexInstanceVarEnd - powerBuilderFileType.IndexStart);
                            Match match = Regex.Match(textInstanceRange, @"string\s*dataobject\s*\=\s*\""(?<dataobj>\w+)""", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                string dataObjectSource = match.Groups["dataobj"].Value;
                                initialDataObject = dataObjectSource;
                                //typeNode.Text = string.Format(@"{0} ""{2}"" : {1}", powerBuilderFileType.Name, powerBuilderFileType.InheritFrom, initialDataObject);
                                TreeNode dnode = new TreeNode();
                                dnode.Text = initialDataObject;
                                dnode.Tag = new FilePositionItem()
                                            {
                                                LineNumberStart = MainEngine.Instance.GetTextLineByCharIndex(powerBuilderFileType.IndexStart + match.Groups["dataobj"].Index),
                                                ItemType = "DataObjectSource",
                                                Name = dataObjectSource
                                            };
                                dataObjNode.Tag = new FilePositionItem()
                                {
                                    LineNumberStart = MainEngine.Instance.GetTextLineByCharIndex(powerBuilderFileType.IndexStart + match.Groups["dataobj"].Index),
                                    ItemType = "DataObjectList"
                                };
                                dataObjNode.Nodes.Add(dnode);
                            }

                            int length = powerBuilderFileType.IndexEnd - powerBuilderFileType.IndexStart;
                            length = length > 0 ? length : fileClass.Text.Length - powerBuilderFileType.IndexStart;
                            textInstanceRange = fileClass.Text.Substring(powerBuilderFileType.IndexStart, length);
                                                        
                            MatchCollection matchesCollection = Regex.Matches(textInstanceRange, @"[\r\n](\s*)(this\.)?dataobject\s*\=\s*?(?<dataobj>(.)+)", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            MatchCollection matchesCollection2 = Regex.Matches(fileClass.Text, $@"[\r\n](\s*){powerBuilderFileType.Name}\.dataobject\s*\=\s*?(?<dataobj>(.)+)", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                            Action <MatchCollection,int> action = (MatchCollection matches, int indexOffset) =>
                                            {

                                                foreach (Match m in matches)
                                                {
                                                    TreeNode dnode = new TreeNode();
                                                    string dataObjectSource = m.Groups["dataobj"].Value;
                                                    dnode.Text = dataObjectSource;
                                                    dnode.Tag = new FilePositionItem()
                                                                {
                                                                    LineNumberStart = MainEngine.Instance.GetTextLineByCharIndex(indexOffset + m.Groups["dataobj"].Index),
                                                                    ItemType = "DataObjectSource",
                                                                    Name = dataObjectSource
                                                                };
                                                    dataObjNode.Nodes.Add(dnode);
                                                }
                                            };
                            action.Invoke(matchesCollection, powerBuilderFileType.IndexStart);
                            action.Invoke(matchesCollection2,0);
                            int count = dataObjNode.Nodes.Count;
                            if (count > 1) { dataObjNode.Text = $"Dataobjects ({count})"; }
                            typeNode.Text = string.Format(@"{0} ""{2}""{3}: {1}", powerBuilderFileType.Name, powerBuilderFileType.InheritFrom, initialDataObject,count > 1 ? " ("+count+") ":" ");
                        }

                        // Events
                        foreach (var filePositionItem in powerBuilderFileType.Events.OrderBy(o => o.Name))
                        {
                            var eventNode = new TreeNode();
                            eventNode.Text = $"{filePositionItem.Name}{(filePositionItem.ItemParameters.Length > 0 ? " (...) " : " () ")}[{filePositionItem.LineNumberEnd - filePositionItem.LineNumberStart}]";
                            eventNode.Tag = filePositionItem;
                            eventNode.ImageIndex = 6;
                            eventNode.ToolTipText = $"{filePositionItem.Name}{" (" + filePositionItem.ItemParameters + ") "}[{filePositionItem.LineNumberEnd - filePositionItem.LineNumberStart}]";
                            ;
                            eventNode.SelectedImageIndex = eventNode.ImageIndex;
                            if (filePositionItem.IsHasCode == false) { eventNode.ForeColor = Color.DimGray; }
                            typeNode.Nodes.Add(eventNode);
                        }

                        //Functions
                        foreach (var filePositionItem in powerBuilderFileType.Functions.OrderBy(o => o.Name))
                        {
                            var eventNode = new TreeNode();
                            eventNode.Text = $"{filePositionItem.Name}{(filePositionItem.ItemParameters.Length > 0 ? " (...) " : " () ")}: {(filePositionItem.ReturnType == "subroutine" ? "" : filePositionItem.ReturnType)} [{filePositionItem.LineNumberEnd - filePositionItem.LineNumberStart}]";
                            eventNode.Tag = filePositionItem;
                            eventNode.ImageIndex = 10;
                            eventNode.ToolTipText = $"{filePositionItem.Name}{" (" + filePositionItem.ItemParameters + ") "}: {(filePositionItem.ReturnType == "subroutine" ? "" : filePositionItem.ReturnType)} [{filePositionItem.LineNumberEnd - filePositionItem.LineNumberStart}]";
                            ;
                            eventNode.SelectedImageIndex = eventNode.ImageIndex;
                            if (filePositionItem.IsHasCode == false) { eventNode.ForeColor = Color.DimGray; }
                            typeNode.Nodes.Add(eventNode);
                        }
                        treeView1.Nodes.Add(typeNode);
                    }
                }
                treeView1.EndUpdate();
            }
            catch (Exception ex)
            {
                
            }
        }

        private FilePositionItem GetVarValueFromType(PowerBuilderFileType powerBuilderFileType, string text, FileClass fileClass, string varRegex)
        {
            string textInstanceRange = fileClass.Text.Substring(powerBuilderFileType.IndexStart, powerBuilderFileType.IndexInstanceVarEnd - powerBuilderFileType.IndexStart);
            Match match = Regex.Match(textInstanceRange, varRegex, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string dataObjectSource = match.Groups["dataobj"].Value;
                return new FilePositionItem()
                       {
                           LineNumberStart = MainEngine.Instance.GetTextLineByCharIndex(powerBuilderFileType.IndexStart + match.Groups["dataobj"].Index),
                           ItemType = "Variable",
                           Name = dataObjectSource
                };
            }
            return null;
        }

        private void AddNodeByRegex(FileClass fileClass, string nodeName, string regex, string format, int imageIndex, string itemType)
        {
            MatchCollection matchCollection = Regex.Matches(fileClass.Text, regex, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            var typeNode = new TreeNode();
            typeNode.Expand();
            typeNode.Text = string.Format(nodeName, matchCollection.Count);
            typeNode.ImageIndex = imageIndex;
            typeNode.SelectedImageIndex = typeNode.ImageIndex;
            typeNode.NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);
            foreach (Match match in matchCollection)
            {
                var node = new TreeNode();                
                
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = node.ImageIndex;
                var filePositionItem = new FilePositionItem();
                filePositionItem.FormatForNodeText = format;
                filePositionItem.IndexItemStart = match.Groups[1].Index;
                filePositionItem.IndexItemEnd = filePositionItem.IndexItemStart + 10;
                filePositionItem.ItemType = itemType;
                filePositionItem.Name = match.Groups[1].Value;
                filePositionItem.NameType = match.Groups[2].Value;
                node.Text = string.Format(format, match.Groups[1], match.Groups[2]);
                //                filePositionItem.IndexItemEnd = eventMatch.Groups[5].Index + powerBuilderFileType.IndexInstanceVarEnd;
                //                filePositionItem.IsHasCode = eventMatch.Groups[4].Length > 5;
                filePositionItem.LineNumberStart = MainEngine.Instance.GetTextLineByCharIndex(filePositionItem.IndexItemStart);
                filePositionItem.LineNumberEnd = MainEngine.Instance.GetTextLineByCharIndex(filePositionItem.IndexItemEnd);
                node.Tag = filePositionItem;
                // child nodes for Calcualted expressions
                MatchCollection matchExpresionCollection = Regex.Matches(match.Groups[0].Value, @"(?<prop>\w+)\=\""(?<val>[^\""]*)\~t(?<expr>.*?)\""", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                foreach (Match m in matchExpresionCollection)
                {
                    var nodeExpr = new TreeNode();
                    nodeExpr.Text = string.Format("{0}=  {2}", m.Groups[1], m.Groups[2], m.Groups[3]);
                    nodeExpr.ImageIndex = 14;
                    nodeExpr.SelectedImageIndex = node.ImageIndex;
                    nodeExpr.Tag = filePositionItem;
//                    nodeExpr.NodeFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Italic);
                    nodeExpr.ForeColor = Color.Gray;
                    node.Nodes.Add(nodeExpr);
                }
                // Child nodes for dropdown date windows
                matchExpresionCollection = Regex.Matches(match.Groups[0].Value, @"(?<dddwname>dddw\.(name|datacolumn|displaycolumn))\=(?<val>\w+)\s", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                foreach (Match m in matchExpresionCollection)
                {
                    var nodeExpr = new TreeNode();
                    nodeExpr.Text = string.Format("{0}=  {1}", m.Groups[1], m.Groups[2], m.Groups[3]);
                    nodeExpr.ImageIndex = 14;
                    nodeExpr.SelectedImageIndex = node.ImageIndex;
                    filePositionItem = new FilePositionItem();
                    filePositionItem.IndexItemStart = match.Groups[1].Index;
                    filePositionItem.IndexItemEnd = filePositionItem.IndexItemStart + 10;
                    filePositionItem.LineNumberStart = MainEngine.Instance.GetTextLineByCharIndex(filePositionItem.IndexItemStart);
                    filePositionItem.LineNumberEnd = MainEngine.Instance.GetTextLineByCharIndex(filePositionItem.IndexItemEnd);
                    filePositionItem.Name = m.Groups[2].Value;
                    filePositionItem.ItemType = "dddw";
                    nodeExpr.Tag = filePositionItem;     
                    nodeExpr.ForeColor = Color.Gray;
                    node.Nodes.Add(nodeExpr);
                }
                typeNode.Nodes.Add(node);
            }
            treeView1.Nodes.Add(typeNode);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //treeView1.BeginUpdate();
            if (e.Node != null && e.X > 40)
            {
                if (toolStripButton3.Checked)
                {                    
                    if ((e.Node.Parent != null && e.Node.Parent.IsExpanded == false) || e.Node.Nodes.Count > 0 && e.Node.IsExpanded == false)
                    {
                        treeView1.CollapseAll();
                        e.Node.Expand();
                    }
                }
                SourceContainerDocument.ObjecExplorerNodeClicked(e.Node);
                //treeView1.EndUpdate();
            }
            ExpandChildeDataWindowsNode(e.Node);
            //treeView1.EndUpdate();
        }

        private void ExpandChildeDataWindowsNode(TreeNode node)
        {
            foreach (TreeNode cnode in node.Nodes)
            {
                FilePositionItem filePositionItem = cnode.Tag as FilePositionItem;
                if (filePositionItem != null && filePositionItem.ItemType == "DataObjectList")
                {
                    cnode.Expand();
                }
            }
        }

        public void HighlightObjectByLine(int fromLine)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                foreach (TreeNode objNode in node.Nodes)
                {
//                    var powerBuilderFileType = (objNode.Tag as PowerBuilderFileType);
//                    int lineNumber = 0;
//                    if (powerBuilderFileType != null) { lineNumber = powerBuilderFileType.LineNumberStart; }
//                    else
//                    {
                    var filePositionItem = (objNode.Tag as FilePositionItem);
                    if (filePositionItem != null)
                    {
                        if (filePositionItem.LineNumberStart <= fromLine && filePositionItem.LineNumberEnd >= fromLine)
                        {
                            objNode.EnsureVisible();
                            treeView1.SelectedNode = objNode;
                            return;
                        }
                    }
                }
                var powerBuilderFileType = (node.Tag as PowerBuilderFileType);
                if (powerBuilderFileType != null)
                {
                    if (powerBuilderFileType.LineNumberStart <= fromLine && powerBuilderFileType.LineNumberInstanceVarsEnd >= fromLine)
                    {
                        node.EnsureVisible();
                        treeView1.SelectedNode = node;
                        return;
                    }
                }
            }
            treeView1.SelectedNode = null;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //toolStripButton3.Checked = false;
            treeView1.ExpandAll();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            toolStripButton3.Checked = !toolStripButton3.Checked;            
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var textToSearch = GetNodeTargetText(false);
            SourceContainerDocument.FindTextAllInSource(textToSearch); 
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var textToSearch = GetNodeTargetText(true);
            MainEngine.Instance.FindFileName(textToSearch);
        }

        private string GetNodeTargetText(bool useInheritFrom)
        {
            if (treeView1.SelectedNode == null) { return null; }            
            PowerBuilderFileType powerBuilderFileType = treeView1.SelectedNode.Tag as PowerBuilderFileType;
            string textToSearch = "";
            if (powerBuilderFileType == null)
            {
                var filePositionItem = treeView1.SelectedNode.Tag as FilePositionItem;
                if (filePositionItem != null && filePositionItem.ItemType == "DataObjectSource") { textToSearch = filePositionItem.Name; }
                if (filePositionItem != null && filePositionItem.ItemType == "function") { textToSearch = filePositionItem.Name; }
                if (filePositionItem != null && filePositionItem.ItemType == "event") { textToSearch = filePositionItem.Name; }
                if (filePositionItem != null && filePositionItem.ItemType == "dddw") { textToSearch = filePositionItem.Name; }
            }
            else
            {
                if (useInheritFrom == true && powerBuilderFileType.Name.ToLower().StartsWith("u")) { textToSearch = powerBuilderFileType.InheritFrom; }
                else
                { textToSearch = powerBuilderFileType.Name; }
            }
            return textToSearch;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.X > 40)
            {
                var filePositionItem = e.Node.Tag as FilePositionItem;
                if (filePositionItem != null)
                {
                    if (filePositionItem.ItemType == "sqlArgument")
                    {
                        string promptValue = Prompt.ShowDialog(":"+filePositionItem.Name, filePositionItem.ArgumentValue);
                        filePositionItem.ArgumentValue = promptValue;
                        e.Node.Text = string.Format(filePositionItem.FormatForNodeText, filePositionItem.Name+"= "+ filePositionItem.ArgumentValue, filePositionItem.NameType);
                    }
                }
            }
            //if (e.Node != null && e.X > 40)
            //{
            //    SourceContainerDocument.ObjecExplorerNodeDoubleClicked(e.Node);                  
            //}                       
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
             SourceContainerDocument.ObjecExplorerNodeDoubleClicked(treeView1.SelectedNode);
        }

        public List<FilePositionItem> GetFilePositionItems()
        {
            List<FilePositionItem> list = new List<FilePositionItem>();
            foreach (TreeNode treeView1Node1 in treeView1.Nodes)
            {
                foreach (TreeNode treeView1Node in treeView1Node1.Nodes)
                {
                    if (treeView1Node.Tag is FilePositionItem)
                    {
                        list.Add(treeView1Node.Tag as FilePositionItem);
                    }
                }
            }
            return list;
        }
    }
}
