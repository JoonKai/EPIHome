using System;
using System.Windows.Media;

namespace EPI.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Color → "RRGGBB" 문자열
        /// </summary>
        public static string ToRRGGBBText(this Color color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// "RRGGBB" 문자열 → Color
        /// </summary>
        public static Color ToColor(this string hex)
        {
            hex = hex?.Trim().TrimStart('#') ?? string.Empty;
            if (hex.Length == 6)
            {
                return Color.FromRgb(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16));
            }
            if (hex.Length == 8)
            {
                return Color.FromArgb(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16),
                    Convert.ToByte(hex.Substring(6, 2), 16));
            }
            return Colors.Transparent;
        }
    }
}
