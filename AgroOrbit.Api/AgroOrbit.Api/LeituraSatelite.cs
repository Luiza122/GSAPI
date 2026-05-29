namespace AgroOrbit.Api;

public class LeituraSatelite
{
    private LeituraSatelite()
    {
    }

    public LeituraSatelite(int talhaoId, int sateliteId, decimal indiceSaude, decimal umidadeEstimada, DateTime capturadoEmUtc)
    {
        if (indiceSaude < 0 || indiceSaude > 1)
            throw new RegraNegocioException("O índice de saúde deve estar entre 0 e 1.");

        if (umidadeEstimada < 0 || umidadeEstimada > 100)
            throw new RegraNegocioException("A umidade estimada deve estar entre 0 e 100.");

        TalhaoId = talhaoId;
        SateliteId = sateliteId;
        IndiceSaude = indiceSaude;
        UmidadeEstimada = umidadeEstimada;
        CapturadoEmUtc = capturadoEmUtc;
    }

    public int Id { get; private set; }
    public int TalhaoId { get; private set; }
    public int SateliteId { get; private set; }
    public decimal IndiceSaude { get; private set; }
    public decimal UmidadeEstimada { get; private set; }
    public DateTime CapturadoEmUtc { get; private set; }
}
