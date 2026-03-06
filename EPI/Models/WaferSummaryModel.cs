namespace EPI.Models
{
    public class WaferSummaryModel
    {
        public string WaferId { get; set; }
        public string Folder { get; set; }
        public string FileName { get; set; }
        public double RealLaserPower { get; set; }

        public double PwAvg { get; set; }
        public double PwStdDev { get; set; }
        public double PwMin { get; set; }
        public double PwMax { get; set; }

        public double PiAvg { get; set; }
        public double PiStdDev { get; set; }
        public double PiMin { get; set; }
        public double PiMax { get; set; }

        public double FwhmAvg { get; set; }
        public double FwhmStdDev { get; set; }
        public double FwhmMin { get; set; }
        public double FwhmMax { get; set; }

        public double ThickAvg { get; set; }
        public double ThickStdDev { get; set; }
        public double ThickMin { get; set; }
        public double ThickMax { get; set; }
    }
}
