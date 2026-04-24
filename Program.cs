using ClinicalTrialsApi.Middleware;
using ClinicalTrialsApi.Services;
using ClinicalTrialsApi.Data;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

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

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorLoggingMiddleware>();

app.MapControllers();
app.MapGet("/", () => new { Message = "Hello world" });
app.Run();
