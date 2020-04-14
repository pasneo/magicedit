using System;
using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class CommandUnitTests
    {

        [TestMethod]
        public void TestActionHandling()
        {

            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandAddAction("grab"));
            f.AddCommand(new CommandAddAction("examine"));

            f.AddCommand(new CommandRemoveAction("grab"));
            f.AddCommand(new CommandAddAction("drop"));

            MapObject @object = new MapObject();
            @object.Name = "some_object";
            @object.Variables.Add(new ObjectVariable("logical", "isDwarf", false));
            @object.Variables.Add(new ObjectVariable("logical", "isElf", false));

            @object.Scheme = new Scheme("some_scheme");
            @object.Scheme.CompiledScheme = new CompiledScheme();
            @object.Scheme.CompiledScheme.AddAction(new SchemeFunction("grab"));
            @object.Scheme.CompiledScheme.AddAction(new SchemeFunction("examine"));
            @object.Scheme.CompiledScheme.AddAction(new SchemeFunction("drop"));

            Config config = new Config();
            config.AddScheme(@object.Scheme);

            Game game = new Game(config);
            game._AddObject(@object);

            //Execute function on object
            f.Execute(@object, new Character(), game);

            Assert.IsFalse(@object.AvailableActions.Contains("grab"));
            Assert.IsTrue(@object.AvailableActions.Contains("examine"));
            Assert.IsTrue(@object.AvailableActions.Contains("drop"));

        }

        [TestMethod]
        public void TestClasslistHandling()
        {

            ClassList races = new ClassList("races");
            races.AddClass(new Class("dwarf"));
            races.AddClass(new Class("elf"));

            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandOf("race", "actor", "_0"));
            f.AddCommand(new CommandEquals("_0", "dwarf", "isDwarf"));
            f.AddCommand(new CommandOf("race", "actor", "_0"));
            f.AddCommand(new CommandEquals("_0", "elf", "isElf"));

            MapObject @object = new MapObject();
            @object.Name = "some_object";
            @object.Scheme = new Scheme("some_scheme");
            @object.Variables.Add(new ObjectVariable("logical", "isDwarf", false));
            @object.Variables.Add(new ObjectVariable("logical", "isElf", false));

            Config config = new Config();
            config.AddScheme(@object.Scheme);
            config.AddClassList(races);

            Game game = new Game(config);
            game._AddObject(@object);

            Character actor = new Character();
            actor.Scheme = new Scheme("character");
            actor.Variables.Add(new ObjectVariable("races", "race", races.GetClassByName("dwarf")));

            //Execute function on object
            f.Execute(@object, actor, game);

            Assert.AreEqual(@object.GetVariableByName("isDwarf").Value, true);
            Assert.AreEqual(@object.GetVariableByName("isElf").Value, false);

        }

        [TestMethod]
        public void TestCommandIs()
        {
            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandIs("me", "john", "isJohn"));
            f.AddCommand(new CommandIs("me", "james", "isJames"));
            f.AddCommand(new CommandIs("me", "happy", "isHappy"));
            f.AddCommand(new CommandIs("me", "angry", "isAngry"));
            f.AddCommand(new CommandIs("me", "forbidden", "isForbidden"));

            //Create sample object
            MapObject @object = new MapObject();
            @object.Name = "john";
            @object.Attributes.Add(new ObjectAttribute(ObjectAttributeType.Set, "happy"));
            @object.Attributes.Add(new ObjectAttribute(ObjectAttributeType.Forbid, "forbidden"));
            @object.Scheme = new Scheme("john_scheme");
            @object.Variables.Add(new ObjectVariable("logical", "isJohn", false));
            @object.Variables.Add(new ObjectVariable("logical", "isJames", false));
            @object.Variables.Add(new ObjectVariable("logical", "isHappy", false));
            @object.Variables.Add(new ObjectVariable("logical", "isAngry", false));
            @object.Variables.Add(new ObjectVariable("logical", "isForbidden", false));

            Config config = new Config();
            config.AddScheme(@object.Scheme);
            Game game = new Game(config);

            game._AddObject(@object);

            //Execute function on object
            f.Execute(@object, new Character(), game);

            Assert.AreEqual(@object.GetVariableByName("isJohn").Value, true);
            Assert.AreEqual(@object.GetVariableByName("isJames").Value, false);
            Assert.AreEqual(@object.GetVariableByName("isHappy").Value, true);
            Assert.AreEqual(@object.GetVariableByName("isAngry").Value, false);
            Assert.AreEqual(@object.GetVariableByName("isForbidden").Value, false);

        }
    }
}
