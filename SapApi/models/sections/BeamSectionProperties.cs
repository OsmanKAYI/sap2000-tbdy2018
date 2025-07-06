namespace SAP2000.models.sections
{
    public class BeamSectionProperties : BaseFrameSectionProperties
    {
        public string RebarMaterialName { get; set; }
        public double CoverTop { get; set; }
        public double CoverBottom { get; set; }
    }
}
