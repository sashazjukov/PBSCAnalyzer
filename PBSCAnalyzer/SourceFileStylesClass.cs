using System.Drawing;
using System.Drawing.Drawing2D;
using FastColoredTextBoxNS;

namespace PBSCAnalyzer
{
    public static class SourceFileStylesClass
    {
        public static readonly TextStyle BrownStyle = new TextStyle(Brushes.Chocolate, null, FontStyle.Regular);
        public static readonly TextStyle EventFunctionStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        public static readonly TextStyle CommentsStyle = new TextStyle(new SolidBrush(Color.FromArgb(218, 150, 150, 150)), null, FontStyle.Italic);
        public static readonly TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        public static readonly TextStyle TrueStyle = new TextStyle(new SolidBrush(Color.FromArgb(255, 255, 0, 247)), null, FontStyle.Regular);
        public static readonly TextStyle FalseStyle = new TextStyle(new SolidBrush(Color.FromArgb(185, 255, 0, 247)), null, FontStyle.Regular);
        public static readonly TextStyle SaddleBrown = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Bold);
        public static readonly TextStyle SqlCoulmnsBrown = new TextStyle(new SolidBrush(Color.FromArgb(255, 39, 134, 148)), null, FontStyle.Bold);
        public static readonly MarkerStyle SearchRelultInSourcePanelStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(80, Color.LightSkyBlue)));

        public static MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        public static MarkerStyle WhiteRangeStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.FromArgb(254, 172, 172))));
        public static  TextStyle TypeStyle = new TextStyle(Brushes.Black, null, FontStyle.Bold);
        public static Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(80, Color.Lime)));        
        public static TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        public static TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        public static TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Bold);
        public static TextStyle OperatorStyle = new TextStyle(Brushes.Red, null, FontStyle.Bold);

       public static Font SourceEditorFont = new Font("Consolas", 9.75f, FontStyle.Regular);
       public static Style FunctionCallStyle =new TextStyle(new SolidBrush(Color.FromArgb(255,145,0,198)), null, FontStyle.Bold | FontStyle.Italic);
        public static Style SqlMainKeywordsStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        public static Style PbKeywordsStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        public static Style SqlOtherKeywordsStyle = new TextStyle(new SolidBrush(Color.FromArgb(209,48,54,255)), null, FontStyle.Regular);
        public static Style PbSpecialSourceKeywordsStyle = new TextStyle(Brushes.Brown, (Brush)null, FontStyle.Italic);
    }
}