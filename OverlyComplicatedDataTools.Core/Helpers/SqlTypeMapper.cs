using System;
using System.Collections.Generic;
using System.Text;

namespace OverlyComplicatedDataTools.Core.Helpers
{
    public static class SqlTypeMapper
    {
        private static Dictionary<Type, string> s_SqlTypeMapping;

        public static string Get(Type type)
        {
            if (s_SqlTypeMapping == null){
                s_SqlTypeMapping = new Dictionary<Type, string>()
                {
                    {typeof(long), "BIGINT" },
                    {typeof(long?), "BIGINT" },
                    {typeof(int), "INT" },
                    {typeof(int?), "INT" },
                    {typeof(short), "SMALLINT" },
                    {typeof(short?), "SMALLINT" },
                    {typeof(byte), "TINYINT" },
                    {typeof(byte?), "TINYINT" },
                    {typeof(bool), "BIT" },
                    {typeof(bool?), "BIT" },

                    {typeof(double), "FLOAT" },
                    {typeof(double?), "FLOAT" },
                    {typeof(float), "FLOAT" },
                    {typeof(float?), "FLOAT" },
                    {typeof(decimal), "FLOAT" },
                    {typeof(decimal?), "FLOAT" },

                    {typeof(DateTime), "DATETIME" },
                    {typeof(DateTime?), "DATETIME" },

                    {typeof(char), "NVARCHAR(1)" },
                    {typeof(char?), "NVARCHAR(1)" },
                    {typeof(string), "NVARCHAR(MAX)" }
                };
            }
            if (!s_SqlTypeMapping.ContainsKey(type))
            {
                return null;
            }
            return s_SqlTypeMapping[type];
        }

    }
}
