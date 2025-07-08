using SAP2000.models.loads;
using SAP2000v1;
using System.Collections.Generic;
using System.Linq;

namespace SAP2000.services.builders.loads
{
    public class LoadCombinationBuilder
    {
        private readonly cSapModel _sapModel;

        private const double LiveLoadParticipationFactor = 0.3;
        private const string DeadLoadCase = "Ölü";
        private const string LiveLoadCase = "Hareketli";
        private List<string> SuperDeadLoadCases = new List<string> { "Duvar", "Kaplama", "SivaSap" };
        private List<string> RoofLiveLoadCases = new List<string> { "Cati Hareketli" };
        private string[] WindLoadCases = { "+Wix", "-Wix", "+Wiy", "-Wiy" };
        private string SeismicLoadCaseEx = "Ex";
        private string SeismicLoadCaseEy = "Ey";
        private string SeismicLoadCaseEz = "Ez";
        public LoadCombinationBuilder(cSapModel sapModel)
        {
            this._sapModel = sapModel;
        }

        public void defineAllCombinations() // Metot adı DefineAllCombinations olarak düzeltildi
        {
            var allCombos = new List<LoadCombination>();

            var gCombo = new LoadCombination("G");
            gCombo.addCase(DeadLoadCase, 1.0); // Metot adı AddCase olarak düzeltildi
            foreach (var sdead in SuperDeadLoadCases)
            {
                if (!string.IsNullOrWhiteSpace(sdead))
                    gCombo.addCase(sdead, 1.0); // Metot adı AddCase olarak düzeltildi
            }
            allCombos.Add(gCombo);

            var qCombo = new LoadCombination("Q");
            qCombo.addCase(LiveLoadCase, 1.0); // Metot adı AddCase olarak düzeltildi
            foreach (var rLive in RoofLiveLoadCases)
            {
                if (!string.IsNullOrWhiteSpace(rLive))
                    qCombo.addCase(rLive, 1.0); // Metot adı AddCase olarak düzeltildi
            }
            allCombos.Add(qCombo);

            // "G" ve "Q" kombinasyonlarını başlangıçta betonarme tasarım için işaretle
            // Bu satırlar artık döngü içinde tüm kombinasyonlar için çağrılacak, bu yüzden burada tekrara gerek yok.
            // _sapModel.DesignConcrete.SetComboStrength("G",true);
            // _sapModel.DesignConcrete.SetComboStrength("Q", true);

            allCombos.AddRange(CreateGravityCombinations(gCombo.Name, qCombo.Name)); // Metot adı CreateGravityCombinations olarak düzeltildi
            allCombos.AddRange(CreateWindCombinations(gCombo.Name, qCombo.Name));     // Metot adı CreateWindCombinations olarak düzeltildi
            allCombos.AddRange(CreateAllSeismicCombinations(gCombo.Name, qCombo.Name)); // Metot adı CreateAllSeismicCombinations olarak düzeltildi

            var seismicCombos = allCombos.Where(c => c.Name.Contains(SeismicLoadCaseEx) || c.Name.Contains(SeismicLoadCaseEy)).ToList();
            if (seismicCombos.Any())
            {
                var envelopeCombo = new LoadCombination("ZARF (DEPREMLI)");
                foreach (var seismic in seismicCombos)
                {
                    envelopeCombo.addCombo(seismic.Name, 1.0); // Metot adı AddCombo olarak düzeltildi
                }
                allCombos.Add(envelopeCombo); // Zarf kombinasyonunu da genel listeye ekle
            }

            foreach (var combo in allCombos)
            {
                BuildCombinationInSap2000(combo); // Metot adı BuildCombinationInSap2000 olarak düzeltildi
                // Her oluşturulan kombinasyonu betonarme dayanım tasarımı için seç
                _sapModel.DesignConcrete.SetComboStrength(combo.Name, true);
            }
        }

        private List<LoadCombination> CreateGravityCombinations(string gName, string qName) // Metot adı CreateGravityCombinations olarak düzeltildi
        {
            var combos = new List<LoadCombination>();
            combos.Add(new LoadCombination($"1.4*{gName} + 1.6*{qName}")
                .addCombo(gName, 1.4) // Metot adı AddCombo olarak düzeltildi
                .addCombo(qName, 1.6)); // Metot adı AddCombo olarak düzeltildi
            return combos;
        }

        private List<LoadCombination> CreateWindCombinations(string gName, string qName) // Metot adı CreateWindCombinations olarak düzeltildi
        {
            var combos = new List<LoadCombination>();
            foreach (var w in WindLoadCases)
            {
                string comboName = $"1.2*{gName} + 1.6*({w}) + 0.5*{qName}";
                combos.Add(new LoadCombination(comboName)
                    .addCombo(gName, 1.2) // Metot adı AddCombo olarak düzeltildi
                    .addCombo(w, 1.6)      // Metot adı AddCase olarak düzeltildi
                    .addCombo(qName, 0.5)); // Metot adı AddCombo olarak düzeltildi
            }
            return combos;
        }

        private List<LoadCombination> CreateAllSeismicCombinations(string gName, string qName) // Metot adı CreateAllSeismicCombinations olarak düzeltildi
        {
            var combos = new List<LoadCombination>();
            double n = LiveLoadParticipationFactor;
            string ex = SeismicLoadCaseEx;
            string ey = SeismicLoadCaseEy;
            string ez = SeismicLoadCaseEz;

            var dominantFactors = new[]
            {
                new { Dom = ex, DomFactor = 1.0, Sub = ey, SubFactor = 0.3 },
                new { Dom = ey, DomFactor = 1.0, Sub = ex, SubFactor = 0.3 }
            };

            var signs = new[] { 1.0, -1.0 };

            foreach (var dom in dominantFactors)
            {
                foreach (var signDom in signs)
                {
                    foreach (var signSub in signs)
                    {
                        foreach (var signEz in signs)
                        {
                            combos.Add(new LoadCombination($"{gName} + {n}*{qName} {signDom.SignStr()}{dom.DomFactor}*{dom.Dom} {signSub.SignStr()}{dom.SubFactor}*{dom.Sub} {signEz.SignStr()}{ez}")
                                .addCombo(gName, 1.0) // Metot adı AddCombo olarak düzeltildi
                                .addCombo(qName, n)   // Metot adı AddCombo olarak düzeltildi
                                .addCase(dom.Dom, signDom * dom.DomFactor) // Metot adı AddCase olarak düzeltildi
                                .addCase(dom.Sub, signSub * dom.SubFactor) // Metot adı AddCase olarak düzeltildi
                                .addCase(ez, signEz)); // Metot adı AddCase olarak düzeltildi

                            combos.Add(new LoadCombination($"0.9*{gName} {signDom.SignStr()}{dom.DomFactor}*{dom.Dom} {signSub.SignStr()}{dom.SubFactor}*{dom.Sub} {signEz.SignStr()}{ez}")
                                .addCombo(gName, 0.9) // Metot adı AddCombo olarak düzeltildi
                                .addCase(dom.Dom, signDom * dom.DomFactor) // Metot adı AddCase olarak düzeltildi
                                .addCase(dom.Sub, signSub * dom.SubFactor) // Metot adı AddCase olarak düzeltildi
                                .addCase(ez, signEz)); // Metot adı AddCase olarak düzeltildi
                        }
                    }
                }
            }
            return combos;
        }

        private void BuildCombinationInSap2000(LoadCombination combo) // Metot adı BuildCombinationInSap2000 olarak düzeltildi
        {
            int comboType = 0;
            if (combo.Name.ToUpper().Contains("ZARF"))
            {
                comboType = 1; // Envelope tipi
            }
            _sapModel.RespCombo.Add(combo.Name, comboType);

            foreach (var component in combo.Components)
            {
                string componentName = component.Key;
                eCNameType componentType = component.Value.Type;
                double factor = component.Value.Factor;
                _sapModel.RespCombo.SetCaseList(combo.Name, ref componentType, componentName, factor);
            }
        }
    }

    public static class NumericExtensions
    {
        public static string SignStr(this double value)
        {
            return value >= 0 ? "+ " : "- ";
        }
    }
}
