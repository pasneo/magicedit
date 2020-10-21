using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public class AbilityMaxValue : INotifyPropertyChanged
    {
        public int MaxValue { get; set; }

        private int _remainingValue;

        public int RemainingValue
        {
            get { return _remainingValue; }
            set
            {
                _remainingValue = value;
                NotifyPropertyChanged();
            }
        }

        public AbilityMaxValue(int max)
        {
            MaxValue = max;
            RemainingValue = max;
        }

        public void Reset()
        {
            RemainingValue = MaxValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    /// <summary>
    /// Interaction logic for UCEAbilityModifier.xaml
    /// </summary>
    public partial class UCEAbilityRow : UserControl
    {
        
        private ObjectVariable Ability { get; set; }
        private AbilityMaxValue AbilityMaxValue { get; set; }

        public UCEAbilityRow(ObjectVariable ability, AbilityMaxValue abilityMaxValue)
        {
            InitializeComponent();

            Ability = ability;
            AbilityMaxValue = abilityMaxValue;

            tbAbilityName.Text = ability.Name;
            nbAbilityValue.Value = (int)ability.Value;

            AbilityMaxValue.RemainingValue -= (int)ability.Value;
        }

        private void nbAbilityValue_ValueIncreased(UCEAbilityButton sender)
        {
            if (AbilityMaxValue.RemainingValue <= 0)
            {
                nbAbilityValue.Value--;
                return;
            }
            int value = (int)Ability.Value;
            value++;
            Ability.Value = value;
            AbilityMaxValue.RemainingValue--;
        }

        private void nbAbilityValue_ValueDecreased(UCEAbilityButton sender)
        {
            int value = (int)Ability.Value;
            value--;
            Ability.Value = value;
            AbilityMaxValue.RemainingValue++;
        }
    }
}
