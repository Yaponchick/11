using Microsoft.VisualStudio.TestTools.UnitTesting;
using _11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _11.Tests
{
    [TestClass()]
    public class LogicTests
    {
        [TestMethod()]
        public void CountingBTest()
        {
            double A = 1000;
            double B = 50;

            int result = Logic.CountingB(A, B);

            Assert.AreEqual(47, result);
        }
        [TestMethod()]
        public void CountingC()
        {
            double A = 1000;
            double C = 2000;

            int result = Logic.CountingC(A, C);

            Assert.AreEqual(36, result);
        }
    }
}