namespace magicedit
{

    public class VariableTypes
    {
        public static string Logical = "logical";
        public static string Number = "number";
        public static string Text = "text";
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