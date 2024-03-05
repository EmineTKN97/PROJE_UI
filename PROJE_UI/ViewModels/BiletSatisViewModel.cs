using PROJE_UI.Models;

namespace PROJE_UI.ViewModels
{
    public class BiletSatisViewModel
    {
        public List<City> Cities { get; set; }
        public List<District> Districts { get; set; }
        public List<Museum> Museums { get; set;}
        public List<Ticket> Tickets { get; set; }
        public BiletSatisViewModel()
        {
            Cities = new List<City>();
            Districts = new List<District>();
            Museums = new List<Museum>();
            Tickets = new List<Ticket>();   
        }


    }
}
