using SAP2000.models.seismic; 
using SAP2000.services.builders.sections;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SAP2000.services.builders.preparations 
{
    public class SeismicLoadBuilder
    {
        private readonly cSapModel _sapModel;
        private readonly TBDY2018CoefficientService _coefficientService;
        private const string TABLE_NAME = "Auto Seismic - TSC-2018";

        public SeismicLoadBuilder(cSapModel sapModel)
        {
            _sapModel = sapModel ?? throw new ArgumentNullException(nameof(sapModel));
            _coefficientService = new TBDY2018CoefficientService();
        }

        public void defineSeismicLoads(SeismicParameters parameters)
        {
            var (fs, f1) = _coefficientService.calculateCoefficients(parameters.Ss, parameters.S1, parameters.SiteClass);

            int tableVersion = 0;
            string[] fields = null;
            int numRec = 0;
            string[] tableData = null;

            int ret = _sapModel.DatabaseTables.GetTableForEditingArray(TABLE_NAME, "All", ref tableVersion, ref fields, ref numRec, ref tableData);
            if (ret != 0 || fields == null || fields.Length == 0)
            {
                throw new InvalidOperationException($"SAP2000 '{TABLE_NAME}' tablosu alınamadı.");
            }

            int numCols = fields.Length;

            var indices = new Dictionary<string, int>
            {
                { "Dir", Array.IndexOf(fields, "Dir") },
                { "R", Array.IndexOf(fields, "R") },
                { "D", Array.IndexOf(fields, "D") },
                { "I", Array.IndexOf(fields, "I") },
                { "Ss", Array.IndexOf(fields, "Ss") },
                { "S1", Array.IndexOf(fields, "S1") },
                { "TL", Array.IndexOf(fields, "TL") },
                { "SiteClass", Array.IndexOf(fields, "SiteClass") },
                { "Fs", Array.IndexOf(fields, "Fs") },
                { "F1", Array.IndexOf(fields, "F1") }
            };

            for (int i = 0; i < numRec; i++)
            {
                int rowIndex = i * numCols;

                string loadPatName = tableData[rowIndex + Array.IndexOf(fields, "LoadPat")];
                if (loadPatName.StartsWith("Ex")) tableData[rowIndex + indices["Dir"]] = "X";
                if (loadPatName.StartsWith("Ey")) tableData[rowIndex + indices["Dir"]] = "Y";
                if (loadPatName.StartsWith("Ez")) tableData[rowIndex + indices["Dir"]] = "Z";

                tableData[rowIndex + indices["R"]] = parameters.R.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + indices["D"]] = parameters.D.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + indices["I"]] = parameters.I.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + indices["Ss"]] = parameters.Ss.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + indices["S1"]] = parameters.S1.ToString(CultureInfo.InvariantCulture);
                tableData[rowIndex + indices["TL"]] = "6";
                tableData[rowIndex + indices["SiteClass"]] = parameters.SiteClass;

                tableData[rowIndex + indices["Fs"]] = fs.ToString("F3", CultureInfo.InvariantCulture);
                tableData[rowIndex + indices["F1"]] = f1.ToString("F3", CultureInfo.InvariantCulture);
            }

            ret = _sapModel.DatabaseTables.SetTableForEditingArray(TABLE_NAME, ref tableVersion, ref fields, numRec, ref tableData);
            if (ret != 0)
            {
                throw new InvalidOperationException($"SAP2000 '{TABLE_NAME}' tablosu güncellenemedi. Hata kodu: {ret}");
            }

            int fatalError = 0, numErrors = 0, numWarnings = 0, numInfo = 0;
            string msg = "";
            ret = _sapModel.DatabaseTables.ApplyEditedTables(false, ref fatalError, ref numErrors, ref numWarnings, ref numInfo, ref msg);
            if (fatalError > 0 || numErrors > 0 || ret != 0)
            {
                throw new Exception($"Otomatik deprem yükü parametreleri uygulanırken hata oluştu: {msg}, RET: {ret}");
            }
        }
    }
}
