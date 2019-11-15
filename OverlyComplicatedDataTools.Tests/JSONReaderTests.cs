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


        [TestMethod]
        public void JSONRealisticZip()
        {
            var reader = new OCDTJSONReader(".\\TestFiles\\TestJSON-realist-large.zip");
            var expectedCount = 1000000;
            var counter = 0;

            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Region")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Country")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Item Type")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Sales Channel")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Order Priority")));
            Assert.AreEqual(typeof(DateTime), reader.GetFieldType(reader.GetOrdinal("Order Date")));
            Assert.AreEqual(typeof(int), reader.GetFieldType(reader.GetOrdinal("Order ID")));
            Assert.AreEqual(typeof(DateTime), reader.GetFieldType(reader.GetOrdinal("Ship Date")));
            Assert.AreEqual(typeof(int), reader.GetFieldType(reader.GetOrdinal("Units Sold")));
            Assert.AreEqual(typeof(float), reader.GetFieldType(reader.GetOrdinal("Unit Price")));
            Assert.AreEqual(typeof(float), reader.GetFieldType(reader.GetOrdinal("Unit Cost")));
            Assert.AreEqual(typeof(float), reader.GetFieldType(reader.GetOrdinal("Total Revenue")));
            Assert.AreEqual(typeof(float), reader.GetFieldType(reader.GetOrdinal("Total Cost")));
            Assert.AreEqual(typeof(float), reader.GetFieldType(reader.GetOrdinal("Total Profit")));

            while (reader.Read())
            {
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Region")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Country")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Item Type")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Sales Channel")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Order Priority")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Order Date")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Order ID")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Ship Date")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Units Sold")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Unit Price")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Unit Cost")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Total Revenue")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Total Cost")));
                Assert.IsNotNull(reader.GetValue(reader.GetOrdinal("Total Profit")));

                Assert.AreEqual(typeof(string), reader.GetValue(reader.GetOrdinal("Region")).GetType());
                Assert.AreEqual(typeof(string), reader.GetValue(reader.GetOrdinal("Country")).GetType());
                Assert.AreEqual(typeof(string), reader.GetValue(reader.GetOrdinal("Item Type")).GetType());
                Assert.AreEqual(typeof(string), reader.GetValue(reader.GetOrdinal("Sales Channel")).GetType());
                Assert.AreEqual(typeof(string), reader.GetValue(reader.GetOrdinal("Order Priority")).GetType());
                Assert.AreEqual(typeof(DateTime), reader.GetValue(reader.GetOrdinal("Order Date")).GetType());
                Assert.AreEqual(typeof(int), reader.GetValue(reader.GetOrdinal("Order ID")).GetType());
                Assert.AreEqual(typeof(DateTime), reader.GetValue(reader.GetOrdinal("Ship Date")).GetType());
                Assert.AreEqual(typeof(int), reader.GetValue(reader.GetOrdinal("Units Sold")).GetType());
                Assert.AreEqual(typeof(float), reader.GetValue(reader.GetOrdinal("Unit Price")).GetType());
                Assert.AreEqual(typeof(float), reader.GetValue(reader.GetOrdinal("Unit Cost")).GetType());
                Assert.AreEqual(typeof(float), reader.GetValue(reader.GetOrdinal("Total Revenue")).GetType());
                Assert.AreEqual(typeof(float), reader.GetValue(reader.GetOrdinal("Total Cost")).GetType());
                Assert.AreEqual(typeof(float), reader.GetValue(reader.GetOrdinal("Total Profit")).GetType());

                counter++;
            }
            Assert.AreEqual(expectedCount, counter);
            Assert.AreEqual(expectedCount, reader.RecordsAffected);
        }

        [TestMethod]
        public void GenerateJson()
        {
            var csvReader = new OCDTCSVReader(".\\TestFiles\\TestCSV-realist-large.zip");
            using (var fileStream = new FileStream(".\\TestFiles\\TestJSON-realist-large.json", FileMode.OpenOrCreate))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartArray();
                        while (csvReader.Read())
                        {
                            jsonWriter.WriteStartObject();
                            foreach(var column  in csvReader.Columns)
                            {
                                jsonWriter.WritePropertyName(column);
                                jsonWriter.WriteValue(csvReader.GetValue(csvReader.GetOrdinal(column)));
                            }
                            jsonWriter.WriteEndObject();
                        }
                        jsonWriter.WriteEndArray();
                    }
                }
            }
        }
    }
}
