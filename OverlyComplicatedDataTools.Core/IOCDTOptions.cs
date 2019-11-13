using System;
using System.Collections.Generic;
using System.Text;

namespace OverlyComplicatedDataTools.Core
{
    public interface IOCDTOptions
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string FilePath { get; set; }
    }
}
