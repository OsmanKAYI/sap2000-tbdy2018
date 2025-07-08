using SAP2000.models.placements;
using SAP2000.services.builders.materials;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SAP2000.services.builders.placements
{
    public class GridSystemBuilder : ISap2000Builder<GridSystemData>
    {
        public void build(cSapModel sapModel, GridSystemData gridData)
        {
            sapModel.File.NewSolidBlock(0, 0, 0, true, "Defult", 0, 0, 0);

            string tableName = "Grid Lines";
            int tableVersion = 0;
            string[] fields = null;
            int numRec = 0;
            string[] tableData = null;

            int ret = sapModel.DatabaseTables.GetTableForEditingArray(tableName, "All", ref tableVersion, ref fields, ref numRec, ref tableData);
            if (ret != 0 || fields == null || fields.Length == 0)
            {
                throw new Exception($"SAP2000 '{tableName}' tablosu alınamadı. Model doğru başlatılamamış olabilir.");
            }

            var newTableDataList = new List<string>();
            int numCols = fields.Length;

            for (int i = 0; i < gridData.XCoordinates.Count; i++)
            {
                string gridId = Convert.ToChar('A' + i).ToString();
                newTableDataList.AddRange(createGridRow(fields, "X", gridId, gridData.XCoordinates[i]));
            }

            for (int i = 0; i < gridData.YCoordinates.Count; i++)
            {
                string gridId = (i + 1).ToString();
                newTableDataList.AddRange(createGridRow(fields, "Y", gridId, gridData.YCoordinates[i]));
            }

            for (int i = 0; i < gridData.ZCoordinates.Count; i++)
            {
                string gridId = $"Z{i}"; 
                newTableDataList.AddRange(createGridRow(fields, "Z", gridId, gridData.ZCoordinates[i]));
            }

            int newNumRec = newTableDataList.Count / numCols;
            string[] newTableData = newTableDataList.ToArray();

            ret = sapModel.DatabaseTables.SetTableForEditingArray(tableName, ref tableVersion, ref fields, newNumRec, ref newTableData);
            if (ret != 0)
            {
                throw new Exception("SAP2000 grid tablosu güncellenemedi. Hata kodu: " + ret);
            }

            bool applyToAll = false; int fatalError = 0, numErrors = 0, numWarnings = 0, numInfo = 0;
            string msg = "";
            sapModel.DatabaseTables.ApplyEditedTables(applyToAll, ref fatalError, ref numErrors, ref numWarnings, ref numInfo, ref msg);
            if (fatalError > 0 || numErrors > 0)
            {
                throw new Exception($"Grid sistemi uygulanırken hata oluştu: {msg}");
            }

            sapModel.View.RefreshView();
        }

        private string[] createGridRow(string[] fields, string axisDir, string gridId, double coordinate)
        {
            var rowData = new string[fields.Length];
            for (int i = 0; i < rowData.Length; i++)
            {
                rowData[i] = "";
            }

            rowData[Array.IndexOf(fields, "CoordSys")] = "GLOBAL";
            rowData[Array.IndexOf(fields, "AxisDir")] = axisDir;
            rowData[Array.IndexOf(fields, "GridID")] = gridId;
            rowData[Array.IndexOf(fields, "XRYZCoord")] = coordinate.ToString(CultureInfo.InvariantCulture);
            rowData[Array.IndexOf(fields, "LineType")] = "Primary";
            rowData[Array.IndexOf(fields, "Visible")] = "Yes";
            rowData[Array.IndexOf(fields, "BubbleLoc")] = "End";

            return rowData;
        }
    }
}
