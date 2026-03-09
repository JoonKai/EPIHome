using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace EPI.Wafers
{
    public class ColorMapService
    {
        // 무지개 팔레트: Violet → Red (EPIWaferMapRange 기본 팔레트와 동일 방향)
        private static readonly Color[] Palette =
        {
            Color.FromRgb(0xFF, 0x00, 0xFF), // Magenta  (top  = 최댓값)
            Color.FromRgb(0x80, 0x00, 0xFF),
            Color.FromRgb(0x00, 0x00, 0xFF), // Blue
            Color.FromRgb(0x00, 0x80, 0xFF),
            Color.FromRgb(0x00, 0xFF, 0xFF), // Cyan
            Color.FromRgb(0x00, 0xFF, 0x80),
            Color.FromRgb(0x00, 0xFF, 0x00), // Green
            Color.FromRgb(0x80, 0xFF, 0x00),
            Color.FromRgb(0xFF, 0xFF, 0x00), // Yellow
            Color.FromRgb(0xFF, 0x80, 0x00),
            Color.FromRgb(0xFF, 0x00, 0x00), // Red      (bottom = 최솟값)
        };

        /// <summary>
        /// value → Color (rangeStart ~ rangeEnd 선형 매핑)
        /// </summary>
        public Color GetColor(double value, double rangeStart, double rangeEnd)
        {
            double span = rangeEnd - rangeStart;
            if (span <= 0.0) return Palette[Palette.Length / 2];

            // 0.0(최솟값=Red) ~ 1.0(최댓값=Magenta)
            double t = Math.Max(0.0, Math.Min(1.0, (value - rangeStart) / span));

            // Palette는 Magenta(0) → Red(끝) 이므로 인덱스 반전
            double pos = (1.0 - t) * (Palette.Length - 1);
            int lo = (int)Math.Floor(pos);
            int hi = Math.Min(lo + 1, Palette.Length - 1);
            double fraction = pos - lo;

            return LerpColor(Palette[lo], Palette[hi], fraction);
        }

        /// <summary>
        /// 히스토그램 분포 계산
        /// </summary>
        public List<long> BuildDistribution(List<double> values, int bucketCount,
                                            double rangeStart, double rangeEnd)
        {
            var dist = new long[bucketCount];
            double span = rangeEnd - rangeStart;
            if (span <= 0.0 || values == null) return new List<long>(dist);

            foreach (var v in values)
            {
                int idx = (int)((v - rangeStart) / span * bucketCount);
                idx = Math.Max(0, Math.Min(bucketCount - 1, idx));
                dist[idx]++;
            }
            return new List<long>(dist);
        }

        private static Color LerpColor(Color a, Color b, double t)
        {
            return Color.FromRgb(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t));
        }
    }
}
