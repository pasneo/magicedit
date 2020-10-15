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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace magicedit
{
    /// <summary>
    /// Interaction logic for UCETextListElem.xaml
    /// </summary>
    public partial class UCETextListElem : UserControl
    {

        public event RoutedEventHandler DeleteClicked;

        private string _content;
        public new string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                tbContent.Text = Content;
            }
        }

        public UCETextListElem()
        {
            InitializeComponent();
        }

        private void hyDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteClicked?.Invoke(this, e);
        }
    }
}
