using SAP2000.models;
using SAP2000.models.materials;
using System;
using System.Collections.Generic;

namespace SAP2000.factories
{
    public class RebarMaterialFactory : IMaterialFactory
    {
        public IMaterialProperties CreateMaterial(Dictionary<string, object> parameters)
        {
            return new RebarMaterialProperties
            {
                MaterialName = (string)parameters["MaterialName"],
                Fy = Convert.ToDouble(parameters["Fy"]),
                Fu = Convert.ToDouble(parameters["Fu"]),
            };
        }
    }
}
