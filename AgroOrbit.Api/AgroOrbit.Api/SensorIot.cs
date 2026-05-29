namespace AgroOrbit.Api;

public class SensorIot : EquipamentoMonitoramento
{
    private SensorIot()
    {
    }

    public SensorIot(string nome, string codigo, int fazendaId, string grandezaMonitorada)
        : base(nome, codigo, fazendaId)
    {
        GrandezaMonitorada = string.IsNullOrWhiteSpace(grandezaMonitorada) ? "Umidade do solo" : grandezaMonitorada.Trim();
    }

    public string GrandezaMonitorada { get; private set; } = string.Empty;
    public override TipoEquipamento Tipo => TipoEquipamento.SensorIot;

    public override string DescreverOperacao()
    {
        return $"Coleta dados de {GrandezaMonitorada} em tempo real por IoT.";
    }
}
