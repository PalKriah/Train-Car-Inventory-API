using System.Collections.Generic;

namespace Train_Car_Inventory_App.Models.Payloads
{
    public class TrainCarStatisticsBySerialNumber
    {
        public string SerialNumber { get; set; }
        public List<YearData> YearData { get; set; } = new List<YearData>();
    }

    public class YearData
    {
        public int Year { get; set; }
        public int Manufactured { get; set; }
        public int Scrapped { get; set; }
    }
}