using EPI.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EPI.Wafers
{
    public class MapFileParser
    {
        private int _idxNumber = 0, _idxX = 1, _idxY = 2;
        private int _idxPeakWl = 3, _idxDomWl = 4, _idxFwhm = 5;
        private int _idxPeakInt = 6, _idxIntInt = 7, _idxPhotoDetect = 8;
        private int _idxReflection = 9, _idxTransmittance = 10, _idxThickness = 11;
        private string[] _headers;

        public (List<MapCellModel> Cells, List<string> Parameters) Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"파일을 찾을 수 없습니다: {filePath}");

            var cells = new List<MapCellModel>();

            using (var reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine))
                    throw new InvalidDataException("헤더가 없는 빈 파일입니다.");

                _headers = headerLine.Split(',')
                                     .Select(h => h.Trim())
                                     .ToArray();
                ParseHeaderIndices();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var cell = ParseDataLine(line);
                    if (cell != null) cells.Add(cell);
                }
            }

            cells.Sort((a, b) => a.Number.CompareTo(b.Number));
            return (cells, _headers.Skip(3).ToList());
        }

        private void ParseHeaderIndices()
        {
            for (int i = 0; i < _headers.Length; i++)
            {
                // ✅ 정확한 컬럼명 매칭 (대소문자 무시)
                // Contains 대신 Equals 또는 앞에 "Second " 없는 경우만 매칭
                string h = _headers[i];
                string hl = h.ToLowerInvariant().Trim();

                if (hl == "number") _idxNumber = i;
                else if (hl == "x") _idxX = i;
                else if (hl == "y") _idxY = i;
                else if (hl == "peakwavelength") _idxPeakWl = i;
                else if (hl == "dominantwavelength") _idxDomWl = i;
                else if (hl == "fwhm") _idxFwhm = i;
                else if (hl == "peakintensity") _idxPeakInt = i;
                else if (hl == "integratedintensity") _idxIntInt = i;
                else if (hl == "photodetect") _idxPhotoDetect = i;
                else if (hl == "reflection") _idxReflection = i;
                else if (hl == "transmittance") _idxTransmittance = i;
                else if (hl == "thickness") _idxThickness = i;
            }
        }

        private MapCellModel ParseDataLine(string line)
        {
            try
            {
                var parts = line.Split(',');
                if (parts.Length < 4) return null;

                var cell = new MapCellModel
                {
                    Number = ParseInt(parts, _idxNumber),
                    X = ParseInt(parts, _idxX),
                    Y = ParseInt(parts, _idxY),
                    PeakWavelength = ParseDouble(parts, _idxPeakWl),
                    DominantWavelength = ParseDouble(parts, _idxDomWl),
                    Fwhm = ParseDouble(parts, _idxFwhm),
                    PeakIntensity = ParseDouble(parts, _idxPeakInt),
                    IntegratedIntensity = ParseDouble(parts, _idxIntInt),
                    PhotoDetect = ParseDouble(parts, _idxPhotoDetect),
                    Reflection = ParseDouble(parts, _idxReflection),
                    Transmittance = ParseDouble(parts, _idxTransmittance),
                    Thickness = ParseDouble(parts, _idxThickness),
                };

                for (int i = 3; i < _headers.Length && i < parts.Length; i++)
                {
                    if (double.TryParse(parts[i].Trim(), NumberStyles.Float,
                                        CultureInfo.InvariantCulture, out double val))
                        cell.AllValues[_headers[i]] = val;
                }

                return cell;
            }
            catch { return null; }
        }

        private int ParseInt(string[] parts, int idx)
        {
            if (idx < 0 || idx >= parts.Length) return 0;
            return int.TryParse(parts[idx].Trim(), out int v) ? v : 0;
        }

        private double ParseDouble(string[] parts, int idx)
        {
            if (idx < 0 || idx >= parts.Length) return 0.0;
            return double.TryParse(parts[idx].Trim(), NumberStyles.Float,
                                   CultureInfo.InvariantCulture, out double v) ? v : 0.0;
        }
    }
}
