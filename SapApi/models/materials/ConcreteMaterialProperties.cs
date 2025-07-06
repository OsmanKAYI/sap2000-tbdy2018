using SAP2000v1;
using System;

namespace SAP2000.models.materials
{
    public class ConcreteMaterialProperties : IMaterialProperties
    {
        public string MaterialName { get; set; }
        public eMatType MaterialType => eMatType.Concrete;
        public double Fck { get; set; }

        public double ModulusOfElasticity => 3250 * Math.Sqrt(Fck) + 14000;
        public double PoissonRatio => 0.2;
        public double ThermalCoeff => 0.00001;
        public double UnitWeight => 0.000025;    }
}
