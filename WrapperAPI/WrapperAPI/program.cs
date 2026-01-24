using Microsoft.Extensions.DependencyInjection;
using System;
using WrapperAPI.Interfaces.ICampingRepositories;
using WrapperAPI.Interfaces.IGiteRepositories;
using WrapperAPI.Interfaces.IGiteRepository;
using WrapperAPI.Interfaces.IHotelRepositories;
using WrapperAPI.Interfaces.IRestaurantRepositories;
using WrapperAPI.Repositories.CampingRepositories;
using WrapperAPI.Repositories.GiteRepositories;
using WrapperAPI.Repositories.HotelRepositories;
using WrapperAPI.Repositories.RestaurantRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Houdt de namen zoals ze zijn
    });
builder.Services.AddScoped<ITafelRepository, TafelRepository>();
builder.Services.AddScoped<IGiteRepository, GiteRepository>();
builder.Services.AddScoped<IGiteGastRepository, GastGiteRepository>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

// --- HTTP CLIENT REGISTRATIES ---

// 1. Boeking Repository
builder.Services.AddHttpClient<IBoekingRepository, BoekingRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlCamping"]
        ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
});

// 2. Gebruiker Repository (TOEGEVOEGD)
builder.Services.AddHttpClient<IGebruikerRepository, GebruikerRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlCamping"]
        ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
});

// 3. Accommodatie Repository (TOEGEVOEGD)
builder.Services.AddHttpClient<IAccommodatieRepository, AccommodatieRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlCamping"]
        ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
});
// 4. Camping Repository
builder.Services.AddHttpClient<ICampingRepository, CampingRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlCamping"]
        ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
// 5. Restaurant Repository
builder.Services.AddHttpClient<IReserveringRepository, ReserveringRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    // Zorg dat deze key exact zo in je appsettings.json staat
    var baseUrl = configuration["ExternalApi:BaseUrlRestaurant"]
        ?? "https://webapp-lgpteam-restaurant-marconnes.azurewebsites.net";
    if (string.IsNullOrEmpty(baseUrl))
    {
        throw new Exception("BaseUrlRestaurant niet gevonden in appsettings.json!");
    }
    client.BaseAddress = new Uri(baseUrl);
});

// Vergeet niet de TafelRepository ook te registreren, want ReserveringRepository heeft deze nodig!
builder.Services.AddHttpClient<ITafelRepository, TafelRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlRestaurant"]
        ?? "https://webapp-lgpteam-restaurant-marconnes.azurewebsites.net";

    client.BaseAddress = new Uri(baseUrl);
});

// 6. Hotel Repository
builder.Services.AddHttpClient<IHotelRepository, HotelRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlHotel"]
        ?? "https://app-lemarconnes-api-dev-z4b7skvxakgla.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Vergeet niet de GastRepository toe te voegen
builder.Services.AddHttpClient<IHotelGastRepository, GastHotelRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlHotel"]
        ?? "https://app-lemarconnes-api-dev-z4b7skvxakgla.azurewebsites.net"; 
    client.BaseAddress = new Uri(baseUrl);
});

// 7. Gite Repository
builder.Services.AddHttpClient<IGiteRepository, GiteRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlGite"]
        ?? "https://app-lemarconnes-gite-dev-z4b7skvxakgla.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Vergeet niet de GastRepository toe te voegen
builder.Services.AddHttpClient<IHotelGastRepository, GastHotelRepository>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ExternalApi:BaseUrlGite"]
        ?? "https://app-lemarconnes-gite-dev-z4b7skvxakgla.azurewebsites.net";
    client.BaseAddress = new Uri(baseUrl);
});


// Swagger configuratie
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();