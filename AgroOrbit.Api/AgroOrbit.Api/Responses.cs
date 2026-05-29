namespace AgroOrbit.Api;

public record FazendaResumoResponse(
    int Id,
    string Nome,
    string Proprietario,
    string Cidade,
    string Estado,
    decimal AreaHectares,
    int TotalTalhoes,
    int TotalEquipamentos);

public record EquipamentoResponse(
    int Id,
    string Nome,
    string Codigo,
    TipoEquipamento Tipo,
    StatusEquipamento Status,
    string Operacao);

public record AlertaResumoResponse(
    int Id,
    string Tipo,
    string Nivel,
    string Status,
    string Mensagem,
    DateTime GeradoEmUtc);

public record TalhaoDashboardResponse(
    int TalhaoId,
    string Nome,
    string Cultura,
    string StatusAtual,
    decimal? UltimoIndiceSaude,
    decimal? UltimaUmidadeSolo,
    decimal? UltimaAnomaliaDrone);

public record DashboardResponse(
    int FazendaId,
    string Fazenda,
    DateTime AtualizadoEmUtc,
    DateTime AtualizadoEmHorarioBrasilia,
    int TotalTalhoes,
    int AlertasAbertos,
    decimal MediaSaudeLavoura,
    IReadOnlyCollection<TalhaoDashboardResponse> Talhoes,
    IReadOnlyCollection<AlertaResumoResponse> UltimosAlertas);

public record RelatorioSemanalResponse(
    int Id,
    int FazendaId,
    DateTime InicioUtc,
    DateTime FimUtc,
    int TotalAlertas,
    decimal MediaSaude,
    string Resumo,
    DateTime GeradoEmUtc);
