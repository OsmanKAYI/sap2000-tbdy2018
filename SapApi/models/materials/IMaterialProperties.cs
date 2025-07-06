using SAP2000v1;

namespace SAP2000.models.materials
// Temel mal özelliklerini oluşturmak için arayüz
{
    public interface IMaterialProperties
    {
        string MaterialName { get; set; }
        eMatType MaterialType { get; }
        double ModulusOfElasticity { get; }
        double PoissonRatio { get; }
        double ThermalCoeff { get; }
        double UnitWeight { get; }
    }
}
