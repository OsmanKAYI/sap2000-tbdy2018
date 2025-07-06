using SAP2000.models.placements;
using SAP2000v1;
using System.Collections.Generic;
using System.Linq;

namespace SAP2000.services.builders.placements
{
    public class FrameObjectsBuilder
    {
        private readonly cSapModel _sapModel;
        private readonly List<ColumnPlacementInfo> _columnplacements;
        private readonly List<BeamPlacementInfo> _beamplacements;
        private readonly List<double> _storyZCoordinates;

        public FrameObjectsBuilder(cSapModel sapModel, List<ColumnPlacementInfo> columnplacements, List<BeamPlacementInfo> beamplacements, GridSystemData gridData)
        {
            _sapModel = sapModel;
            _columnplacements = columnplacements;
            _beamplacements = beamplacements;
            _storyZCoordinates = gridData.ZCoordinates;
        }

        public void BuildAll()
        {
            BuildColumns();
            BuildBeams();
        }

        private void BuildColumns()
        {
            if (_columnplacements == null || !_columnplacements.Any()) return;

            for (int i = 0; i < _storyZCoordinates.Count - 1; i++)
            {
                double z1 = _storyZCoordinates[i];
                double z2 = _storyZCoordinates[i + 1];

                foreach (var colPlacement in _columnplacements)
                {
                    string frameName = "";                    _sapModel.FrameObj.AddByCoord(
                        colPlacement.X, colPlacement.Y, z1,                        colPlacement.X, colPlacement.Y, z2,                        ref frameName,
                        colPlacement.SectionName,
                        colPlacement.ColumnName + $"_Z{i + 1}",                        "Global"
                    );
                }
            }
        }

        private void BuildBeams()
        {
            if (_beamplacements == null || !_beamplacements.Any()) return;

            foreach (double z in _storyZCoordinates.Where(z => z > 0))
            {
                foreach (var beamPlacement in _beamplacements)
                {
                    var startCol = _columnplacements.FirstOrDefault(c => c.ColumnName == beamPlacement.StartColumnName);
                    var endCol = _columnplacements.FirstOrDefault(c => c.ColumnName == beamPlacement.EndColumnName);

                    if (startCol != null && endCol != null)
                    {
                        string frameName = "";
                        _sapModel.FrameObj.AddByCoord(
                            startCol.X, startCol.Y, z,                            endCol.X, endCol.Y, z,                            ref frameName,
                            beamPlacement.SectionName,
                            beamPlacement.BeamName + $"_Z{z}",                            "Global"
                        );

                        if (!string.IsNullOrEmpty(frameName))
                        {
                            double[] doubles = new double[3];
                            _sapModel.FrameObj.SetInsertionPoint(frameName, 8, false, true, ref doubles, ref doubles);
                        }
                    }

                }
            }
        }
    }
}
