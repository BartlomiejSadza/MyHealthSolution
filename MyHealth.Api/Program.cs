using MyHealth.Api.Clients;
using MyHealth.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration - obsługa lokalnego i Azure deploymentu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            // Lokalne URLs
            "http://localhost:3000", "https://localhost:3000", 
            "http://localhost:3001", "https://localhost:3001",
            // Azure Container Apps URLs - konkretny URL zamiast wildcard
            "https://myhealth-frontend.happysea-444138bb.eastus.azurecontainerapps.io"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// ZMIANA: klient HTTP do lokalnego modelu ML zamiast Databricks
// Obsługa różnych środowisk: lokalne Docker vs Azure Container Apps
var mlModelUrl = Environment.GetEnvironmentVariable("ML_MODEL_URL") 
    ?? "http://ml-model:5000"; // Fallback dla lokalnego Docker

builder.Services.AddHttpClient<IModelClient, LocalModelClient>(
    client =>
    {
        client.BaseAddress = new Uri(mlModelUrl);
        client.Timeout = TimeSpan.FromSeconds(30); // Timeout dla predykcji
    });

// serwisy zdrowotne
builder.Services.AddScoped<IHealthService, HealthService>();
builder.Services.AddScoped<AdvancedHealthAnalyzer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware
app.UseCors("AllowFrontend");

app.MapControllers();
app.Run();
