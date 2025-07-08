using SAP2000.models.placements;
using SAP2000v1;
using System;
using System.Globalization;
using System.Linq;

namespace SAP2000.services.builders.loads
{
    public class WindLoadBuilder
    {
        private readonly cSapModel _sapModel;

        public WindLoadBuilder(cSapModel sapModel)
        {
            this._sapModel = sapModel;
        }

        public void defineWindLoads(GridSystemData gridData)
        {
            if (gridData?.ZCoordinates == null || gridData.ZCoordinates.Count < 2)
            {
                return;
            }

            double minZ = gridData.ZCoordinates.First();
            double maxZ = gridData.ZCoordinates.Last();
            double buildingHeightMeters = (maxZ - minZ) / 1000.0; double windSpeed = GetWindSpeedForHeight(buildingHeightMeters);

            string tableName = "Auto Wind - TS 498-97";
            int tableVersion = 0;
            string[] fields = null;
            int numRec = 0;
            string[] tableData = null;
            int ret = _sapModel.DatabaseTables.GetTableForEditingArray(tableName, "All", ref tableVersion, ref fields, ref numRec, ref tableData);
            if (ret != 0 || fields == null || fields.Length == 0 || tableData == null)
            {
                throw new Exception($"SAP2000 '{tableName}' tablosu alınamadı.");
            }

            int numCols = fields.Length;
            int exposeFromIndex = Array.IndexOf(fields, "ExposeFrom");
            int userZIndex = Array.IndexOf(fields, "UserZ");
            int maxZIndex = Array.IndexOf(fields, "MaxZ");
            int minZIndex = Array.IndexOf(fields, "MinZ");
            int windSpeedIndex = Array.IndexOf(fields, "WindSpeed");
            int angleIndex = Array.IndexOf(fields, "Angle");
            int windwardCpIndex = Array.IndexOf(fields, "WindwardCp");
            int leewardCpIndex = Array.IndexOf(fields, "LeewardCp");

            if (new[] { exposeFromIndex, userZIndex, maxZIndex, minZIndex, windSpeedIndex, angleIndex, windwardCpIndex, leewardCpIndex }.Contains(-1))
            {
                throw new Exception($"'{tableName}' tablosunda gerekli sütunlardan biri veya daha fazlası bulunamadı.");
            }

            for (int i = 0; i < numRec; i++)
            {
                int rowIndex = i * numCols;

                tableData[rowIndex + exposeFromIndex] = "AreaObjects";
                tableData[rowIndex + userZIndex] = "Yes";
                tableData[rowIndex + maxZIndex] = maxZ.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + minZIndex] = minZ.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + windSpeedIndex] = windSpeed.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + angleIndex] = "";
                tableData[rowIndex + windwardCpIndex] = "";
                tableData[rowIndex + leewardCpIndex] = "";
            }

            ret = _sapModel.DatabaseTables.SetTableForEditingArray(tableName, ref tableVersion, ref fields, numRec, ref tableData);
            if (ret != 0)
            {
                throw new Exception($"SAP2000 '{tableName}' tablosu güncellenemedi.");
            }

            string msg = "";
            _sapModel.DatabaseTables.ApplyEditedTables(false, ref ret, ref ret, ref ret, ref ret, ref msg);
        }

        private double GetWindSpeedForHeight(double heightInMeters)
        {
            if (heightInMeters <= 8) return 28;
            if (heightInMeters <= 20) return 36;
            if (heightInMeters <= 100) return 42;
            return 46;
        }
    }
}