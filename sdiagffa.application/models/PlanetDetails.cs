namespace sdiagffa.application.models
{
    public class PlanetDetails
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Terrain { get; set; } = string.Empty;
        public string Climate { get; set; } = string.Empty;
        public string SurfaceWater { get; set; } = string.Empty;
        public string Gravity { get; set; } = string.Empty;
        public string Population { get; set; } = string.Empty;
        public string Diameter { get; set; } = string.Empty;
    }
}