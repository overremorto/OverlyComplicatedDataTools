using Microsoft.Extensions.Configuration;
using OverlyComplicatedDataTools.Core.Readers;
using OverlyComplicatedDataTools.Core.Writers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;

namespace OverlyComplicatedDataTools
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new OCDTOptions()
            {
                ConnectionString = "Server=.;Database=DataEngineerToolkitTest;Trusted_Connection=True;",
                FilePath = "C:\\Git\\OverlyComplicatedDataTools\\OverlyComplicatedDataTools\\TestFiles\\TestCSV-out-of-order-small.csv",
                TableName = "dbo.TestTableCSVOutOfOrder"
            };
            var writer = new OCDTSqlDataWriter(options);

            //var dataTable = new DataTable();
            //dataTable.Columns.Add("ID", typeof(int));
            //dataTable.Columns.Add("Value", typeof(string));
            //dataTable.TableName = options.TableName;

            //for (var i = 0; i < 1000000; i++)
            //{
            //    var row = dataTable.NewRow();
            //    row["ID"] = i;
            //    row["Value"] = Encoding.UTF8.GetString(BytesFromInt(i));
            //    dataTable.Rows.Add(row);
            //}
            //using (var fs = new FileStream(options.FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            //{
            //    using (var sw = new StreamWriter(fs))
            //    {
            //        sw.WriteLine($"ID,Value,Value2,Value3,Value4,Value5,Value6,Value7,Value8,Value9,Value10,Value11,Value12,Value13,Value14,Value15,Value16,Value17,Value18,Value19,Value20");
            //        for (var i = 0; i < 1000000; i++)
            //        {
            //            sw.WriteLine($"{i},{Encoding.UTF8.GetString(BytesFromInt((i % 42) + 48))}");
            //        }
            //    }
            //}

            using (var dataTableReader = new OCDTCSVReader(options.FilePath))
            {
                writer.Write(dataTableReader).Wait();
                dataTableReader.Close();
            }
        }

        static byte[] BytesFromInt(int intValue)
        {
            var hex = intValue.ToString("X");
            if (hex.Length % 2 != 0)
            {
                hex = "0" + hex;
            }
            return ConvertHexStringToByteArray(hex);
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
