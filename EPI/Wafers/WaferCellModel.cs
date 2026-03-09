using System.Collections.Generic;

namespace EPI.Wafers
{
    public class WaferCellModel
    {
        public int Number { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        // ✅ 실측값 파라미터 (0이 아닌 값들만)
        public double PeakWavelength { get; set; }
        public double DominantWavelength { get; set; }
        public double FWHM { get; set; }
        public double PeakIntensity { get; set; }
        public double IntegratedIntensity { get; set; }
        public double PhotoDetect { get; set; }
        public double Reflection { get; set; }
        public double Transmittance { get; set; }
        public double Thickness { get; set; }
        public double ZPos { get; set; }
        public double BlueReflection { get; set; }
    }
}