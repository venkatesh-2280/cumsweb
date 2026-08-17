using System.Data;

namespace STAWeb.Models
{
    public class PanModel
    {
        public string DividendName { get; set; }        
        public string DividendYear { get; set; }
        public string Remarks { get; set; }
        public string DividendAcNo { get; set; }       
        public string UploadFile { get; set; }       
        
    }

    public class PanValidationModel
    {
        public int dividendgid { get; set; }
        public int compgrpgid { get; set; }
        public DateTime benposdate { get; set; }               

    }

    public class PanRow
    {
        public string PAN { get; set; }
        public string PAN_Status { get; set; }
    }
    public class PanUpdateModel
    {
        public int dividendgid { get; set; }
        public List<PanRow> panList { get; set; }

    }
}
