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
    /// Interaction logic for UCEAbilityModifier.xaml
    /// </summary>
    public partial class UCEAbilityModifier : UserControl
    {

        private AbilityModifier AbilityModifier { get; set; }
        private Class Class { get; set; }

        public UCEAbilityModifier(Class @class, string abilityName)
        {
            InitializeComponent();

            Class = @class;

            AbilityModifier = Class.GetAbilityModifier(abilityName);
            if (AbilityModifier == null) AbilityModifier = new AbilityModifier(abilityName);

            tbAbilityName.Text = abilityName;
            nbAbilityValue.Value = AbilityModifier.Value;
        }

        private void nbAbilityValue_ValueChanged(UCENumericButton sender)
        {
            AbilityModifier.Value = nbAbilityValue.Value;

            if (AbilityModifier.Value != 0 && !Class.Modifiers.Contains(AbilityModifier)) Class.AddModifier(AbilityModifier);
            else if (AbilityModifier.Value == 0) Class.Modifiers.Remove(AbilityModifier);
        }
    }
}
