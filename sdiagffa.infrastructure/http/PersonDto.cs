using System.Text.Json.Serialization;

namespace sdiagffa.infrastructure.http
{
    internal class PersonDto
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        [JsonPropertyName("birth_year")]
        public string BirthYear { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
        public string Mass { get; set; } = string.Empty;
        [JsonPropertyName("skin_color")]
        public string SkinColor { get; set; } = string.Empty;
        [JsonPropertyName("eye_color")]
        public string EyeColor { get; set; } = string.Empty;
        public string Homeworld { get; set; } = string.Empty;
        public IEnumerable<string> Films { get; set; } = Enumerable.Empty<string>();
        public string Url { get; set; } = string.Empty;
    }
}