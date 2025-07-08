using SAP2000v1;
using SAP2000.services.builders.materials;
using SAP2000.models.sections;

namespace SAP2000.services.builders.sections
{
    public class SlabBuilder : ISap2000Builder<SlabSectionProperties>
    {
        public void build(cSapModel sapModel, SlabSectionProperties props)
        {
            double[] modifierForSlap = new double[] {
                0.25,
                0.25, // TBDY 2018'e uygun olacak şekilde üretilen tüm kesitlere rijitlik çarpanları atanır TABLO 4.2
                0.25,
                0.25, // TBDY 2018'e uygun olacak şekilde üretilen tüm kesitlere rijitlik çarpanları atanır TABLO 4.2
                0.25,
                0.25,
                1,
                1
            };
            sapModel.PropArea.SetModifiers(props.SectionName, ref modifierForSlap);
            sapModel.PropArea.SetShell(
                Name: props.SectionName,
                ShellType: 1,                
                MatProp: props.SlabMaterialName,
                MatAng: 0,
                Thickness: props.Thickness,
                Bending: 16,
                Color: 0,                
                Notes: "",                
                GUID: ""            
                );
        }
    }
}
