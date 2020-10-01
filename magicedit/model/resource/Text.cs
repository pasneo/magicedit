using Newtonsoft.Json;
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

        [JsonConstructor]
        public Text(string id, string content)
        {
            ID = id;
            Content = content;
        }

        public string CropContent(int n)
        {
            if (Content == null) return "";
            if (Content.Length <= n) return Content.Replace("\n", " ").Replace("\r", "");
            return Content.Replace("\n", " ").Replace("\r", "").Substring(0, n) + "...";
        }

    }
}
