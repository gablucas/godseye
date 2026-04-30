namespace GodsEye.Shared.Response
{
    public class ProcedureResponse
    {
        public int Erro { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int Id { get; set; }

        public static ProcedureResponse Error(string? message = null) => new ProcedureResponse
        {
            Erro = 1,
            Mensagem = message ?? "Houve um erro ao executar a procedure!",
            Id = 0
        };
    }
}
