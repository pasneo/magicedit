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

    public class SchemeNameCollisionEED : EditorErrorDescriptor
    {
        public Scheme Scheme { get; set; }

        private int index = 0;
        private List<Scheme> Schemes = new List<Scheme>();

        public bool ContainsCollision => Schemes.Count > 1;

        public SchemeNameCollisionEED(Scheme scheme)
        {
            Message = $"More than one scheme exists with the name '{scheme.Name}'";
            ErrorType = ErrorTypes.Error;
            Schemes.Add(scheme);
        }

        public void AddScheme(Scheme scheme)
        {
            Schemes.Add(scheme);
        }

        public override void NavigateToSource()
        {
            Scheme = Schemes[index];
            MainWindow.Current.SelectMenu(MainWindow.Menus.Schemes, this);
            index = (index + 1) % Schemes.Count;
        }
    }

    public class SchemeObjectNameCollisionEED : EditorErrorDescriptor
    {
        public Object Object { get; set; }
        public Scheme Scheme { get; set; }

        bool NavigateToScheme = true;

        public SchemeObjectNameCollisionEED(Object @object, Scheme scheme)
        {
            Object = @object;
            Scheme = scheme;

            Message = $"An object and a scheme exists with the same name '{scheme.Name}'";
            ErrorType = ErrorTypes.Warning;
        }

        public override void NavigateToSource()
        {
            if (NavigateToScheme) MainWindow.Current.SelectMenu(MainWindow.Menus.Schemes, new SchemeNameCollisionEED(Scheme));
            else
            {
                if (Object.TypeTag == ObjectTypeTags.MapObject) MainWindow.Current.SelectMenu(MainWindow.Menus.Objects, this);
                else if (Object.TypeTag == ObjectTypeTags.Item) MainWindow.Current.SelectMenu(MainWindow.Menus.Items, this);
                else if (Object.TypeTag == ObjectTypeTags.Spell) MainWindow.Current.SelectMenu(MainWindow.Menus.Spells, this);
            }
            NavigateToScheme = !NavigateToScheme;
        }
    }

    public class ObjectNameCollisionEED : EditorErrorDescriptor
    {
        public Object Object { get; set; }

        private int index = 0;
        private List<Object> Objects = new List<Object>();

        public bool ContainsCollision => Objects.Count > 1;

        public ObjectNameCollisionEED(Object @object)
        {
            Message = $"More than one object exists with the ID '{@object.Id}'";
            ErrorType = ErrorTypes.Error;
            Objects.Add(@object);
        }

        public void AddObject(Object @object)
        {
            Objects.Add(@object);
        }

        public override void NavigateToSource()
        {
            Object = Objects[index];

            if (Object.TypeTag == ObjectTypeTags.MapObject) MainWindow.Current.SelectMenu(MainWindow.Menus.Objects, this);
            else if (Object.TypeTag == ObjectTypeTags.Item) MainWindow.Current.SelectMenu(MainWindow.Menus.Items, this);
            else if (Object.TypeTag == ObjectTypeTags.Spell) MainWindow.Current.SelectMenu(MainWindow.Menus.Spells, this);

            index = (index + 1) % Objects.Count;
        }
    }

}
