namespace AgroOrbit.Api;

public record CriarFazendaRequest(string Nome, string Proprietario, string Cidade, string Estado, decimal AreaHectares);

public record CriarTalhaoRequest(string Nome, string Cultura, decimal AreaHectares, double Latitude, double Longitude);

public record CriarSateliteRequest(string Nome, string Codigo, int FazendaId, string ProvedorImagem, int RevisitaHoras);

public record CriarDroneRequest(string Nome, string Codigo, int FazendaId, int AutonomiaMinutos, string RotaPadrao);

public record CriarSensorIotRequest(string Nome, string Codigo, int FazendaId, string GrandezaMonitorada);

public record CriarLeituraSateliteRequest(int TalhaoId, int SateliteId, decimal IndiceSaude, decimal UmidadeEstimada, DateTime? CapturadoEmUtc);

public record CriarLeituraSensorRequest(int TalhaoId, int SensorIotId, decimal UmidadeSolo, decimal Temperatura, DateTime? CapturadoEmUtc);

public record CriarVarreduraDroneRequest(int TalhaoId, int DroneId, string UrlImagem, decimal? PercentualAnomalia, DateTime? CapturadoEmUtc);

public record AtualizarStatusTalhaoRequest(string StatusAtual);
