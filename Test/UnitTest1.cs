using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCharacter()
        {
            Character character = new Character();
            character.AddItem(new Object("i1", "item"));
            character.AddItem(new Object("i2", "item"));
            Assert.AreEqual(character.CountItem("item"), 2);
            Assert.AreEqual(character.CountItem("something"), 0);
        }
    }
}
