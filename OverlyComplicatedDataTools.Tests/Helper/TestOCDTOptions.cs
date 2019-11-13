using OverlyComplicatedDataTools.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace OverlyComplicatedDataTools.Tests.Helper
{
    class TestOCDTOptions : IOCDTOptions
    {
        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    return "Server=.;Database=DataEngineerToolkitTest;Trusted_Connection=True;";
                }
                return _connectionString;
            }
            set { _connectionString = value; }
        }
        public string TableName { get; set; }
        public string FilePath { get; set; }
    }
}
