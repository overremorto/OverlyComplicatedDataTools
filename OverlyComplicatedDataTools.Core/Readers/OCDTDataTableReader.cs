using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OverlyComplicatedDataTools.Core.Readers
{
    public class OCDTDataTableReader : IOCDTDataReader
    {
        private DataTable _dataTable;
        public OCDTDataTableReader(DataTable dataTable)
        {
            _dataTable = dataTable;
        }

        private DataRow _currentRow;
        private int _rowIndex;

        public object this[int i] => _currentRow[i];

        public object this[string name] => _currentRow[name];

        public int Depth => 1;

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => _dataTable.Columns.Count;

        public string[] Columns
        {
            get
            {
                var columns = new string[FieldCount];

                for (var i = 0; i < FieldCount; i++)
                {
                    columns[i] = _dataTable.Columns[i].ColumnName;
                }

                return columns;
            }
        }

        public void Close()
        {
        }

        public void Dispose()
        {
            _dataTable.Dispose();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            return _dataTable.Columns[i].DataType;
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            return _dataTable.Columns[i].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            return _dataTable.Columns[name].Ordinal;
        }

        public DataTable GetSchemaTable()
        {
            return _dataTable;
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            return _currentRow[i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return _currentRow[i] == null;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            if (_rowIndex >= _dataTable.Rows.Count)
            {
                return false;
            }

            _currentRow = _dataTable.Rows[_rowIndex++];
            return true;
        }
    }
}
