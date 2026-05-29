namespace AgroOrbit.Api;

public class RelatorioSemanal
{
    private RelatorioSemanal()
    {
    }

    public RelatorioSemanal(int fazendaId, DateTime inicioUtc, DateTime fimUtc, int totalAlertas, decimal mediaSaude, string resumo)
    {
        if (fimUtc <= inicioUtc)
            throw new RegraNegocioException("A data final do relatório deve ser maior que a data inicial.");

        FazendaId = fazendaId;
        InicioUtc = inicioUtc;
        FimUtc = fimUtc;
        TotalAlertas = totalAlertas;
        MediaSaude = mediaSaude;
        Resumo = resumo;
        GeradoEmUtc = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int FazendaId { get; private set; }
    public DateTime InicioUtc { get; private set; }
    public DateTime FimUtc { get; private set; }
    public int TotalAlertas { get; private set; }
    public decimal MediaSaude { get; private set; }
    public string Resumo { get; private set; } = string.Empty;
    public DateTime GeradoEmUtc { get; private set; }
}
