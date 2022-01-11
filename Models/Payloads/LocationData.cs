using System.Collections.Generic;

namespace Train_Car_Inventory_App.Models.Payloads
{
    public class LocationData
    {
        public string Name { get; set; }
        
        public string Owner { get; set; }
        
        public List<TrainCarSerialData> TrainCarSerialData { get; set; }
    }

    public class TrainCarSerialData
    {
        public string SerialNumber { get; set; }
        
        public int TotalNumber { get; set; }
        
        public double AverageAge { get; set; }
        
        public int ScrapedCount { get; set; }
    }
}