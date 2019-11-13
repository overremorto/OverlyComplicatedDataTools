using OverlyComplicatedDataTools.Core;
using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
namespace OverlyComplicatedDataTools
{
    public class OCDTOptions : IOCDTOptions
    {
        [Option('c', "connectionstring", Required =true, HelpText = "Connection String")]
        public string ConnectionString { get; set; }
        [Option('f', "filepath", Required = true, HelpText = "File Path")]
        public string FilePath { get; set; }
        [Option('t', "tablename", Required = true, HelpText = "Table Name")]
        public string TableName { get; set; }
    }
}
