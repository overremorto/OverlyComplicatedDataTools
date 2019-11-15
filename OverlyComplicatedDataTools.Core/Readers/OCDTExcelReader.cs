using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
    public class OCDTExcelReader : IOCDTDataReader
    {
        private string _filename;
        private SpreadsheetDocument _spreadsheetDocument;
        private WorkbookPart _workbookPart;
        private WorksheetPart _worksheetPart;
        private OpenXmlReader _reader;
        private string[] _columns;
        private Type[] _columnTypes;
        private MethodInfo[] _parseMethods;
        private Dictionary<string, int> _columnOrdinal;
        private string[] _rowData;

        public string[] Columns { get { return _columns; } }

        public OCDTExcelReader(string filename)
        {
            _filename = filename;
            _spreadsheetDocument = SpreadsheetDocument.Open(_filename, false);
            _workbookPart = _spreadsheetDocument.WorkbookPart;
            _worksheetPart = _workbookPart.WorksheetParts.First();

            _reader = OpenXmlReader.Create(_worksheetPart);

            SetupColumns();
        }

        private void SetupColumns()
        {
            var columns = new List<string>();
            string cellAddress = null;
            string cellValue = null;
            var rows = new Dictionary<string, string>();
            var row = 0;
            while (_reader.Read() && row < 2)
            {
                if (_reader.ElementType == typeof(Row))
                {
                    row++;
                }
                else if (_reader.ElementType == typeof(Cell))
                {
                    cellAddress = _reader.Attributes[0].Value;
                    rows[cellAddress] = GetCellValue((Cell)_reader.LoadCurrentElement(), _workbookPart);
                }
            }

            _columns = columns.ToArray();
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
            _reader?.Close();
            _reader = OpenXmlReader.Create(_worksheetPart);

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
            _reader?.Close();
            _spreadsheetDocument?.Close();
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _spreadsheetDocument?.Dispose();
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
            //if (_streamReader.EndOfStream)
            //{
            //    return false;
            //}
            _rowData = Parse();
            _rowIndex++;
            return true;
        }

        private string[] Parse()
        {
            var results = new string[_columns.Length];
            while (_reader.Read())
            {
                if (_reader.ElementType == typeof(CellValue))
                {
                    results[0]= _reader.GetText();
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

        private string GetCellValue(Cell c, WorkbookPart workbookPart)
        {
            string cellValue = string.Empty;
            if (c.DataType != null && c.DataType == CellValues.SharedString)
            {
                SharedStringItem ssi =
                    workbookPart.SharedStringTablePart.SharedStringTable
                        .Elements<SharedStringItem>()
                        .ElementAt(int.Parse(c.CellValue.InnerText));
                if (ssi.Text != null)
                {
                    cellValue = ssi.Text.Text;
                }
            }
            else
            {
                if (c.CellValue != null)
                {
                    cellValue = c.CellValue.InnerText;
                }
            }
            return cellValue;
        }
    }
}
