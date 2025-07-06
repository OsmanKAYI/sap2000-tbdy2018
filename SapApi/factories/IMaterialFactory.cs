using SAP2000.models;
using SAP2000.models.materials;
using System.Collections.Generic;

namespace SAP2000.factories
{
    public interface IMaterialFactory
    {
        IMaterialProperties CreateMaterial(Dictionary<string, object> parameters);
    }
}
