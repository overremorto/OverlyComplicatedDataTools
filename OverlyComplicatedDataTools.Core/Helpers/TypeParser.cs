using System;
using System.Collections.Generic;
using System.Text;

namespace OverlyComplicatedDataTools.Core.Helpers
{
    public static class TypeParser
    {
        public static Dictionary<Type, int> TYPE_PRIORITY_MAP = new Dictionary<Type, int>()
        {
            { typeof(DateTime), 100 },
            { typeof(int), 90 },
            { typeof(decimal), 80 },
            { typeof(float), 70 },
            { typeof(double), 60 },
            { typeof(Guid), 50 },
            { typeof(bool), 40 },
            { typeof(char), 10 },
            { typeof(string), 0 }
        };


        public static Type DetermineType(string type)
        {
            int intResult;
            decimal decimalResult;
            float floatResult;
            double doubleResult;
            DateTime dateTimeResult;
            Guid guidResult;
            bool boolResult;
            char charResult;
            string stringResult;
            if (int.TryParse(type, out intResult))
            {
                return typeof(int);
            }
            else if (double.TryParse(type, out doubleResult))
            {
                return typeof(double);
            }
            else if (decimal.TryParse(type, out decimalResult))
            {
                return typeof(decimal);
            }
            else if (float.TryParse(type, out floatResult))
            {
                return typeof(float);
            }
            else if (DateTime.TryParse(type, out dateTimeResult))
            {
                return typeof(DateTime);
            }
            else if (Guid.TryParse(type, out guidResult))
            {
                return typeof(Guid);
            }
            else if (bool.TryParse(type, out boolResult))
            {
                return typeof(bool);
            }
            else if (char.TryParse(type, out charResult))
            {
                return typeof(char);
            }
            else
            {
                return typeof(string);
            }
        }

        public static object ConvertType(object obj, Type type)
        {
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return Convert.ToBoolean(obj);
            }
            else if (type == typeof(byte) || type == typeof(byte?))
            {
                return Convert.ToByte(obj);
            }
            else if (type == typeof(char) || type == typeof(char?))
            {
                return Convert.ToChar(obj);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return Convert.ToDateTime(obj);
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return Convert.ToDecimal(obj);
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return Convert.ToDouble(obj);
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                return Convert.ToInt16(obj);
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                return Convert.ToInt32(obj);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                return Convert.ToInt64(obj);
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                return Convert.ToSingle(obj);
            }
            else if (type == typeof(string))
            {
                return obj?.ToString() as string;
            }

            return null as object;
        }
    }
}
