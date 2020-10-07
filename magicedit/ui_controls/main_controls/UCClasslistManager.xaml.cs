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
    /// Interaction logic for UCClasslistManager.xaml
    /// </summary>
    public partial class UCClasslistManager : MainUserControl
    {
        public UCClasslistManager()
        {
            InitializeComponent();
        }

        public override void Open()
        {
            classListEditor.RebuildTree();
            classListEditor.Refresh();
        }

        private void bView_Click(object sender, RoutedEventArgs e)
        {
            if (classListCodeEditor.Visibility == Visibility.Hidden)
            {
                classListCodeEditor.GenerateCode();
                classListCodeEditor.SyntaxCheckTimer.Start();
                classListCodeEditor.Visibility = Visibility.Visible;
                classListEditor.Visibility = Visibility.Hidden;
            }
            else
            {
                if (!classListCodeEditor.GenerateDataFromCode())
                {
                    var res = MessageBox.Show(
                        "By switching views you may lose any unsaved changes. We recommend fixing errors first. Switch anyway?",
                        "Errors in code",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.No) return;
                }
                classListCodeEditor.SyntaxCheckTimer.Stop();
                classListEditor.RebuildTree();
                classListCodeEditor.Visibility = Visibility.Hidden;
                classListEditor.Visibility = Visibility.Visible;
            }
        }
    }
}
