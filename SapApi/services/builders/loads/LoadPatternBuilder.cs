using SAP2000v1;
using System;
using System.Collections.Generic;

namespace SAP2000.services.builders.loads
{
    public class LoadPatternBuilder
    {
        private readonly cSapModel _sapModel;

        public LoadPatternBuilder(cSapModel sapModel)
        {
            this._sapModel = sapModel;
            _sapModel.LoadPatterns.ChangeName("DEAD", "Ölü");
            _sapModel.LoadCases.ChangeName("DEAD", "Ölü");
        }

        public void defineLoadPatterns()
        {
            string tableName = "Load Pattern Definitions";
            int tableVersion = 0;
            string[] fields = null;
            int numRec = 0;
            string[] tableData = null;

            int ret = _sapModel.DatabaseTables.GetTableForEditingArray(tableName, "All", ref tableVersion, ref fields, ref numRec, ref tableData);
            if (ret != 0 || fields == null || fields.Length == 0)
            {
                throw new Exception($"SAP2000 '{tableName}' tablosu alınamadı.");
            }

            var loadPatternsToAdd = new List<Tuple<string, string, int, string>>
            {
                Tuple.Create("Duvar", "Super Dead", 0, ""),
                Tuple.Create("Kaplama", "Super Dead", 0, ""),
                Tuple.Create("Sapsiva", "Super Dead", 0, ""),
                Tuple.Create("Hareketli", "Live", 0, ""),
                Tuple.Create("Cati Hareketli", "Roof Live", 0, ""),
                Tuple.Create("Ex", "Quake", 0, "TSC-2018"),
                Tuple.Create("Ex-Bod", "Quake", 0, "TSC-2018"),
                Tuple.Create("Ey", "Quake", 0, "TSC-2018"),
                Tuple.Create("Ey-Bod", "Quake", 0, "TSC-2018"),
                Tuple.Create("Ez", "Quake", 0, "TSC-2018"),
                Tuple.Create("+Wix", "Wind", 0, "TS 498-97"),
                Tuple.Create("-Wix", "Wind", 0, "TS 498-97"),
                Tuple.Create("+Wiy", "Wind", 0, "TS 498-97"),
                Tuple.Create("-Wiy", "Wind", 0, "TS 498-97"),
                Tuple.Create("Kar", "Snow", 0, "")
            };


            var newTableDataList = new List<string>();
            int numCols = fields.Length;

            foreach (var lp in loadPatternsToAdd)
            {
                var rowData = new string[numCols];
                for (int i = 0; i < rowData.Length; i++) { rowData[i] = ""; }

                rowData[Array.IndexOf(fields, "LoadPat")] = lp.Item1;
                rowData[Array.IndexOf(fields, "DesignType")] = lp.Item2;
                rowData[Array.IndexOf(fields, "SelfWtMult")] = lp.Item3.ToString();
                rowData[Array.IndexOf(fields, "AutoLoad")] = lp.Item4;

                newTableDataList.AddRange(rowData);
            }

            int newNumRec = loadPatternsToAdd.Count;
            string[] newTableData = newTableDataList.ToArray();

            ret = _sapModel.DatabaseTables.SetTableForEditingArray(tableName, ref tableVersion, ref fields, newNumRec, ref newTableData);
            if (ret != 0)
            {
                throw new Exception($"SAP2000 '{tableName}' tablosu güncellenemedi. Hata kodu: " + ret);
            }

            bool applyToAll = false;
            int fatalError = 0, numErrors = 0, numWarnings = 0, numInfo = 0;
            string msg = "";
            _sapModel.DatabaseTables.ApplyEditedTables(applyToAll, ref fatalError, ref numErrors, ref numWarnings, ref numInfo, ref msg);
            if (fatalError > 0 || numErrors > 0)
            {
                throw new Exception($"Yük desenleri uygulanırken hata oluştu: {msg}");
            }

            createAnalysisCases(loadPatternsToAdd);

        }

        private void createAnalysisCases(List<Tuple<string, string, int, string>> loadPatterns)
        {
            for (int i = 0; i < loadPatterns.Count; i++)
            {
                var lp = loadPatterns[i];
                string loadPatternName = lp.Item1;

                _sapModel.LoadCases.StaticLinear.SetCase(loadPatternName);

                var loadTypes = new string[] { "Load" }; var loadNames = new string[] { loadPatternName }; var scaleFactors = new double[] { 1.0 };
                _sapModel.LoadCases.StaticLinear.SetLoads(
                    loadPatternName,
                    1,
                    ref loadTypes,
                    ref loadNames,
                    ref scaleFactors
                );
            }
        }
    }
}
