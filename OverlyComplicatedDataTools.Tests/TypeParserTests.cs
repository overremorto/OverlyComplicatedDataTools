using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverlyComplicatedDataTools.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class TypeParserTests
    {
        [TestMethod]
        public void TestParserMap()
        {
        }

        [TestMethod]
        public void TestDetermineTypeInt()
        {
            Assert.AreEqual(typeof(int), TypeParser.DetermineType("123"));
        }


        [TestMethod]
        public void TestDetermineTypeDouble()
        {
            Assert.AreEqual(typeof(double), TypeParser.DetermineType("123.123"));
            Assert.AreEqual(typeof(double), TypeParser.DetermineType("9349.02"));

        }

        [TestMethod]
        public void TestDetermineTypeDateTime()
        {
            Assert.AreEqual(typeof(DateTime), TypeParser.DetermineType("11/20/2019"));
            Assert.AreEqual(typeof(DateTime), TypeParser.DetermineType("2019-11-20"));
        }

        [TestMethod]
        public void TestDetermineTypeGuid()
        {
            Assert.AreEqual(typeof(Guid), TypeParser.DetermineType(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        public void TestDetermineTypeBool()
        {
            Assert.AreEqual(typeof(bool), TypeParser.DetermineType("True"));
            Assert.AreEqual(typeof(bool), TypeParser.DetermineType("False"));
            Assert.AreEqual(typeof(bool), TypeParser.DetermineType("true"));
            Assert.AreEqual(typeof(bool), TypeParser.DetermineType("false"));
        }

        [TestMethod]
        public void TestDetermineTypeChar()
        {
            Assert.AreEqual(typeof(char), TypeParser.DetermineType("a"));
        }

        [TestMethod]
        public void TestDetermineTypeString()
        {
            Assert.AreEqual(typeof(string), TypeParser.DetermineType("test string"));
        }
    }
}
