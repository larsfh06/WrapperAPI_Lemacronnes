using System.Text;
using System.Text.Json;
using WrapperAPI.Interfaces.IGiteRepository;
using WrapperAPI.Models.GiteModels;

namespace WrapperAPI.Repositories.GiteRepositories
{
    public class GiteRepository : IGiteRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _adminKey;
        private readonly string _userKey;
        private readonly string _authHeader; // De variabele header-naam

        public GiteRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://app-lemarconnes-gite-dev-z4b7skvxakgla.azurewebsites.net");

            // 1. Haal de keys op
            _adminKey = configuration["ExternalApi:HotelAdminAPIKey"];
            _userKey = configuration["ExternalApi:HotelUserAPIKey"];

            // 2. Stel hier de juiste header-naam in die de API verwacht
            // Als de API 'ApiKey' verwacht, verander je dit hieronder:
            _authHeader = "X-Api-Key";
        }

        // Helper maakt nu gebruik van de dynamische _authHeader
        private void SwitchKey(string key)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(_authHeader))
            {
                _httpClient.DefaultRequestHeaders.Remove(_authHeader);
            }
            _httpClient.DefaultRequestHeaders.Add(_authHeader, key);
        }

        public IEnumerable<BoekingResponseDTO> GetAllReserveringen()
        {
            SwitchKey(_adminKey);
            var response = _httpClient.GetAsync("api/Reserveringen").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return JsonSerializer.Deserialize<IEnumerable<BoekingResponseDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BoekingResponseDTO>();
            }
            return new List<BoekingResponseDTO>();
        }

        public BoekingResponseDTO GetReserveringenById(int id)
        {
            SwitchKey(_adminKey); // Veranderd naar adminKey, want inzien is meestal admin
            var response = _httpClient.GetAsync($"api/Reserveringen/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return JsonSerializer.Deserialize<BoekingResponseDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public BoekingResponseDTO CreateReservering(BoekingRequestDTO reservering)
        {
            SwitchKey(_userKey);

            var json = JsonSerializer.Serialize(reservering);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            // VERBETERING: Haal de schuine streep aan het begin weg om // te voorkomen
            var response = _httpClient.PostAsync("api/Reserveringen/boeken", data).Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return JsonSerializer.Deserialize<BoekingResponseDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // VERBETERING: Lees de ERROR BODY uit. Dit vertelt je EXACT welk veld fout is.
            var errorBody = response.Content.ReadAsStringAsync().Result;
            throw new Exception($"Gite API Error: {response.StatusCode} - Details: {errorBody}");
        }

        public void UpdateReservering(BoekingResponseDTO reservering)
        {
            SwitchKey(_adminKey);
            var json = JsonSerializer.Serialize(reservering);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync($"api/Reserveringen/{reservering.reserveringID}", data).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Update mislukt bij Gite API");
            }
        }

        public void DeleteReservering(int id)
        {
            SwitchKey(_adminKey);
            var response = _httpClient.DeleteAsync($"api/Reserveringen/{id}").Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Verwijderen mislukt bij Gite API");
            }
        }
    }
}