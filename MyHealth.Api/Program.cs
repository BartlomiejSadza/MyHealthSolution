using MyHealth.Api.Clients;
using MyHealth.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost:3001", "https://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// klient HTTP do Databricks
builder.Services.AddHttpClient<IModelClient, DatabricksModelClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["Databricks:Endpoint"]);
    });

// serwis zdrowotny
builder.Services.AddScoped<IHealthService, HealthService>();

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
