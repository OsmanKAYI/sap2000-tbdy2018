using SAP2000v1;
using SAP2000.models;
using SAP2000.services.builders.materials;
using SAP2000.models.sections;

namespace SAP2000.services.builders.sections
{
    public class BeamBuilder : ISap2000Builder<BeamSectionProperties>
    {
        public void Build(cSapModel sapModel, BeamSectionProperties props)
        {
            sapModel.PropFrame.SetRectangle(props.SectionName, props.MaterialName, props.Depth, props.Width,3);

            double[] modifierForBeam = new double[] {
                1,
                1,
                1,
                1,
                0.35, // TBDY 2018'e uygun olacak şekilde üretilen tüm kesitlere rijitlik çarpanları atanır TABLO 4.2
                0.35,
                1,
                1
            };

            sapModel.PropFrame.SetModifiers(props.SectionName, ref modifierForBeam);

            sapModel.PropFrame.SetRebarBeam(
                Name: props.SectionName,
                MatPropLong: props.RebarMaterialName,
                MatPropConfine: props.RebarMaterialName,
                CoverTop: props.CoverTop,
                CoverBot: props.CoverBottom,
                TopLeftArea: 100,   
                TopRightArea: 100, 
                BotLeftArea: 100,  
                BotRightArea: 100  
            );
        }
    }
}
