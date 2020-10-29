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
    /// Interaction logic for GameInitWindow.xaml
    /// </summary>
    public partial class GameInitWindow : Window
    {

        public Game Game { get; private set; }

        public Config Config { get; set; }
        public AbilityMaxValue AbilityMaxValue { get; set; }

        public List<Player> Players = new List<Player>();

        /*
         * Player number can be set (up to the number of spawn points, and minimum 1)
         * 
         * Each player can set:
         *  - abilities (sum of one character's ability points cannot be more than Config.CharacterConfig.StartingAbilityPoints)
         *  - classes (one class must be selected from each classlist)
         *  - visual for character (can only be selected from Config.Visuals)
         * 
         */

        public GameInitWindow()
        {
            InitializeComponent();
        }

        public GameInitWindow(Config config)
        {
            Config = config;
            AbilityMaxValue = new AbilityMaxValue(Config.CharacterConfig.StartingAbilityPoints);

            InitializeComponent();

            lMaxPlayers.Content = Config.Map.GetSpawnerCount();
        }

        private void BuildAbilityList(Character character)
        {
            spAbilities.Children.Clear();

            AbilityMaxValue.Reset();

            foreach (var ability in Config.CharacterConfig.Abilities)
            {
                var characterAbility = character.GetAbilityByName(ability.Name);
                if (characterAbility == null)
                {
                    characterAbility = character.AddAbility(ability.Name);
                }
                spAbilities.Children.Add(new UCEAbilityRow(characterAbility, AbilityMaxValue));
            }
        }

        private void BuildClassList(Character character)
        {
            spClasses.Children.Clear();

            foreach(ClassList classList in Config.ClassLists)
            {
                ObjectVariable classVariable = character.GetVariableByName(classList.Name, Config);
                if (classVariable == null)
                {
                    classVariable = new ObjectVariable(classList.Name, classList.Name, null);
                    character.Variables.Add(classVariable);
                }

                UCEClassSelector classSelector = new UCEClassSelector(classList, classVariable);
                spClasses.Children.Add(classSelector);
            }
        }

        private ListBoxItem AddListBoxItem(Player player)
        {
            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Tag = player;

            listBoxItem.Content = player.Character.Id;

            list.Items.Add(listBoxItem);

            return listBoxItem;
        }

        private void ClearInfo()
        {
            gridInfo.Visibility = Visibility.Hidden;
        }

        private void RefreshInfo()
        {
            if (list.SelectedItem != null)
            {
                Player player = (Player)((ListBoxItem)list.SelectedItem).Tag;
                Character character = player.Character;
                
                iVisualImage.Source = DefaultResources.GetVisualImageOrDefault(character.Visual);

                tbName.Text = character.Id;

                BuildAbilityList(character);
                BuildClassList(character);

                gridInfo.Visibility = Visibility.Visible;
                visualSelector.Visibility = Visibility.Hidden;
            }
            else ClearInfo();
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            string newId = IdGenerator.Generate("ch");
            while (Players.Where(p => p.Character.Id == newId).Count() > 0) newId = IdGenerator.Generate("ch");

            Character character = new Character(newId, newId);
            Player player = new Player(character);

            Players.Add(player);
            var item = AddListBoxItem(player);

            list.SelectedItem = item;

            bStartGame.IsEnabled = true;

            if (Players.Count == Config.Map.GetSpawnerCount())
            {
                bAdd.IsEnabled = false;
            }
        }

        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var item = ((ListBoxItem)list.SelectedItem);
            Player player = (Player)item.Tag;
            player.Character.Id = player.Character.Name = tbName.Text;
            item.Content = tbName.Text;

        }

        private void iVisualImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (visualSelector.IsVisible)
                    visualSelector.Visibility = Visibility.Hidden;
                else
                {
                    visualSelector.RefreshList();
                    visualSelector.Visibility = Visibility.Visible;
                }
            }
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = ((ListBoxItem)list.SelectedItem);
            Player player = (Player)item.Tag;

            list.Items.Remove(item);
            Players.Remove(player);

            list.SelectedItem = null;

            if (Players.Count == 0) bStartGame.IsEnabled = false;

            bAdd.IsEnabled = true;
        }

        private void visualSelector_OnVisualSelected(Visual selectedVisual)
        {
            visualSelector.Visibility = Visibility.Hidden;
            if (list.SelectedItem == null || ((ListBoxItem)list.SelectedItem).Tag == null) return;
            var item = ((ListBoxItem)list.SelectedItem);
            Player player = (Player)item.Tag;
            player.Character.Visual = selectedVisual;
            RefreshInfo();
        }

        private void bStartGame_Click(object sender, RoutedEventArgs e)
        {
            //check if all ids are unique (and not existing in Config's objects also)
            //check if all players have selected their classes

            List<string> Errors = new List<string>();

            foreach(Player player in Players)
            {
                if (Players.Where(p => p.Character.Id == player.Character.Id).Count() != 1)
                {
                    Errors.Add($"More than one player has the same name: '{player.Character.Id}'.");
                }
                if (Config.GetObjectById(player.Character.Id) != null)
                {
                    Errors.Add($"The name '{player.Character.Id}' is already defined elsewhere.");
                }
                
                foreach(var classList in Config.ClassLists)
                {
                    var classVar = player.Character.GetVariableByName(classList.Name, Config);
                    if (classVar == null || classVar.Value == null)
                    {
                        Errors.Add($"{player.Character.Id} has not chosen class for '{classList.Name}'.");
                    }
                }

            }

            Errors = Errors.Distinct().ToList();

            if (Errors.Count > 0)
            {
                MessageBox.Show(string.Join("\r\n", Errors));
            }
            else
            {
                try
                {
                    Game = new Game(Config);
                    Game.SetupPlayers(Players);
                    Game.Start();
                    Log.Write("Game successfully started.");
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
    }
}
