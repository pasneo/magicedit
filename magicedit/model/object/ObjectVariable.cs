namespace magicedit
{

    public class VariableTypes
    {
        public static string Logical = "logical";   //bool
        public static string Number = "number";     //int
        public static string Text = "text";         //class Text, not string
        public static string Unknown = "unknown";
        public static string Object = "object";
    }

    public class ObjectVariable
    {
        public string Type;
        public string Name;
        public string FullName;
        public object Value;

        public ObjectVariable(string type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

    }
}