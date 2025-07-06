using SAP2000v1;
using System;

namespace SAP2000.services.builders.materials
{
    public static class Sap2000MaterialBuilderFactory
    {
        public static IMaterialBuilder GetBuilder(eMatType type)
        {
            switch (type)
            {
                case eMatType.Concrete: return new ConcreteMaterialBuilder();
                case eMatType.Rebar: return new RebarMaterialBuilder();
                default: throw new NotSupportedException("Desteklenmeyen malzeme tipi: " + type);
            }
        }
    }
}
