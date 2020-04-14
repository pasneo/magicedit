﻿using System;
using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class CommandUnitTests
    {
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
