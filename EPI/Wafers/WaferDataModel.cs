using EPI.Models;
using System.Collections.Generic;

namespace EPI.Wafers
{
    public class WaferDataModel
    {
        public string WaferId { get; set; } = string.Empty;
        public List<MapCellModel> Cells { get; set; } = new List<MapCellModel>();
        public List<string> AvailableParameters { get; set; } = new List<string>();
    }
}
