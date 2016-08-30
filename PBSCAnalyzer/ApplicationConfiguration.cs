using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using Westwind.Utilities.Configuration;

namespace PBSCAnalyzer
{
    public class UserCommandsConfiguration : AppConfiguration
    {
        public UserCommandsConfiguration()
        {
            UserCommands = new List<UserCommand>();            
        }

        public List<UserCommand> UserCommands { get; set; }
    }

    public class ApplicationConfiguration : AppConfiguration
    {
        private Font _font;
        public string storedFont;

        public ApplicationConfiguration()
        {
            test = "test";
            WorkSpaces = new List<WorkSpaceItem>();

            CurrentWorkSpaceName = "Default";
            //            SourceEditorMyFont = new MyFont();
            //SourceEditorFont = SourceFileStylesClass.SourceEditorFont;            
        }

        public string test { get; set; }
        public List<WorkSpaceItem> WorkSpaces { get; set; }
        public string CurrentWorkSpaceName { get; set; }

        [XmlIgnore]
        //[NonSerialized]
        public Font SourceEditorFont
        {
            get
            {
                if (_font != null) { return _font; }
                if (string.IsNullOrEmpty(storedFont)) { return SourceFileStylesClass.SourceEditorFont; }
                var cvt = new FontConverter();
                _font = cvt.ConvertFromString(storedFont) as Font ?? SourceFileStylesClass.SourceEditorFont;
                return _font;
            }
            set
            {
                var cvt = new FontConverter();
                if (value != null)
                {
                    storedFont = cvt.ConvertToString(value);
                    _font = value;
                }
            }
        }
    }
}