﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;


namespace magicedit.language.classlist
{

    public class Classlist_lang_SemanticVisitor : classlist_langBaseVisitor<object>
    {

        private Config Config
        {
            get { return Project.Current?.Config; }
        }

        public List<ErrorDescriptor> Errors { get; set; } = new List<ErrorDescriptor>();

        List<ClassList> classLists = new List<ClassList>();
        ClassList currentClassList = null;

        public override object VisitClassListName([NotNull] classlist_langParser.ClassListNameContext context)
        {
            //check if classlist already exists

            bool exists = false;

            foreach (var classList in classLists)
            {
                if (classList.Name == context.GetText())
                {
                    Errors.Add(new ErrorDescriptor("Classlist '" + classList.Name + "' already exists", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                ClassList classList = new ClassList();
                classList.Name = context.GetText();
                classLists.Add(classList);
                currentClassList = classList;
            }

            return base.VisitClassListName(context);
        }

        public override object VisitClassListShownName([NotNull] classlist_langParser.ClassListShownNameContext context)
        {
            string stringConst = context.GetText();

            if (Config.GetStringConstByName(stringConst) == null)
            {
                Errors.Add(new ErrorDescriptor($"String const '{stringConst}' does not exist.", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));
            }

            return base.VisitClassListShownName(context);
        }

        public override object VisitClassName([NotNull] classlist_langParser.ClassNameContext context)
        {
            //Check if class already exists in current list

            if (currentClassList == null) return base.VisitClassName(context);

            bool exists = false;

            foreach (var @class in currentClassList.Classes)
            {
                if (@class.Name == context.GetText())
                {
                    Errors.Add(new ErrorDescriptor("Class '" + @class.Name + "' already exists in classlist", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                Class c = new Class();
                c.Name = context.GetText();
                currentClassList.AddClass(c);
            }

            return base.VisitClassName(context);
        }

        public override object VisitClassShownName([NotNull] classlist_langParser.ClassShownNameContext context)
        {
            string stringConst = context.GetText();

            if (Config.GetStringConstByName(stringConst) == null)
            {
                Errors.Add(new ErrorDescriptor($"String const '{stringConst}' does not exist.", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));
            }

            return base.VisitClassShownName(context);
        }

        public override object VisitAbility([NotNull] classlist_langParser.AbilityContext context)
        {
            //check if ability exists

            bool exists = false;
            foreach (var ability in Config.CharacterConfig.Abilities)
            {
                if (ability.Name == context.GetText())
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                Errors.Add(new ErrorDescriptor("Ability '" + context.GetText() + "' does not exist", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));

            return base.VisitAbility(context);
        }

        public override object VisitAttribute([NotNull] classlist_langParser.AttributeContext context)
        {
            //check if attribute exists

            /*bool exists = false;
            foreach (var attribute in Config.CharacterConfig.SpecialAttributes)
            {
                if (attribute.VarName == context.GetText())
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                Errors.Add(new ErrorDescriptor("Attribute '" + context.GetText() + "' does not exist", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));
                */
            return base.VisitAttribute(context);
        }

        public override object VisitItem([NotNull] classlist_langParser.ItemContext context)
        {
            string itemName = context.GetText();

            foreach (var obj in Config.Objects)
            {
                if (obj.TypeTag == ObjectTypeTags.Item && obj.Name == itemName) return base.VisitItem(context);
            }

            Errors.Add(new ErrorDescriptor($"Item '{itemName}' does not exist.", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));

            return base.VisitItem(context);
        }

        public override object VisitSpell([NotNull] classlist_langParser.SpellContext context)
        {
            string spellName = context.GetText();

            var spell = Config.GetObjectById(spellName);

            if (spell == null || spell.TypeTag != ObjectTypeTags.Spell)
            {
                Errors.Add(new ErrorDescriptor($"Spell '{spellName}' does not exist.", context.Start.Line, context.Start.Column, context.Start.StartIndex, context.Start.StopIndex));
            }

            return base.VisitSpell(context);
        }

    }

}
