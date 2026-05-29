namespace AgroOrbit.Api;

public class VarreduraDrone
{
    private VarreduraDrone()
    {
    }

    public VarreduraDrone(int talhaoId, int droneId, string urlImagem, decimal percentualAnomalia, DateTime capturadoEmUtc)
    {
        if (string.IsNullOrWhiteSpace(urlImagem))
            throw new RegraNegocioException("A URL ou identificação da imagem do drone é obrigatória.");

        if (percentualAnomalia < 0 || percentualAnomalia > 100)
            throw new RegraNegocioException("O percentual de anomalia deve estar entre 0 e 100.");

        TalhaoId = talhaoId;
        DroneId = droneId;
        UrlImagem = urlImagem.Trim();
        PercentualAnomalia = percentualAnomalia;
        CapturadoEmUtc = capturadoEmUtc;
    }

    public int Id { get; private set; }
    public int TalhaoId { get; private set; }
    public int DroneId { get; private set; }
    public string UrlImagem { get; private set; } = string.Empty;
    public decimal PercentualAnomalia { get; private set; }
    public DateTime CapturadoEmUtc { get; private set; }
}
