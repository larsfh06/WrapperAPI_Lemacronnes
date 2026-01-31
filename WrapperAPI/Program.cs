using BookingOrchestrationApi.Interfaces.Repositories;
using BookingOrchestrationApi.Interfaces.Services;
using BookingOrchestrationApi.Repositories;
using BookingOrchestrationApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var campingBaseUrl = builder.Configuration["ExternalApis:Camping:BaseUrl"] 
    ?? throw new InvalidOperationException("Camping API base URL not configured");
var restaurantBaseUrl = builder.Configuration["ExternalApis:Restaurant:BaseUrl"] 
    ?? throw new InvalidOperationException("Restaurant API base URL not configured");
var hotelBaseUrl = builder.Configuration["ExternalApis:Hotel:BaseUrl"] 
    ?? throw new InvalidOperationException("Hotel API base URL not configured");
var giteBaseUrl = builder.Configuration["ExternalApis:Gite:BaseUrl"] 
    ?? throw new InvalidOperationException("Gite API base URL not configured");

builder.Services.AddHttpClient<ICampingRepository, CampingRepository>(client =>
{
    client.BaseAddress = new Uri(campingBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IRestaurantRepository, RestaurantRepository>(client =>
{
    client.BaseAddress = new Uri(restaurantBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IHotelRepository, HotelRepository>(client =>
{
    client.BaseAddress = new Uri(hotelBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IGiteRepository, GiteRepository>(client =>
{
    client.BaseAddress = new Uri(giteBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("CampingApi", client =>
{
    client.BaseAddress = new Uri(campingBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IBookingOrchestrationService, BookingOrchestrationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking Orchestration API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
