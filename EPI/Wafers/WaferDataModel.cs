using System.Collections.Generic;

namespace EPI.Wafers
{
    public class WaferDataModel
    {
        public string WaferId { get; set; } = string.Empty;
        public List<WaferCellModel> Cells { get; set; } = new List<WaferCellModel>();
        public List<string> AvailableParameters { get; set; } = new List<string>();
    }
}
