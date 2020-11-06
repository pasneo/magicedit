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
    /// Interaction logic for UCEValidationMessage.xaml
    /// </summary>
    public partial class UCEValidationMessage : UserControl
    {
        private EditorErrorDescriptor ErrorDescriptor { get; set; }

        public UCEValidationMessage()
        {
            InitializeComponent();
        }

        public UCEValidationMessage(EditorErrorDescriptor errorDescriptor)
        {
            ErrorDescriptor = errorDescriptor;

            InitializeComponent();
        }
    }
}
