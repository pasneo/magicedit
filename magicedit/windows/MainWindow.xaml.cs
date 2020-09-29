﻿using System;
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

        public enum Menus
        {
            Visuals,
            StringConsts,
            SquareTypes,
            Character,
            Map
        }

        public static MainWindow Current { get; private set; }

        MainUserControl currentMainUC = null;

        public MainWindow()
        {
            Current = this;

            Project project = new Project();
            project.SetAsCurrent();
            Project.Current.Config.Visuals.Add(new Visual("axe", "C:\\Users\\Mathias\\Desktop\\Tananyag\\Diplomatervezes 1\\Project\\sample resources\\axe.png"));
            Project.Current.Config.Visuals.Add(new Visual("sword", "C:\\Users\\Mathias\\Desktop\\Tananyag\\Diplomatervezes 1\\Project\\sample resources\\sword.jpg"));

            InitResources();
            
            InitializeComponent();

            tviVisuals.Tag = new UCVisualManager();
            tviTexts.Tag = new UCStringConstManager();
            tviSquareTypes.Tag = new UCSquareTypeManager();
            tviCharacterSettings.Tag = new UCCharacterManager();
            tviMap.Tag = new UCMapManager();
            tviObjectSchemes.Tag = new UCObjectSchemeManager();
            tviObjects.Tag = new UCObjectManager();

        }

        private void InitResources()
        {
            Config config = Project.Current.Config;

            config.Visuals.Add(new Visual("forest", "images/terrain/forest.png", true));
            config.Visuals.Add(new Visual("grass", "images/terrain/grass.png", true));
            config.Visuals.Add(new Visual("village", "images/terrain/village.png", true));
            config.Visuals.Add(new Visual("desert", "images/terrain/desert.png", true));

            Project.Current.Config.Map.SquareTypes.Add(new SquareType("forest", config.GetVisualById("forest")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("grass", config.GetVisualById("grass")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("village", config.GetVisualById("village")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("desert", config.GetVisualById("desert")));
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

        public void SelectMenu(Menus menu)
        {
            switch(menu)
            {
                case Menus.SquareTypes:
                    tviSquareTypes.IsSelected = true;
                    break;
            }
        }

    }
}
