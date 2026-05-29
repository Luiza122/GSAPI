using System.Text.Json.Serialization;
using AgroOrbit.Api.Data;
using AgroOrbit.Api.Middleware;
using AgroOrbit.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AgroDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IAnaliseImagemService, AnaliseImagemService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddSingleton<ITimeZoneService, TimeZoneService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AgroDbContext>();
    DbSeeder.Seed(context);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
