using System.Collections.Generic;

namespace PBSCAnalyzer
{
    public class UserTextSnippet
    {
        public string Caption = "";
        public string Text = "";
        public bool IsMenu = false;
        public List<UserTextSnippet> SubSnippets { get; set; }
        public string PlaceName;
        public string HotKey;
    }
}