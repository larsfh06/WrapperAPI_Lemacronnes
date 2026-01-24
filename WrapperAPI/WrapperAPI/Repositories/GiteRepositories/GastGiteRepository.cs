using System.Text.Json;
using WrapperAPI.Interfaces.IGiteRepositories;
using WrapperAPI.Models.GiteModels;

namespace WrapperAPI.Repositories.HotelRepositories
{
    public class GastGiteRepository : IGiteGastRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public GastGiteRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ExternalApi:BaseUrlGite"];
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // We gebruiken de ADMIN KEY om de volledige lijst te mogen opvragen
            var adminKey = configuration["ExternalApi:HotelAdminAPIKey"];

            if (!string.IsNullOrEmpty(adminKey))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-Api-Key");
                _httpClient.DefaultRequestHeaders.Add("X-Api-Key", adminKey);
            }
        }

        public GastDTO GetGiteGastById(int id)
        {
            // We roepen het verzamel-endpoint aan waar de Admin wél rechten op heeft
            var url = $"{_baseUrl.TrimEnd('/')}/api/Gasten";

            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gite API Gastenlijst Error: {response.StatusCode} op URL: {url}");
            }

            var jsonString = response.Content.ReadAsStringAsync().Result;

            try
            {
                // De API geeft een lijst terug (Array [])
                var lijst = JsonSerializer.Deserialize<List<GastDTO>>(jsonString, _jsonOptions);

                // We zoeken de specifieke gast met LINQ
                var gast = lijst?.FirstOrDefault(g => g.gastID == id);

                if (gast == null)
                {
                    throw new Exception($"Gast met ID {id} niet gevonden in de gite database.");
                }

                return gast;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Fout bij het verwerken van de gastenlijst: {ex.Message}");
            }
        }
    }
}