using SAP2000.models.seismic;
using SAP2000.services.builders.sections;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SAP2000.services.builders.loads
{
    public class SeismicLoadBuilder
    {
        private readonly cSapModel _sapModel;
        private readonly TBDY2018CoefficientService _coefficientService;

        public SeismicLoadBuilder(cSapModel sapModel)
        {
            _sapModel = sapModel;
            _coefficientService = new TBDY2018CoefficientService();
        }

        public void DefineSeismicLoads(SeismicParameters parameters)
        {
            var (fs, f1) = _coefficientService.CalculateCoefficients(parameters.Ss, parameters.S1, parameters.SiteClass);
            
            string tableName = "Auto Seismic - TSC-2018";
            int tableVersion = 0;
            string[] fields = null;
            int numRec = 0;
            string[] tableData = null;

            int ret = _sapModel.DatabaseTables.GetTableForEditingArray(tableName, "All", ref tableVersion, ref fields, ref numRec, ref tableData);
            if (ret != 0 || fields == null || fields.Length == 0)
            {
                throw new Exception($"SAP2000 '{tableName}' tablosu alınamadı.");
            }

            var loadPatterns = new[] { "Ex", "Ex-Bod", "Ey", "Ey-Bod", "Ez" };
            var directions = new[] { "X", "X", "Y", "Y", "Z" };            var newTableDataList = new List<string>();
            int numCols = fields.Length;

            for (int i = 0; i < loadPatterns.Length; i++)
            {
                var rowData = new string[numCols];
                for (int j = 0; j < rowData.Length; j++) { rowData[j] = ""; }

                rowData[Array.IndexOf(fields, "LoadPat")] = loadPatterns[i];
                rowData[Array.IndexOf(fields, "Dir")] = directions[i];
                rowData[Array.IndexOf(fields, "PercentEcc")] = "0.05";
                rowData[Array.IndexOf(fields, "PeriodCalc")] = "Prog Calc";
                rowData[Array.IndexOf(fields, "CtAndX")] = "0.10m, 0.75";
                rowData[Array.IndexOf(fields, "R")] = parameters.R.ToString(CultureInfo.InvariantCulture);
                rowData[Array.IndexOf(fields, "D")] = parameters.D.ToString(CultureInfo.InvariantCulture);
                rowData[Array.IndexOf(fields, "I")] = parameters.I.ToString(CultureInfo.InvariantCulture);
                rowData[Array.IndexOf(fields, "Ss")] = parameters.Ss.ToString(CultureInfo.InvariantCulture);
                rowData[Array.IndexOf(fields, "S1")] = parameters.S1.ToString(CultureInfo.InvariantCulture);
                rowData[Array.IndexOf(fields, "TL")] = "6";
                rowData[Array.IndexOf(fields, "SiteClass")] = parameters.SiteClass;
                rowData[Array.IndexOf(fields, "Fs")] = fs.ToString("F3", CultureInfo.InvariantCulture);                rowData[Array.IndexOf(fields, "F1")] = f1.ToString("F3", CultureInfo.InvariantCulture);

                newTableDataList.AddRange(rowData);
            }
            string[] newTableData = newTableDataList.ToArray();
            string msg = "";
            ret = _sapModel.DatabaseTables.SetTableForEditingArray(tableName, ref tableVersion, ref fields, loadPatterns.Length, ref newTableData);
            if (ret != 0) throw new Exception($"SAP2000 '{tableName}' tablosu güncellenemedi.");

            _sapModel.DatabaseTables.ApplyEditedTables(false, ref ret, ref ret, ref ret, ref ret, ref msg);
        }
    }
}
