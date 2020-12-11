using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public enum Menus
        {
            Visuals,
            StringConsts,
            SquareTypes,
            Character,
            Map,
            Schemes,
            Objects,
            Items,
            Spells
        }

        public static MainWindow Current { get; private set; }

        MainUserControl currentMainUC = null;

        private string SavePath { get; set; }

        private EditorErrorDescriptor CurrentError { get; set; }

        public MainWindow()
        {
            Current = this;

            Project project = new Project();
            project.SetAsCurrent();

            InitializeComponent();
            
            tviVisuals.Tag = new UCVisualManager();
            tviTexts.Tag = new UCStringConstManager();
            tviSquareTypes.Tag = new UCSquareTypeManager();
            tviCharacterSettings.Tag = new UCCharacterManager();
            tviMap.Tag = new UCMapManager();
            tviSchemes.Tag = new UCObjectSchemeManager();
            tviObjects.Tag = new UCObjectManager("Objects", ObjectTypeTags.MapObject);
            tviItems.Tag = new UCObjectManager("Items", ObjectTypeTags.Item);
            tviSpells.Tag = new UCObjectManager("Spells", ObjectTypeTags.Spell);
            tviClasslists.Tag = new UCClasslistManager();

        }

        private void mRun_Click(object sender, RoutedEventArgs e)
        {
            //Project.Current.Game = new Game(Project.Current.Config);
            //GameWindow gameWindow = new GameWindow();
            //gameWindow.Show();

            GameInitWindow gameInitWindow = new GameInitWindow(Project.Current.Config);
            bool? result = gameInitWindow.ShowDialog();

            if (result == true)
            {
                Game game = gameInitWindow.Game;

                Project.Current.Game = game;

                GameWindow gameWindow = new GameWindow(game);
                gameWindow.ShowDialog();
            }

        }

        private void tvExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem selectedItem = (TreeViewItem)tvExplorer.SelectedItem;

            currentMainUC?.Close();
            gridMainUC.Children.Clear();

            if (selectedItem != null && selectedItem.Tag != null && (selectedItem.Tag is MainUserControl))
            {
                MainUserControl uc = (MainUserControl)selectedItem.Tag;

                uc.Open(CurrentError);
                CurrentError = null;

                gridMainUC.Children.Add(uc);
                currentMainUC = uc;
            }

        }

        private void SelectMenu(TreeViewItem item)
        {
            item.IsSelected = false;
            item.IsSelected = true;
        }

        public void SelectMenu(Menus menu, EditorErrorDescriptor error = null)
        {
            CurrentError = error;

            switch(menu)
            {
                case Menus.SquareTypes:
                    SelectMenu(tviSquareTypes);
                    break;
                case Menus.Schemes:
                    SelectMenu(tviSchemes);
                    break;
                case Menus.Objects:
                    SelectMenu(tviObjects);
                    break;
                case Menus.Items:
                    SelectMenu(tviItems);
                    break;
                case Menus.Spells:
                    SelectMenu(tviSpells);
                    break;
            }
        }
        
        private void mOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                SavePath = openFileDialog.FileName;
                Project.Current.Config = Config.Load(SavePath);
                currentMainUC?.Close();
                gridMainUC.Children.Clear();
            }
        }

        private void mSave_Click(object sender, RoutedEventArgs e)
        {
            SaveAs(SavePath);
        }

        private bool SaveAs(string path = null)
        {
            if (path == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON file (*.json)|*.json";

                if (saveFileDialog.ShowDialog() == true)
                    SavePath = saveFileDialog.FileName;
                else
                    return false;
            }

            if (SavePath != null)
            {
                Project.Current?.Config?.Save(SavePath);
            }

            return true;
        }

        private void mSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void mValidate_Click(object sender, RoutedEventArgs e)
        {
            lbErrorList.Items.Clear();

            var eeds = Project.Current.Config.Validate();

            foreach(var eed in eeds)
            {
                AddError(eed);
            }
        }

        private void AddError(EditorErrorDescriptor eed)
        {
            ListBoxItem item = new ListBoxItem();

            item.Content = eed.Message;

            if (eed.ErrorType == ErrorTypes.Error)
                item.Foreground = Brushes.Red;
            else if (eed.ErrorType == ErrorTypes.Warning)
                item.Foreground = Brushes.Orange;

            item.Tag = eed;

            lbErrorList.Items.Add(item);
        }

        private void lbErrorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBoxItem)lbErrorList.SelectedItem;

            lbErrorList.SelectedItem = null;

            if (item == null) return;

            var eed = (EditorErrorDescriptor)item.Tag;
            eed.NavigateToSource();
        }

        private void mExit_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private bool Exit()
        {
            var result = MessageBox.Show("Do you want to save the project?", "Save Project", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Cancel) return false;

            if (result == MessageBoxResult.Yes)
            {
                if (!SaveAs(SavePath)) return false;
            }

            Application.Current.Shutdown();
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!Exit()) e.Cancel = true;
        }

    }
}
