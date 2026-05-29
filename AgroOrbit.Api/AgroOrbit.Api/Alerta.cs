namespace AgroOrbit.Api;

public class Alerta
{
    private Alerta()
    {
    }

    public Alerta(int fazendaId, int talhaoId, TipoAlerta tipo, NivelAlerta nivel, string mensagem, DateTime geradoEmUtc)
    {
        if (string.IsNullOrWhiteSpace(mensagem))
            throw new RegraNegocioException("A mensagem do alerta é obrigatória.");

        FazendaId = fazendaId;
        TalhaoId = talhaoId;
        Tipo = tipo;
        Nivel = nivel;
        Mensagem = mensagem.Trim();
        Status = StatusAlerta.Aberto;
        GeradoEmUtc = geradoEmUtc;
    }

    public int Id { get; private set; }
    public int FazendaId { get; private set; }
    public int TalhaoId { get; private set; }
    public TipoAlerta Tipo { get; private set; }
    public NivelAlerta Nivel { get; private set; }
    public string Mensagem { get; private set; } = string.Empty;
    public StatusAlerta Status { get; private set; }
    public DateTime GeradoEmUtc { get; private set; }
    public DateTime? ResolvidoEmUtc { get; private set; }

    public void MarcarEmAnalise()
    {
        Status = StatusAlerta.EmAnalise;
    }

    public void Resolver()
    {
        Status = StatusAlerta.Resolvido;
        ResolvidoEmUtc = DateTime.UtcNow;
    }
}
