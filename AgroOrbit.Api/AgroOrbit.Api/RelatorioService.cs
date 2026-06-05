using Microsoft.EntityFrameworkCore;

namespace AgroOrbit.Api;

public class RelatorioService : IRelatorioService
{
    private readonly AgroDbContext _db;
    private readonly ILogger<RelatorioService> _logger;

    public RelatorioService(AgroDbContext db, ILogger<RelatorioService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<RelatorioSemanalResponse> GerarRelatorioSemanalAsync(int fazendaId, DateTime inicioUtc, DateTime fimUtc)
    {
        if (fimUtc <= inicioUtc)
            throw new RegraNegocioException("A data final deve ser maior que a data inicial.");

        var existe = await _db.Fazendas.AnyAsync(f => f.Id == fazendaId);
        if (!existe)
            throw new RecursoNaoEncontradoException("Fazenda nao encontrada.");

        var talhaoIds = await _db.Talhoes.Where(t => t.FazendaId == fazendaId).Select(t => t.Id).ToListAsync();
        var totalAlertas = await _db.Alertas.CountAsync(a => a.FazendaId == fazendaId && a.GeradoEmUtc >= inicioUtc && a.GeradoEmUtc <= fimUtc);

        var indicesSaude = await _db.LeiturasSatelite
            .Where(l => talhaoIds.Contains(l.TalhaoId) && l.CapturadoEmUtc >= inicioUtc && l.CapturadoEmUtc <= fimUtc)
            .Select(l => l.IndiceSaude)
            .ToListAsync();

        var mediaSaude = indicesSaude.Any()
            ? indicesSaude.Average()
            : 0m;

        var resumo = $"No periodo analisado, a fazenda apresentou {totalAlertas} alerta(s) e media de saude {mediaSaude:N2}.";

        var relatorio = new RelatorioSemanal(fazendaId, inicioUtc, fimUtc, totalAlertas, Math.Round(mediaSaude, 2), resumo);
        _db.RelatoriosSemanais.Add(relatorio);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Relatorio semanal gerado para fazenda {FazendaId}", fazendaId);

        return new RelatorioSemanalResponse(relatorio.Id, relatorio.FazendaId, relatorio.InicioUtc, relatorio.FimUtc, relatorio.TotalAlertas, relatorio.MediaSaude, relatorio.Resumo, relatorio.GeradoEmUtc);
    }
}
