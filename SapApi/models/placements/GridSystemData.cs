using System.Collections.Generic;

namespace SAP2000.models.placements
{
    // girilen kolon konumlarına göre grid sistem verilerini tutan sınıf
    public class GridSystemData
    {
        public List<double> XCoordinates { get; set; }
        public List<double> YCoordinates { get; set; }
        public List<double> ZCoordinates { get; set; }

        public GridSystemData()
        {
            XCoordinates = new List<double>();
            YCoordinates = new List<double>();
            ZCoordinates = new List<double>();
        }
    }
}
