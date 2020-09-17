﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit
{
    public class IdGenerator
    {

        private static Random random = new Random();

        public static string Generate(string prefix, int codeLength = 5)
        {
            string code = "";
            for (int i = 0; i < codeLength; ++i) code += (char)random.Next('a', 'z');
            return prefix + "_" + code;
        }
    }
}
