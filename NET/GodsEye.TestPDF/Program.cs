using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Companion;
using GodsEye.TestPDF;

QuestPDF.Settings.License = LicenseType.Community;


var nomes = new[] {
    "Gabriel", "Thiago", "Junior", "Ana", "Carlos", "Fernanda", "Ricardo",
    "Juliana", "Marcos", "Patrícia", "Felipe", "Camila", "Bruno", "Larissa",
    "Eduardo", "Mariana", "Lucas", "Beatriz", "Rodrigo", "Isabela"
};

var cargos = new[] {
    "Desenvolvedor", "Administrador", "Comercial", "Analista", "Gerente",
    "Designer", "Suporte", "RH", "Financeiro", "Marketing"
};

var departamentos = new[] {
    "TI", "Administrativo", "Vendas", "Operações", "Gestão",
    "Criação", "Atendimento", "Pessoas", "Finanças", "Comunicação"
};

var status = new[] { "Ativo", "Inativo", "Férias", "Afastado" };

var random = new Random(42);

var teste = new List<TestModel>();

for (int i = 1; i <= 100; i++)
{
    var nome = nomes[random.Next(nomes.Length)];
    var cargo = cargos[random.Next(cargos.Length)];
    var depto = departamentos[random.Next(departamentos.Length)];
    var st = status[random.Next(status.Length)];
    var salario = random.Next(2000, 15000);

    teste.Add(new TestModel
    {
        Name = $"{nome} {(char)('A' + random.Next(26))}. Silva",
        Cargo = cargo,
        Departamento = depto,
        Email = $"{nome.ToLower()}.{i:D3}@empresa.com.br",
        Telefone = $"(47) 9{random.Next(1000, 9999)}-{random.Next(1000, 9999)}",
        Status = st,
        Salario = $"R$ {salario:N2}"
    });
}

// Documento
var document = new ReportDocument<TestModel>("Relatório de teste", teste);
document.Teste("Nome", x => x.Name);
document.Teste("Cargo", x => x.Cargo);
document.Teste("Departamento", x => x.Departamento);
document.Teste("E-mail", x => x.Email);
document.Teste("Telefone", x => x.Telefone);
document.Teste("Status", x => x.Status);
document.Teste("Salário", x => x.Salario);

document.ShowInCompanion();