namespace AgroOrbit.Api;

public class Drone : EquipamentoMonitoramento
{
    private Drone()
    {
    }

    public Drone(string nome, string codigo, int fazendaId, int autonomiaMinutos, string rotaPadrao)
        : base(nome, codigo, fazendaId)
    {
        AutonomiaMinutos = autonomiaMinutos <= 0 ? 30 : autonomiaMinutos;
        RotaPadrao = string.IsNullOrWhiteSpace(rotaPadrao) ? "Rota geral da fazenda" : rotaPadrao.Trim();
    }

    public int AutonomiaMinutos { get; private set; }
    public string RotaPadrao { get; private set; } = string.Empty;
    public override TipoEquipamento Tipo => TipoEquipamento.Drone;

    public override string DescreverOperacao()
    {
        return $"Realiza varredura aérea pela rota '{RotaPadrao}' com autonomia de {AutonomiaMinutos} minutos.";
    }
}
