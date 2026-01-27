using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WrapperAPI.Interfaces.IRestaurantRepositories;
using WrapperAPI.Models.CampingModels;
using WrapperAPI.Models.GiteModels;
using WrapperAPI.Models.RestaurantModels;

namespace WrapperAPI.Repositories.RestaurantRepositories
{
    public class ReserveringRepository : IReserveringRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _campingUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        // Gebruik de TafelRepository voor verrijking binnen het restaurant domein
        private readonly ITafelRepository _tafelRepository;

        public ReserveringRepository(HttpClient client, IConfiguration config, ITafelRepository tafelRepository)
        {
            _httpClient = client;
            _tafelRepository = tafelRepository;

            _baseUrl = config["ExternalApi:BaseUrlRestaurant"]
                ?? "https://webapp-lgpteam-restaurant-marconnes.azurewebsites.net";

            _campingUrl = config["ExternalApi:BaseUrlCamping"]
                ?? "https://webapp-lgpteam-camping-marconnes.azurewebsites.net";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public IEnumerable<RecievingReservering> GetAllReserveringen()
        {
            var url = $"{_baseUrl}/api/Reserveringen";
            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                // Als dit gebeurt, klopt de URL of de verbinding met Azure niet
                var error = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Azure API Error {response.StatusCode}: {error}");
            }

            var jsonString = response.Content.ReadAsStringAsync().Result;

            // DEBUG: Dit is de belangrijkste stap. 
            // Kijk in je 'Output' venster in Visual Studio wat hier geprint wordt.
            System.Diagnostics.Debug.WriteLine($"RAW JSON VAN AZURE: {jsonString}");

            try
            {
                var result = JsonSerializer.Deserialize<List<RecievingReservering>>(jsonString, _jsonOptions);

                // Verrijking
                if (result != null)
                {
                    foreach (var res in result)
                    {
                        if (res.tafelID > 0)
                        {
                            res.tafel = _tafelRepository.GetTafelsById(res.tafelID);
                        }
                    }
                }
                return result ?? new List<RecievingReservering>();
            }
            catch (JsonException ex)
            {
                // Als dit afgaat, matcht je Model toch niet met de JSON
                throw new Exception($"Mapping Error: De JSON van Azure past niet in het Reservering model. Fout: {ex.Message}");
            }
        }

        public RecievingReservering GetReserveringenById(int id)
        {
            var url = $"{_baseUrl}/api/Reserveringen/{id}";
            var response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) return null;

            var jsonString = response.Content.ReadAsStringAsync().Result;
            RecievingReservering res = null;

            try
            {
                res = JsonSerializer.Deserialize<RecievingReservering>(jsonString, _jsonOptions);
            }
            catch (JsonException)
            {
                var lijst = JsonSerializer.Deserialize<List<RecievingReservering>>(jsonString, _jsonOptions);
                res = lijst?.FirstOrDefault();
            }

            // OPTIONEEL: Verrijk de reservering met volledige Tafel informatie
            if (res != null && res.tafelID > 0)
            {
                res.tafel = _tafelRepository.GetTafelsById(res.tafelID);
            }

            return res;
        }

        public RecievingReservering CreateReservering(SendingReservering reservering, int gebruikerID)
        {

            //Hier de boeking verwerking naar gebruiker toevoegen
            var url = $"{_campingUrl}/api/Boeking/0/{gebruikerID}/0?BetalingID=0&IncludeGebruiker=false&IncludeAccommodatie=false&IncludeBetalingen=false";
            var result = _httpClient.GetAsync(url).Result;
            result.EnsureSuccessStatusCode();
            var boekingen = JsonSerializer.Deserialize<List<Boeking>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            int gevondenBoekingID = 0;
            if (boekingen != null)
            {
                var passendeBoeking = boekingen.FirstOrDefault(result =>
                    reservering.datumTijd >= result.checkInDatum &&
                    reservering.datumTijd <= result.checkOutDatum);

                if (passendeBoeking != null)
                {
                    gevondenBoekingID = passendeBoeking.BoekingID;
                }
            }

            reservering.boekingID = gevondenBoekingID;


            // 1. Maak JSON van je 'Sending' model
            var jsonContent = JsonSerializer.Serialize(reservering);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync("api/Reserveringen", content).Result;

            if (!response.IsSuccessStatusCode)
            {
                var error = response.Content.ReadAsStringAsync().Result;
                throw new Exception($"Azure Error: {response.StatusCode} - {error}");
            }

            var jsonString = response.Content.ReadAsStringAsync().Result.Trim();

            // 2. Als Azure "true" teruggeeft
            if (jsonString.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return new RecievingReservering
                {
                    reserveringID = 0,
                    datumTijd = reservering.datumTijd,
                    aantalPersonen = reservering.aantalVolwassenen + reservering.aantalJongeKinderen + reservering.aantalOudereKinderen,
                    tafelID = reservering.tafelID
                };
            }

            // 3. FIX: Check op de specifieke foutmelding
            if (jsonString.Contains("Deze tafel is niet beschikbaar", StringComparison.OrdinalIgnoreCase))
            {
                return new RecievingReservering { reserveringID = -1 };
            }

            // 4. Als Azure wel JSON teruggeeft (begint met {), lees het dan uit
            if (jsonString.StartsWith("{"))
            {
                return JsonSerializer.Deserialize<RecievingReservering>(jsonString, _jsonOptions)
                       ?? new RecievingReservering();
            }

            // Fallback voor onbekende tekst responses
            throw new Exception($"Onverwacht antwoord van server: {jsonString}");
        }
        public void UpdateReservering(RecievingReservering reservering)
        {
            var jsonContent = JsonSerializer.Serialize(reservering, _jsonOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync($"{_baseUrl}/api/Reserveringen/{reservering.reserveringID}", content).Result;
            response.EnsureSuccessStatusCode();
        }

        public void DeleteReservering(int id)
        {
            var response = _httpClient.DeleteAsync($"{_baseUrl}/api/Reserveringen/{id}").Result;
            response.EnsureSuccessStatusCode();
        }

    }
}