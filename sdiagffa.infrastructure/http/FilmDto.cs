using System.Text.Json.Serialization;

namespace sdiagffa.infrastructure.http
{
    internal class FilmDto
    {
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }
        [JsonPropertyName("episode_id")]
        public int EpisodeId { get; set; }
        public string Director { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public IEnumerable<string> Characters { get; set; } = Enumerable.Empty<string>();
        public string Url { get; set; } = string.Empty;
    }
}