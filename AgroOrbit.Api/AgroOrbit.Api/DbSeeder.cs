namespace AgroOrbit.Api;

public static class DbSeeder
{
    public static void Seed(AgroDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Fazendas.Any())
            return;

        var fazenda = new Fazenda("Fazenda Horizonte Verde", "Grupo Agro Demo", "Ribeirão Preto", "SP", 1200);
        db.Fazendas.Add(fazenda);
        db.SaveChanges();

        var talhaoNorte = new Talhao("Talhão Norte", "Soja", 420, -21.1701, -47.8103, fazenda.Id);
        var talhaoSul = new Talhao("Talhão Sul", "Milho", 380, -21.1760, -47.8150, fazenda.Id);
        db.Talhoes.AddRange(talhaoNorte, talhaoSul);

        var satelite = new Satelite("Satélite AgroOrbital", "SAT-001", fazenda.Id, "NASA/INPE", 24);
        var drone = new Drone("Drone Varredura 01", "DRN-001", fazenda.Id, 45, "Rota Norte/Sul");
        var sensor = new SensorIot("Sensor Solo 01", "IOT-001", fazenda.Id, "Umidade do solo e temperatura");
        db.Equipamentos.AddRange(satelite, drone, sensor);
        db.SaveChanges();

        db.LeiturasSatelite.Add(new LeituraSatelite(talhaoNorte.Id, satelite.Id, 0.82m, 63m, DateTime.UtcNow.AddHours(-12)));
        db.LeiturasSensor.Add(new LeituraSensor(talhaoNorte.Id, sensor.Id, 61m, 28m, DateTime.UtcNow.AddHours(-3)));
        db.VarredurasDrone.Add(new VarreduraDrone(talhaoSul.Id, drone.Id, "imagens/drone/talhao-sul-001.jpg", 8m, DateTime.UtcNow.AddHours(-5)));
        db.SaveChanges();
    }
}
