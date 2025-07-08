using SAP2000.models.materials;
using SAP2000v1;
using System;

namespace SAP2000.services.builders.materials
{
    public class ConcreteMaterialBuilder : IMaterialBuilder
    {
        public void build(cSapModel sapModel, IMaterialProperties material) 
        {
            if (!(material is ConcreteMaterialProperties concrete)) return;

            string name = concrete.MaterialName;

            int ret = sapModel.PropMaterial.SetMaterial(name, eMatType.Concrete);

            if (ret != 0)
            {
                throw new Exception($"Malzeme {name} tanımlanırken hata oluştu (SetMaterial). Hata kodu: {ret}");
            }

            double modulusOfElasticity = 2.75e+10; 
            double poissonRatio = 0.2;
            double thermalCoeff = 9.9e-6; // /C

            if (concrete.ModulusOfElasticity != 0) modulusOfElasticity = concrete.ModulusOfElasticity;
            if (concrete.PoissonRatio != 0) poissonRatio = concrete.PoissonRatio;
            if (concrete.ThermalCoeff != 0) thermalCoeff = concrete.ThermalCoeff;


            ret = sapModel.PropMaterial.SetMPIsotropic(name, modulusOfElasticity, poissonRatio, thermalCoeff);
            if (ret != 0)
            {
                throw new Exception($"Malzeme {name} izotropik özellikleri ayarlanırken hata oluştu (SetMPIsotropic). Hata kodu: {ret}");
            }

            double unitWeight = 2.5E-5; 
            if (concrete.UnitWeight != 0) unitWeight = concrete.UnitWeight; 

            ret = sapModel.PropMaterial.SetWeightAndMass(name, 1, unitWeight); 
            if (ret != 0)
            {
                throw new Exception($"Malzeme {name} ağırlık ve kütle ayarlanırken hata oluştu (SetWeightAndMass). Hata kodu: {ret}");
            }

            double fc = concrete.Fck; 

            // TBDY-2018'e göre tipik değerler veya genel kabul görmüş değerler:
            bool isLightweight = false;
            double fcsfactor = 0; // Hafif beton olmadığı için 0
            int ssType = 2; // Parametric - Mander (Genellikle daha detaylı modelleme için tercih edilir)
            int ssHysType = 2; // Takeda (Non-lineer analizler için yaygın)
            double strainAtfc = 0.0022; // Tipik değer
            double strainUltimate = 0.005; // Tipik değer
            double finalSlope = -0.1; // Tipik değer

            ret = sapModel.PropMaterial.SetOConcrete_1(
                name,
                fc,
                isLightweight,
                fcsfactor,
                ssType,
                ssHysType,
                strainAtfc,
                strainUltimate,
                finalSlope
            );

            if (ret != 0)
            {
                throw new Exception($"Malzeme {name} beton özellikleri ayarlanırken hata oluştu (SetOConcrete_1). Hata kodu: {ret}");
            }
        }
    }
}
