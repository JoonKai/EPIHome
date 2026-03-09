using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace EPI.Wafers
{
    public class SumFileParser
    {
        public WaferSummaryModel Parse(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            var summary = new WaferSummaryModel();
            var keyValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var lines = File.ReadAllLines(filePath);
                int i = 0;

                // Key=Value 섹션
                while (i < lines.Length)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrWhiteSpace(line)) { i++; continue; }
                    int eq = line.IndexOf('=');
                    if (eq > 0)
                    {
                        keyValues[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
                        i++;
                    }
                    else break;
                }

                summary.WaferId = GetStr(keyValues, "WaferID") ?? GetStr(keyValues, "WaferName") ?? string.Empty;
                summary.Folder = GetStr(keyValues, "Folder") ?? string.Empty;
                summary.FileName = GetStr(keyValues, "FileName") ?? string.Empty;
                summary.RealLaserPower = GetDbl(keyValues, "RealLaserPower");

                // 통계 테이블 헤더 찾기
                string[] headers = null;
                while (i < lines.Length)
                {
                    var line = lines[i].Trim();
                    if (line.StartsWith("Item", StringComparison.OrdinalIgnoreCase))
                    {
                        headers = line.Split(',');
                        for (int k = 0; k < headers.Length; k++) headers[k] = headers[k].Trim();
                        i++;
                        break;
                    }
                    i++;
                }
                if (headers == null) return summary;

                // 통계 행
                var rows = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
                while (i < lines.Length)
                {
                    var line = lines[i].Trim();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var parts = line.Split(',');
                        if (parts.Length > 0) rows[parts[0].Trim()] = parts;
                    }
                    i++;
                }

                var pw = GetStats(rows, headers, "PeakWavelength");
                summary.PwAvg = pw[0]; summary.PwStdDev = pw[1];
                summary.PwMin = pw[2]; summary.PwMax = pw[3];

                var pi = GetStats(rows, headers, "PeakIntensity");
                summary.PiAvg = pi[0]; summary.PiStdDev = pi[1];
                summary.PiMin = pi[2]; summary.PiMax = pi[3];

                var fw = GetStats(rows, headers, "FWHM");
                summary.FwhmAvg = fw[0]; summary.FwhmStdDev = fw[1];
                summary.FwhmMin = fw[2]; summary.FwhmMax = fw[3];

                var th = GetStats(rows, headers, "Thickness");
                summary.ThickAvg = th[0]; summary.ThickStdDev = th[1];
                summary.ThickMin = th[2]; summary.ThickMax = th[3];
            }
            catch { }

            return summary;
        }

        // [0]=Avg [1]=StdDev [2]=Min [3]=Max
        private double[] GetStats(Dictionary<string, string[]> rows, string[] headers, string colName)
        {
            int col = Array.FindIndex(headers,
                h => h.IndexOf(colName, StringComparison.OrdinalIgnoreCase) >= 0);
            if (col < 0) return new double[4];

            return new double[]
            {
                GetStat(rows, "Average", col),
                GetStat(rows, "StdDev",  col),
                GetStat(rows, "Min",     col),
                GetStat(rows, "Max",     col),
            };
        }

        private double GetStat(Dictionary<string, string[]> rows, string rowKey, int col)
        {
            if (!rows.TryGetValue(rowKey, out var parts) || col >= parts.Length) return 0.0;
            return double.TryParse(parts[col].Trim(), NumberStyles.Float,
                                   CultureInfo.InvariantCulture, out double v) ? v : 0.0;
        }

        private string GetStr(Dictionary<string, string> d, string key)
            => d.TryGetValue(key, out string v) ? v : null;

        private double GetDbl(Dictionary<string, string> d, string key)
        {
            if (!d.TryGetValue(key, out string v)) return 0.0;
            return double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double r) ? r : 0.0;
        }
    }
}
