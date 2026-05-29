namespace AgroOrbit.Api;

public enum TipoEquipamento
{
    Satelite = 1,
    Drone = 2,
    SensorIot = 3
}

public enum StatusEquipamento
{
    Ativo = 1,
    EmManutencao = 2,
    Inativo = 3
}

public enum NivelAlerta
{
    Baixo = 1,
    Medio = 2,
    Alto = 3,
    Critico = 4
}

public enum TipoAlerta
{
    Seca = 1,
    Praga = 2,
    AnomaliaVisual = 3,
    FalhaEquipamento = 4,
    RiscoClimatico = 5
}

public enum StatusAlerta
{
    Aberto = 1,
    EmAnalise = 2,
    Resolvido = 3
}
