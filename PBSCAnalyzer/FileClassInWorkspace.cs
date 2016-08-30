using System;

namespace PBSCAnalyzer
{
    [Serializable]
    public class FileClassInWorkspace
    {
        public FileClassInWorkspace()
        {
            
        }

        public string FilePath { get; set; }
        public string Name { get; set; }        
        public bool IsSql { get; set; }
    }
}