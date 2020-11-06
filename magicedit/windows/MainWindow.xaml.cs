﻿using Microsoft.Win32;
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
            Project.Current.Config.Visuals.Add(new Visual("axe", "C:\\Users\\Mathias\\Desktop\\Tananyag\\Diplomatervezes 1\\Project\\sample resources\\axe.png"));
            Project.Current.Config.Visuals.Add(new Visual("sword", "C:\\Users\\Mathias\\Desktop\\Tananyag\\Diplomatervezes 1\\Project\\sample resources\\sword.jpg"));

            InitResources();
            
            InitializeComponent();

            tviVisuals.Tag = new UCVisualManager();
            tviTexts.Tag = new UCStringConstManager();
            tviSquareTypes.Tag = new UCSquareTypeManager();
            tviCharacterSettings.Tag = new UCCharacterManager();
            tviMap.Tag = new UCMapManager();
            tviSchemes.Tag = new UCObjectSchemeManager();
            tviObjects.Tag = new UCObjectManager("Objects", ObjectTypeTags.MapObject);
            //tviItemCategories.Tag = new UCItemCategoryManager();
            //tviSpellCategories.Tag = new UCSpellCategoryManager();
            tviItems.Tag = new UCObjectManager("Items", ObjectTypeTags.Item);
            tviSpells.Tag = new UCObjectManager("Spells", ObjectTypeTags.Spell);
            tviClasslists.Tag = new UCClasslistManager();

        }

        private void InitResources()
        {
            Config config = Project.Current.Config;

            config.Visuals.Add(new Visual("mage", "images/objects/mage.png", true));
            config.Visuals.Add(new Visual("soldier", "images/objects/soldier.png", true));

            config.Visuals.Add(new Visual("forest", "images/terrain/forest.png", true));
            config.Visuals.Add(new Visual("grass", "images/terrain/grass.png", true));
            config.Visuals.Add(new Visual("village", "images/terrain/village.png", true));
            config.Visuals.Add(new Visual("desert", "images/terrain/desert.png", true));
            config.Visuals.Add(new Visual("stone", "images/terrain/stone.png", true));
            config.Visuals.Add(new Visual("wall", "images/terrain/wall.png", true));
            config.Visuals.Add(new Visual("wall_opening", "images/terrain/wall-opening.png", true));

            Project.Current.Config.Map.SquareTypes.Add(new SquareType("forest", config.GetVisualById("forest")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("grass", config.GetVisualById("grass")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("village", config.GetVisualById("village")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("desert", config.GetVisualById("desert")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("stone", config.GetVisualById("stone")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("wall", config.GetVisualById("wall")));
            Project.Current.Config.Map.SquareTypes.Add(new SquareType("wall_opening", config.GetVisualById("wall_opening")));

            //TODO: remove the following lines
            ClassList races = new ClassList("Races");
            races.Classes.Add(new Class("Dwarf"));
            races.Classes.Add(new Class("Elf"));

            ClassList skills = new ClassList("Skills");
            skills.Classes.Add(new Class("Smith"));
            skills.Classes.Add(new Class("Hunter"));
            skills.Classes.Add(new Class("Potter"));

            Project.Current.Config.ClassLists.Add(races);
            Project.Current.Config.ClassLists.Add(skills);

            Project.Current.Config.CharacterConfig.Abilities.Add(new ObjectVariable("number", "STRENGTH", 0));
            Project.Current.Config.CharacterConfig.Abilities.Add(new ObjectVariable("number", "DEFENSE", 0));
            Project.Current.Config.CharacterConfig.Abilities.Add(new ObjectVariable("number", "DEXTERITY", 0));
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

        private void SaveAs(string path = null)
        {
            if (path == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON file (*.json)|*.json";

                if (saveFileDialog.ShowDialog() == true)
                    SavePath = saveFileDialog.FileName;
                else
                    return;
            }

            if (SavePath != null)
            {
                Project.Current?.Config?.Save(SavePath);
            }

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
    }
}
