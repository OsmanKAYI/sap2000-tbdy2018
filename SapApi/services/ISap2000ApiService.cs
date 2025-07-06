using SAP2000.models.seismic;
using SAP2000.models.placements;
using System.Collections.Generic;
using SAP2000.models.sections;
using SAP2000.models.materials;

namespace SAP2000.services
{
    // SAP2000 API ile iletişim kurduğumuz servisi arayüzü
    public interface ISap2000ApiService
    {
        void CreateProjectInNewModel(
            GridSystemData gridData,
            SeismicParameters seismicParameters,            
            List<IMaterialProperties> materials,
            List<ISectionProperties> sections,
            List<ColumnPlacementInfo> columnplacements,
            List<BeamPlacementInfo> beamplacements,
            bool makeVisible);
    }
}
