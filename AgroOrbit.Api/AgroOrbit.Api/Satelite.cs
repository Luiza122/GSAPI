namespace AgroOrbit.Api;

public class Satelite : EquipamentoMonitoramento
{
    private Satelite()
    {
    }

    public Satelite(string nome, string codigo, int fazendaId, string provedorImagem, int revisitaHoras)
        : base(nome, codigo, fazendaId)
    {
        ProvedorImagem = string.IsNullOrWhiteSpace(provedorImagem) ? "NASA/INPE" : provedorImagem.Trim();
        RevisitaHoras = revisitaHoras <= 0 ? 24 : revisitaHoras;
    }

    public string ProvedorImagem { get; private set; } = string.Empty;
    public int RevisitaHoras { get; private set; }
    public override TipoEquipamento Tipo => TipoEquipamento.Satelite;

    public override string DescreverOperacao()
    {
        return $"Recebe imagens orbitais do provedor {ProvedorImagem} a cada {RevisitaHoras} horas.";
    }
}
