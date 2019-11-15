using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OverlyComplicatedDataTools.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
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
        private Stream _fileStream;
        private Stream _stream;
        private StreamReader _streamReader;
        private ZipArchiveEntry _zipEntry;
        private List<string> _columns;
        private Type[] _columnTypes;
        private Dictionary<string, int> _columnOrdinal;
        private JsonTextReader _jsonReader;
        private JObject _columnObj;
        private object[] _rowData;
        private bool _streamDone;

        private const string OPTIONALLY_ENCLOSED_BY = "\"";
        private const string COLUMN_DELIMITER = ",";


        public string[] Columns { get { return _columns.ToArray(); } }

        public OCDTJSONReader(string filename)
        {
            _filename = filename;
            _fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (Path.GetExtension(_filename) == ".zip")
            {
                var zip = new ZipArchive(_fileStream);
                _zipEntry = zip.Entries.FirstOrDefault(e => Path.GetExtension(e.FullName) == ".json");
                _stream = _zipEntry.Open();
            }
            else
            {
                _stream = _fileStream;
            }
            _streamReader = new StreamReader(_stream);
            _jsonReader = new JsonTextReader(_streamReader);
            SetupColumns();
        }

        private void SetupColumns()
        {
            var columnDictionary = new Dictionary<string, Type>();
            _columnOrdinal = new Dictionary<string, int>();
            _columns = new List<string>();
            // determin column types
            while (ReadColumns() && _rowIndex < 500)
            {
                foreach(var child in _columnObj.Children())
                {
                    if (child.Type == JTokenType.Property)
                    {
                        var property = (child as JProperty);
                        if (property.Value != null)
                        {
                            var type = JsonTypeMapper.Get(property.Value?.Type);
                            if (!columnDictionary.ContainsKey(property.Name) || columnDictionary[property.Name] == null || (type != null && TypeParser.TYPE_PRIORITY_MAP.ContainsKey(type) && TypeParser.TYPE_PRIORITY_MAP[type] < TypeParser.TYPE_PRIORITY_MAP[columnDictionary[property.Name]]))
                            {
                                columnDictionary[property.Name] = type;
                            }
                            if (!_columns.Contains(property.Name))
                            {
                                _columns.Add(property.Name);
                                _columnOrdinal[property.Name] = _columns.Count - 1;
                            }
                        }
                    }
                }
                //x.Children().Select(c=> c.Nam)
            }

            _columnTypes = new Type[_columnOrdinal.Count];
            foreach (var kvp in columnDictionary)
            {
                _columnTypes[_columnOrdinal[kvp.Key]] = kvp.Value;
            }

            //_columnTypes = new Type[_columns.Length];
            //_parseMethods = new MethodInfo[_columns.Length];

            //for (var i = 0; i < _parseMethods.Length; i++)
            //{
            //    var type = _columnTypes[i];
            //    if (type != typeof(string))
            //    {
            //        _parseMethods[i] = type.GetMethod("Parse", new[] { typeof(string) });
            //    }
            //}

            // reset stream position to data rows
            
            _streamDone = false;
            if (_zipEntry != null)
            {
                _jsonReader.Close();
                _stream.Close();
                _stream.Dispose();
                _streamReader.Close();
                _streamReader.Dispose();
                _stream = _zipEntry.Open();
                _streamReader = new StreamReader(_stream);
                _jsonReader = new JsonTextReader(_streamReader);
            }
            else
            {
                _stream.Seek(0, SeekOrigin.Begin);
            }
            _jsonReader = new JsonTextReader(_streamReader);

            _rowIndex = 0;

            
        }


        private int _rowIndex;

        public object this[int i] => _rowData[i];

        public object this[string name] => _rowData[_columnOrdinal[name]];

        public int Depth => 1;

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => _rowIndex;

        public int FieldCount => _columns.Count;

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
            return (bool)_rowData[i];
        }

        public byte GetByte(int i)
        {
            return (byte)_rowData[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)_rowData[i];
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
            return (DateTime)_rowData[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)_rowData[i];
        }

        public double GetDouble(int i)
        {
            return (double)_rowData[i];
        }

        public Type GetFieldType(int i)
        {
            return _columnTypes[i];
        }

        public float GetFloat(int i)
        {
            return (float)_rowData[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)_rowData[i];
        }

        public short GetInt16(int i)
        {
            return (short)_rowData[i];
        }

        public int GetInt32(int i)
        {
            return (int)_rowData[i];
        }

        public long GetInt64(int i)
        {
            return (long)_rowData[i];
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
            return (string)_rowData[i];
        }

        public object GetValue(int i)
        {
            return _rowData[i];
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
            if (_streamDone)
            {
                return false;
            }
            _rowData = Parse();
            if (_streamDone)
            {
                return false;
            }
            _rowIndex++;
            return true;
        }

        private object[] Parse()
        {
            while (_jsonReader.Read())
            {
                if (_jsonReader.TokenType == JsonToken.StartObject)
                {
                    var rowData = new object[_columns.Count];
                    var result = (IDictionary<string, object>)JsonSerializer.Create().Deserialize<ExpandoObject>(_jsonReader);
                    foreach (var key in result.Keys)
                    {
                        var columnOrdinal = _columnOrdinal[key];
                        rowData[columnOrdinal] = TypeParser.ConvertType(result[key], _columnTypes[columnOrdinal]);
                    }

                    return rowData;
                }
            }
            _streamDone = true;
            return null;
        }

        private bool ReadColumns()
        {
            while (_jsonReader.Read())
            {
                if (_jsonReader.TokenType == JsonToken.StartObject)
                {
                    _columnObj = JObject.Load(_jsonReader);
                    return true;
                }
            }
            _streamDone = true;
            return false;
        }
    }
}
