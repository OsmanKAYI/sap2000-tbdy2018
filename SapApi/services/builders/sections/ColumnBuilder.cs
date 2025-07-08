using SAP2000v1;
using SAP2000.models;
using SAP2000.services.builders.materials;
using SAP2000.models.sections;
using System;

namespace SAP2000.services.builders.sections
{
    public class ColumnBuilder : ISap2000Builder<ColumnSectionProperties>
    {
        int ret = 0;
        public void build(cSapModel sapModel, ColumnSectionProperties props)
        {
            sapModel.PropFrame.SetRectangle(props.SectionName, props.MaterialName, props.Depth, props.Width);

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

            ret = sapModel.PropFrame.SetRebarColumn(
                Name: props.SectionName,
                MatPropLong: props.RebarMaterialName,
                MatPropConfine: props.RebarMaterialName,
                Pattern: 1,
                ConfineType: 1,
                Cover: props.ConcreteCover,
                NumberCBars: 4,
                NumberR3Bars: 3,
                NumberR2Bars: 3,
                RebarSize: "#9",
                TieSize: "#4",
                TieSpacingLongit: 150,
                Number2DirTieBars: 2,
                Number3DirTieBars: 2,
                ToBeDesigned: true
            );

            if (ret != 0)
            {
                throw new Exception($"Kolon Oluşturulamadı: SAP2000 API error {ret} while setting rebar for column section {props.SectionName}");
            }
        }
    }
}
