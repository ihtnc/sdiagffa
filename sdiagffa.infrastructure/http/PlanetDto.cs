using System.Text.Json.Serialization;

namespace sdiagffa.infrastructure.http
{
    internal class PlanetDto
    {
        public string Name { get; set; } = string.Empty;
        public string Terrain { get; set; } = string.Empty;
        public string Climate { get; set; } = string.Empty;
        [JsonPropertyName("surface_water")]
        public string SurfaceWater { get; set; } = string.Empty;
        public string Gravity { get; set; } = string.Empty;
        public string Population { get; set; } = string.Empty;
        public string Diameter { get; set; } = string.Empty;
        public IEnumerable<string> Residents { get; set; } = Enumerable.Empty<string>();
        public string Url { get; set; } = string.Empty;
    }
}