using System.Collections.Generic;

namespace PBSCAnalyzer.types
{
    public class PowerBuilderFileType
    {
        public List<FilePositionItem> Events = new List<FilePositionItem>();
        public List<FilePositionItem> Functions = new List<FilePositionItem>();
        public string Name;
        public int LineNumber { get; set; }
        public string InheritFrom { get; set; }
        public int LineNumberStart { get; set; }
        public int LineNumberInstanceVarsEnd { get; set; }
        public int IndexStart { get; set; }
        public int IndexEnd { get; set; }
        public int IndexInstanceVarStart { get; set; }
        public int IndexInstanceVarEnd { get; set; }
        public bool IsMainType { get; set; }
        public int LineNumberEnd { get; set; }
    }
}