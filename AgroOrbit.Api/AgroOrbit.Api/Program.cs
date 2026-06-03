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
        await context.Response.WriteAsJsonAsync(new
        {
            status,
            mensagem = erro?.Message ?? "Erro interno",
            horarioUtc = DateTime.UtcNow
        });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

// CRUD - Fazendas
app.MapGet("/api/fazendas", async (AgroDbContext db) =>
    Results.Ok(await db.Fazendas.AsNoTracking().ToListAsync()));

app.MapGet("/api/fazendas/{id:int}", async (int id, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

    return fazenda is null
        ? Results.NotFound(new { mensagem = "Fazenda não encontrada." })
        : Results.Ok(fazenda);
})
.WithName("GetFazendaById")
.WithOpenApi()
.Produces<Fazenda>(200)
.Produces(404);

app.MapPost("/api/fazendas", async (CriarFazendaRequest r, AgroDbContext db) =>
{
    var fazenda = new Fazenda(r.Nome, r.Proprietario, r.Cidade, r.Estado, r.AreaHectares);
    db.Fazendas.Add(fazenda);
    await db.SaveChangesAsync();

    return Results.Created($"/api/fazendas/{fazenda.Id}", fazenda);
});

app.MapPut("/api/fazendas/{id:int}", async (int id, CriarFazendaRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(id);

    if (fazenda is null)
        return Results.NotFound(new { mensagem = "Fazenda não encontrada." });

    db.Entry(fazenda).Property(nameof(Fazenda.Nome)).CurrentValue = r.Nome.Trim();
    db.Entry(fazenda).Property(nameof(Fazenda.Proprietario)).CurrentValue = string.IsNullOrWhiteSpace(r.Proprietario) ? "Não informado" : r.Proprietario.Trim();
    db.Entry(fazenda).Property(nameof(Fazenda.Cidade)).CurrentValue = string.IsNullOrWhiteSpace(r.Cidade) ? "Não informada" : r.Cidade.Trim();
    db.Entry(fazenda).Property(nameof(Fazenda.Estado)).CurrentValue = string.IsNullOrWhiteSpace(r.Estado) ? "SP" : r.Estado.Trim().ToUpper();
    db.Entry(fazenda).Property(nameof(Fazenda.AreaHectares)).CurrentValue = r.AreaHectares;

    await db.SaveChangesAsync();

    return Results.Ok(fazenda);
})
.WithName("UpdateFazenda")
.WithOpenApi()
.Produces<Fazenda>(200)
.Produces(404);

app.MapDelete("/api/fazendas/{id:int}", async (int id, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(id);

    if (fazenda is null)
        return Results.NotFound(new { mensagem = "Fazenda não encontrada." });

    db.Fazendas.Remove(fazenda);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteFazenda")
.WithOpenApi()
.Produces(204)
.Produces(404);

// CRUD - Talhões
app.MapGet("/api/fazendas/{fazendaId:int}/talhoes", async (int fazendaId, AgroDbContext db) =>
{
    var talhoes = await db.Talhoes
        .AsNoTracking()
        .Where(t => t.FazendaId == fazendaId)
        .ToListAsync();

    return Results.Ok(talhoes);
});

app.MapGet("/api/talhoes/{id:int}", async (int id, AgroDbContext db) =>
{
    var talhao = await db.Talhoes.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

    return talhao is null
        ? Results.NotFound(new { mensagem = "Talhão não encontrado." })
        : Results.Ok(talhao);
})
.WithName("GetTalhaoById")
.WithOpenApi()
.Produces<Talhao>(200)
.Produces(404);

app.MapPost("/api/fazendas/{fazendaId:int}/talhoes", async (int fazendaId, CriarTalhaoRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(fazendaId);

    if (fazenda is null)
        throw new RecursoNaoEncontradoException($"Fazenda {fazendaId} não encontrada.");

    var talhao = new Talhao(r.Nome, r.Cultura, r.AreaHectares, r.Latitude, r.Longitude, fazendaId);
    db.Talhoes.Add(talhao);
    await db.SaveChangesAsync();

    return Results.Created($"/api/fazendas/{fazendaId}/talhoes/{talhao.Id}", talhao);
});

app.MapPut("/api/talhoes/{id:int}", async (int id, CriarTalhaoRequest r, AgroDbContext db) =>
{
    var talhao = await db.Talhoes.FindAsync(id);

    if (talhao is null)
        return Results.NotFound(new { mensagem = "Talhão não encontrado." });

    db.Entry(talhao).Property(nameof(Talhao.Nome)).CurrentValue = r.Nome.Trim();
    db.Entry(talhao).Property(nameof(Talhao.Cultura)).CurrentValue = string.IsNullOrWhiteSpace(r.Cultura) ? "Não informada" : r.Cultura.Trim();
    db.Entry(talhao).Property(nameof(Talhao.AreaHectares)).CurrentValue = r.AreaHectares;
    db.Entry(talhao).Property(nameof(Talhao.Latitude)).CurrentValue = r.Latitude;
    db.Entry(talhao).Property(nameof(Talhao.Longitude)).CurrentValue = r.Longitude;
    db.Entry(talhao).Property(nameof(Talhao.AtualizadoEm)).CurrentValue = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(talhao);
})
.WithName("UpdateTalhao")
.WithOpenApi()
.Produces<Talhao>(200)
.Produces(404);

app.MapDelete("/api/talhoes/{id:int}", async (int id, AgroDbContext db) =>
{
    var talhao = await db.Talhoes.FindAsync(id);

    if (talhao is null)
        return Results.NotFound(new { mensagem = "Talhão não encontrado." });

    db.Talhoes.Remove(talhao);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteTalhao")
.WithOpenApi()
.Produces(204)
.Produces(404);

// CRUD - Equipamentos
app.MapGet("/api/equipamentos", async (AgroDbContext db) =>
{
    var lista = await db.Equipamentos.AsNoTracking().ToListAsync();

    return Results.Ok(lista.Select(e => new EquipamentoResponse(
        e.Id,
        e.Nome,
        e.Codigo,
        e.Tipo,
        e.Status,
        e.DescreverOperacao()
    )));
});

app.MapGet("/api/equipamentos/{id:int}", async (int id, AgroDbContext db) =>
{
    var equipamento = await db.Equipamentos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

    return equipamento is null
        ? Results.NotFound(new { mensagem = "Equipamento não encontrado." })
        : Results.Ok(new EquipamentoResponse(
            equipamento.Id,
            equipamento.Nome,
            equipamento.Codigo,
            equipamento.Tipo,
            equipamento.Status,
            equipamento.DescreverOperacao()
        ));
})
.WithName("GetEquipamentoById")
.WithOpenApi()
.Produces<EquipamentoResponse>(200)
.Produces(404);

app.MapPost("/api/equipamentos/satelites", async (CriarSateliteRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(r.FazendaId);

    if (fazenda is null)
        throw new RecursoNaoEncontradoException($"Fazenda {r.FazendaId} não encontrada.");

    var codigoUnico = await CodigoHelper.GerarCodigoUnicoAsync(db, r.Codigo);
    var satelite = new Satelite(r.Nome, codigoUnico, r.FazendaId, r.ProvedorImagem, r.RevisitaHoras);

    db.Equipamentos.Add(satelite);
    await db.SaveChangesAsync();

    return Results.Created($"/api/equipamentos/satelites/{satelite.Id}", new EquipamentoResponse(
        satelite.Id,
        satelite.Nome,
        satelite.Codigo,
        satelite.Tipo,
        satelite.Status,
        satelite.DescreverOperacao()
    ));
});

app.MapPost("/api/equipamentos/drones", async (CriarDroneRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(r.FazendaId);

    if (fazenda is null)
        throw new RecursoNaoEncontradoException($"Fazenda {r.FazendaId} não encontrada.");

    var codigoUnico = await CodigoHelper.GerarCodigoUnicoAsync(db, r.Codigo);
    var drone = new Drone(r.Nome, codigoUnico, r.FazendaId, r.AutonomiaMinutos, r.RotaPadrao);

    db.Equipamentos.Add(drone);
    await db.SaveChangesAsync();

    return Results.Created($"/api/equipamentos/drones/{drone.Id}", new EquipamentoResponse(
        drone.Id,
        drone.Nome,
        drone.Codigo,
        drone.Tipo,
        drone.Status,
        drone.DescreverOperacao()
    ));
});

app.MapPost("/api/equipamentos/sensores-iot", async (CriarSensorIotRequest r, AgroDbContext db) =>
{
    var fazenda = await db.Fazendas.FindAsync(r.FazendaId);

    if (fazenda is null)
        throw new RecursoNaoEncontradoException($"Fazenda {r.FazendaId} não encontrada.");

    var codigoUnico = await CodigoHelper.GerarCodigoUnicoAsync(db, r.Codigo);
    var sensor = new SensorIot(r.Nome, codigoUnico, r.FazendaId, r.GrandezaMonitorada);

    db.Equipamentos.Add(sensor);
    await db.SaveChangesAsync();

    return Results.Created($"/api/equipamentos/sensores-iot/{sensor.Id}", new EquipamentoResponse(
        sensor.Id,
        sensor.Nome,
        sensor.Codigo,
        sensor.Tipo,
        sensor.Status,
        sensor.DescreverOperacao()
    ));
});

app.MapPut("/api/equipamentos/{id:int}/status/{status}", async (int id, StatusEquipamento status, AgroDbContext db) =>
{
    var equipamento = await db.Equipamentos.FindAsync(id);

    if (equipamento is null)
        return Results.NotFound(new { mensagem = "Equipamento não encontrado." });

    db.Entry(equipamento)
        .Property(nameof(EquipamentoMonitoramento.Status))
        .CurrentValue = status;

    await db.SaveChangesAsync();

    return Results.Ok(new EquipamentoResponse(
        equipamento.Id,
        equipamento.Nome,
        equipamento.Codigo,
        equipamento.Tipo,
        equipamento.Status,
        equipamento.DescreverOperacao()
    ));
});

app.MapDelete("/api/equipamentos/{id:int}", async (int id, AgroDbContext db) =>
{
    var equipamento = await db.Equipamentos.FindAsync(id);

    if (equipamento is null)
        return Results.NotFound(new { mensagem = "Equipamento não encontrado." });

    db.Equipamentos.Remove(equipamento);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteEquipamento")
.WithOpenApi()
.Produces(204)
.Produces(404);

// Monitoramento
app.MapPost("/api/monitoramento/leituras-satelite", async (CriarLeituraSateliteRequest r, AgroDbContext db, IAlertaService alertas) =>
{
    var leitura = new LeituraSatelite(r.TalhaoId, r.SateliteId, r.IndiceSaude, r.UmidadeEstimada, r.CapturadoEmUtc ?? DateTime.UtcNow);
    db.LeiturasSatelite.Add(leitura);

    var gerados = await alertas.AvaliarLeituraSateliteAsync(leitura);

    return Results.Created($"/api/monitoramento/leituras-satelite/{leitura.Id}", new
    {
        leitura,
        alertasGerados = gerados.Count,
        alertas = gerados
    });
});

app.MapPost("/api/monitoramento/leituras-sensor", async (CriarLeituraSensorRequest r, AgroDbContext db, IAlertaService alertas) =>
{
    var leitura = new LeituraSensor(r.TalhaoId, r.SensorIotId, r.UmidadeSolo, r.Temperatura, r.CapturadoEmUtc ?? DateTime.UtcNow);
    db.LeiturasSensor.Add(leitura);

    var gerados = await alertas.AvaliarLeituraSensorAsync(leitura);

    return Results.Created($"/api/monitoramento/leituras-sensor/{leitura.Id}", new
    {
        leitura,
        alertasGerados = gerados.Count,
        alertas = gerados
    });
});

app.MapPost("/api/monitoramento/varreduras-drone", async (CriarVarreduraDroneRequest r, AgroDbContext db, IAlertaService alertas, IAnaliseImagemService analise) =>
{
    var percentual = r.PercentualAnomalia ?? analise.CalcularPercentualAnomalia(r.UrlImagem, r.TalhaoId);
    var varredura = new VarreduraDrone(r.TalhaoId, r.DroneId, r.UrlImagem, percentual, r.CapturadoEmUtc ?? DateTime.UtcNow);
    db.VarredurasDrone.Add(varredura);

    var gerados = await alertas.AvaliarVarreduraDroneAsync(varredura);

    return Results.Created($"/api/monitoramento/varreduras-drone/{varredura.Id}", new
    {
        varredura,
        alertasGerados = gerados.Count,
        alertas = gerados
    });
});

// Consulta e relatórios
app.MapGet("/api/alertas", async (AgroDbContext db) =>
    Results.Ok(await db.Alertas.AsNoTracking().OrderByDescending(a => a.GeradoEmUtc).ToListAsync()));

app.MapGet("/api/dashboard/{fazendaId:int}", async (int fazendaId, IDashboardService dashboard) =>
    Results.Ok(await dashboard.ObterDashboardAsync(fazendaId)));

app.MapPost("/api/relatorios/semanal/{fazendaId:int}", async (int fazendaId, IRelatorioService relatorio) =>
{
    var fim = DateTime.UtcNow;
    var inicio = fim.AddDays(-7);
    var resultado = await relatorio.GerarRelatorioSemanalAsync(fazendaId, inicio, fim);

    return Results.Ok(resultado);
});

app.Run();
