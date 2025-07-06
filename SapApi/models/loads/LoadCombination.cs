using SAP2000v1;
using System.Collections.Generic;

namespace SAP2000.models.loads{
    // yük durumları ve kombinasyonlar için kullanılan sınıf
    public class LoadCombination
    {
        public string Name { get; set; }

        public Dictionary<string, (eCNameType Type, double Factor)> Components { get; private set; }

        public LoadCombination(string name)
        {
            Name = name;
            Components = new Dictionary<string, (eCNameType, double)>();
        }

        public LoadCombination AddCase(string loadCaseName, double factor)
        {
            Components[loadCaseName] = (eCNameType.LoadCase, factor);
            return this;        }

        public LoadCombination AddCombo(string comboName, double factor)
        {
            Components[comboName] = (eCNameType.LoadCombo, factor);
            return this;
        }
    }
}