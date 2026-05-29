using Microsoft.EntityFrameworkCore;

namespace AgroOrbit.Api;

public class DashboardService : IDashboardService
{
    private readonly AgroDbContext _db;
    private readonly ITimeZoneService _timeZone;

    public DashboardService(AgroDbContext db, ITimeZoneService timeZone)
    {
        _db = db;
        _timeZone = timeZone;
    }

    public async Task<DashboardResponse> ObterDashboardAsync(int fazendaId)
    {
        var fazenda = await _db.Fazendas.AsNoTracking().FirstOrDefaultAsync(f => f.Id == fazendaId)
            ?? throw new RecursoNaoEncontradoException("Fazenda nao encontrada.");

        var talhoes = await _db.Talhoes.AsNoTracking().Where(t => t.FazendaId == fazendaId).ToListAsync();
        var itens = new List<TalhaoDashboardResponse>();

        foreach (var talhao in talhoes)
        {
            var sat = await _db.LeiturasSatelite.AsNoTracking().Where(l => l.TalhaoId == talhao.Id).OrderByDescending(l => l.CapturadoEmUtc).FirstOrDefaultAsync();
            var sen = await _db.LeiturasSensor.AsNoTracking().Where(l => l.TalhaoId == talhao.Id).OrderByDescending(l => l.CapturadoEmUtc).FirstOrDefaultAsync();
            var dro = await _db.VarredurasDrone.AsNoTracking().Where(v => v.TalhaoId == talhao.Id).OrderByDescending(v => v.CapturadoEmUtc).FirstOrDefaultAsync();

            itens.Add(new TalhaoDashboardResponse(talhao.Id, talhao.Nome, talhao.Cultura, talhao.StatusAtual, sat?.IndiceSaude, sen?.UmidadeSolo, dro?.PercentualAnomalia));
        }

        var ids = talhoes.Select(t => t.Id).ToList();
        var media = await _db.LeiturasSatelite.AsNoTracking().Where(l => ids.Contains(l.TalhaoId)).Select(l => (decimal?)l.IndiceSaude).AverageAsync() ?? 0m;
        var abertos = await _db.Alertas.AsNoTracking().CountAsync(a => a.FazendaId == fazendaId && a.Status != StatusAlerta.Resolvido);
        var ultimos = await _db.Alertas.AsNoTracking().Where(a => a.FazendaId == fazendaId).OrderByDescending(a => a.GeradoEmUtc).Take(5).Select(a => new AlertaResumoResponse(a.Id, a.Tipo.ToString(), a.Nivel.ToString(), a.Status.ToString(), a.Mensagem, a.GeradoEmUtc)).ToListAsync();

        var agora = DateTime.UtcNow;
        return new DashboardResponse(fazenda.Id, fazenda.Nome, agora, _timeZone.ConverterParaHorarioBrasilia(agora), talhoes.Count, abertos, Math.Round(media, 2), itens, ultimos);
    }
}
