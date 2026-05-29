namespace AgroOrbit.Api;

public static class ClassificadorRiscoHelper
{
    public static NivelAlerta ClassificarPorPercentual(decimal percentual)
    {
        if (percentual >= 75) return NivelAlerta.Critico;
        if (percentual >= 50) return NivelAlerta.Alto;
        if (percentual >= 25) return NivelAlerta.Medio;
        return NivelAlerta.Baixo;
    }
}

public class AnaliseImagemService : IAnaliseImagemService
{
    public decimal CalcularPercentualAnomalia(string urlImagem, int talhaoId)
    {
        if (string.IsNullOrWhiteSpace(urlImagem))
            throw new RegraNegocioException("A imagem do drone é obrigatória para análise.");

        var valor = Math.Abs(HashCode.Combine(urlImagem, talhaoId, DateTime.UtcNow.Date)) % 31;
        return valor;
    }
}

public class TimeZoneService : ITimeZoneService
{
    public DateTime ConverterParaHorarioBrasilia(DateTime dataUtc)
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dataUtc, DateTimeKind.Utc), timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            return dataUtc.AddHours(-3);
        }
        catch (InvalidTimeZoneException)
        {
            return dataUtc.AddHours(-3);
        }
    }
}
