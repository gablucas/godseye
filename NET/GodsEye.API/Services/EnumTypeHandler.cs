using Dapper;
using System.Data;

namespace GodsEye.API.Services
{
    public class EnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
    {
        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = value.ToString();
        }

        public override T Parse(object value)
        {
            return Enum.Parse<T>(value.ToString(), ignoreCase: true);
        }
    }
}
