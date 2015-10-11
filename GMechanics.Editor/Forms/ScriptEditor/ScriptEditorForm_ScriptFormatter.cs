using System;
using System.Windows.Forms;

namespace GMechanics.Editor.Forms.ScriptEditor
{
    public partial class ScriptEditorForm
    {
        private void OpenNewBlock()
        {
            teScript.BeginUpdate();
            teScript.InsertText("}");
            teScript.SelectionStart--;
            teScript.EndUpdate();
        }

        private void FormatNewBlock(KeyPressEventArgs e)
        {
            if (teScript.SelectionStart > 0)
            {
                string text = teScript.Text;

                char l = '\0';
                int lIdx = teScript.SelectionStart, lSpaces = 1;
                while (--lIdx >= 0)
                {
                    char c = text[lIdx];
                    if (!char.IsWhiteSpace(c))
                    {
                        l = c;
                        break;
                    }
                }

                if (l == '{' && lIdx > 0)
                {
                    lSpaces = lIdx;
                    while (--lSpaces > 0)
                    {
                        char c = text[lSpaces];
                        if (c == '\n')
                        {
                            lSpaces = (lIdx - (lSpaces + 1)) / 4 + 1;
                            break;
                        }
                    }
                }

                char r = '\0';
                int rIdx = teScript.SelectionStart, length = text.Length;
                if (rIdx < length)
                {
                    do
                    {
                        char c = text[rIdx];
                        if (!char.IsWhiteSpace(c))
                        {
                            r = c;
                            break;
                        }
                    } while (++rIdx < length);
                }

                if (l == '{' && r == '}')
                {
                    e.Handled = true;
                    teScript.BeginUpdate();
                    teScript.SelectionStart = lIdx + 1;
                    teScript.SelectionLength = rIdx - lIdx;
                    teScript.SelectedText = string.Format("{0}{1}{0}{2}",
                        Environment.NewLine, "".PadLeft(lSpaces, '\t'),
                        "}".PadLeft(lSpaces, '\t'));
                    teScript.SelectionStart = lIdx + 3 + (lSpaces * 4);
                    teScript.EndUpdate();
                }
            }
        }

        private void BeginQuotedString()
        {
            teScript.BeginUpdate();
            teScript.InsertText("\"");
            teScript.SelectionStart--;
            teScript.EndUpdate();
        }

        private void OpenNewParentheses()
        {
            teScript.BeginUpdate();
            teScript.InsertText(")");
            teScript.SelectionStart--;
            teScript.EndUpdate();
        }

        private void TeScriptKeyPressing(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '{':
                    {
                        OpenNewBlock();
                        break;
                    }
                case '\r':
                    {
                        FormatNewBlock(e);
                        break;
                    }
                case '"':
                    {
                        BeginQuotedString();
                        break;
                    }
                case '(':
                    {
                        OpenNewParentheses();
                        break;
                    }
            }
        }
    }
}