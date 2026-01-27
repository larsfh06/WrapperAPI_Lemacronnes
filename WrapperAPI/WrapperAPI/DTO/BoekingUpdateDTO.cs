namespace WrapperAPI.NewFolder
{
    public class BoekingUpdateDTO
    {
        public int GebruikerID { get; set; }
        public int AccommodatieID { get; set; }
        public DateTime checkInDatum { get; set; }
        public DateTime checkOutDatum { get; set; }
        public byte AantalVolwassenen { get; set; } // Verander int naar byte
        public byte AantalJongeKinderen { get; set; } // Verander int naar byte
        public byte AantalOudereKinderen { get; set; } // Verander int naar byte
        public string? Opmerking { get; set; }
        public bool Cancelled { get; set; }
    }
}
