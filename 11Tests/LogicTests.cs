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
        //Тест 1
        //Ситуация - Ежемесячное увеличение превышает 50 рублей
        [TestMethod()]
        public void CountingBTest()
        {
            double A = 1000;
            double B = 50;

            int result = Logic.CountingB(A, B);

            Assert.AreEqual(47, result);
        }
        //Тест 2
        //Ситуация - Размер вклада превышает 2000 рублей
        [TestMethod()]
        public void CountingCTest()
        {
            double A = 1000;
            double C = 2000;

            int result = Logic.CountingC(A, C);

            Assert.AreEqual(36, result);
        }
        //Тест 3
        //Ситуация - Размер вклада равняется размеру суммы, которую необходимо превысить
        [TestMethod()]
        public void CountingCTest1()
        {
            double A = 1000;
            double C = 1000;

            int result = Logic.CountingC(A, C);

            Assert.AreEqual(1, result);
        }
        //Тест 4
        //Ситуация - Сумма, которую необходимо превысить меньше начального вклада
        [TestMethod()]
        public void CountingCTest2()
        {
            double A = 2000;
            double C = 1000;

            int result = Logic.CountingC(A, C);

            Assert.AreEqual(0, result);
        }

    }
}