using Microsoft.VisualStudio.TestTools.UnitTesting;
using OverlyComplicatedDataTools.Core.Helpers;
using OverlyComplicatedDataTools.Tests.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace OverlyComplicatedDataTools.Tests
{
    [TestClass]
    public class TableNameParserTests
    {
        [TestMethod]
        public async Task TestDefaultSchema()
        {
            var options = new TestOCDTOptions();
            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                var parser = new TableNameParser(sqlConnection);
                var defaultSchema = await parser.DefaultSchema();
                Assert.AreEqual("dbo", defaultSchema);
            }
        }

        [TestMethod]
        public void FullyQualifiedTableName()
        {
            var parsedTable = new ParsedTableName();
            parsedTable.TableName = "TestTable";
            parsedTable.SchemaName = "myschema";
            Assert.AreEqual("[myschema].[TestTable]", parsedTable.FullyQualifiedTableName);

            parsedTable.DatabaseName = "MyDatabase";
            Assert.AreEqual("[MyDatabase].[myschema].[TestTable]", parsedTable.FullyQualifiedTableName);
            
            parsedTable.ServerName = "MyServer";
            Assert.AreEqual("[MyServer].[MyDatabase].[myschema].[TestTable]", parsedTable.FullyQualifiedTableName);

            parsedTable.TableName = "[TestTable]";
            parsedTable.SchemaName = "[myschema]";
            Assert.AreEqual("[MyServer].[MyDatabase].[myschema].[TestTable]", parsedTable.FullyQualifiedTableName);

            parsedTable.DatabaseName = "[MyDatabase]";
            parsedTable.ServerName = "[MyServer]";
            Assert.AreEqual("[MyServer].[MyDatabase].[myschema].[TestTable]", parsedTable.FullyQualifiedTableName);
        }

        [TestMethod]
        public async Task TestParseTableName()
        {
            var options = new TestOCDTOptions();
            using (var sqlConnection = new SqlConnection(options.ConnectionString))
            {
                var parser = new TableNameParser(sqlConnection);
                var parsedTable = await parser.ParseTableName("MyTable");
                Assert.AreEqual("[dbo].[MyTable]", parsedTable.FullyQualifiedTableName);

                parsedTable = await parser.ParseTableName("myschema.MyTable");
                Assert.AreEqual("[myschema].[MyTable]", parsedTable.FullyQualifiedTableName);

                parsedTable = await parser.ParseTableName("MyDatabase.myschema.MyTable");
                Assert.AreEqual("[MyDatabase].[myschema].[MyTable]", parsedTable.FullyQualifiedTableName);

                parsedTable = await parser.ParseTableName("MyServer.MyDatabase.myschema.MyTable");
                Assert.AreEqual("[MyServer].[MyDatabase].[myschema].[MyTable]", parsedTable.FullyQualifiedTableName);

                parsedTable = await parser.ParseTableName("[myschema].[MyTable]");
                Assert.AreEqual("[myschema].[MyTable]", parsedTable.FullyQualifiedTableName);
            }
        }

    }
}
