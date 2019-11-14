using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverlyComplicatedDataTools.Core;
using OverlyComplicatedDataTools.Core.Readers;
using OverlyComplicatedDataTools.Tests.Helper;
using System;
using System.Collections.Generic;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class CSVReaderTests
    {
        [TestMethod]
        public void CSVBasic()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-easy-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 27725,Value = "lM" },
                new TestReadObject(){ID = 27726,Value = "lN"},
                new TestReadObject(){ID = 27727,Value = "lO"},
                new TestReadObject(){ID = 27728,Value = "lP"},
                new TestReadObject(){ID = 27729,Value = "lQ"},
                new TestReadObject(){ID = 27730,Value = "lR"},
                new TestReadObject(){ID = 27731,Value = "lS"},
                new TestReadObject(){ID = 27732,Value = "lT"},
                new TestReadObject(){ID = 27733,Value = "lU"},
                new TestReadObject(){ID = 27734,Value = "lV"},
                new TestReadObject(){ID = 27735,Value = "lW"},
                new TestReadObject(){ID = 27736,Value = "lX"},
                new TestReadObject(){ID = 27737,Value = "lY"},
                new TestReadObject(){ID = 27738,Value = "lZ"},


            };
            Assert.AreEqual("ID", reader.Columns[0]);
            Assert.AreEqual("Value", reader.Columns[1]);
            Assert.AreEqual(2, reader.FieldCount);
            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, (int)reader.GetValue(0));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(1));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }

        [TestMethod]
        public void CSVQuotes()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-quotes-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 1,Value = "\"H\"i!" },
                new TestReadObject(){ID = 2,Value = "\"Good\" Bye"},
                new TestReadObject(){ID = 3,Value = "Test"},
            };

            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, reader.GetInt32(0));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(1));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);

        }

        [TestMethod]
        public void CSVLarge()
        {
        }

        [TestMethod]
        public void CSVCommas()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-comma-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 1,Value = "Hello,World" },
                new TestReadObject(){ID = 2,Value = "Good,Bye"},
                new TestReadObject(){ID = 3,Value = "Test"},
            };

            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, reader.GetInt32(0));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(1));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }

        [TestMethod]
        public void CSVCommas2()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-comma-multiple-columns-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 1,Value = "Hello,World", Value2="Mornin' World" },
                new TestReadObject(){ID = 2,Value = "Good,Bye", Value2="See Ya, Later"},
                new TestReadObject(){ID = 3,Value = "Test", Value2 = "Test2"},
                new TestReadObject(){ID = 4,Value = "lots", Value2 = "of , , , , , , , , , , , , , , , , , commas,,,,,,,,,,,,,,,,,,,"}
            };

            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, reader.GetInt32(0));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(1));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }

        [TestMethod]
        public void CSVLineBreaks()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-linebreak-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 1,Value = @"Hello

World" },
                new TestReadObject(){ID = 2,Value = @"Good






Bye"},
                new TestReadObject(){ID = 3,Value = "Test"},
            };

            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, reader.GetInt32(0));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(1));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }

        [TestMethod]
        public void CSVLineBreaksAndCommas()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-linebreak-comma-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 1,Value = @"Hello
,
World," },
                new TestReadObject(){ID = 2,Value = @",Good
,
,,
,,
,
,,
,
,Bye,"},
                new TestReadObject(){ID = 3,Value = "Test"},
            };

            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, reader.GetInt32(0));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(1));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }

        [TestMethod]
        public void CSVUnorderedColumns()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-out-of-order-small.csv");
            var expectedResults = new List<TestReadObject>()
            {
                new TestReadObject(){ID = 27725,Value = "lM" },
                new TestReadObject(){ID = 27726,Value = "lN"},
                new TestReadObject(){ID = 27727,Value = "lO"},
                new TestReadObject(){ID = 27728,Value = "lP"},
                new TestReadObject(){ID = 27729,Value = "lQ"},
                new TestReadObject(){ID = 27730,Value = "lR"},
                new TestReadObject(){ID = 27731,Value = "lS"},
                new TestReadObject(){ID = 27732,Value = "lT"},
                new TestReadObject(){ID = 27733,Value = "lU"},
                new TestReadObject(){ID = 27734,Value = "lV"},
                new TestReadObject(){ID = 27735,Value = "lW"},
                new TestReadObject(){ID = 27736,Value = "lX"},
                new TestReadObject(){ID = 27737,Value = "lY"},
                new TestReadObject(){ID = 27738,Value = "lZ"},
            };

            var counter = 0;
            while (reader.Read())
            {
                Assert.AreEqual(expectedResults[counter].ID, reader.GetInt32(reader.GetOrdinal("ID")));
                Assert.AreEqual(expectedResults[counter].Value, (string)reader.GetValue(reader.GetOrdinal("Value")));
                counter++;
            }
            Assert.AreEqual(expectedResults.Count, counter);
            Assert.AreEqual(expectedResults.Count, reader.RecordsAffected);
        }


        [TestMethod]
        public void CSVRealisticZip()
        {
            var reader = new OCDTCSVReader(".\\TestFiles\\TestCSV-realist-large.zip");
            var expectedCount = 1000000;
            var counter = 0;

            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Region")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Country")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Item Type")));
            Assert.AreEqual(typeof(string), reader.GetFieldType(reader.GetOrdinal("Sales Channel")));
            Assert.AreEqual(typeof(char), reader.GetFieldType(reader.GetOrdinal("Order Priority")));
            Assert.AreEqual(typeof(DateTime), reader.GetFieldType(reader.GetOrdinal("Order Date")));
            Assert.AreEqual(typeof(int), reader.GetFieldType(reader.GetOrdinal("Order ID")));
            Assert.AreEqual(typeof(DateTime), reader.GetFieldType(reader.GetOrdinal("Ship Date")));
            Assert.AreEqual(typeof(int), reader.GetFieldType(reader.GetOrdinal("Units Sold")));
            Assert.AreEqual(typeof(double), reader.GetFieldType(reader.GetOrdinal("Unit Price")));
            Assert.AreEqual(typeof(double), reader.GetFieldType(reader.GetOrdinal("Unit Cost")));
            Assert.AreEqual(typeof(double), reader.GetFieldType(reader.GetOrdinal("Total Revenue")));
            Assert.AreEqual(typeof(double), reader.GetFieldType(reader.GetOrdinal("Total Cost")));
            Assert.AreEqual(typeof(double), reader.GetFieldType(reader.GetOrdinal("Total Profit")));

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
                Assert.AreEqual(typeof(char), reader.GetValue(reader.GetOrdinal("Order Priority")).GetType());
                Assert.AreEqual(typeof(DateTime), reader.GetValue(reader.GetOrdinal("Order Date")).GetType());
                Assert.AreEqual(typeof(int), reader.GetValue(reader.GetOrdinal("Order ID")).GetType());
                Assert.AreEqual(typeof(DateTime), reader.GetValue(reader.GetOrdinal("Ship Date")).GetType());
                Assert.AreEqual(typeof(int), reader.GetValue(reader.GetOrdinal("Units Sold")).GetType());
                Assert.AreEqual(typeof(double), reader.GetValue(reader.GetOrdinal("Unit Price")).GetType());
                Assert.AreEqual(typeof(double), reader.GetValue(reader.GetOrdinal("Unit Cost")).GetType());
                Assert.AreEqual(typeof(double), reader.GetValue(reader.GetOrdinal("Total Revenue")).GetType());
                Assert.AreEqual(typeof(double), reader.GetValue(reader.GetOrdinal("Total Cost")).GetType());
                Assert.AreEqual(typeof(double), reader.GetValue(reader.GetOrdinal("Total Profit")).GetType());

                counter++;
            }
            Assert.AreEqual(expectedCount, counter);
            Assert.AreEqual(expectedCount, reader.RecordsAffected);
        }
    }
}
