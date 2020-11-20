using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for FuncCodeWindow.xaml
    /// </summary>
    public partial class FuncCodeWindow : Window
    {
        public FuncCodeWindow(string schemeCode, string funcCode)
        {
            using (var stream = new System.IO.StreamReader("language/scheme/func_lang_highlight.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.RegisterHighlighting("Func", new string[0],
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance));
                }
            }

            InitializeComponent();

            tbSchemeCode.Text = schemeCode;
            tbFuncCode.Text = funcCode;
        }
    }
}
