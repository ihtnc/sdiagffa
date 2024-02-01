using sdiagffa.application.models;

namespace sdiagffa.host.models
{
    public class FindPersonResponse
    {
        public int Count { get; set; }
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }
        public IEnumerable<PersonDetails> Results { get; set; } = Enumerable.Empty<PersonDetails>();
    }
}