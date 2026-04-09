using Dapper;
using System.Data;
using System.Runtime.InteropServices;

namespace GodsEye.Infrastructure.Services
{
    public class FloatArrayHandler : SqlMapper.TypeHandler<float[]>
    {
        // Quando você envia para o banco (Insert/Update)
        public override void SetValue(IDbDataParameter parameter, float[] value)
        {
            parameter.Value = value;
        }

        // Quando você lê do banco (Select)
        public override float[] Parse(object value)
        {
            if (value is byte[] bytes)
            {
                if (bytes.Length == 0) return Array.Empty<float>();
                return MemoryMarshal.Cast<byte, float>(bytes).ToArray();
            }

            // Se cair aqui, o Dapper está recebendo algo que NÃO é binário (provavelmente String/JSON)
            throw new InvalidOperationException($"Dapper retornou {value?.GetType().Name} em vez de byte[]. Verifique se a Procedure está retornando a coluna pura.");
        }
    }
}
