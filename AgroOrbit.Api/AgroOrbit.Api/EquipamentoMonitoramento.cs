namespace AgroOrbit.Api;

public abstract class EquipamentoMonitoramento : IMonitoravel
{
    protected EquipamentoMonitoramento()
    {
    }

    protected EquipamentoMonitoramento(string nome, string codigo, int fazendaId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraNegocioException("O nome do equipamento é obrigatório.");

        if (string.IsNullOrWhiteSpace(codigo))
            throw new RegraNegocioException("O código do equipamento é obrigatório.");

        Nome = nome.Trim();
        Codigo = codigo.Trim().ToUpper();
        FazendaId = fazendaId;
        Status = StatusEquipamento.Ativo;
        CriadoEm = DateTime.UtcNow;
    }

    public int Id { get; protected set; }
    public string Nome { get; protected set; } = string.Empty;
    public string Codigo { get; protected set; } = string.Empty;
    public StatusEquipamento Status { get; protected set; }
    public DateTime CriadoEm { get; protected set; }
    public int FazendaId { get; protected set; }
    public Fazenda? Fazenda { get; protected set; }
    public abstract TipoEquipamento Tipo { get; }

    public bool EstaOperacional()
    {
        return Status == StatusEquipamento.Ativo;
    }

    public abstract string DescreverOperacao();
}
