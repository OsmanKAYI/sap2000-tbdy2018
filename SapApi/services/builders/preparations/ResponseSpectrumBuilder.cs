using SAP2000.models.seismic;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SAP2000.services.builders.preparations
{
    public class ResponseSpectrumBuilder
    {
        private readonly cSapModel _sapModel;
        private const string TABLE_NAME = "Function - Response Spectrum - TSC-2018";

        public ResponseSpectrumBuilder(cSapModel sapModel)
        {
            _sapModel = sapModel ?? throw new ArgumentNullException(nameof(sapModel));
        }

        public void defineResponseSpectrumFunctions(SeismicParameters parameters)
        {

            int tableVersion = 0;
            string[] fields = null;
            int numRec = 0;
            string[] tableData = null;

            int ret = _sapModel.DatabaseTables.GetTableForEditingArray(TABLE_NAME, "All", ref tableVersion, ref fields, ref numRec, ref tableData);
            if (ret != 0 || fields == null || fields.Length == 0)
            {
                throw new InvalidOperationException($"SAP2000 '{TABLE_NAME}' tablosu alınamadı. Modelde bu fonksiyon tipi tanımlı olmayabilir.");
            }

            var newTableDataList = new List<string>();

            string[] horizontalRow = createSpectrumRow(fields, parameters, "TBDY2018 Yatay", "Horizontal");
            newTableDataList.AddRange(horizontalRow);

            string[] verticalRow = createSpectrumRow(fields, parameters, "TBDY2018 Düşey", "Vertical");
            newTableDataList.AddRange(verticalRow);

            int newNumRec = 2;
            string[] newTableData = newTableDataList.ToArray();

            ret = _sapModel.DatabaseTables.SetTableForEditingArray(TABLE_NAME, ref tableVersion, ref fields, newNumRec, ref newTableData);
            if (ret != 0)
            {
                throw new InvalidOperationException($"SAP2000 '{TABLE_NAME}' tablosu güncellenemedi. Hata kodu: {ret}");
            }

            int fatalError = 0, numErrors = 0, numWarnings = 0, numInfo = 0;
            string msg = "";
            // ApplyToAll parametresini 'false' yapmak, sadece bu tablodaki değişikliği uygular.
            // Bu, daha kontrollü bir yaklaşımdır.
            ret = _sapModel.DatabaseTables.ApplyEditedTables(false, ref fatalError, ref numErrors, ref numWarnings, ref numInfo, ref msg);
            if (fatalError > 0 || numErrors > 0 || ret != 0)
            {
                throw new Exception($"Tepki spektrumu fonksiyonları uygulanırken hata oluştu: {msg}, RET: {ret}");
            }
        }

        private string[] createSpectrumRow(string[] fields, SeismicParameters parameters, string functionName, string specDir)
        {
            var rowData = new string[fields.Length];

            for (int i = 0; i < rowData.Length; i++)
            {
                rowData[i] = "";
            }

            rowData[Array.IndexOf(fields, "Name")] = functionName;
            rowData[Array.IndexOf(fields, "FuncDamp")] = "0.05";
            rowData[Array.IndexOf(fields, "SpecDir")] = specDir;
            rowData[Array.IndexOf(fields, "Ss")] = parameters.Ss.ToString(CultureInfo.InvariantCulture);
            rowData[Array.IndexOf(fields, "S1")] = parameters.S1.ToString(CultureInfo.InvariantCulture);
            rowData[Array.IndexOf(fields, "TL")] = "6";
            rowData[Array.IndexOf(fields, "SiteClass")] = parameters.SiteClass;
            rowData[Array.IndexOf(fields, "R")] = parameters.R.ToString(CultureInfo.InvariantCulture);
            rowData[Array.IndexOf(fields, "D")] = parameters.D.ToString(CultureInfo.InvariantCulture);
            rowData[Array.IndexOf(fields, "I")] = parameters.I.ToString(CultureInfo.InvariantCulture);

            return rowData;
        }
    }
}
