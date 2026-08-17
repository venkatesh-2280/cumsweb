using System.Data;

namespace STAWeb.Models
{
    public class RuleEngineModel
    {
        public string RuleName { get; set; }
        public string TDSType { get; set; }
        public string BeneCategory { get; set; }
        public string PanStatus { get; set; }
        public string PanCategory { get; set; }
        public string TDSRate { get; set; }
        
    }
    
    public class RuleEngineNewModel
    {
        public int rulegid { get; set; }
        public string rulename { get; set; }
        public int tdstype { get; set; }
        public int benecategory { get; set; }
        public int panstatus { get; set; }
        public int pancat { get; set; }
        public decimal tdsrate { get; set; }
        public string user { get; set; }
        public string action { get; set; }

    }

    public class RuleEngineListRequest
    {
        public int rulegid { get; set; }
        public string action { get; set; }
    }

}
