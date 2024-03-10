namespace PROJE_UI.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public string MuseumName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid CostId { get; set; }
        public DateTime Time { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public long UserIdentity { get; set; }
        public int DateOfBirthYear { get; set; }
    }
}
