using System.Data;

namespace STAWeb.Models
{
    public class DividendMasterModel
    {
        public int dividendgid { get; set; }
        public string companyName { get; set; }
        public string isin { get; set; }
        public string rbicode { get; set; }
        public string dividendYear { get; set; }
        public string dividendType { get; set; }
        public string remarks { get; set; }
        public double dividendRate { get; set; }
        public double faceValue { get; set; }
        public double dividendPerShare { get; set; }
        public string bankName { get; set; }
        public string ifscCode { get; set; }
        public string dividendAccountNo { get; set; }
        public DateOnly cutoffDate { get; set; }
        public DateOnly paymentDate { get; set; }
        public string user { get; set; }
        public string action { get; set; }
    }

    public class DividendListRequest
    {
        public int dividendgid { get; set; }
        public string action { get; set; }
    }
}
