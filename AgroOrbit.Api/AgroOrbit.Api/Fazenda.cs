namespace AgroOrbit.Api;

public class Fazenda
{
    private Fazenda()
    {
    }

    public Fazenda(string nome, string proprietario, string cidade, string estado, decimal areaHectares)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraNegocioException("O nome da fazenda é obrigatório.");

        if (areaHectares <= 0)
            throw new RegraNegocioException("A área da fazenda deve ser maior que zero.");

        Nome = nome.Trim();
        Proprietario = string.IsNullOrWhiteSpace(proprietario) ? "Não informado" : proprietario.Trim();
        Cidade = string.IsNullOrWhiteSpace(cidade) ? "Não informada" : cidade.Trim();
        Estado = string.IsNullOrWhiteSpace(estado) ? "SP" : estado.Trim().ToUpper();
        AreaHectares = areaHectares;
        CriadaEm = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Proprietario { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;
    public decimal AreaHectares { get; private set; }
    public DateTime CriadaEm { get; private set; }
}
