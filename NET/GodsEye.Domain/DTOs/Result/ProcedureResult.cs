namespace GodsEye.Domain.DTOs.Result
{
    public class ProcedureResult
    {
        public int Erro { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int Id { get; set; }

        public static ProcedureResult Error(string? message = null) => new ProcedureResult
        {
            Erro = 1,
            Mensagem = message ?? "Houve um erro ao executar a procedure!",
            Id = 0
        };
    }
}
