using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PBSCAnalyzer
{
    public class UserTextSnippet
    {
        public string Caption = "";
        public string Text = "";
        public bool IsMenu = false;
        public List<UserTextSnippet> SubSnippets = new List<UserTextSnippet>();
        public string PlaceName;
        public string HotKey;

        [XmlIgnore]
        [NonSerialized]
        public UserTextSnippet ParentUserTextSnippet;
    }
}