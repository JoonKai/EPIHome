using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EPI.Wafers
{
    public class WaferFileService
    {
        private readonly MapFileParser _mapParser = new MapFileParser();

        public WaferDataModel LoadFromMapFile(string mapFilePath)
        {
            if (!File.Exists(mapFilePath))
                throw new FileNotFoundException($"파일이 없습니다: {mapFilePath}");

            var (cells, parameters) = _mapParser.Parse(mapFilePath);

            return new WaferDataModel
            {
                WaferId = Path.GetFileNameWithoutExtension(mapFilePath),
                Cells = cells,
                AvailableParameters = parameters
            };
        }

        public static double EstimateStepSize(WaferDataModel data)
        {
            if (data?.Cells == null || data.Cells.Count < 2) return 1.0;

            var xSet = new HashSet<int>();
            foreach (var c in data.Cells) xSet.Add(c.X);

            var sorted = new List<int>(xSet);
            sorted.Sort();
            if (sorted.Count < 2) return 1.0;

            double minDiff = double.MaxValue;
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                double d = sorted[i + 1] - sorted[i];
                if (d > 0 && d < minDiff) minDiff = d;
            }
            return minDiff == double.MaxValue ? 1.0 : minDiff;
        }

        public static double EstimateWaferSize(WaferDataModel data, double stepSize, double edgeExclusion)
        {
            if (data?.Cells == null || data.Cells.Count == 0) return 24.0;

            int maxRadius = 0;
            foreach (var c in data.Cells)
            {
                int r = Math.Max(Math.Abs(c.X), Math.Abs(c.Y));
                if (r > maxRadius) maxRadius = r;
            }

            return (maxRadius + 0.5) * stepSize * 2.0 + edgeExclusion * 2.0;
        }
    }
}
