using PROJE_UI.Models;

namespace PROJE_UI
{
    public class MuseumApiResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string messageTR { get; set; }
        public int systemTime { get; set; }
        public string endpoint { get; set; }
        public int rowCount { get; set; }
        public int creditUsed { get; set; }
       public List<Museum> data { get; set; }
    }
}
