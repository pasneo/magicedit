﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Item = magicedit.Object;
using Spell = magicedit.Object;

namespace magicedit
{
    public class Character : MapObject
    {
        public List<Item> Items;
        public List<Spell> Spells;
    }
}