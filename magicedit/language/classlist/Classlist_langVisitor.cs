﻿using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicedit.language.classlist
{
    public class Classlist_langVisitor : classlist_langBaseVisitor<object>
    {

        private Config Config
        {
            get { return Project.Current?.Config; }
        }

        ClassList currentClassList = null;
        Class currentClass = null;

        public override object VisitClassListExpr([NotNull] classlist_langParser.ClassListExprContext context)
        {
            //add classlist and save as current classlist

            ClassList classList = new ClassList();
            classList.Name = context.classListName().GetText();

            Config.ClassLists.Add(classList);
            currentClassList = classList;

            return base.VisitClassListExpr(context);
        }

        public override object VisitClassName([NotNull] classlist_langParser.ClassNameContext context)
        {
            //Create new class and save it as current class
            Class c = new Class();
            c.Name = context.STR().GetText();

            if (currentClassList == null)
                throw new Exception("currentClassList == null");

            currentClassList.Classes.Add(c);
            currentClass = c;

            return base.VisitClassName(context);
        }

        public override object VisitAbilityLine([NotNull] classlist_langParser.AbilityLineContext context)
        {
            //add ability modif

            AbilityModifier modif = new AbilityModifier();

            var contextValue = context.value().VALUE();
            if (contextValue is ErrorNodeImpl) return base.VisitAbilityLine(context);

            modif.AbilityName = context.ability().GetText();
            modif.Value = int.Parse(context.value().GetText());

            if (context.abilityModifier().GetText() == "-") modif.Value = -modif.Value;

            currentClass.Modifiers.Add(modif);

            return base.VisitAbilityLine(context);
        }

        public override object VisitAttributeLine([NotNull] classlist_langParser.AttributeLineContext context)
        {
            //add attribute modif

            AttributeModifier modif = new AttributeModifier();

            modif.Option = (context.attributeOption().GetText() == "set" ? AttributeModifier.AttributeModifierOptions.SET : AttributeModifier.AttributeModifierOptions.FORBID);
            modif.AttributeName = context.attribute().GetText();

            currentClass.Modifiers.Add(modif);

            return base.VisitAttributeLine(context);
        }

        public override object VisitItemLine([NotNull] classlist_langParser.ItemLineContext context)
        {
            //TODO: add item modif
            return base.VisitItemLine(context);
        }

    }
}
