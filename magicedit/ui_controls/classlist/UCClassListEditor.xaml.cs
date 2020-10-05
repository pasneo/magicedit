using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.AddIn;
using System.ComponentModel.Design;
using magicedit.language.classlist;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for UCClassListEditor.xaml
    /// </summary>
    public partial class UCClassListEditor : UserControl
    {
        private ClassList currentClassList = null;
        private Class currentClass = null;

        private Config Config
        {
            get { return Project.Current?.Config; }
        }

        ITextMarkerService textMarkerService;

        DispatcherTimer syntaxCheckTimer = new DispatcherTimer();

        public UCClassListEditor()
        {

            using (var stream = new System.IO.StreamReader("language/classlist/classlist_lang_highlight.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.RegisterHighlighting("Classes", new string[0],
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance));
                }
            }

            InitializeComponent();
            InitializeTextMarkerService();

            syntaxCheckTimer.Interval = new TimeSpan(0, 0, 1);
            syntaxCheckTimer.Tick += SyntaxCheckTimer_Tick;

        }

        void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(tbCode.Document);
            tbCode.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            tbCode.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)tbCode.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        void RemoveAllErrorHighlight()
        {
            textMarkerService.RemoveAll(m => true);
            //tbErrorList.Text = "";
            lbErrorList.Items.Clear();
        }

        void AddErrorHighlight(int start, int length)
        {
            ITextMarker marker = textMarkerService.Create(start, length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
        }


        

        private bool IsClassExisting(string varName)
        {
            if (currentClassList != null)
                return currentClassList.IsClassExisting(varName);
            throw new Exception("Error");
        }

        /* ************************* */

        private void GenerateCode()
        {

            string code = "";
            tbCode.Text = code;

            if (Config.ClassLists.Count == 0) return;

            //Generate code with T4 template
            classlist_lang_gen_template codeGenerator = new classlist_lang_gen_template();
            tbCode.Text = codeGenerator.TransformText();

        }

        public class MyErrorListener : IAntlrErrorListener<int>
        {
            public List<ErrorDescriptor> Errors { get; private set; }

            public MyErrorListener(List<ErrorDescriptor> errors)
            {
                Errors = errors;
            }

            public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] int offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
            {
                if (e != null && e.OffendingToken != null)
                    Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine, e.OffendingToken.StartIndex, e.OffendingToken.StopIndex));
                else
                    Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine));
            }
        }

        public class MyTokenErrorListener : IAntlrErrorListener<IToken>
        {
            public List<ErrorDescriptor> Errors { get; private set; }

            public MyTokenErrorListener(List<ErrorDescriptor> errors)
            {
                Errors = errors;
            }

            public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
            {
                if (offendingSymbol != null)
                    Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine, offendingSymbol.StartIndex, offendingSymbol.StopIndex));
                else
                    Errors.Add(new ErrorDescriptor(msg, line, charPositionInLine));
            }
        }

        public static IParseTree GenerateAST(string code, ref List<ErrorDescriptor> errors)
        {
            var inputStream = new AntlrInputStream(code);
            var lexer = new classlist_langLexer(inputStream);

            lexer.RemoveErrorListeners();
            MyErrorListener errorListener = new MyErrorListener(errors);
            lexer.AddErrorListener(errorListener);

            var tokenStream = new CommonTokenStream(lexer);
            var parser = new classlist_langParser(tokenStream);

            parser.RemoveErrorListeners();
            MyTokenErrorListener errorListener2 = new MyTokenErrorListener(errors);
            parser.AddErrorListener(errorListener2);

            var context = parser.doc();

            return context;
        }

        private int GetFirstIndexInLine(TextEditor textBox, int line)
        {
            return textBox.Document.Lines[line].Offset;
        }

        private int GetLengthOfLine(TextEditor textBox, int line)
        {
            return textBox.Document.Lines[line].Length;
        }

        private void HighlightErrors(List<ErrorDescriptor> errors)
        {
            foreach (var error in errors)
            {
                if (error.HasExactPosition)
                    AddErrorHighlight(error.StartPosition, error.EndPosition - error.StartPosition + 1);
                else
                    AddErrorHighlight(GetFirstIndexInLine(tbCode, error.Line - 1) + error.Position, 1);
                //tbErrorList.Text += "[" + error.Line + ":" + error.Position + "] " + error.Message + "\n";

                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = "[" + error.Line + ":" + error.Position + "] " + error.Message;
                listBoxItem.Tag = error;
                lbErrorList.Items.Add(listBoxItem);
            }
        }

        private List<ErrorDescriptor> CheckSyntax(out IParseTree tree)
        {
            List<ErrorDescriptor> syntaxErrors = new List<ErrorDescriptor>();
            tree = GenerateAST(tbCode.Text, ref syntaxErrors);

            return syntaxErrors;
        }

        private List<ErrorDescriptor> CheckSemantics(IParseTree tree)
        {
            var visitor = new Classlist_lang_SemanticVisitor();
            visitor.Visit(tree);

            return visitor.Errors;
        }

        private List<ErrorDescriptor> CheckAndSortAllErrors()
        {

            IParseTree tree;
            List<ErrorDescriptor> errors = CheckSyntax(out tree);
            errors = errors.Concat(CheckSemantics(tree)).ToList();

            errors.Sort((e1, e2) => ((e1.Line == e2.Line) ? e1.Position.CompareTo(e2.Position) : e1.Line.CompareTo(e2.Line)));

            return errors;
        }

        //Return true if generation is successfull
        private bool GenerateDataFromCode()
        {

            Config.ClassLists.Clear();

            //List<ErrorDescriptor> syntaxErrors = new List<ErrorDescriptor>();
            IParseTree tree;
            List<ErrorDescriptor> syntaxErrors = CheckSyntax(out tree);
            List<ErrorDescriptor> semanticErrors = CheckSemantics(tree);

            if (syntaxErrors.Count > 0 || semanticErrors.Count > 0)
            {
                RemoveAllErrorHighlight();
                HighlightErrors(syntaxErrors);
                HighlightErrors(semanticErrors);
                return false;
            }

            var visitor = new Classlist_langVisitor();
            visitor.Visit(tree);

            //TODO: generate content based on Config.Classlists (that was filled by the code compiler above)

            return true;
        }

        private void SyntaxCheckTimer_Tick(object sender, EventArgs e)
        {
            syntaxCheckTimer.Stop();

            RemoveAllErrorHighlight();

            List<ErrorDescriptor> errors = CheckAndSortAllErrors();
            HighlightErrors(errors);
        }

        private void lbErrorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbErrorList.SelectedItem != null)
            {
                ListBoxItem selectedItem = (ListBoxItem)lbErrorList.SelectedItem;
                ErrorDescriptor error = (ErrorDescriptor)selectedItem.Tag;
                tbCode.ScrollTo(error.Line, error.Position);

                if (error.HasExactPosition)
                {
                    try
                    {
                        tbCode.Select(error.StartPosition, error.EndPosition - error.StartPosition + 1);
                    }
                    catch (ArgumentOutOfRangeException) { }
                }
                else
                {

                    try
                    {
                        tbCode.Select(GetFirstIndexInLine(tbCode, error.Line - 1) + error.Position, 1);
                    }
                    catch (ArgumentOutOfRangeException) { }
                }

                lbErrorList.SelectedItem = null;
            }
        }

        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text)
            {
                Text = text;
            }

            public ImageSource Image => null;

            public string Text { get; set; }

            public object Content
            {
                get { return this.Text; }
            }

            public object Description
            {
                get { return "Description for " + this.Text; }
            }

            public double Priority => 1.0;

            public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, this.Text);
            }
        }

        CompletionWindow completionWindow;

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            syntaxCheckTimer.Stop();
            syntaxCheckTimer.Start();
        }

        private void tbCode_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                // Open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(tbCode.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                completionWindow.Show();
                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
        }
    }
}
