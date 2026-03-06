using System.Collections.Generic;

namespace EPI.Models
{
    public class MapCellModel
    {
        public int Number { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double PeakWavelength { get; set; }
        public double DominantWavelength { get; set; }
        public double Fwhm { get; set; }
        public double PeakIntensity { get; set; }
        public double IntegratedIntensity { get; set; }
        public double PhotoDetect { get; set; }
        public double Reflection { get; set; }
        public double Transmittance { get; set; }
        public double Thickness { get; set; }
        public Dictionary<string, double> AllValues { get; set; } = new Dictionary<string, double>();
    }
}
