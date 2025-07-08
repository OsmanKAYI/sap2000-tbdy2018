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
        private List<string> RoofLiveLoadCases = new List<string> { "Cati Hareketli"};
        private string[] WindLoadCases = { "+Wix", "-Wix", "+Wiy", "-Wiy" };
        private string SeismicLoadCaseEx = "Ex";
        private string SeismicLoadCaseEy = "Ey";
        private string SeismicLoadCaseEz = "Ez";
        public LoadCombinationBuilder(cSapModel sapModel)
        {
            this._sapModel = sapModel;
        }

        public void defineAllCombinations()
        {
            var allCombos = new List<LoadCombination>();

            var gCombo = new LoadCombination("G");
            gCombo.addCase(DeadLoadCase, 1.0);
            foreach (var sdead in SuperDeadLoadCases)
            {
                if (!string.IsNullOrWhiteSpace(sdead))
                    gCombo.addCase(sdead, 1.0);
            }
            allCombos.Add(gCombo);

            var qCombo = new LoadCombination("Q");
            qCombo.addCase(LiveLoadCase, 1.0);
            foreach (var rLive in RoofLiveLoadCases)
            {
                if (!string.IsNullOrWhiteSpace(rLive))
                    qCombo.addCase(rLive, 1.0);
            }
            allCombos.Add(qCombo);

            allCombos.AddRange(createGravityCombinations(gCombo.Name, qCombo.Name));
            allCombos.AddRange(createWindCombinations(gCombo.Name, qCombo.Name));
            allCombos.AddRange(createAllSeismicCombinations(gCombo.Name, qCombo.Name));

            var seismicCombos = allCombos.Where(c => c.Name.Contains(SeismicLoadCaseEx) || c.Name.Contains(SeismicLoadCaseEy)).ToList();
            if (seismicCombos.Any())
            {
                var envelopeCombo = new LoadCombination("ZARF (DEPREMLI)");
                foreach (var seismic in seismicCombos)
                {
                    envelopeCombo.addCombo(seismic.Name, 1.0);
                }
            }

            foreach (var combo in allCombos)
            {
                buildCombinationInSap2000(combo);
            }
        }

        private List<LoadCombination> createGravityCombinations(string gName, string qName)
        {
            var combos = new List<LoadCombination>();
            combos.Add(new LoadCombination($"1.4*{gName} + 1.6*{qName}")
                .addCombo(gName, 1.4)
                .addCombo(qName, 1.6));
            return combos;
        }

        private List<LoadCombination> createWindCombinations(string gName, string qName)
        {
            var combos = new List<LoadCombination>();
            foreach (var w in WindLoadCases)
            {
                string comboName = $"1.2*{gName} + 1.6*({w}) + 0.5*{qName}";
                combos.Add(new LoadCombination(comboName)
                    .addCombo(gName, 1.2)
                    .addCase(w, 1.6)
                    .addCombo(qName, 0.5));
            }
            return combos;
        }

        private List<LoadCombination> createAllSeismicCombinations(string gName, string qName)
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
                                .addCombo(gName, 1.0)
                                .addCombo(qName, n)
                                .addCase(dom.Dom, signDom * dom.DomFactor)
                                .addCase(dom.Sub, signSub * dom.SubFactor)
                                .addCase(ez, signEz));

                            combos.Add(new LoadCombination($"0.9*{gName} {signDom.SignStr()}{dom.DomFactor}*{dom.Dom} {signSub.SignStr()}{dom.SubFactor}*{dom.Sub} {signEz.SignStr()}{ez}")
                                .addCombo(gName, 0.9)
                                .addCase(dom.Dom, signDom * dom.DomFactor)
                                .addCase(dom.Sub, signSub * dom.SubFactor)
                                .addCase(ez, signEz));
                        }
                    }
                }
            }

            return combos;
        }

        private void buildCombinationInSap2000(LoadCombination combo)
        {
            int comboType = 0; if (combo.Name.ToUpper().Contains("ZARF"))
            {
                comboType = 1;
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