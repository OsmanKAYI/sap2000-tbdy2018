
using SAP2000v1;

namespace EtabsApi.services.builders.preparations
{
    class MassSourceBuilder
    {
        cSapModel _sapModel;
        public MassSourceBuilder(cSapModel sapModel)
        {
            _sapModel = sapModel;
        }

        public void DefineMassSources()
        {
            _sapModel.SourceMass.ChangeName("MSSSRC1", "MassSource");
            string[] massSourceLoads = { "Ölü", "Hareketli" };
            double[] massSourceMultipleOfLoad = { 1, 0.3 };
            _sapModel.SourceMass.SetMassSource("MassSource", false, false, true, true, 2, ref massSourceLoads, ref massSourceMultipleOfLoad);
        }
    }
}
