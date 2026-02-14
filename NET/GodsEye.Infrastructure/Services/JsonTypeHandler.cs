using Dapper;
using System.Data;
using System.Text.Json;

namespace GodsEye.Infrastructure.Services
{
    public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        public override void SetValue(IDbDataParameter parameter, T? value)
        {
            parameter.Value = (value == null)
            ? DBNull.Value
            : JsonSerializer.Serialize(value);

            parameter.DbType = DbType.String;
        }

        public override T Parse(object value)
        {
            if (value is string json) 
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return default;
        }
    }
}
