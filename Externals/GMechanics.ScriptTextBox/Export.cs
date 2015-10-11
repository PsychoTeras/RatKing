﻿using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace GMechanics.ScriptTextBox
{
    /// <summary>
    /// Exports colored text as HTML
    /// </summary>
    /// <remarks>At this time only TextStyle renderer is supported. Other styles is not exported.</remarks>
    public class ExportToHTML
    {
        /// <summary>
        /// Use nbsp; instead space
        /// </summary>
        public bool UseNbsp { get; set; }
        /// <summary>
        /// Use nbsp; instead space in beginning of line
        /// </summary>
        public bool UseForwardNbsp { get; set; }
        /// <summary>
        /// Use original font
        /// </summary>
        public bool UseOriginalFont { get; set; }
        /// <summary>
        /// Use style tag instead style attribute
        /// </summary>
        public bool UseStyleTag { get; set; }
        /// <summary>
        /// Use br tag instead \n
        /// </summary>
        public bool UseBr { get; set; }

        ScriptTextBoxControl tb;

        public ExportToHTML()
        {
            UseNbsp = true;
            UseOriginalFont = true;
            UseStyleTag = true;
            UseBr = true;
        }

        public string GetHtml(ScriptTextBoxControl tb)
        {
            this.tb = tb;
            Range sel = new Range(tb);
            sel.SelectAll();
            return GetHtml(sel);
        }
        
        public string GetHtml(Range r)
        {
            this.tb = r.tb;
            Dictionary<StyleIndex, object> styles = new Dictionary<StyleIndex, object>();
            StringBuilder sb = new StringBuilder();
            StringBuilder tempSB = new StringBuilder();
            StyleIndex currentStyleId = StyleIndex.None;
            r.Normalize();
            int currentLine = r.Start.iLine;
            styles[currentStyleId] = null;
            //
            if(UseOriginalFont)
                sb.AppendFormat("<font style=\"font-family: {0}, monospace; font-size: {1}px; line-height: {2}px;\">", r.tb.Font.Name, r.tb.CharHeight - r.tb.LineInterval, r.tb.CharHeight);
            //
            bool hasNonSpace = false;
            foreach (Place p in r)
            {
                Char c = r.tb[p.iLine][p.iChar];
                if (c.style != currentStyleId)
                {
                    Flush(sb, tempSB, currentStyleId);
                    currentStyleId = c.style;
                    styles[currentStyleId] = null;
                }

                if(p.iLine!=currentLine)
                {
                    for (int i = currentLine; i < p.iLine;i++)
                        tempSB.AppendLine(UseBr?"<br>":"");
                    currentLine = p.iLine;
                    hasNonSpace = false;
                }
                switch(c.c)
                {
                    case ' ':
                        if((hasNonSpace || !UseForwardNbsp) && !UseNbsp)
                            goto default;

                        tempSB.Append("&nbsp;");                            
                        break;
                    case '<':
                        tempSB.Append("&lt;"); break;
                    case '>':
                        tempSB.Append("&gt;"); break;
                    case '&':
                        tempSB.Append("&amp;"); break;
                    default:
                        hasNonSpace = true;
                        tempSB.Append(c.c); break;
                }
            }
            Flush(sb, tempSB, currentStyleId);

            if (UseOriginalFont)
                sb.AppendLine("</font>");

            //build styles
            if (UseStyleTag)
            {
                tempSB.Length = 0;
                tempSB.AppendLine("<style type=\"text/css\">");
                foreach (var styleId in styles.Keys)
                    tempSB.AppendFormat(".fctb{0}{{ {1} }}\r\n", GetStyleName(styleId), GetCss(styleId));
                tempSB.AppendLine("</style>");

                sb.Insert(0, tempSB.ToString());
            }

            return sb.ToString();
        }

        private string GetCss(StyleIndex styleIndex)
        {
            List<Style> styles = new List<Style>();
            //find text renderer
            TextStyle textStyle = null;
            int mask = 1;
            bool hasTextStyle = false;
            for (int i = 0; i < tb.Styles.Length; i++)
            {
                if (tb.Styles[i] != null && ((int)styleIndex & mask) != 0)
                if (tb.Styles[i].IsExportable)
                {
                    var style = tb.Styles[i];
                    styles.Add(style);

                    bool isTextStyle = style is TextStyle;
                    if (isTextStyle)
                        if (!hasTextStyle || tb.AllowSeveralTextStyleDrawing)
                        {
                            hasTextStyle = true;
                            textStyle = style as TextStyle;
                        }
                }
                mask = mask << 1;
            }
            //add TextStyle css
            string result = "";
            
            if (!hasTextStyle)
            {
                //draw by default renderer
                result = tb.DefaultStyle.GetCSS();
            }
            else
            {
                result = textStyle.GetCSS();
            }
            //add non TextStyle css
            foreach(var style in styles)
            if (style != textStyle)
                result += style.GetCSS();

            return result;
        }

        public static string GetColorAsString(Color color)
        {
            if(color==Color.Transparent)
                return "";
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }

        string GetStyleName(StyleIndex styleIndex)
        {
            return styleIndex.ToString().Replace(" ", "").Replace(",", "");
        }

        private void Flush(StringBuilder sb, StringBuilder tempSB, StyleIndex currentStyle)
        {
            //find textRenderer
            //var textStyle = styles.Where(s => s is TextStyle).FirstOrDefault();
            //
            if (tempSB.Length == 0)
                return;
            if (UseStyleTag)
                sb.AppendFormat("<font class=fctb{0}>{1}</font>", GetStyleName(currentStyle), tempSB.ToString());
            else
            {
                string css = GetCss(currentStyle);
                if(css!="")
                    sb.AppendFormat("<font style=\"{0}\">", css);
                sb.Append(tempSB.ToString());
                if (css != "")
                    sb.Append("</font>");
            }
            tempSB.Length = 0;
        }
    }
}
