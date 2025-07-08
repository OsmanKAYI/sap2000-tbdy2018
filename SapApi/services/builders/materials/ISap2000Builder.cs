using SAP2000v1;

namespace SAP2000.services.builders.materials
{
    public interface ISap2000Builder<T>
    {
        void build(cSapModel sapModel, T properties);
    }
}
