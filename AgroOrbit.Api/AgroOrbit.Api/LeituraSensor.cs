namespace AgroOrbit.Api;

public class LeituraSensor
{
    private LeituraSensor()
    {
    }

    public LeituraSensor(int talhaoId, int sensorIotId, decimal umidadeSolo, decimal temperatura, DateTime capturadoEmUtc)
    {
        if (umidadeSolo < 0 || umidadeSolo > 100)
            throw new RegraNegocioException("A umidade do solo deve estar entre 0 e 100.");

        if (temperatura < -10 || temperatura > 70)
            throw new RegraNegocioException("A temperatura informada está fora do intervalo esperado para a lavoura.");

        TalhaoId = talhaoId;
        SensorIotId = sensorIotId;
        UmidadeSolo = umidadeSolo;
        Temperatura = temperatura;
        CapturadoEmUtc = capturadoEmUtc;
    }

    public int Id { get; private set; }
    public int TalhaoId { get; private set; }
    public int SensorIotId { get; private set; }
    public decimal UmidadeSolo { get; private set; }
    public decimal Temperatura { get; private set; }
    public DateTime CapturadoEmUtc { get; private set; }
}
