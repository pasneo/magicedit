namespace magicedit
{

    public class VariableTypes
    {
        public static string Logical = "logical";   //bool
        public static string Number = "number";     //int
        public static string Text = "text";         //class Text, not string
        public static string Unknown = "unknown";
        public static string Object = "object";
        public static string Ability = "ability";

        public static bool Contains(string type)
        {
            return
                type == Logical ||
                type == Number ||
                type == Text ||
                type == Unknown ||
                type == Object ||
                type == Ability;
        }
    }

    public class ObjectVariable
    {
        public string Type;
        public string Name;
        public object Value;

        public ObjectVariable(string type, string name, object value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        public ObjectVariable Copy()
        {
            ObjectVariable copy = new ObjectVariable(Type, Name, Value);
            return copy;
        }

    }
}