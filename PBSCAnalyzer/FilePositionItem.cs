namespace PBSCAnalyzer.types
{
    public class FilePositionItem
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public int IndexItemStart { get; set; }
        public int IndexItemEnd { get; set; }
        public string ItemType { get; set; }
        public string ItemParameters { get; set; }
        public string ReturnType { get; set; }
        public bool IsHasCode { get; set; }
        public int LineNumberStart { get; set; }
        public int LineNumberEnd { get; set; }
        public PowerBuilderFileType PowerBuilderFileType { get; set; }
        /// <summary>
        ///  for sql arguments, type of argument
        /// </summary>
        public string NameType { get; set; }
        /// <summary>
        /// test value for sql argument
        /// </summary>
        public string ArgumentValue { get; set; }

        public string FormatForNodeText { get; set; }
    }
}