﻿using SAP2000v1;

namespace SAP2000.models.materials
{
    public class RebarMaterialProperties : IMaterialProperties
    {
        public string MaterialName { get; set; }
        public eMatType MaterialType => eMatType.Rebar;
        public double Fy { get; set; }
        public double Fu { get; set; }

        public double ModulusOfElasticity => 200000;
        public double PoissonRatio => 0.3;
        public double ThermalCoeff => 0.0000117;
        public double UnitWeight => 0.0000785;    }
}
