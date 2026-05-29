namespace AgroOrbit.Api;

public interface IAlertaService
{
    Task<IReadOnlyCollection<Alerta>> AvaliarLeituraSateliteAsync(LeituraSatelite leitura);
    Task<IReadOnlyCollection<Alerta>> AvaliarLeituraSensorAsync(LeituraSensor leitura);
    Task<IReadOnlyCollection<Alerta>> AvaliarVarreduraDroneAsync(VarreduraDrone varredura);
}

public interface IAnaliseImagemService
{
    decimal CalcularPercentualAnomalia(string urlImagem, int talhaoId);
}

public interface IDashboardService
{
    Task<DashboardResponse> ObterDashboardAsync(int fazendaId);
}

public interface IRelatorioService
{
    Task<RelatorioSemanalResponse> GerarRelatorioSemanalAsync(int fazendaId, DateTime inicioUtc, DateTime fimUtc);
}

public interface ITimeZoneService
{
    DateTime ConverterParaHorarioBrasilia(DateTime dataUtc);
}
