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
    }
}
