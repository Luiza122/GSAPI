namespace AgroOrbit.Api;

public class Talhao
{
    private Talhao()
    {
    }

    public Talhao(string nome, string cultura, decimal areaHectares, double latitude, double longitude, int fazendaId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraNegocioException("O nome do talhão é obrigatório.");

        if (areaHectares <= 0)
            throw new RegraNegocioException("A área do talhão deve ser maior que zero.");

        Nome = nome.Trim();
        Cultura = string.IsNullOrWhiteSpace(cultura) ? "Não informada" : cultura.Trim();
        AreaHectares = areaHectares;
        Latitude = latitude;
        Longitude = longitude;
        FazendaId = fazendaId;
        StatusAtual = "Sem leitura recente";
        AtualizadoEm = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Cultura { get; private set; } = string.Empty;
    public decimal AreaHectares { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string StatusAtual { get; private set; } = string.Empty;
    public DateTime AtualizadoEm { get; private set; }
    public int FazendaId { get; private set; }
    public Fazenda? Fazenda { get; private set; }

    public void AtualizarStatus(string novoStatus)
    {
        if (string.IsNullOrWhiteSpace(novoStatus))
            throw new RegraNegocioException("O status do talhão não pode ser vazio.");

        StatusAtual = novoStatus.Trim();
        AtualizadoEm = DateTime.UtcNow;
    }
}
