using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OverlyComplicatedDataTools.Core;
using OverlyComplicatedDataTools.Core.Readers;
using OverlyComplicatedDataTools.Tests.Helper;
using System;
using System.Collections.Generic;
using System.IO;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class ExcelReaderTests
    {
        [TestMethod]
        public void ExcelBasic()
        {
            var reader = new OCDTExcelReader(".\\TestFiles\\TestExcel-easy-small.xlsx");
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
