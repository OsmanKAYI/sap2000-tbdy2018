using SAP2000v1;
using SAP2000.models;
using SAP2000.services.builders.materials;
using SAP2000.models.sections;

namespace SAP2000.services.builders.sections
{
    public class ColumnBuilder : ISap2000Builder<ColumnSectionProperties>
    {
        public void Build(cSapModel sapModel, ColumnSectionProperties props)
        {
            sapModel.PropFrame.SetRectangle(props.SectionName, props.MaterialName, props.Depth, props.Width, -2);

            double[] modifierForColumn = new double[] {
                1,
                1,
                1,
                1,
                0.7, // TBDY 2018'e uygun olacak şekilde üretilen tüm kesitlere rijitlik çarpanları atanır TABLO 4.2
                0.7,
                1,
                1
            };

            sapModel.PropFrame.SetModifiers(props.SectionName, ref modifierForColumn);

            sapModel.PropFrame.SetRebarColumn(
                Name: props.SectionName,
                MatPropLong: props.RebarMaterialName,
                MatPropConfine: props.RebarMaterialName,
                Pattern: 1,                
                ConfineType: 1,                
                Cover: props.ConcreteCover,
                NumberCBars: 4,
                NumberR3Bars: 3,
                NumberR2Bars: 3,
                RebarSize: "16",                
                TieSize: "8",                
                TieSpacingLongit: 150,
                Number2DirTieBars: 2,
                Number3DirTieBars: 2,
                ToBeDesigned: true
            );
        }
    }
}
