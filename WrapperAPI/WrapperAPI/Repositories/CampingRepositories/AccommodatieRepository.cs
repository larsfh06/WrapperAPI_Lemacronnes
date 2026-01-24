using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using WrapperAPI.Models.CampingModels;
using WrapperAPI.Interfaces.ICampingRepositories;

namespace WrapperAPI.Repositories.CampingRepositories
{
    public class AccommodatieRepository : IAccommodatieRepository
    {
        private readonly ICampingRepository _campingRepository;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public AccommodatieRepository(HttpClient client, IConfiguration config, ICampingRepository campingRepository)
        {
            _httpClient = client;
            _campingRepository = campingRepository;
            _baseUrl = config["ExternalApi:BaseUrlCamping"]
                ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public IEnumerable<Accommodatie> GetAllAccommodaties()
        {
            try
            {
                // Pas de route aan naar de werkelijke route van de externe API
                var response = _httpClient.GetAsync($"{_baseUrl}/api/Accommodatie").Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<List<Accommodatie>>(jsonString, _jsonOptions) ?? new List<Accommodatie>();
                }
                return new List<Accommodatie>();
            }
            catch
            {
                return new List<Accommodatie>();
            }
        }

        public Accommodatie GetAccommodatieById(int id)
        {
            var url = $"{_baseUrl}/api/Accommodatie/{id}?CampingID=0&IncludeCamping=false&BoekingID=0&IncludeBoeking=false";
            var response = _httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().Result;
            Accommodatie acc = null;

            try
            {
                acc = JsonSerializer.Deserialize<Accommodatie>(jsonString, _jsonOptions);
            }
            catch (JsonException)
            {
                // Als het een lijst is, pak de eerste
                var lijst = JsonSerializer.Deserialize<List<Accommodatie>>(jsonString, _jsonOptions);
                acc = lijst?.FirstOrDefault();
            }

            // VERRIJKING: Haal de camping op als die nog null is
            if (acc != null && acc.CampingID > 0)
            {
                acc.Camping = _campingRepository.GetCampingById(acc.CampingID);
            }

            return acc;
        }
    }
}