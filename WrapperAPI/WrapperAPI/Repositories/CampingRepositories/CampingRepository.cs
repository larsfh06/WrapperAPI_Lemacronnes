using System.Text.Json;
using WrapperAPI.Interfaces.ICampingRepositories;
using WrapperAPI.Models.CampingModels;

namespace WrapperAPI.Repositories.CampingRepositories
{
    public class CampingRepository : ICampingRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public CampingRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ExternalApi:BaseUrlCamping"] ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public Camping GetCampingById(int id)
        {
            // Pas de URL aan naar de juiste route die je hebt gevonden
            var url = $"{_baseUrl}/api/Camping/{id}/0/false?AccommodatieID=0&IncludeAccommodatie=false";
            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var jsonString = response.Content.ReadAsStringAsync().Result;

            try
            {
                // 1. Probeer als enkel object
                return JsonSerializer.Deserialize<Camping>(jsonString, _jsonOptions);
            }
            catch (JsonException)
            {
                // 2. Probeer als lijst en pak de eerste (als de API met [ ] stuurt)
                var lijst = JsonSerializer.Deserialize<List<Camping>>(jsonString, _jsonOptions);
                return lijst?.FirstOrDefault();
            }
        }
    }
}
