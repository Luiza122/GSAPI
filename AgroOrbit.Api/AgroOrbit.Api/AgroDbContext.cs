using Microsoft.EntityFrameworkCore;

namespace AgroOrbit.Api;

public class AgroDbContext : DbContext
{
    public AgroDbContext(DbContextOptions<AgroDbContext> options) : base(options)
    {
    }

    public DbSet<Fazenda> Fazendas => Set<Fazenda>();
    public DbSet<Talhao> Talhoes => Set<Talhao>();
    public DbSet<EquipamentoMonitoramento> Equipamentos => Set<EquipamentoMonitoramento>();
    public DbSet<Satelite> Satelites => Set<Satelite>();
    public DbSet<Drone> Drones => Set<Drone>();
    public DbSet<SensorIot> SensoresIot => Set<SensorIot>();
    public DbSet<LeituraSatelite> LeiturasSatelite => Set<LeituraSatelite>();
    public DbSet<LeituraSensor> LeiturasSensor => Set<LeituraSensor>();
    public DbSet<VarreduraDrone> VarredurasDrone => Set<VarreduraDrone>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<RelatorioSemanal> RelatoriosSemanais => Set<RelatorioSemanal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipamentoMonitoramento>()
            .HasDiscriminator<TipoEquipamento>("TipoEquipamento")
            .HasValue<Satelite>(TipoEquipamento.Satelite)
            .HasValue<Drone>(TipoEquipamento.Drone)
            .HasValue<SensorIot>(TipoEquipamento.SensorIot);

        modelBuilder.Entity<Fazenda>().Property(f => f.AreaHectares).HasPrecision(12, 2);
        modelBuilder.Entity<Talhao>().Property(t => t.AreaHectares).HasPrecision(12, 2);
        modelBuilder.Entity<LeituraSatelite>().Property(l => l.IndiceSaude).HasPrecision(5, 2);
        modelBuilder.Entity<LeituraSatelite>().Property(l => l.UmidadeEstimada).HasPrecision(5, 2);
        modelBuilder.Entity<LeituraSensor>().Property(l => l.UmidadeSolo).HasPrecision(5, 2);
        modelBuilder.Entity<LeituraSensor>().Property(l => l.Temperatura).HasPrecision(5, 2);
        modelBuilder.Entity<VarreduraDrone>().Property(v => v.PercentualAnomalia).HasPrecision(5, 2);
        modelBuilder.Entity<RelatorioSemanal>().Property(r => r.MediaSaude).HasPrecision(5, 2);
    }
}
