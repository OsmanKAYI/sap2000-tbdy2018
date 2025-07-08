using SAP2000.models.materials;
using SAP2000v1;
using System;

namespace SAP2000.services.builders.materials
{
    public class RebarMaterialBuilder : IMaterialBuilder
    {
        public void build(cSapModel sapModel, IMaterialProperties material) 
        {
            if (!(material is RebarMaterialProperties rebar)) return;

            string name = rebar.MaterialName;

            int ret = sapModel.PropMaterial.SetMaterial(name, eMatType.Rebar); 

            if (ret != 0)
            {
                throw new Exception($"Donatı malzemesi {name} tanımlanırken hata oluştu (SetMaterial). Hata kodu: {ret}");
            }

            double modulusOfElasticity = 2.0e+11; 
            double poissonRatio = 0.3;
            double thermalCoeff = 1.17e-5; 

            if (rebar.ModulusOfElasticity != 0) modulusOfElasticity = rebar.ModulusOfElasticity;
            if (rebar.PoissonRatio != 0) poissonRatio = rebar.PoissonRatio;
            if (rebar.ThermalCoeff != 0) thermalCoeff = rebar.ThermalCoeff;

            ret = sapModel.PropMaterial.SetMPIsotropic(name, modulusOfElasticity, poissonRatio, thermalCoeff);
            if (ret != 0)
            {
                throw new Exception($"Donatı malzemesi {name} izotropik özellikleri ayarlanırken hata oluştu (SetMPIsotropic). Hata kodu: {ret}");
            }

            double unitWeight = 7.85E-5; 
            if (rebar.UnitWeight != 0) unitWeight = rebar.UnitWeight;

            ret = sapModel.PropMaterial.SetWeightAndMass(name, 1, unitWeight); // 1 = Weight per unit volume
            if (ret != 0)
            {
                throw new Exception($"Donatı malzemesi {name} ağırlık ve kütle ayarlanırken hata oluştu (SetWeightAndMass). Hata kodu: {ret}");
            }

            // Fy: Minimum akma gerilmesi (örn. B420C için 420 MPa)
            // Fu: Minimum çekme gerilmesi (örn. B420C için 500-550 MPa)
            // SSType: Gerilme-birim şekil değiştirme eğisi tipi (1 =Parametric -Simple,2 = Parametric - Park)
            // SSHysType: Gerilme-birim şekil değiştirme histerezis tipi (2 = Takeda, genellikle non-lineer analiz için)
            // StrainAtHardening: Pekiştirmede birim şekil değiştirme (örn. 0.02)
            // StrainUltimate: Nihai birim şekil değiştirme (örn. 0.09)
            // FinalSlope: Nihai eğim çarpanı (örn. -0.1)
            // UseCaltransSSDefaults: Caltrans varsayılanlarını kullanıp kullanmama (false)

            double fy = rebar.Fy;
            double fu = rebar.Fu;

            // TBDY-2018'e göre tipik değerler veya genel kabul görmüş değerler:
            int ssType = 2; // Parametric - Park (Donatı için yaygın)
            int ssHysType = 2; // Takeda (Non-lineer analizler için yaygın)
            double strainAtHardening = 0.02; // Tipik değer
            double strainUltimate = 0.09; // Tipik değer
            double finalSlope = -0.1; // Tipik değer
            bool useCaltransSSDefaults = false; // Genellikle Türkiye projelerinde kullanılmaz


            ret = sapModel.PropMaterial.SetORebar_1(
                name,
                fy,
                fu,
                fy, 
                fu, 
                ssType,
                ssHysType,
                strainAtHardening,
                strainUltimate,
                finalSlope,
                useCaltransSSDefaults
            );

            if (ret != 0)
            {
                throw new Exception($"Donatı malzemesi {name} özellikleri ayarlanırken hata oluştu (SetORebar_1). Hata kodu: {ret}");
            }
        }
    }
}
