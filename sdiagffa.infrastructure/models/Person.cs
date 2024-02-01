namespace sdiagffa.infrastructure.models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string BirthYear { get; set; } = string.Empty;
        public string Height { get; set; } = string.Empty;
        public string Mass { get; set; } = string.Empty;
        public string SkinColor { get; set; } = string.Empty;
        public string EyeColor { get; set; } = string.Empty;
        public int? HomeworldId { get; set; }
        public IEnumerable<int> FilmIds { get; set; } = Enumerable.Empty<int>();
    }
}