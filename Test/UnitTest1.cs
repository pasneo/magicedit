using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCharacterCountItem()
        {
            Character character = new Character();
            character.AddItemWithoutCopy(new Object("i1", "item"));
            character.AddItemWithoutCopy(new Object("i2", "item"));
            Assert.AreEqual(character.CountItem("item"), 2);
            Assert.AreEqual(character.CountItem("something"), 0);
        }

        [TestMethod]
        public void TestCharacterKnowsSpell()
        {
            Character character = new Character();
            character.AddSpell(new Object("s1", "spell"));
            Assert.AreEqual(character.KnowsSpell("spell"), true);
            Assert.AreEqual(character.KnowsSpell("something"), false);
        }

        [TestMethod]
        public void TestClassList()
        {
            ClassList classList = new ClassList("races");
            classList.AddClass(new Class("dwarf",
                new AttributeModifier(AttributeModifier.AttributeModifierOptions.SET, "use_hammer"),
                new AttributeModifier(AttributeModifier.AttributeModifierOptions.FORBID, "use_bow")
                ));

            classList.AddClass(new Class("elf",
                new AttributeModifier(AttributeModifier.AttributeModifierOptions.SET, "use_bow")
                ));

            Assert.IsTrue(classList.IsClassExisting("dwarf"));
            Assert.IsTrue(classList.IsClassExisting("elf"));
            Assert.IsFalse(classList.IsClassExisting("human"));

            Class dwarf = classList.GetClassByName("dwarf");
            Class elf = classList.GetClassByName("elf");

            Assert.IsTrue(dwarf.ContainsAttributeModifier("use_hammer"));
            Assert.IsTrue(dwarf.ContainsAttributeModifier("use_bow"));

            Assert.AreEqual(dwarf.GetAttributeModifier("use_bow").Option, AttributeModifier.AttributeModifierOptions.FORBID);

        }

    }
}
