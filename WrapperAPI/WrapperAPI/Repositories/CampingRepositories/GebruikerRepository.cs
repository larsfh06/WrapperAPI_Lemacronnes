using System.Text.Json;
using WrapperAPI.Interfaces.ICampingRepositories;
using WrapperAPI.Models.CampingModels;

namespace WrapperAPI.Repositories.CampingRepositories
{
    public class GebruikerRepository : IGebruikerRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public GebruikerRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ExternalApi:BaseUrlCamping"]; // Zelfde basis URL
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public Gebruiker GetGebruikerById(int id)
        {
            var url = $"{_baseUrl}/api/Gebruiker/{id}/ALL/ALL?BoekingID=0&IncludeBoekingen=false";
            var response = _httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().Result;

            try
            {
                // Probeer eerst als enkel object
                return JsonSerializer.Deserialize<Gebruiker>(jsonString, _jsonOptions);
            }
            catch (JsonException)
            {
                // Als dat faalt, probeer als lijst en pak de eerste
                var lijst = JsonSerializer.Deserialize<List<Gebruiker>>(jsonString, _jsonOptions);
                return lijst?.FirstOrDefault();
            }
        }
    }
}
