using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit.language.scheme
{
    public class SchemeLangSemanticVisitor : scheme_langBaseVisitor<object>
    {
        public Scheme Scheme { get; set; }
        public Config Config { get; set; }

        public SchemeLangSemanticVisitor(Scheme scheme, Config config)
        {
            Scheme = scheme;
            Config = config;
        }

    }
}
