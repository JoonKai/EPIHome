using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace EPI.EPIColors
{
    public class ColorSets
    {
        /// <summary>
        /// 기본 Rainbow 팔레트 (파랑 → 초록 → 노랑 → 빨강)
        /// </summary>
        public IEnumerable<Color> GetColors()
        {
            return new List<Color>
            {
                FromHex("000080"), FromHex("010F84"), FromHex("031F88"), FromHex("05308C"),
                FromHex("074190"), FromHex("095294"), FromHex("0B6498"), FromHex("0E779C"),
                FromHex("108AA1"), FromHex("139DA5"), FromHex("15A9A1"), FromHex("1BB189"),
                FromHex("21B971"), FromHex("27C158"), FromHex("2BC54B"), FromHex("32CD32"),
                FromHex("41D030"), FromHex("50D32E"), FromHex("61D52B"), FromHex("73D829"),
                FromHex("85DB27"), FromHex("99DD24"), FromHex("AEE022"), FromHex("C4E21F"),
                FromHex("DBE51D"), FromHex("E8DD1A"), FromHex("EAC918"), FromHex("EDB415"),
                FromHex("F09E12"), FromHex("F2870F"), FromHex("F56E0C"), FromHex("F85409"),
                FromHex("FA3906"), FromHex("FD1D03"), FromHex("FF0000"), FromHex("F60606"),
                FromHex("E11313"), FromHex("CD1D1D"), FromHex("B92424"), FromHex("A52A2A")
            };
        }

        private static Color FromHex(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length == 6)
            {
                return Color.FromRgb(
                    Convert.ToByte(hex.Substring(0, 2), 16),
                    Convert.ToByte(hex.Substring(2, 2), 16),
                    Convert.ToByte(hex.Substring(4, 2), 16));
            }
            return Colors.Transparent;
        }
    }
}
