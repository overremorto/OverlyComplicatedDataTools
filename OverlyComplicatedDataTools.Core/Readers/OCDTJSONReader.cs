using OverlyComplicatedDataTools.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OverlyComplicatedDataTools.Core.Readers
{
    public class OCDTJSONReader : IOCDTDataReader
    {
        private string _filename;
        private FileStream _fileStream;
        private StreamReader _streamReader;
        private string[] _columns;
        private Type[] _columnTypes;
        private MethodInfo[] _parseMethods;
        private Dictionary<string, int> _columnOrdinal;
        private string[] _rowData;

        private const string OPTIONALLY_ENCLOSED_BY = "\"";
        private const string COLUMN_DELIMITER = ",";


        public string[] Columns { get { return _columns; } }

        public OCDTJSONReader(string filename)
        {
            _filename = filename;
            _fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            _streamReader = new StreamReader(_fileStream);
            SetupColumns();
        }

        private void SetupColumns()
        {
            var columnLine = _streamReader.ReadLine();
            _columns = columnLine.Split(",");
            _columnOrdinal = new Dictionary<string, int>();
            for (var i = 0; i < _columns.Length; i++)
            {
                _columnOrdinal[_columns[i]] = i;
            }
            _columnTypes = new Type[_columns.Length];
            _parseMethods = new MethodInfo[_columns.Length];

            // determin column types
            while (Read() && _rowIndex < 500)
            {
                for (var i = 0; i < _columns.Length; i++)
                {
                    var unparsed = (string)_rowData[i];
                    var possibleType = TypeParser.DetermineType(unparsed);
                    if (_columnTypes[i] == null || TypeParser.TYPE_PRIORITY_MAP[possibleType] > TypeParser.TYPE_PRIORITY_MAP[_columnTypes[i]])
                    {
                        _columnTypes[i] = possibleType;
                    }
                }
            }


            for (var i = 0; i < _parseMethods.Length; i++)
            {
                var type = _columnTypes[i];
                if (type != typeof(string))
                {
                    _parseMethods[i] = type.GetMethod("Parse", new[] { typeof(string) });
                }
            }

            // reset stream position to data rows
            _fileStream.Seek(0, SeekOrigin.Begin);
            _streamReader.DiscardBufferedData();
            columnLine = _streamReader.ReadLine();

            _rowIndex = 0;
        }


        private int _rowIndex;

        public object this[int i] => _rowData[i];

        public object this[string name] => _rowData[_columnOrdinal[name]];

        public int Depth => 1;

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => _rowIndex;

        public int FieldCount => _columns.Length;

        public void Close()
        {
            _streamReader?.Close();
            _fileStream?.Close();
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
            _fileStream?.Dispose();
        }

        public bool GetBoolean(int i)
        {
            return bool.Parse(_rowData[i]);
        }

        public byte GetByte(int i)
        {
            return byte.Parse(_rowData[i]);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return char.Parse(_rowData[i]);
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
            return _columnTypes[i].Name;
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.Parse(_rowData[i]);
        }

        public decimal GetDecimal(int i)
        {
            return decimal.Parse(_rowData[i]);
        }

        public double GetDouble(int i)
        {
            return double.Parse(_rowData[i]);
        }

        public Type GetFieldType(int i)
        {
            return _columnTypes[i];
        }

        public float GetFloat(int i)
        {
            return float.Parse(_rowData[i]);
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse(_rowData[i]);
        }

        public short GetInt16(int i)
        {
            return short.Parse(_rowData[i]);
        }

        public int GetInt32(int i)
        {
            return int.Parse(_rowData[i]);
        }

        public long GetInt64(int i)
        {
            return long.Parse(_rowData[i]);
        }

        public string GetName(int i)
        {
            return _columns[i];
        }

        public int GetOrdinal(string name)
        {
            return _columnOrdinal[name];
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return _rowData[i];
        }

        public object GetValue(int i)
        {
            return ConvertType(i);
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return _rowData[i] == null;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            if (_streamReader.EndOfStream)
            {
                return false;
            }
            _rowData = Parse();
            _rowIndex++;
            return true;
        }

        private string[] Parse()
        {
            var text = _streamReader.ReadLine();
            var results = new string[_columns.Length];
            var split = text.Split(COLUMN_DELIMITER);
            var columnIndex = 0;
            for (var i = 0; i < split.Length; i++)
            {
                var quoteOpenForCell = false;
                if (split[i].StartsWith(OPTIONALLY_ENCLOSED_BY))
                {
                    var stringBuilder = new StringBuilder();
                    while (!split[i].EndsWith(OPTIONALLY_ENCLOSED_BY) || (split[i] == OPTIONALLY_ENCLOSED_BY && !quoteOpenForCell))
                    {
                        quoteOpenForCell = true;
                        stringBuilder.Append(split[i]);
                        i++;
                        if (i >= split.Length)
                        {
                            text = _streamReader.ReadLine();
                            split = text.Split(COLUMN_DELIMITER);
                            split[0] = Environment.NewLine + split[0];
                            i = 0;
                        }
                        else
                        {
                            stringBuilder.Append(COLUMN_DELIMITER);
                        }
                    }

                    stringBuilder.Append(split[i]);
                    var str = stringBuilder.ToString();
                    results[columnIndex] = str.Substring(1, stringBuilder.Length - 2).Replace("\\\"", "\"");
                    columnIndex++;
                }
                else
                {
                    results[columnIndex] = split[i];
                    columnIndex++;
                }
            }

            return results;
        }

        private object ConvertType(int ordinal)
        {
            var type = _columnTypes?.Length > ordinal ? _columnTypes[ordinal] : null;
            var parseMethod = _parseMethods?.Length > ordinal ? _parseMethods[ordinal] : null;
            var str = _rowData[ordinal];

            if (parseMethod == null)
            {
                return str;
            }

            var parsedValue = parseMethod.Invoke(null, new[] { str });
            return parsedValue;
        }
    }
}
