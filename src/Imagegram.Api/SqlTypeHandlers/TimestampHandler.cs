using System;
using System.Data;
using System.Linq;
using Dapper;

namespace Imagegram.Api.SqlTypeMaps
{
    public class TimestampHandler : SqlMapper.TypeHandler<long>
    {
        public override long Parse(object value)
        {
            if (value is byte[] bytes) // timestamp
                return BitConverter.ToInt64(bytes.Reverse().ToArray());
            
            return Convert.ToInt64(value);
        }

        public override void SetValue(IDbDataParameter parameter, long value)
        {
            parameter.Value = value.ToString();
        }
    }
}