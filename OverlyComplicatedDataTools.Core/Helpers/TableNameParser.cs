using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace OverlyComplicatedDataTools.Core.Helpers
{
    public class TableNameParser
    {
        private SqlConnection _sqlConnection;
        public TableNameParser(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public async Task<ParsedTableName> ParseTableName(string tableName)
        {
            var parsedTable = new ParsedTableName();
            var tableNameParts = tableName.Split(".");
            if (tableNameParts.Length == 0)
            {
                return null;
            }

            parsedTable.TableName = tableNameParts[tableNameParts.Length - 1];

            if (tableNameParts.Length > 1)
            {
                parsedTable.SchemaName = tableNameParts[tableNameParts.Length - 2];
            }
            if (tableNameParts.Length > 2)
            {
                parsedTable.DatabaseName = tableNameParts[tableNameParts.Length - 3];
            }

            if (tableNameParts.Length > 3)
            {
                parsedTable.ServerName = tableNameParts[tableNameParts.Length - 4];
            }

            if (string.IsNullOrEmpty(parsedTable.SchemaName))
            {
                parsedTable.SchemaName = await DefaultSchema();
            }

            return parsedTable;
        }

        public async Task<string> DefaultSchema()
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.Open();
            }
            using (var cmd = _sqlConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT SCHEMA_NAME()";
                return (string)(await cmd.ExecuteScalarAsync());
            }
        }
    }

    public class ParsedTableName
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }

        public string FullyQualifiedTableName
        {
            get
            {
                var serverName = !ServerName?.StartsWith("[") == true ? $"[{ServerName}]": ServerName;
                var databaseName = !DatabaseName?.StartsWith("[") == true ? $"[{DatabaseName}]" : DatabaseName;
                var schemaName = !SchemaName?.StartsWith("[") == true ? $"[{SchemaName}]" : SchemaName;
                var tableName = !TableName?.StartsWith("[") == true ? $"[{TableName}]" : TableName;

                return $"{(!string.IsNullOrEmpty(serverName) ? (serverName + ".") : "")}{(!string.IsNullOrEmpty(databaseName) ? (databaseName + ".") : "")}{schemaName}.{tableName}";
            }
        }
    }
}
