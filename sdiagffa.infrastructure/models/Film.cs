namespace sdiagffa.infrastructure.models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
        public int EpisodeId { get; set; }
        public string Director { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public IEnumerable<int> CharacterIds { get; set; } = Enumerable.Empty<int>();
    }
}