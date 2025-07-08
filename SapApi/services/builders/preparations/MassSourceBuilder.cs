using SAP2000v1;
using System;

namespace SAP2000.services.builders.preparations
{
    class MassSourceBuilder
    {
        int ret = 0;
        cSapModel _sapModel;
        public MassSourceBuilder(cSapModel sapModel)
        {
            _sapModel = sapModel;
        }

        public void defineMassSources()
        {
            ret = _sapModel.SourceMass.ChangeName("MSSSRC1", "MassSource");
            string[] massSourceLoads = { "Ölü", "Duvar", "Kaplama", "Sapsiva", "Hareketli", "Cati Hareketli" };
            double[] massSourceMultipleOfLoad = { 1, 1, 1, 1, 0.3, 0.3 };
            ret = _sapModel.SourceMass.SetMassSource("MassSource", false, false, true, true, 6, ref massSourceLoads, ref massSourceMultipleOfLoad);

            if (ret != 0)
            {
                throw new Exception("Mass Source, işleminde hata oldu.");
            }
        }

    }
}
