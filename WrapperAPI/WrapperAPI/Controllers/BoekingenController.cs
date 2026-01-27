using Microsoft.AspNetCore.Mvc;
using WrapperAPI.Interfaces.ICampingRepositories;
using WrapperAPI.Interfaces.IGiteRepositories;
using WrapperAPI.Interfaces.IGiteRepository;
using WrapperAPI.Interfaces.IHotelRepositories; // Nieuw
using WrapperAPI.Interfaces.IRestaurantRepositories;
using WrapperAPI.Models;
using WrapperAPI.Models.CampingModels;
using WrapperAPI.Models.GiteModels; // Voor Hotel DTO's
using WrapperAPI.Models.RestaurantModels;
using WrapperAPI.NewFolder;
using WrapperAPI.Repositories;


namespace WrapperAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoekingenController : ControllerBase
    {
        bool AlGereserveerd = false;
        int GefaaldeID = 0;
        string GefaaldeDatum = "";
        private readonly IBoekingRepository _boekingRepository;
        private readonly IGebruikerRepository _gebruikerRepository;
        private readonly IAccommodatieRepository _accommodatieRepository;
        private readonly IReserveringRepository _reserveringRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IHotelGastRepository _hotelGastRepository;
        private readonly IGiteRepository _giteRepository;
        private readonly IGiteGastRepository _giteGastRepository;

        // FIXED: Nu worden alle repositories correct toegewezen
        public BoekingenController(
            IBoekingRepository boekingRepository,
            IGebruikerRepository gebruikerRepository,
            IAccommodatieRepository accommodatieRepository,
            IReserveringRepository reserveringRepository,
            IHotelRepository hotelRepository,
            IHotelGastRepository hotelGastRepository,
            IGiteGastRepository giteGastRepository,
            IGiteRepository giteRepository)
        {
            _boekingRepository = boekingRepository;
            _gebruikerRepository = gebruikerRepository;
            _accommodatieRepository = accommodatieRepository;
            _reserveringRepository = reserveringRepository;
            _hotelRepository = hotelRepository;
            _hotelGastRepository = hotelGastRepository;
            _giteRepository = giteRepository;
            _giteGastRepository = giteGastRepository;
        }

        private void VerwerkHotel(FlexiItemDTO item, List<int> idLijst)
        {
            var gastInfo = _hotelGastRepository.GetHotelGastById(item.GastID);
            if (gastInfo == null) throw new Exception("Gast niet gevonden");

            var hotelRequest = new BoekingRequestDTO
            {
                gastNaam = gastInfo.naam,
                gastEmail = gastInfo.email,
                gastTel = gastInfo.tel ?? "",
                gastStraat = gastInfo.straat,
                gastHuisnr = gastInfo.huisnr,
                gastPostcode = gastInfo.postcode,
                gastPlaats = gastInfo.plaats,
                gastLand = gastInfo.land,
                eenheidID = item.EenheidID,
                startDatum = item.StartDatum,
                eindDatum = item.EindDatum,
                aantalPersonen = item.AantalVolwassenen + item.AantalJongeKinderen + item.AantalOudereKinderen,
                platformID = item.Platform
            };

            var result = _hotelRepository.CreateReservering(hotelRequest);
            if (result.foutMelding == "Deze eenheid is niet beschikbaar in de gekozen periode.")
            {
                GefaaldeID = hotelRequest.eenheidID;
                GefaaldeDatum = hotelRequest.startDatum.ToString();
                AlGereserveerd = true;
            }
            else
            {
                idLijst.Add(result.reserveringID);
            }
        }

        private void VerwerkGite(FlexiItemDTO item, List<int> idLijst)
        {
            
            var gastInfo = _giteGastRepository.GetGiteGastById(item.GastID);
            if (gastInfo == null) throw new Exception("Gast niet gevonden");

            var giteRequest = new BoekingRequestDTO
            {
                gastNaam = gastInfo.naam,
                gastEmail = gastInfo.email,
                gastTel = gastInfo.tel ?? "",
                gastStraat = gastInfo.straat,
                gastHuisnr = gastInfo.huisnr,
                gastPostcode = gastInfo.postcode,
                gastPlaats = gastInfo.plaats,
                gastLand = gastInfo.land,
                eenheidID = item.EenheidID,
                startDatum = item.StartDatum,
                eindDatum = item.EindDatum,
                aantalPersonen = item.AantalVolwassenen + item.AantalJongeKinderen + item.AantalOudereKinderen,
                platformID = item.Platform
            };

            var result = _giteRepository.CreateReservering(giteRequest);
            if (result.foutMelding == "Deze eenheid is niet beschikbaar in de gekozen periode.")
            {
                GefaaldeID = giteRequest.eenheidID;
                GefaaldeDatum = giteRequest.startDatum.ToString();
                AlGereserveerd = true;
            }
            else
            {
                idLijst.Add(result.reserveringID);
            }
        }

        private void VerwerkCamping(FlexiItemDTO item, List<int> idLijst)
        {
            var nieuweBoeking = new Boeking
            {
                GebruikerID = item.GastID,
                AccommodatieID = item.EenheidID, // Hier is EenheidID de kampeerplek
                CheckInDatum = item.StartDatum,
                CheckOutDatum = item.EindDatum,
                AantalVolwassenen = (byte)item.AantalVolwassenen,
                AantalJongeKinderen = (byte)item.AantalJongeKinderen,
                AantalOudereKinderen = (byte)item.AantalOudereKinderen,
                Opmerking = item.Opmerking ?? ""
            };

            var result = _boekingRepository.CreateBoeking(nieuweBoeking);
            if (result.BoekingID == -1)
            {
                GefaaldeID = nieuweBoeking.AccommodatieID;
                GefaaldeDatum = nieuweBoeking.CheckInDatum.ToString();
                AlGereserveerd = true;
            }
            else
            {
                idLijst.Add(result.BoekingID);
            }
        }

        private void VerwerkRestaurant(FlexiItemDTO item, List<int> idLijst)
        {
            var restRequest = new SendingReservering // Of hoe je restaurant model ook heet
            {
                boekingID = item.GastID,
                tafelID = item.EenheidID,
                datumTijd = item.StartDatum, // Meestal alleen startdatum/tijd nodig
                aantalVolwassenen = item.AantalVolwassenen, 
                aantalJongeKinderen = item.AantalJongeKinderen, 
                aantalOudereKinderen = item.AantalOudereKinderen
            };

            var result = _reserveringRepository.CreateReservering(restRequest, restRequest.boekingID);
            if (result.reserveringID == -1)
            {
                GefaaldeID = restRequest.tafelID;
                GefaaldeDatum = restRequest.datumTijd.ToString();
                AlGereserveerd = true;

            }
            else
            {
                idLijst.Add(result.reserveringID);
            }
        }

        [HttpPost("create-flexi")]
        public ActionResult CreateFlexi([FromBody] FlexiCombiDTO dto)
        {
            if (dto == null) return BadRequest("Geen data ontvangen.");
            var resultaten = new
            {
                CampingIDs = new List<int>(),
                RestaurantIDs = new List<int>(),
                HotelIDs = new List<int>(), // Nieuwe lijst voor hotel resultaten
                GiteIDs = new List<int>(),
                Errors = new List<string>()
            };

            try
            {
                foreach (var item in dto.Boekingen)
                {
                    try
                    {
                        switch (item.AccommodatieType?.ToLower())
                        {
                            case "hotel":
                                VerwerkHotel(item, resultaten.HotelIDs);
                                break;
                            case "gite":
                                VerwerkGite(item, resultaten.GiteIDs);
                                break;
                            case "camping":
                                VerwerkCamping(item, resultaten.CampingIDs);
                                break;
                            case "restaurant":
                                VerwerkRestaurant(item, resultaten.RestaurantIDs);
                                break;
                            default:
                                resultaten.Errors.Add($"Onbekend type: {item.AccommodatieType}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        resultaten.Errors.Add($"{item.AccommodatieType} fout: {ex.Message}");
                    }
                }
                if (AlGereserveerd == true)
                {
                    AlGereserveerd = false;
                    foreach (var item in resultaten.CampingIDs)
                    {
                        _boekingRepository.DeleteBoeking(item);
                    }
                    foreach (var item in resultaten.RestaurantIDs)
                    {
                        _reserveringRepository.DeleteReservering(item);
                    }
                    foreach (var item in resultaten.HotelIDs)
                    {
                        _hotelRepository.DeleteReservering(item);
                    }
                    foreach (var item in resultaten.GiteIDs)
                    {
                        _giteRepository.DeleteReservering(item);
                    }
                    return Ok(new
                    {
                        Bericht = "Verwerking gefaald",
                        Details = $"De reservering met accommodatie nummer {GefaaldeID} en datum {GefaaldeDatum} is al gereserveerd."
                    });
                }
                return Ok(new
                {
                    Bericht = "Verwerking voltooid",
                    Details = resultaten
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Kritieke fout: {ex.Message}");
            }
        }
    }
}