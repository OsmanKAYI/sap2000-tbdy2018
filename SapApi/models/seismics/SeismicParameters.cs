namespace SAP2000.models.seismic
{
    // Spectrum verilerini tutulduğu sınıf
    public class SeismicParameters
    {
        public double Ss { get; set; }
        public double S1 { get; set; }
        public string SiteClass { get; set; }
        public double R { get; set; }
        public double D { get; set; }
        public double I { get; set; }
    }
}