
using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class CommandUnitTests
    {

        [TestMethod]
        public void TestAbilities()
        {

            ClassList races = new ClassList("races");
            races.AddClass(new Class("dwarf", new AbilityModifier("STR", 2)));
            races.AddClass(new Class("elf", new AbilityModifier("STR", 3)));

            ClassList jobs = new ClassList("jobs");
            jobs.AddClass(new Class("smith", new AbilityModifier("STR", 4)));
            jobs.AddClass(new Class("mage", new AbilityModifier("STR", -1)));

            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandSetVariable("strength", "STR"));

            MapObject @object = new MapObject();
            @object.Name = "some_object";
            @object.Scheme = new Scheme("some_scheme");

            @object.Variables.Add(new ObjectVariable(VariableTypes.Ability, "STR", 5));                 //STR = 5
            @object.Variables.Add(new ObjectVariable(VariableTypes.Number, "strength", 0));

            Config config = new Config();
            config.AddScheme(@object.Scheme);
            config.AddClassList(races);
            config.AddClassList(jobs);

            @object.Variables.Add(new ObjectVariable("races", "race", races.GetClassByName("dwarf")));  //dwarf: STR+2
            @object.Variables.Add(new ObjectVariable("jobs", "job", jobs.GetClassByName("mage")));      //mage:  STR-1

            Game game = new Game(config);
            game._AddObject(@object);

            //Execute function on object
            f.Execute(@object, new Character(), game);

            Assert.AreEqual(6, @object.GetVariableByName("strength", config).Value);

        }

        [TestMethod]
        public void TestSpellHandling()
        {
            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandAddSpell("actor", "fireball"));
            f.AddCommand(new CommandRemoveSpell("actor", "snowstorm"));
            f.AddCommand(new CommandAddSpell("actor", "snowstorm"));
            f.AddCommand(new CommandRemoveSpell("actor", "fireball"));

            Object fireball = new Object("fireball", "fireball");
            Object snowstorm = new Object("snowstorm", "snowstorm");

            MapObject @object = new MapObject();
            @object.Name = "some_object";
            @object.Scheme = new Scheme("some_scheme");

            Character actor = new Character();
            actor.Scheme = new Scheme();
            actor.AddSpell(fireball);

            Config config = new Config();
            config.AddScheme(@object.Scheme);

            Game game = new Game(config);
            game._AddObject(@object);
            game._AddObject(fireball);
            game._AddObject(snowstorm);

            //Execute function on object
            f.Execute(@object, actor, game);

            Assert.IsTrue(actor.KnowsSpell("snowstorm"));
            Assert.IsFalse(actor.KnowsSpell("fireball"));
        }

        [TestMethod]
        public void TestCommandAddItem()
        {
            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandAddItem("actor", "1", "sword"));
            f.AddCommand(new CommandCreateVariable("number", "n"));
            f.AddCommand(new CommandSetVariable("n", "3"));
            f.AddCommand(new CommandAddItem("actor", "n", "apple"));

            Scheme itemScheme = new Scheme("item");

            Object sword = new Object("sword", "sword");
            Object apple = new Object("apple", "apple");
            sword.Scheme = apple.Scheme = itemScheme;

            MapObject @object = new MapObject();
            @object.Name = "some_object";
            @object.Scheme = new Scheme("some_scheme");

            Character actor = new Character();
            actor.Scheme = new Scheme();

            Config config = new Config();
            config.AddScheme(@object.Scheme);
            config.AddScheme(itemScheme);

            Game game = new Game(config);
            game._AddObject(@object);
            game._AddObject(sword);
            game._AddObject(apple);

            actor.AddItem(game, sword);

            //Execute function on object
            f.Execute(@object, actor, game);

            Assert.AreEqual(2, actor.CountItem("sword"));
            Assert.AreEqual(3, actor.CountItem("apple"));
        }

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

            Assert.AreEqual(@object.GetVariableByName("isDwarf", config).Value, true);
            Assert.AreEqual(@object.GetVariableByName("isElf", config).Value, false);

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

            Assert.AreEqual(@object.GetVariableByName("isJohn", config).Value, true);
            Assert.AreEqual(@object.GetVariableByName("isJames", config).Value, false);
            Assert.AreEqual(@object.GetVariableByName("isHappy", config).Value, true);
            Assert.AreEqual(@object.GetVariableByName("isAngry", config).Value, false);
            Assert.AreEqual(@object.GetVariableByName("isForbidden", config).Value, false);

        }

        [TestMethod]
        public void TestClassvars()
        {
            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandOf("race", "actor", "ThorinRace"));
            f.AddCommand(new CommandIs("ThorinRace", "dwarf", "ThorinIsDwarf"));
            f.AddCommand(new CommandOf("friend", "actor", "_0"));
            f.AddCommand(new CommandOf("race", "_0", "ThorinsFriendRace"));
            f.AddCommand(new CommandIs("ThorinsFriendRace", "elf", "ThorinsFriendIsElf"));
            f.AddCommand(new CommandOf("race", "me", "LegolasRace"));
            f.AddCommand(new CommandIs("LegolasRace", "elf", "LegolasIsElf"));

            Character Thorin = new Character("Thorin", "Thorin");
            Object Legolas = new Object("Legolas", "Legolas");

            ClassList classList = new ClassList("races");
            Class dwarf = new Class("dwarf");
            Class elf = new Class("elf");
            classList.AddClass(dwarf);
            classList.AddClass(elf);

            Thorin.Scheme = new Scheme("thorin_scheme");
            Thorin.Variables.Add(new ObjectVariable("races", "race", dwarf));       //Thorin is dwarf
            Thorin.Variables.Add(new ObjectVariable("object", "friend", Legolas));  //Legolas is Thorin's friend

            Legolas.Scheme = new Scheme("legolas_scheme");
            Legolas.Variables.Add(new ObjectVariable("races", "race", elf));        //Legolas is elf

            Legolas.Variables.Add(new ObjectVariable("logical", "ThorinIsDwarf", false));
            Legolas.Variables.Add(new ObjectVariable("logical", "ThorinsFriendIsElf", false));
            Legolas.Variables.Add(new ObjectVariable("logical", "LegolasIsElf", false));
            Legolas.Variables.Add(new ObjectVariable("races", "ThorinRace", false));
            Legolas.Variables.Add(new ObjectVariable("races", "ThorinsFriendRace", false));
            Legolas.Variables.Add(new ObjectVariable("races", "LegolasRace", false));

            Config config = new Config();
            config.AddClassList(classList);

            Game game = new Game(config);

            f.Execute(Legolas, Thorin, game);

            Assert.AreEqual(true, Legolas.GetVariableByName("ThorinIsDwarf", config).Value);
            Assert.AreEqual(true, Legolas.GetVariableByName("ThorinsFriendIsElf", config).Value);
            Assert.AreEqual(true, Legolas.GetVariableByName("LegolasIsElf", config).Value);

            Assert.AreEqual(dwarf, Legolas.GetVariableByName("ThorinRace", config).Value);
            Assert.AreEqual(elf, Legolas.GetVariableByName("ThorinsFriendRace", config).Value);
            Assert.AreEqual(elf, Legolas.GetVariableByName("LegolasRace", config).Value);

        }

        [TestMethod]
        public void TestSetOfMultiLevel()
        {
            SchemeFunction f = new SchemeFunction();
            f.AddCommand(new CommandOf("friend", "Jack", "_0"));        // strength of friend of Jack = 10
            f.AddCommand(new CommandSetOf("strength", "_0", "10"));

            Object dummy = new Object();
            dummy.Id = "dummy";

            Object friend = new Object();
            friend.Scheme = new Scheme("friend_scheme");
            friend.Variables.Add(new ObjectVariable("number", "strength", "5"));

            MapObject jack = new MapObject();
            jack.Scheme = new Scheme("jack_scheme");
            jack.Id = "Jack";
            jack.Variables.Add(new ObjectVariable("object", "friend", friend));

            Config config = new Config();
            config.AddScheme(friend.Scheme);
            config.AddScheme(jack.Scheme);

            Game game = new Game(config);
            game._AddObject(friend);
            game._AddObject(jack);
            game._AddObject(dummy);

            f.Execute(dummy, new Character(), game);

            Assert.AreEqual(10, friend.Variables[0].Value);

        }

    }
}
