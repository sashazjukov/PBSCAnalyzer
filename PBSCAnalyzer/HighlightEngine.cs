using FastColoredTextBoxNS;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PBSCAnalyzer
{
    public static class HighlightEngine
    {
        public static void SetSourceRules(object sender, bool isSql, FileClass fileClass)
        {

            var fctb = (FastColoredTextBox)sender;
            var range = fctb.Range;
            //var range = fctb.Range;
            range.BeginUpdate();
            range.ClearStyle(StyleIndex.All);

            if (fileClass.FilePath.Contains(".srd") && !isSql)
            {
                range.SetStyle(SourceFileStylesClass.TypeStyle, @"(?<range>(\s*|\()(\w|\.)+)\s*\=", RegexOptions.Compiled);
            }
            //  range.SetStyle(SourceFileStylesClass.WhiteRangeStyle, @"(?si)([\n](public|private|protected)(\sfunction)?|^(public|private|protected)(\sfunction)?)\s*(?<functype>\w+)\s*(?<functname>\w+)\s*(\((?<params>[^\)]*?)\));+(?<body>.*?)(?<endfunct>end(\sfunction|\ssubroutine))");

            //SQL Arguments
            range.SetStyle(SourceFileStylesClass.MaroonStyle, @"(:\w*)");

            //Operators
            range.SetStyle(SourceFileStylesClass.OperatorStyle, @"(\.|\(|\)|\=|\[|\]|\>|\<|\+|\-|\,|\;|\*|\&)");

            //Data Types
            const string pbDataTypes = @"(varchar|nvarchar|DATAWINDOWCHILD|subroutine|Blob|Integer|Int|Boolean|Byte|Long|Char|character|Real|Date|String|DateTime|Time|Decimal|Dec|UnsignedInteger|UnsignedInt|UInt|Double|UnsignedLong|ULong)";
            range.SetStyle(SourceFileStylesClass.SaddleBrown, @"\b" + pbDataTypes + @"\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //string highlighting
            range.SetStyle(SourceFileStylesClass.BrownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'", RegexOptions.Compiled);

            //number highlighting
            range.SetStyle(SourceFileStylesClass.MagentaStyle, @"\b(\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+|true|false)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //True False highlighting
            //            range.SetStyle(SourceFileStylesClass.TrueStyle, @"\b(true)\b",RegexOptions.IgnoreCase);
            //            range.SetStyle(SourceFileStylesClass.FalseStyle, @"\b(false)\b",RegexOptions.IgnoreCase);

            string SpecialWords = @"(?<range>\b(readonly|ref|public|Private|protected|function|subroutine|global|within|from|type)\b|\btype\b( variables)?|[\r\n]event|[\r\n]end (event|subroutine|function|type|variables))";
            range.SetStyle(SourceFileStylesClass.PbSpecialSourceKeywordsStyle, SpecialWords, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string PbKeyWords = @"\b(on|call|super|HALT|CLOSE|parent|FETCH|OPEN|DECLARE|post|create|Destroy|AND|IF|ELSEIF|END|END IF|ELSE|THEN|FOR|NEXT|TO|choose|case|DO|UNTIL|LOOP|this|not|null|INTO|OR|return|while|GOTO)\b";
            range.SetStyle(SourceFileStylesClass.PbKeywordsStyle, PbKeyWords, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //Function Call Highlight
            range.SetStyle(SourceFileStylesClass.FunctionCallStyle, @"(?<range>(?!" + PbKeyWords + "|" + pbDataTypes + "|\b(WHERE|SET|CASE|WHEN|ON)\b" + @")\b\w+)\s*?\(", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //Type names highlighting            
            range.SetStyle(SourceFileStylesClass.TypeStyle, @"(type){1}\s(?<range>\w+?)\b\s(from)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //Event Function names highlighting          
            //range.SetStyle(SourceFileStylesClass.EventFunctionStyle, @"[\r\n](event|function|public|protected|private){1}\s?(" + pbDataTypes + @"\s+)?(?<range>\w+?)\b", RegexOptions.IgnoreCase);

            range.SetStyle(SourceFileStylesClass.SqlCoulmnsBrown, @"(?<=\.object\.)(?<range>\w+)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            //range.SetStyle(SourceFileStylesClass.CommentsStyle, @"\.(?<range>object)\.", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            //keyword highlighting
            //fctb.Range.SetStyle(BlueStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b");

            if (isSql) { SetSqlStyle(range); }
            else
            {
                //Detect sql code ranges
                IEnumerable<Range> sqlRanges = range.GetRanges(@"(?si)[\r\n][\s]*\b(execute|commit|rollback|select|SELECTBLOB|update|delete|INSERT)\b(.|[\r\n])*?((?!((COMMIT )|(ROLBACK )))using)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                foreach (Range sqlRange in sqlRanges) { SetSqlStyle(sqlRange); }
            }

            // clear comment range from another styles
            string commentRegex = @"(?si)(?!('|""))(?<range>((//|--)[^\n]*)\n|(/\*(.|[\r\n])*?\*/).*?)(?!('|""))";
            foreach (Range range1 in fctb.GetRanges(commentRegex))
            {
                range1.ClearStyle(StyleIndex.All);
            }
            //comment highlighting
            range.SetStyle(SourceFileStylesClass.CommentsStyle, commentRegex, RegexOptions.Singleline | RegexOptions.Compiled);

            //clear folding markers
            range.ClearFoldingMarkers();

            if (!isSql)
            {
                //set folding markers            
                range.SetFoldingMarkers(@"[\n](?<range>event)\b", @"[\n](?<range>end event)\b", RegexOptions.Compiled); //allow to collapse #region blocks
                range.SetFoldingMarkers(@"[\n](?<range>(public|private|protected) (function|subroutine))\b", @"[\n](?<range>end (function|subroutine|prototypes))\b", RegexOptions.IgnoreCase | RegexOptions.Compiled); //allow to collapse #region blocks            
                range.SetFoldingMarkers(@"(?si)[\r\n][\s]*(?<range>((?!END\s)IF))(.|\s])*?THEN(\s*\/\/(.|\s)*?)?", @"(?si)(([\r\n][\s]*)(?<range>END IF)(.|[\r\n])*?)|((?<range>THEN )(\s+.|[^\/]\w+)+?)", RegexOptions.IgnoreCase | RegexOptions.Compiled); //allow to collapse #region blocks            
                range.SetFoldingMarkers(@"(?si)[\r\n][\s]*(?<range>(DO))(.|[\r\n])*?", @"(?si)(([\r\n][\s]*(?<range>LOOP)(.|[\r\n])*?))", RegexOptions.IgnoreCase | RegexOptions.Compiled); //allow to collapse #region blocks            
                range.SetFoldingMarkers(@"(?si)[\r\n][\s]*(?<range>(FOR))(.|[\r\n])*?", @"(?si)(([\r\n][\s]*(?<range>NEXT)(.|[\r\n])*?))", RegexOptions.IgnoreCase | RegexOptions.Compiled); //allow to collapse #region blocks            
                range.SetFoldingMarkers(@"(?si)[\r\n][\s]*(?<range>(choose case))(.|[\r\n])*?", @"(?si)(([\r\n][\s]*(?<range>end choose)(.|[\r\n])*?))", RegexOptions.IgnoreCase | RegexOptions.Compiled); //allow to collapse #region blocks            
                //range.SetFoldingMarkers(@"(?si)[\r\n][\s]*(?<range>((?!choose )case))(.|[\r\n])*?", @"(?si)(([\r\n][\s]*(?<range>(end choose)|((?!choose )case))(.|[\r\n])*?))",  RegexOptions.IgnoreCase | RegexOptions.Singleline); //allow to collapse #region blocks            
                range.SetFoldingMarkers(@"(?si)[\r\n][\s]*(?<range>(COMMIT|ROLBACK|UPDATE|SELECT|SELECTBLOB|INSERT|DELETE|EXECUTE|PREPARE))(.|[\r\n])*?", @"(?si)([\r\n])[\s]*?\b(?<range>((?!((COMMIT )|(ROLBACK )))USING)(.|[\r\n])*?)", RegexOptions.IgnoreCase | RegexOptions.Compiled); //allow to collapse #region blocks            
            }
            range.EndUpdate();


        }

        private static void SetSqlStyle(Range sqlRange)
        {
            const string SqlMainKeyWords = @"\b(ADD|DROP|GO|EXECUTE|IMMEDIATE|SELECT|SELECTBLOB|INTO|where|group|by|Having|with|order|FROM|AS|DELETE|UPDATE|USING|commit|rollback|INSERT|SET|
                                                    drop|CREATE|TABLE|index|alter|View|function|create or replace|column|trigger)\b";
            const string SqlOtherKeyWords = @"\b(REFERENCES|ASC|DESC|FOREIGN|CHECK|dbo|CONSTRAINT|DISTINCT|AND|IF|END|ELSE|when|case|LEFT|RIGHT|OUTER|JOIN|not|in|is|null|union|all|INNER|ON|LIKE|OR|between|new|old|BEGIN|end|THEN|PROCEDURE|OPEN|LOOP|fetch|exit|EXCEPTION|CURSOR)\b";

            sqlRange.ClearStyle(SourceFileStylesClass.PbKeywordsStyle);
            sqlRange.ClearStyle(SourceFileStylesClass.TypeStyle);
            sqlRange.ClearStyle(SourceFileStylesClass.EventFunctionStyle);
            sqlRange.ClearStyle(SourceFileStylesClass.PbSpecialSourceKeywordsStyle);
            sqlRange.SetStyle(SourceFileStylesClass.SqlMainKeywordsStyle, SqlMainKeyWords, RegexOptions.IgnoreCase);
            sqlRange.SetStyle(SourceFileStylesClass.SqlOtherKeywordsStyle, SqlOtherKeyWords, RegexOptions.IgnoreCase);
            sqlRange.SetStyle(SourceFileStylesClass.SqlCoulmnsBrown, @"(?<=(\s|\(|\=|\,)+\w+\.)(?<range>\w+\b)", RegexOptions.IgnoreCase);
        }

        public static void SetSourceRulesHighlightRegex(object sender, string searchedText)
        {
            var fctb = (FastColoredTextBox)sender;
            var range = fctb.Range;
            if (!string.IsNullOrEmpty(searchedText)) { range.SetStyle(SourceFileStylesClass.SearchRelultInSourcePanelStyle, searchedText, RegexOptions.IgnoreCase); }
        }
    }
}