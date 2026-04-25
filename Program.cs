using ClinicalTrialsApi.Middleware;
using ClinicalTrialsApi.Services;
using ClinicalTrialsApi.Data;
using Microsoft.EntityFrameworkCore;
using ClinicalTrialsApi.Models;
using FluentValidation;
using ClinicalTrialsApi.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ITransientOperation, Operation>();
builder.Services.AddScoped<IScopedOperation, Operation>();
builder.Services.AddSingleton<ISingletonOperation, Operation>();

builder.Services.AddDbContext<ClinicalTrialsDbContext>(options =>
    options.UseSqlite("Data Source=clinicaltrials.db"));

builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // Busca todos los validadores en tu assembly y los registra automáticamente.

builder.Services.AddProblemDetails(); // Habilita el formato RFC 7807 para errores HTTP, lo que facilita la gestión de errores en el cliente.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>(); // Registra el manejador global de excepciones, que se encargará de capturar y manejar cualquier excepción no controlada en la aplicación.

var app = builder.Build();

app.UseExceptionHandler(); //activa el middleware de manejo global de excepciones, que captura cualquier excepción no manejada y devuelve una respuesta de error adecuada al cliente.
app.UseStatusCodePages(); //responde con ProblemDetails para códigos de estado HTTP comunes como 400, 404, etc., lo que mejora la experiencia del cliente al recibir errores HTTP.

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Middleware simple que agrega un header custom a todas las responses
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Powered-By"] = "ClinicalTrialsApi-v1";
    await next();
});

// app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseMiddleware<ErrorLoggingMiddleware>();

app.MapControllers();
app.MapGet("/", () => new { Message = "Hello world" });

var minimalGroup = app.MapGroup("/minimal/clinicaltrials")
    .WithTags("Minimal API - Clinical Trials");

minimalGroup.MapGet("/", async (ClinicalTrialsDbContext db) =>
{
    var trials = await db.ClinicalTrials.AsNoTracking().ToListAsync();
    return Results.Ok(trials);
});

minimalGroup.MapGet("/{id:int}", async (int id, ClinicalTrialsDbContext db) =>
{
    var trial = await db.ClinicalTrials.AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == id);

    return trial is not null 
        ? Results.Ok(trial) 
        : Results.NotFound();
});

minimalGroup.MapPost("/", async (ClinicalTrial trial, ClinicalTrialsDbContext db) =>
{
    db.ClinicalTrials.Add(trial);
    await db.SaveChangesAsync();
    return Results.Created($"/minimal/clinicaltrials/{trial.Id}", trial);
});

app.Run();
