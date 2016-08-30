using System.Collections.Generic;
using PBSCAnalyzer.types;

namespace PBSCAnalyzer
{
    public class FileClass
    {
        public FileClass()
        {
            PowerBuilderFileTypes = new List<PowerBuilderFileType>();
        }

        public string FilePath { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool IsIncludedInFilter { get; set; }
        public bool IsOpened { get; set; }
        public ETextState TextState { get; set; }
        public string FileName { get; set; }
        public List<PowerBuilderFileType> PowerBuilderFileTypes { get; set; }
        public bool IsSql { get; set; }
    }
}