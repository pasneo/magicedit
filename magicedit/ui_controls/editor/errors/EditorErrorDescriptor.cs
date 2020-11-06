using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace magicedit
{
    public enum ErrorTypes
    {
        Error,
        Warning
    }

    public abstract class EditorErrorDescriptor
    {
        public string Message { get; set; }
        public ErrorTypes ErrorType { get; set; }

        public EditorErrorDescriptor()
        {
        }

        public EditorErrorDescriptor(string message, ErrorTypes errorType)
        {
            Message = message;
            ErrorType = errorType;
        }
        

        public abstract void NavigateToSource();
    }

    public class InvalidSchemeEED : EditorErrorDescriptor
    {
        public Scheme Scheme { get; set; }

        public InvalidSchemeEED(Scheme scheme)
        {
            Scheme = scheme;

            ErrorType = ErrorTypes.Error;

            if (scheme.Name == null || scheme.Name.Length == 0)
            {
                Message = $"Scheme without name";
            }
            else
            {
                Message = $"Scheme '{scheme.Name}' is invalid";
            }
        }

        public override void NavigateToSource()
        {
            MainWindow.Current.SelectMenu(MainWindow.Menus.Schemes, this);
        }

    }

}
