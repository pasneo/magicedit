using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace magicedit
{

    public abstract class ParameterRow : UserControl
    {
        protected ObjectVariable Value { get; private set; }

        public ParameterRow(string paramName, ObjectVariable value)
        {
            Value = value;
        }
    }

    public class ParameterRowFactory
    {

        public static ParameterRow Create(ObjectVariable param, ObjectVariable value, Config config)
        {
            if (param.Type == VariableTypes.Number || param.Type == VariableTypes.Ability) return new UCEParameterRow_Number(param.Name, value);
            if (param.Type == VariableTypes.Logical) return new UCEParameterRow_Logical(param.Name, value);
            if (param.Type == VariableTypes.Text) return new UCEParameterRow_StringConst(param.Name, value);
            
            return new UCEParameterRow_Object(param.Name, value, config);
        }

    }
}
