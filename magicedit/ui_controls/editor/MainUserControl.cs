using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace magicedit
{
    public class MainUserControl : UserControl
    {
        public virtual void Open(EditorErrorDescriptor eed) { }
        public virtual void Close() { }
    }
}
