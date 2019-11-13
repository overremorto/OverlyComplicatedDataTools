using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OverlyComplicatedDataTools.Core.Readers
{
    public interface IOCDTDataReader : IDataReader
    {
        public string[] Columns { get; }
    }
}
