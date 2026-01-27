using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using WrapperAPI.Models.CampingModels;
using WrapperAPI.Interfaces.ICampingRepositories;
using WrapperAPI.Models.RestaurantModels;

namespace WrapperAPI.Repositories.CampingRepositories
{
    public class BoekingRepository : IBoekingRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public BoekingRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // We gebruiken hier de URL die je in de eerste repository liet zien
            _baseUrl = configuration["ExternalApi:BaseUrlCamping"]
                ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public IEnumerable<Boeking> GetAllBoekingen()
        {
            try
            {
                // Let op: Ik gebruik hier weer het format uit je eerste code snippet omdat die specifieker was
                var url = $"{_baseUrl}/api/Boeking/0/0/0?BetalingID=0&IncludeGebruiker=false&IncludeAccommodatie=false&IncludeBetalingen=false";
                var response = _httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<List<Boeking>>(jsonString, _jsonOptions) ?? new List<Boeking>();
                }

                return new List<Boeking>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Boeking GetBoekingById(int id)
        {
            var url = $"{_baseUrl}/api/Boeking/{id}/0/0?BetalingID=0&IncludeGebruiker=false&IncludeAccommodatie=false&IncludeBetalingen=false";
            var response = _httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<Boeking>(jsonString, _jsonOptions);
        }

        public Boeking CreateBoeking(Boeking boeking)
        {
            var jsonContent = JsonSerializer.Serialize(boeking, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync($"{_baseUrl}/api/Boeking", content).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result.Trim();

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = response.Content.ReadAsStringAsync().Result;
                var reason = response.ReasonPhrase; // Bijv. "Internal Server Error"

                // Soms zit de fout in de headers, maar laten we eerst dit proberen:
                throw new Exception($"Fout: {response.StatusCode} ({reason}) - Body: '{errorBody}'");
            }

            // De fix: Als de API letterlijk "true" teruggeeft, sturen we het 
            // ingestuurde boeking-object terug (eventueel aangevuld met ID)
            if (jsonString.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                var alleBoekingen = GetAllBoekingen();
                var gevondenBoeking = alleBoekingen
            .Where(b => b.GebruikerID == boeking.GebruikerID && b.AccommodatieID == boeking.AccommodatieID)
            .OrderByDescending(b => b.BoekingID)
            .FirstOrDefault();
                boeking.BoekingID = gevondenBoeking.BoekingID;
                return boeking;
            }
            if (jsonString.Equals("false"))
            {
                return new Boeking { BoekingID = -1 };
            }
            try
            {
                return JsonSerializer.Deserialize<Boeking>(jsonString, _jsonOptions);
            }
            catch
            {
                var lijst = JsonSerializer.Deserialize<List<Boeking>>(jsonString, _jsonOptions);
                return lijst?.FirstOrDefault() ?? boeking;
            }
        }
        public void UpdateBoeking(Boeking boeking)
        {
            var jsonContent = JsonSerializer.Serialize(boeking, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync($"{_baseUrl}/api/Boeking/{boeking.BoekingID}", content).Result;
            response.EnsureSuccessStatusCode();
        }

        public void DeleteBoeking(int id)
        {
            var response = _httpClient.DeleteAsync($"{_baseUrl}/api/Boeking/{id}").Result;
            response.EnsureSuccessStatusCode();
        }

        public IEnumerable<Boeking> ZoekBoekingen(DateTime? startDatum = null, DateTime? eindDatum = null, string? klantNaam = null)
        {
            var url = $"{_baseUrl}/api/Boeking/0/0/0?BetalingID=0&IncludeGebruiker=false&IncludeAccommodatie=false&IncludeBetalingen=false";
            var queryParams = new List<string>();

            if (startDatum.HasValue) queryParams.Add($"startDatum={startDatum.Value:yyyy-MM-dd}");
            if (eindDatum.HasValue) queryParams.Add($"eindDatum={eindDatum.Value:yyyy-MM-dd}");
            if (!string.IsNullOrEmpty(klantNaam)) queryParams.Add($"klantNaam={Uri.EscapeDataString(klantNaam)}");

            if (queryParams.Any()) url += "&" + string.Join("&", queryParams);

            var response = _httpClient.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var jsonString = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<List<Boeking>>(jsonString, _jsonOptions) ?? new List<Boeking>();
        }
    }

}