using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using magicedit;
using System.IO;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class SerializationTest
    {

        [TestMethod]
        public void TestConfigSerialization()
        {
            Config config = new Config();

            //Add a string const
            Text text = new Text("Hello");
            config.AddStringConst("hello", text);

            //Add a scheme
            Scheme scheme = new Scheme("MyScheme");
            scheme.Code = "scheme MyScheme { }";
            config.AddScheme(scheme);

            //Add an object
            magicedit.Object @object = new magicedit.Object("obj_id", "obj_name");
            @object.Scheme = scheme;
            @object.Variables.Add(new ObjectVariable("number", "n", 10));
            config.AddObject(@object);

            config.Save("myconfig.mec");

            Config loadedConfig = Config.Load("myconfig.mec");

            Assert.IsNotNull(loadedConfig.GetStringConstByName("hello"));

            Scheme loadedScheme = loadedConfig.GetSchemeByName("MyScheme");
            Assert.IsNotNull(loadedScheme);
            Assert.AreEqual(scheme.Code, loadedScheme.Code);

            magicedit.Object loadedObject = loadedConfig.GetObjectById(@object.Id);
            Assert.IsNotNull(loadedObject);
            Assert.AreSame(loadedObject.Scheme, loadedScheme);

            ObjectVariable loadedVariable = loadedObject.GetVariableByName("n", loadedConfig);
            Assert.IsNotNull(loadedVariable);
            Assert.AreEqual((Int64)10, (Int64)loadedVariable.Value);

        }
    }
}
