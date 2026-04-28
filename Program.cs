using ClinicalTrialsApi.Services;
using ClinicalTrialsApi.Data;
using Microsoft.EntityFrameworkCore;
using ClinicalTrialsApi.Models;
using FluentValidation;
using ClinicalTrialsApi.ExceptionHandling;

// 1. Crea el "builder" obejto que sirve para configurar la app antes de construirla.
var builder = WebApplication.CreateBuilder(args);

// 2. Registra servicio en el contenedor de inyección de dependencias (DI). 
//Esto permite que los controladores y otros componentes puedan solicitar estas dependencias a través de sus constructores.
builder.Services.AddControllers();
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

// Configuracion de CORS para permitir solicitudes desde el frontend (Angular).
const string AngularCorsPolicy = "AngularCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Origen de angular dev server
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 3. Construye la aplicación a partir del builder configurado. Esto crea una instancia de WebApplication que se puede configurar y ejecutar.
var app = builder.Build();

app.UseExceptionHandler(); //activa el middleware de manejo global de excepciones, que captura cualquier excepción no manejada y devuelve una respuesta de error adecuada al cliente.
app.UseStatusCodePages(); //responde con ProblemDetails para códigos de estado HTTP comunes como 400, 404, etc., lo que mejora la experiencia del cliente al recibir errores HTTP.

// 4. Configura el pipeline de middleware de la aplicación. El orden en que se agregan los middleware es importante, ya que determina cómo se procesan las solicitudes y respuestas.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Expone el JSON de Swagger
    app.UseSwaggerUI(); // Expone la interfaz de usuario de Swagger
}

app.UseCors(AngularCorsPolicy); // Aplica la política de CORS definida anteriormente, lo que permite que el frontend (Angular) realice solicitudes a esta API sin problemas de CORS.

// Middleware de seguridad y autorización (comentado para desarrollo)
// app.UseHttpsRedirection(); // Redirige automáticamente las solicitudes HTTP a HTTPS, lo que mejora la seguridad de la aplicación.
app.UseAuthorization(); // Expone la UI de Swagger solo en desarrollo.

// Middleware simple que agrega un header custom a todas las responses
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Powered-By"] = "ClinicalTrialsApi-v1";
    await next();
});

// app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseMiddleware<ErrorLoggingMiddleware>();

app.MapControllers(); // Mapea las rutas a los controladores, lo que permite que las solicitudes HTTP se dirijan a los métodos de acción correspondientes en los controladores.
app.MapGet("/", () => new { Message = "Hello world" });

// =====================================================
// Ejemplo Minimal API: endpoints paralelos en /minimal
// =====================================================
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

// 5. Ejecuta la aplicación, lo que inicia el servidor web y comienza a escuchar las solicitudes entrantes.
app.Run();
