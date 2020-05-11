using System;
using magicedit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class SchemeLangOptimizerUnitTest
    {
        [TestMethod]
        public void TestSolveDoubleJumps()
        {

            SchemeFunction func = new SchemeFunction("f");

            func.AddCommand(new CommandJump(3));
            func.AddCommand(new CommandJump(5));
            func.AddCommand(new CommandJump(4));
            func.AddCommand(new CommandJump(2));
            func.AddCommand(new CommandJump(1));
            func.AddCommand(new CommandJumpIfFalse("true", 6));

            SchemeLangOptimizer.SolveDoubleJumps(func);

            int line0 = ((CommandJumpBase)func.Commands[0]).Line;
            int line1 = ((CommandJumpBase)func.Commands[1]).Line;
            int line2 = ((CommandJumpBase)func.Commands[2]).Line;
            int line3 = ((CommandJumpBase)func.Commands[3]).Line;
            int line4 = ((CommandJumpBase)func.Commands[4]).Line;
            int line5 = ((CommandJumpBase)func.Commands[5]).Line;

            Assert.AreEqual(5, line0);
            Assert.AreEqual(5, line1);
            Assert.AreEqual(5, line2);
            Assert.AreEqual(5, line3);
            Assert.AreEqual(5, line4);
            Assert.AreEqual(6, line5);

        }
    }
}
