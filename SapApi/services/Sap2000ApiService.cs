using EtabsApi.services.builders;
using EtabsApi.services.builders.preparations;
using SAP2000.models.seismic;
using SAP2000.models.placements;
using SAP2000.services.builders.preparations;
using SAP2000v1;
using System;
using System.Collections.Generic;
using System.Linq;
using SAP2000.services.builders.sections;
using SAP2000.services.builders.materials;
using SAP2000.services.builders.loads;
using SAP2000.services.builders.placements;
using SAP2000.models.materials;
using SAP2000.models.sections;

namespace SAP2000.services
{
    //SAP2000 API servisi, SAP2000 modelini oluşturmak ve yapı elemanlarını tanımlamak için kullanılır. 
    // Çeşitli sınıfların builder'larını kullanarak, malzeme, kesit, yük ve yerleşim bilgilerini SAP2000 modeline aktarır.

    public class Sap2000ApiService : ISap2000ApiService
    {
        public void CreateProjectInNewModel(GridSystemData gridData, SeismicParameters seismicParameters, List<IMaterialProperties> materials, List<ISectionProperties> sections, List<ColumnPlacementInfo> columnplacements, List<BeamPlacementInfo> beamplacements, bool makeVisible)
        {
            cOAPI sapObject = null;
            cSapModel sapModel = null;

            try
            {
                cHelper helper = new Helper();
                sapObject = helper.CreateObjectProgID("CSI.SAP2000.API.SapObject");
                sapObject.ApplicationStart(eUnits.N_mm_C);
                sapModel = sapObject.SapModel;

                new StartTheProject(sapModel).CreateNewModel();
                
                if (gridData != null) new GridSystemBuilder().Build(sapModel, gridData);
                new LoadPatternBuilder(sapModel).DefineLoadPatterns();
                if (seismicParameters != null) new SeismicLoadBuilder(sapModel).DefineSeismicLoads(seismicParameters);
                if (gridData != null) new WindLoadBuilder(sapModel).DefineWindLoads(gridData);

                if (materials != null && materials.Any())
                {
                    foreach (var material in materials)
                    {
                        Sap2000MaterialBuilderFactory.GetBuilder(material.MaterialType).Build(sapModel, material);
                    }
                }

                if (sections != null && sections.Any())
                {
                    foreach (var section in sections)
                    {
                        if (section is ColumnSectionProperties col) new ColumnBuilder().Build(sapModel, col);
                        else if (section is BeamSectionProperties beam) new BeamBuilder().Build(sapModel, beam);
                        else if (section is SlabSectionProperties slab) new SlabBuilder().Build(sapModel, slab);
                    }
                }
                if (seismicParameters != null) new ResponseSpectrumBuilder(sapModel).DefineResponseSpectrumFunctions(seismicParameters);
                new FrameObjectsBuilder(sapModel, columnplacements, beamplacements, gridData).BuildAll();
                new RestraintBuilder(sapModel).supportJoints();
                new LoadCombinationBuilder(sapModel).DefineAllCombinations();
                new MassSourceBuilder(sapModel).DefineMassSources();
                sapModel.View.RefreshView(0,false);
            }
            catch (Exception ex)
            {
                throw new Exception("SAP2000'e aktarımda hata oldu: \n" + ex.Message);
            }
        }
    }
}
