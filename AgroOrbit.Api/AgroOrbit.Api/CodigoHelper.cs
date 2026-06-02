using Microsoft.EntityFrameworkCore;

namespace AgroOrbit.Api;

public static class CodigoHelper
{
    public static async Task<string> GerarCodigoUnicoAsync(AgroDbContext db, string codigoBase)
    {
        var codigoLimpo = string.IsNullOrWhiteSpace(codigoBase)
            ? "EQP"
            : codigoBase.Trim().ToUpper();

        var codigoFinal = codigoLimpo;
        var contador = 2;

        while (await db.Equipamentos.AnyAsync(e => e.Codigo == codigoFinal))
        {
            codigoFinal = $"{codigoLimpo}-{contador}";
            contador++;
        }

        return codigoFinal;
    }
}
