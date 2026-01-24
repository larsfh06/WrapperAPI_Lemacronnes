using System.Text.Json;
using WrapperAPI.Interfaces.IRestaurantRepositories;
using WrapperAPI.Models.RestaurantModels;

namespace WrapperAPI.Repositories.RestaurantRepositories
{
    public class TafelRepository : ITafelRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public TafelRepository(HttpClient client, IConfiguration config)
        {
            _httpClient = client;

            _baseUrl = config["ExternalApi:BaseUrlRestaurant"]
                ?? "https://webapp-lgpteam-restaurant-marconnes.azurewebsites.net";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public IEnumerable<Tafel> GetAllTafels()
        {
            try
            {
                var response = _httpClient.GetAsync($"{_baseUrl}/api/Tafels").Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<List<Tafel>>(jsonString, _jsonOptions) ?? new List<Tafel>();
                }
                return new List<Tafel>();
            }
            catch
            {
                return new List<Tafel>();
            }
        }

        public Tafel GetTafelsById(int id)
        {
            var url = $"{_baseUrl}/api/Tafels?id={id}";
            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var jsonString = response.Content.ReadAsStringAsync().Result;
            Tafel res = null;

            try
            {
                res = JsonSerializer.Deserialize<Tafel>(jsonString, _jsonOptions);
            }
            catch (JsonException)
            {
                var lijst = JsonSerializer.Deserialize<List<Tafel>>(jsonString, _jsonOptions);
                res = lijst?.FirstOrDefault();
            }

            return res;
        }
    }
}
