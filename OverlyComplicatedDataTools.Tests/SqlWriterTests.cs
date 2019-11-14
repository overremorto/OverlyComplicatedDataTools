using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OverlyComplicatedDataTools.Core;
using OverlyComplicatedDataTools.Core.Helpers;
using OverlyComplicatedDataTools.Core.Readers;
using OverlyComplicatedDataTools.Core.Writers;
using OverlyComplicatedDataTools.Tests.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class SqlWriterTests
    {
        [TestMethod]
        public async Task CreateTable()
        {
            var mockDependency = new Mock<IOCDTDataReader>();
            var columnTypes = new Type[] { typeof(int), typeof(string), typeof(DateTime) };
            var columnNames = new string[] { "ID", "Value", "Date" };

            mockDependency.Setup(x => x.GetFieldType(It.IsAny<int>()))
                .Returns<int>((i) =>
                {
                    return columnTypes[i];
                });


            mockDependency.Setup(x => x.GetName(It.IsAny<int>()))
                .Returns<int>((i) =>
                {
                    return columnNames[i];
                });

            mockDependency.Setup(x => x.FieldCount)
                          .Returns(3);

            var options = new TestOCDTOptions();
            options.TableName = "test.CreateTable";
            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                var parsedTableName = await new TableNameParser(sqlConnection).ParseTableName(options.TableName);
                await DropTable(sqlConnection, parsedTableName);


                var writer = new OCDTSqlDataWriter(options);
                var didCreateTable = await writer.CreateTable(sqlConnection, parsedTableName, mockDependency.Object);
                Assert.AreEqual(true, didCreateTable);


                var actualColumnNames = new string[columnNames.Length];
                var actualColumnTypes = new string[columnTypes.Length];

                using (var cmd = sqlConnection.CreateCommand())
                {
                    cmd.CommandText = @"select c.name as [column], typ.name as [type]
from sys.tables t
join sys.schemas s	on s.schema_id = t.schema_id
join sys.columns c	on c.object_id = t.object_id
join sys.types	typ	on typ.system_type_id = c.system_type_id
where typ.name <> 'sysname'
	and s.name = @schemaName
	and  t.name = @tableName
order by c.column_id";
                    cmd.Parameters.AddWithValue("@schemaName", parsedTableName.SchemaName);
                    cmd.Parameters.AddWithValue("@tableName", parsedTableName.TableName);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var counter = 0;
                        while (await reader.ReadAsync())
                        {
                            actualColumnNames[counter] = reader.GetString(0);
                            actualColumnTypes[counter] = reader.GetString(1);
                            counter++;
                        }

                    }
                }


                Assert.AreEqual("ID", actualColumnNames[0]);
                Assert.AreEqual("Value", actualColumnNames[1]);
                Assert.AreEqual("Date", actualColumnNames[2]);
                Assert.AreEqual("int", actualColumnTypes[0]);
                Assert.AreEqual("nvarchar", actualColumnTypes[1]);
                Assert.AreEqual("datetime", actualColumnTypes[2]);

                didCreateTable = await writer.CreateTable(sqlConnection, parsedTableName, mockDependency.Object);
                Assert.AreEqual(false, didCreateTable);

                await DropTable(sqlConnection, parsedTableName);
            }
        }

        [TestMethod]
        public async Task WriteTable()
        {
            var dataTable = new DataTable();
            dataTable.TableName = "test.WriteTable";
            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Value", typeof(string));
            dataTable.Columns.Add("Date", typeof(DateTime));
            dataTable.Rows.Add(new object[] { 1, "HI!", new DateTime(2019, 11, 12) });
            dataTable.Rows.Add(new object[] { 2, "World!", new DateTime(2019, 11, 11) });
            dataTable.Rows.Add(new object[] { 3, "Bye...", new DateTime(2019, 11, 10) });

            var dataTableReader = new OCDTDataTableReader(dataTable);

            var options = new TestOCDTOptions();
            options.TableName = "test.WriteTable";
            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                var parsedTableName = await new TableNameParser(sqlConnection).ParseTableName(options.TableName);
                await DropTable(sqlConnection, parsedTableName);


                var writer = new OCDTSqlDataWriter(options);
                await writer.Write(dataTableReader);

                var actualData = new Dictionary<string, object>[3];
                using (var cmd = sqlConnection.CreateCommand())
                {
                    cmd.CommandText = $"select * from {parsedTableName.FullyQualifiedTableName} order by ID";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var counter = 0;
                        while (await reader.ReadAsync())
                        {
                            actualData[counter] = new Dictionary<string, object>();
                            actualData[counter][reader.GetName(0)] = reader.GetInt32(0);
                            actualData[counter][reader.GetName(1)] = reader.GetString(1);
                            actualData[counter][reader.GetName(2)] = reader.GetDateTime(2);
                            counter++;
                        }

                    }
                }

                Assert.AreEqual(1, actualData[0]["ID"]);
                Assert.AreEqual("HI!", actualData[0]["Value"]);
                Assert.AreEqual(new DateTime(2019, 11, 12), actualData[0]["Date"]);

                Assert.AreEqual(2, actualData[1]["ID"]);
                Assert.AreEqual("World!", actualData[1]["Value"]);
                Assert.AreEqual(new DateTime(2019, 11, 11), actualData[1]["Date"]);

                Assert.AreEqual(3, actualData[2]["ID"]);
                Assert.AreEqual("Bye...", actualData[2]["Value"]);
                Assert.AreEqual(new DateTime(2019, 11, 10), actualData[2]["Date"]);

                await DropTable(sqlConnection, parsedTableName);
            }
        }

        [TestMethod]
        public async Task DoesTableExist()
        {
            var options = new TestOCDTOptions();
            options.TableName = "test.DoesTableExist";
            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                var parsedTableName = await new TableNameParser(sqlConnection).ParseTableName(options.TableName);
                await DropTable(sqlConnection, parsedTableName);

                var writer = new OCDTSqlDataWriter(options);
                var doesTableExist = await writer.DoesTableExist(sqlConnection, parsedTableName);
                Assert.AreEqual(false, doesTableExist);

                using (var cmd = sqlConnection.CreateCommand())
                {
                    cmd.CommandText = $"CREATE TABLE {parsedTableName.FullyQualifiedTableName} ([ID] INT)";
                    await cmd.ExecuteNonQueryAsync();
                }

                doesTableExist = await writer.DoesTableExist(sqlConnection, parsedTableName);
                Assert.AreEqual(true, doesTableExist);

                await DropTable(sqlConnection, parsedTableName);
            }
        }

        [TestMethod]
        public async Task SqlWriterCSVBasic()
        {
            var options = new TestOCDTOptions();
            options.TableName = "test.TestCSV-easy-small";
            var csvReader = new OCDTCSVReader(".\\TestFiles\\TestCSV-easy-small.csv");
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

            Assert.AreEqual("ID", csvReader.Columns[0]);
            Assert.AreEqual("Value", csvReader.Columns[1]);
            Assert.AreEqual(2, csvReader.FieldCount);

            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                var parsedTableName = await new TableNameParser(sqlConnection).ParseTableName(options.TableName);
                await DropTable(sqlConnection, parsedTableName);


                var writer = new OCDTSqlDataWriter(options);
                await writer.Write(csvReader);

                var actualResults = new List<TestReadObject>();
                using (var cmd = sqlConnection.CreateCommand())
                {
                    cmd.CommandText = $"select * from {parsedTableName.FullyQualifiedTableName} order by ID";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var counter = 0;
                        while (await reader.ReadAsync())
                        {
                            actualResults.Add(new TestReadObject()
                            {
                                ID = reader.GetInt32(0),
                                Value = reader.GetString(1)
                            });
                            counter++;
                        }
                    }
                }

                Assert.AreEqual(expectedResults.Count, actualResults.Count);
                for (var i = 0; i <  expectedResults.Count; i++)
                {
                    Assert.AreEqual(expectedResults[i].ID, actualResults[i].ID, $"failed on row {i}");
                    Assert.AreEqual(expectedResults[i].Value, actualResults[i].Value, $"failed on row {i}");
                }

                await DropTable(sqlConnection, parsedTableName);
            }
        }


        [TestMethod]
        public async Task SqlWriterCSVLargeZip()
        {
            var options = new TestOCDTOptions();
            options.TableName = "test.TestCSV-realist-large";
            var csvReader = new OCDTCSVReader(".\\TestFiles\\TestCSV-realist-large.zip");

            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                var parsedTableName = await new TableNameParser(sqlConnection).ParseTableName(options.TableName);
                await DropTable(sqlConnection, parsedTableName);


                var writer = new OCDTSqlDataWriter(options);
                await writer.Write(csvReader);

                var counter = 0;
                using (var cmd = sqlConnection.CreateCommand())
                {
                    cmd.CommandText = $"select * from {parsedTableName.FullyQualifiedTableName}";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Assert.IsNotNull(reader.GetValue("Region"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Country"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Item Type"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Sales Channel"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Order Priority"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Order Date"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Order ID"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Ship Date"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Units Sold"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Unit Price"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Unit Cost"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Total Revenue"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Total Cost"), $"failed on row {counter}");
                            Assert.IsNotNull(reader.GetValue("Total Profit"), $"failed on row {counter}");
                            counter++;
                        }
                    }
                }

                var expectedCount = 1000000;
                Assert.AreEqual(expectedCount, counter);

                await DropTable(sqlConnection, parsedTableName);
            }
        }

        private async Task DropTable(SqlConnection sqlConnection, ParsedTableName parsedTableName)
        {
            using (var cmd = sqlConnection.CreateCommand())
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }
                cmd.CommandText = $@"
IF (EXISTS (SELECT TOP 1 1
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = @schemaName
                 AND  TABLE_NAME = @tableName))
BEGIN
    DROP TABLE {parsedTableName.FullyQualifiedTableName}
END
";
                cmd.Parameters.AddWithValue("@schemaName", parsedTableName.SchemaName);
                cmd.Parameters.AddWithValue("@tableName", parsedTableName.TableName);

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
