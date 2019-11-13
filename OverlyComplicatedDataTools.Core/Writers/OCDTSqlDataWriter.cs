using OverlyComplicatedDataTools.Core.Helpers;
using OverlyComplicatedDataTools.Core.Readers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OverlyComplicatedDataTools.Core.Writers
{
    public class OCDTSqlDataWriter : IOCDTDataWriter
    {
        private IOCDTOptions _options;
        public OCDTSqlDataWriter(IOCDTOptions options)
        {
            _options = options;
        }

        public async Task Write(IOCDTDataReader reader)
        {
            using (var sqlConnection = new SqlConnection(_options.ConnectionString))
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                var parsedTableName = await new TableNameParser(sqlConnection).ParseTableName(_options.TableName);
                await CreateTable(sqlConnection, parsedTableName, reader);
                using (var sbc = new SqlBulkCopy(sqlConnection))
                {
                    sbc.DestinationTableName = parsedTableName.FullyQualifiedTableName;
                    sbc.BulkCopyTimeout = 60000;
                    sbc.BatchSize = 100000;
                    sbc.EnableStreaming = true;
                    sbc.NotifyAfter = 100000;
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var column = reader.GetName(i);
                        sbc.ColumnMappings.Add(column, column);
                    }

                    sbc.SqlRowsCopied += Sbc_SqlRowsCopied;
                    await sbc.WriteToServerAsync(reader);
                }
            }
        }

        private void Sbc_SqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.WriteLine($"{e.RowsCopied} rows copied");
        }

        public async Task<bool> CreateTable(SqlConnection sqlConnection, ParsedTableName parsedTableName, IOCDTDataReader reader)
        {
            if (sqlConnection.State != ConnectionState.Open)
            {
                sqlConnection.Open();
            }
            
            if (await DoesTableExist(sqlConnection, parsedTableName))
            {
                return false;
            }

            using (var cmd = sqlConnection.CreateCommand())
            {
                var columnString = new StringBuilder();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    columnString.Append($@"
{(columnString.Length> 0 ? "    ,":"     ")}[{reader.GetName(i)}] {SqlTypeMapper.Get(reader.GetFieldType(i))} NULL");
                }
                cmd.CommandText = $"CREATE TABLE {parsedTableName.FullyQualifiedTableName} ({columnString.ToString()})";
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        public async Task<bool> DoesTableExist(SqlConnection sqlConnection, ParsedTableName parsedTableName)
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
    SELECT 1
END
ELSE
BEGIN
    SELECT 0
END
";
                cmd.Parameters.AddWithValue("@schemaName", parsedTableName.SchemaName);
                cmd.Parameters.AddWithValue("@tableName", parsedTableName.TableName);

                return (int)(await cmd.ExecuteScalarAsync()) == 1;
            }
        }
    }
}
