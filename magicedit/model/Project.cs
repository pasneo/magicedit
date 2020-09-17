using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class Project
    {

        public static Project Current { get; set; }

        public Config Config { get; set; }

        /* *** */

        public Project()
        {
            Config = new Config();
        }

        public void SetAsCurrent()
        {
            Current = this;
        }

    }
}
