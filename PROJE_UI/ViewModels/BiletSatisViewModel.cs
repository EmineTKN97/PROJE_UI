using PROJE_UI.Models;

namespace PROJE_UI.ViewModels
{
    public class BiletSatisViewModel
    {
        public List<City> Cities { get; set; }
        public List<District> Districts { get; set; }
        public District district { get; set; }  
        public City city { get; set; }

  
    }
}
