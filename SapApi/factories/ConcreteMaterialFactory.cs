using SAP2000.models.materials;
using System;
using System.Collections.Generic;

namespace SAP2000.factories
{
    public class ConcreteMaterialFactory : IMaterialFactory
    {
        public IMaterialProperties createMaterial(Dictionary<string, object> parameters)
        {
            return new ConcreteMaterialProperties
            {
                MaterialName = (string)parameters["MaterialName"],
                Fck = Convert.ToDouble(parameters["Fck"]),
            };
        }
    }
}
