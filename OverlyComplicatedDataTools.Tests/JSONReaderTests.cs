using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverlyComplicatedDataTools.Core;
using OverlyComplicatedDataTools.Core.Readers;
using OverlyComplicatedDataTools.Tests.Helper;
using System;
using System.Collections.Generic;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class JSONReaderTests
    {
        [TestMethod]
        public void JSONBasic()
        {
            var reader = new OCDTJSONReader(".\\TestFiles\\TestJSON-easy-small.json");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ ID=1, Value="test" },
                new TestReadObject(){ ID=2, Value="test-2" }
            };
            Assert.AreEqual(2, reader.FieldCount);
            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, (int)reader.GetValue(reader.GetOrdinal("ID")));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(reader.GetOrdinal("Value")));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }

        [TestMethod]
        public void JSONSmallZip()
        {
            var reader = new OCDTJSONReader(".\\TestFiles\\TestJSON-easy-small.zip");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ ID=1, Value="test" },
                new TestReadObject(){ ID=2, Value="test-2" }
            };
            Assert.AreEqual(2, reader.FieldCount);
            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, (int)reader.GetValue(reader.GetOrdinal("ID")));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(reader.GetOrdinal("Value")));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }
    }
}
