using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace OverlyComplicatedDataTools.Core.Helpers
{
    public static class JsonTypeMapper
    {
        public static Type Get(JTokenType? type)
        {
            switch (type)
            {
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Date:
                    return typeof(DateTime);
                case JTokenType.Raw:
                case JTokenType.String:
                    return typeof(string);
                case JTokenType.Boolean:
                    return typeof(bool);
                case JTokenType.Float:
                    return typeof(float);
                case JTokenType.Guid:
                    return typeof(Guid);
                case JTokenType.Integer:
                    return typeof(int);
            }

            return null;
        }
    }
}
