using Microsoft.EntityFrameworkCore;

namespace AgroOrbit.Api;

public class AlertaService : IAlertaService
{
    private readonly AgroDbContext _db;
    private readonly ILogger<AlertaService> _logger;

    public AlertaService(AgroDbContext db, ILogger<AlertaService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Alerta>> AvaliarLeituraSateliteAsync(LeituraSatelite leitura)
    {
        var talhao = await _db.Talhoes.FirstOrDefaultAsync(t => t.Id == leitura.TalhaoId)
            ?? throw new RecursoNaoEncontradoException("Talhao nao encontrado.");

        var alertas = new List<Alerta>();

        if (leitura.IndiceSaude < 0.45m)
        {
            alertas.Add(new Alerta(talhao.FazendaId, talhao.Id, TipoAlerta.Praga, NivelAlerta.Alto, "Satelite detectou queda no indice de saude da lavoura.", leitura.CapturadoEmUtc));
            talhao.AtualizarStatus("Atencao: risco de praga");
        }

        if (leitura.UmidadeEstimada < 30m)
        {
            alertas.Add(new Alerta(talhao.FazendaId, talhao.Id, TipoAlerta.Seca, NivelAlerta.Critico, "Satelite identificou baixa umidade e risco de seca.", leitura.CapturadoEmUtc));
            talhao.AtualizarStatus("Critico: risco de seca");
        }

        _db.Alertas.AddRange(alertas);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Alertas gerados por satelite: {Total}", alertas.Count);
        return alertas;
    }

    public async Task<IReadOnlyCollection<Alerta>> AvaliarLeituraSensorAsync(LeituraSensor leitura)
    {
        var talhao = await _db.Talhoes.FirstOrDefaultAsync(t => t.Id == leitura.TalhaoId)
            ?? throw new RecursoNaoEncontradoException("Talhao nao encontrado.");

        var alertas = new List<Alerta>();

        if (leitura.UmidadeSolo < 25m)
        {
            alertas.Add(new Alerta(talhao.FazendaId, talhao.Id, TipoAlerta.Seca, NivelAlerta.Alto, "Sensor IoT registrou baixa umidade do solo.", leitura.CapturadoEmUtc));
            talhao.AtualizarStatus("Atencao: baixa umidade no solo");
        }

        if (leitura.Temperatura > 38m)
            alertas.Add(new Alerta(talhao.FazendaId, talhao.Id, TipoAlerta.RiscoClimatico, NivelAlerta.Medio, "Sensor IoT registrou temperatura elevada.", leitura.CapturadoEmUtc));

        _db.Alertas.AddRange(alertas);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Alertas gerados por IoT: {Total}", alertas.Count);
        return alertas;
    }

    public async Task<IReadOnlyCollection<Alerta>> AvaliarVarreduraDroneAsync(VarreduraDrone varredura)
    {
        var talhao = await _db.Talhoes.FirstOrDefaultAsync(t => t.Id == varredura.TalhaoId)
            ?? throw new RecursoNaoEncontradoException("Talhao nao encontrado.");

        var alertas = new List<Alerta>();

        if (varredura.PercentualAnomalia >= 15m)
        {
            var nivel = ClassificadorRiscoHelper.ClassificarPorPercentual(varredura.PercentualAnomalia);
            alertas.Add(new Alerta(talhao.FazendaId, talhao.Id, TipoAlerta.AnomaliaVisual, nivel, "Drone encontrou anomalia visual na plantacao.", varredura.CapturadoEmUtc));
            talhao.AtualizarStatus("Atencao: anomalia visual por drone");
        }

        _db.Alertas.AddRange(alertas);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Alertas gerados por drone: {Total}", alertas.Count);
        return alertas;
    }
}
