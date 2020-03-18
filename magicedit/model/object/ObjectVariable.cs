namespace magicedit
{
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