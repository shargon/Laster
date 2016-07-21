using Laster.Core.Helpers;
using Laster.Core.Interfaces;
using ScintillaNET;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;

namespace Laster.Core.Forms
{
    public partial class FScriptForm : FRememberForm
    {
        string _Value;
        string DefinedClasses = "";
        ScriptHelper.ScriptOptions _Options;
        public string Value
        {
            get { return _Value; }
            set
            {
                tEdit.Text = value;
                toolStripButton2_Click(null, null);
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            _Value = tEdit.Text;
            base.OnClosing(e);
        }
        public FScriptForm(ScriptHelper.ScriptOptions opt) : base()
        {
            InitializeComponent();

            _Options = opt;
            tEdit.Lexer = Lexer.Cpp;
            tEdit.Margins[0].Type = MarginType.Number;
            tEdit.Margins[0].Width = 35;

            tEdit.SetSelectionBackColor(true, Color.FromArgb(200, 227, 255));

            tEdit.StyleResetDefault();
            tEdit.Styles[Style.Default].Font = "Consolas";
            tEdit.Styles[Style.Default].Size = 10;
            tEdit.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            tEdit.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            tEdit.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            tEdit.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            tEdit.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            tEdit.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            tEdit.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            tEdit.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            tEdit.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            tEdit.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            tEdit.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            tEdit.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            tEdit.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            tEdit.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            tEdit.Styles[Style.Cpp.GlobalClass].ForeColor = Color.FromArgb(43, 145, 175);

            // Set the keywords
            tEdit.SetKeywords(0, "dynamic abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while get set");
            tEdit.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void Decimal Double DateTime TimeSpan Type");

            // Load from assemblies defined Types
            List<string> add = new List<string>();
            foreach (string file in _Options.IncludeFiles)
            {
                bool isGac;
                string f = ScriptHelper.ScriptOptions.GetFile(file, out isGac);

                try
                {
                    Assembly asm = null;
                    Type tAttr = typeof(Attribute);

                    if (!isGac) asm = Assembly.LoadFrom(f);
                    else asm = Assembly.LoadWithPartialName(Path.GetFileNameWithoutExtension(f));

                    if (asm != null)
                    {
                        foreach (Type t in asm.GetExportedTypes())
                        {
                            if (!t.IsPublic) continue;

                            foreach (string nm in _Options.IncludeUsings)
                            {
                                if (t.Namespace != nm) continue;

                                string nam = t.Name;
                                if (t.IsGenericType)
                                {
                                    int ix = nam.IndexOf("`");
                                    if (ix != -1)
                                    {
                                        nam = nam.Substring(0, ix);
                                    }
                                }

                                if (!add.Contains(nam))
                                    add.Add(nam);

                                if (tAttr.IsAssignableFrom(t))
                                {
                                    if (nam.EndsWith("Attribute"))
                                    {
                                        nam = nam.Remove(nam.Length - 9, 9);
                                        if (!add.Contains(nam))
                                            add.Add(nam);
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            DefinedClasses = string.Join(" ", add);
            tEdit.SetKeywords(3, DefinedClasses);
            tEdit.Invalidate();
        }
        int maxLineNumberCharLength;
        void scintilla_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = tEdit.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            tEdit.Margins[0].Width = tEdit.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }
        void toolStripButton3_Click(object sender, EventArgs e) { Close(); }
        void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
            if (tError.Tag == null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        void toolStripButton2_Click(object sender, EventArgs e)
        {
            tError.Tag = null;
            tError.Text = "";

            try
            {
                ScriptHelper helper = ScriptHelper.CreateFromString(tEdit.Text, _Options);
                if (helper != null)
                {
                    string n = DefinedClasses;
                    foreach (Type t in helper.Assembly.GetExportedTypes())
                    {
                        if (n != "") n += " ";
                        n += t.Name;
                    }
                    tEdit.SetKeywords(3, n);
                    tEdit.Invalidate();

                    IScriptProcess sc = helper.CreateNewInstance<IScriptProcess>();
                    if (sc == null) throw (new Exception("Error"));
                }
                else throw (new Exception("Error"));

                tError.ForeColor = Color.Green;
                tError.Text = "OK";

                if (sender != null) tError.Visible = true;
            }
            catch (Exception ex)
            {
                tError.Visible = true;
                tError.Tag = true;
                tError.ForeColor = Color.Red;
                tError.Text = ex.Message;
            }
        }
        void tError_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tError.Visible = false;
        }
        void FScriptForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        Close();
                        break;
                    }
                case Keys.F1:
                case Keys.F5:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        toolStripButton2_Click(sender, e);
                        break;
                    }
                case Keys.F3:
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        toolStripButton1_Click(null, null);
                        break;
                    }
            }
        }
    }
}