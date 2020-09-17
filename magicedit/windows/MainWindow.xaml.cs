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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainUserControl currentMainUC = null;

        public MainWindow()
        {
            InitializeComponent();

            Project project = new Project();
            project.SetAsCurrent();

            Project.Current.Config.Visuals.Add(new Visual("axe", "C:\\Users\\Mathias\\Desktop\\Tananyag\\Diplomatervezes 1\\Project\\sample resources\\axe.png"));

            tviVisuals.Tag = new UCVisualManager();
            tviTexts.Tag = new UCStringConstManager();
            
        }

        private void mRun_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
        }

        private void tvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = (TreeViewItem)tvExplorer.SelectedItem;

            currentMainUC?.Close();
            gridMainUC.Children.Clear();

            if (selectedItem != null && selectedItem.Tag != null && (selectedItem.Tag is MainUserControl))
            {
                MainUserControl uc = (MainUserControl)selectedItem.Tag;
                uc.Open();

                gridMainUC.Children.Add(uc);
                currentMainUC = uc;
            }

        }
    }
}
