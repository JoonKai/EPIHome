using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EPI.Wafers
{
    public class MapFileParser
    {
        public static List<WaferCellModel> Parse(string filePath)
        {
            var cells = new List<WaferCellModel>();
            var lines = File.ReadAllLines(filePath);

            var headers = lines[0].Split(',')
                                  .Select(h => h.Trim())
                                  .ToList();

            int Idx(string name) => headers.IndexOf(name);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                var cols = lines[i].Split(',');

                double Get(string name)
                {
                    int idx = Idx(name);
                    if (idx < 0 || idx >= cols.Length) return 0;
                    return double.TryParse(cols[idx].Trim(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out double v) ? v : 0;
                }

                cells.Add(new WaferCellModel
                {
                    Number = (int)Get("number"),
                    X = (int)Get("x"),
                    Y = (int)Get("y"),
                    PeakWavelength = Get("PeakWavelength"),
                    DominantWavelength = Get("DominantWavelength"),
                    FWHM = Get("FWHM"),
                    PeakIntensity = Get("PeakIntensity"),
                    IntegratedIntensity = Get("IntegratedIntensity"),
                    PhotoDetect = Get("PhotoDetect"),
                    Reflection = Get("Reflection"),
                    Transmittance = Get("Transmittance"),
                    Thickness = Get("Thickness"),
                    ZPos = Get("ZPos"),
                    BlueReflection = Get("BlueReflection"),
                });
            }
            return cells;
        }
    }
}