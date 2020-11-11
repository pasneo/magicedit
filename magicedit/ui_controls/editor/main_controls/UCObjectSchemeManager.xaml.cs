using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.SharpDevelop.Editor;
using magicedit.language;
using magicedit.language.scheme;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for UCStringConstManager.xaml
    /// </summary>
    public partial class UCObjectSchemeManager : MainUserControl
    {

        ITextMarkerService textMarkerService;

        public DispatcherTimer SyntaxCheckTimer = new DispatcherTimer();

        public UCObjectSchemeManager()
        {

            using (var stream = new System.IO.StreamReader("language/scheme/scheme_lang_highlight.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.RegisterHighlighting("Scheme", new string[0],
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance));
                }
            }

            InitializeComponent();
            InitializeTextMarkerService();

            SyntaxCheckTimer.Interval = new TimeSpan(0, 0, 1);
            SyntaxCheckTimer.Tick += SyntaxCheckTimer_Tick;
            SyntaxCheckTimer.Stop();
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

        public override void Open(EditorErrorDescriptor eed)
        {
            RefreshList();

            if (eed != null)
            {
                Scheme scheme = null;

                if (eed is InvalidSchemeEED)
                    scheme = ((InvalidSchemeEED)eed).Scheme;
                else if (eed is SchemeNameCollisionEED)
                    scheme = ((SchemeNameCollisionEED)eed).Scheme;

                if (scheme != null) SelectScheme(scheme);
            }
            else
                GenerateDataFromCode();
        }

        public override void Close()
        {
            GenerateDataFromCode();
        }

        public void SelectScheme(Scheme scheme)
        {
            foreach(var item in list.Items)
            {
                ListBoxItem listBoxItem = (ListBoxItem)item;
                if (listBoxItem.Tag == scheme)
                {
                    list.SelectedItem = item;
                    return;
                }
            }
        }

        public void RefreshList()
        {
            var selectedItemTag = ((ListBoxItem)list.SelectedItem)?.Tag;

            list.Items.Clear();

            var schemes = Project.Current?.Config.Schemes;
            if (schemes == null) return;

            foreach (var scheme in schemes)
            {
                var item = AddListBoxItem(scheme);
                if (scheme == selectedItemTag) list.SelectedItem = item;
            }
            
        }

        private ListBoxItem AddListBoxItem(Scheme scheme)
        {

            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = scheme;

            listBoxItem.Content = scheme.Name;

            list.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            tbCode.Text = "";
            tbCode.IsEnabled = false;
            bDelete.IsEnabled = false;
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                tbCode.IsEnabled = true;
                bDelete.IsEnabled = true;

                Scheme scheme = (Scheme)((ListBoxItem)list.SelectedItem).Tag;
                
                tbCode.Text = scheme.Code;

                SyntaxCheckTimer.Start();
            }
            else ClearInfo();
        }

        private void listTexts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string newId = IdGenerator.Generate("scheme");
            while (Project.Current.Config.GetSchemeByName(newId) != null) newId = IdGenerator.Generate("scheme");

            Scheme scheme = new Scheme(newId);
            Project.Current.Config.AddScheme(scheme);

            var item = AddListBoxItem(scheme);

            list.SelectedItem = item;
        }

        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Scheme scheme = (Scheme)item.Tag;
            scheme.Code = tbCode.Text;
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)list.SelectedItem;
            Scheme scheme = (Scheme)item.Tag;

            Project.Current.Config.RemoveScheme(scheme);

            list.SelectedItem = null;
            list.Items.Remove(item);
        }

        /* CODE HANDLING */

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

        private List<ErrorDescriptor> SortAllErrors(List<ErrorDescriptor> errors)
        {
            errors.Sort((e1, e2) => ((e1.Line == e2.Line) ? e1.Position.CompareTo(e2.Position) : e1.Line.CompareTo(e2.Line)));
            return errors;
        }

        //Return true if generation is successfull
        public bool GenerateDataFromCode()
        {
            RemoveAllErrorHighlight();

            var item = ((ListBoxItem)list.SelectedItem);

            if (item == null) return false;

            Scheme scheme = (Scheme)item.Tag;

            scheme.Code = tbCode.Text;

            if (scheme.Code == null || scheme.Code == "") return true;

            var errors = SchemeLang.CompileWithErrors(scheme, Project.Current.Config);

            item.Content = (scheme?.Name == null ? "<unknown>" : scheme.Name);

            if (errors.Count > 0)
            {
                HighlightErrors(errors);
                return false;
            }

            //var visitor = new Classlist_langVisitor();
            //visitor.Visit(tree);

            //Config.ClassLists = visitor.ClassLists;

            return true;
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

        private void SyntaxCheckTimer_Tick(object sender, EventArgs e)
        {
            SyntaxCheckTimer.Stop();

            GenerateDataFromCode();
        }

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            SyntaxCheckTimer.Stop();
            SyntaxCheckTimer.Start();
        }

    }
}
