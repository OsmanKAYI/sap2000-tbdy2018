using SAP2000v1;
using SAP2000.models;
using SAP2000.services.builders.materials;
using SAP2000.models.sections;

namespace SAP2000.services.builders.sections
{
    public class FrameSectionBuilder : ISap2000Builder<FrameSectionProperties>
    {
        public void Build(cSapModel sapModel, FrameSectionProperties properties)
        {
            sapModel.PropFrame.SetRectangle(properties.SectionName, properties.MaterialName, properties.Depth, properties.Width);
        }
    }
}
