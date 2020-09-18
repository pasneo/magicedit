using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Text
    {
        public string ID { get; set; }
        public string Content { get; set; }

        public Text()
        {
            Content = "";
        }

        public Text(string content)
        {
            Content = content;
        }

        public Text(string id, string content)
        {
            ID = id;
            Content = content;
        }

    }
}
