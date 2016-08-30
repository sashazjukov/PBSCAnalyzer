using System;
using System.Collections.Generic;

namespace PBSCAnalyzer
{
    [Serializable]
    public class WorkSpaceItem
    {
        public WorkSpaceItem()
        {
            SourceFloders = new List<string>();
            FileClasses = new List<FileClassInWorkspace>();
            SolutionSearchHistoryList = new List<string>();
        }

        public string Name { get; set; }
        public List<string> SourceFloders { get; set; }
        public List<FileClassInWorkspace> FileClasses { get; set; }
        public List<string> SolutionSearchHistoryList { get; set; }
    }
}