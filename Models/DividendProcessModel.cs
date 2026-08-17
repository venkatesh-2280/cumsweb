using System.Data;

namespace STAWeb.Models
{
    public class DividendProcessModel
    {
        public string DividendName { get; set; }
        public string DividendAcNo { get; set; }
        public DateTime DematBenposNSDL { get; set; }
        public DateTime DematBenposCDSL { get; set; }
        public DateTime PhysicalMaster { get; set; }
        public string PanMaster { get; set; }
        public string PanMasterUpdate { get; set; }
        
    }
}
