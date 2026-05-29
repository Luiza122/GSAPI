using System.Text.Json.Serialization;
using AgroOrbit.Api;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AgroDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<IAnaliseImagemService, AnaliseImagemService>();
builder.Services.AddSingleton<ITimeZoneService, TimeZoneService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgroDbContext>();
    DbSeeder.Seed(db);
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var erro = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var status = erro switch
        {
            RecursoNaoEncontradoException => 404,
            RegraNegocioException => 400,
            FormatException => 400,
            DbUpdateException => 409,
            _ => 500
        };
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new { status, mensagem = erro?.Message ?? "Erro interno", horarioUtc = DateTime.UtcNow });
    });
});

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/api/fazendas", async (AgroDbContext db) =>
    Results.Ok(await db.Fazendas.AsNoTracking().ToListAsync()));

app.MapPost("/api/fazendas", async (CriarFazendaRequest r, AgroDbContext db) =>
{
    var fazenda = new Fazenda(r.Nome, r.Proprietario, r.Cidade, r.Estado, r.AreaHectares);
    db.Fazendas.Add(fazenda);
    await db.SaveChangesAsync();
    return Results.Created($"/api/fazendas/{fazenda.Id}", fazenda);
});

app.MapPost("/api/fazendas/{fazendaId:int}/talhoes", async (int fazendaId, CriarTalhaoRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(fazendaId);
    if (fazenda == null)
        throw new RecursoNaoEncontradoException($"Fazenda {fazendaId} não encontrada.");
    
    var talhao = new Talhao(r.Nome, r.Cultura, r.AreaHectares, r.Latitude, r.Longitude, fazendaId);
    db.Talhoes.Add(talhao);
    await db.SaveChangesAsync();
    return Results.Created($"/api/fazendas/{fazendaId}/talhoes/{talhao.Id}", talhao);
});

app.MapGet("/api/equipamentos", async (AgroDbContext db) =>
{
    var lista = await db.Equipamentos.AsNoTracking().ToListAsync();
    return Results.Ok(lista.Select(e => new EquipamentoResponse(e.Id, e.Nome, e.Codigo, e.Tipo, e.Status, e.DescreverOperacao())));
});

app.MapPost("/api/equipamentos/satelites", async (CriarSateliteRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(r.FazendaId);
    if (fazenda == null)
        throw new RecursoNaoEncontradoException($"Fazenda {r.FazendaId} não encontrada.");
    
    var satelite = new Satelite(r.Nome, r.Codigo, r.FazendaId, r.ProvedorImagem, r.RevisitaHoras);
    db.Equipamentos.Add(satelite);
    await db.SaveChangesAsync();
    return Results.Created($"/api/equipamentos/satelites/{satelite.Id}", 
        new EquipamentoResponse(satelite.Id, satelite.Nome, satelite.Codigo, satelite.Tipo, satelite.Status, satelite.DescreverOperacao()));
});

app.MapPost("/api/equipamentos/drones", async (CriarDroneRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(r.FazendaId);
    if (fazenda == null)
        throw new RecursoNaoEncontradoException($"Fazenda {r.FazendaId} não encontrada.");
    
    var drone = new Drone(r.Nome, r.Codigo, r.FazendaId, r.AutonomiaMinutos, r.RotaPadrao);
    db.Equipamentos.Add(drone);
    await db.SaveChangesAsync();
    return Results.Created($"/api/equipamentos/drones/{drone.Id}", 
        new EquipamentoResponse(drone.Id, drone.Nome, drone.Codigo, drone.Tipo, drone.Status, drone.DescreverOperacao()));
});

app.MapPost("/api/equipamentos/sensores-iot", async (CriarSensorIotRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(r.FazendaId);
    if (fazenda == null)
        throw new RecursoNaoEncontradoException($"Fazenda {r.FazendaId} não encontrada.");
    
    var sensor = new SensorIot(r.Nome, r.Codigo, r.FazendaId, r.GrandezaMonitorada);
    db.Equipamentos.Add(sensor);
    await db.SaveChangesAsync();
    return Results.Created($"/api/equipamentos/sensores-iot/{sensor.Id}", 
        new EquipamentoResponse(sensor.Id, sensor.Nome, sensor.Codigo, sensor.Tipo, sensor.Status, sensor.DescreverOperacao()));
});

app.MapPost("/api/monitoramento/leituras-satelite", async (CriarLeituraSateliteRequest r, AgroDbContext db, IAlertaService alertas) =>
{
    var leitura = new LeituraSatelite(r.TalhaoId, r.SateliteId, r.IndiceSaude, r.UmidadeEstimada, r.CapturadoEmUtc ?? DateTime.UtcNow);
    db.LeiturasSatelite.Add(leitura);
    var gerados = await alertas.AvaliarLeituraSateliteAsync(leitura);
    return Results.Created($"/api/monitoramento/leituras-satelite/{leitura.Id}", new { leitura, alertasGerados = gerados.Count, alertas = gerados });
});

app.MapPost("/api/monitoramento/leituras-sensor", async (CriarLeituraSensorRequest r, AgroDbContext db, IAlertaService alertas) =>
{
    var leitura = new LeituraSensor(r.TalhaoId, r.SensorIotId, r.UmidadeSolo, r.Temperatura, r.CapturadoEmUtc ?? DateTime.UtcNow);
    db.LeiturasSensor.Add(leitura);
    var gerados = await alertas.AvaliarLeituraSensorAsync(leitura);
    return Results.Created($"/api/monitoramento/leituras-sensor/{leitura.Id}", new { leitura, alertasGerados = gerados.Count, alertas = gerados });
});

app.MapPost("/api/monitoramento/varreduras-drone", async (CriarVarreduraDroneRequest r, AgroDbContext db, IAlertaService alertas, IAnaliseImagemService analise) =>
{
    var percentual = r.PercentualAnomalia ?? analise.CalcularPercentualAnomalia(r.UrlImagem, r.TalhaoId);
    var varredura = new VarreduraDrone(r.TalhaoId, r.DroneId, r.UrlImagem, percentual, r.CapturadoEmUtc ?? DateTime.UtcNow);
    db.VarredurasDrone.Add(varredura);
    var gerados = await alertas.AvaliarVarreduraDroneAsync(varredura);
    return Results.Created($"/api/monitoramento/varreduras-drone/{varredura.Id}", new { varredura, alertasGerados = gerados.Count, alertas = gerados });
});

app.MapGet("/api/alertas", async (AgroDbContext db) =>
    Results.Ok(await db.Alertas.AsNoTracking().OrderByDescending(a => a.GeradoEmUtc).ToListAsync()));

app.MapGet("/api/dashboard/{fazendaId:int}", async (int fazendaId, IDashboardService dashboard) =>
    Results.Ok(await dashboard.ObterDashboardAsync(fazendaId)));

app.MapPost("/api/relatorios/semanal/{fazendaId:int}", async (int fazendaId, IRelatorioService relatorio) =>
{
    var fim = DateTime.UtcNow;
    return Results.Ok(await relatorio.GerarRelatorioSemanalAsync(fazendaId, fim.AddDays(-7), fim));
});

app.Run();
