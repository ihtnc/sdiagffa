using sdiagffa.application.models;

namespace sdiagffa.application.commands.findPersonCommand
{
    public class FindPersonCommandResponse
    {
        public int Count { get; set; }
        public int? NextPageId { get; set; }
        public int? PreviousPageId { get; set; }
        public IEnumerable<PersonDetails> Results { get; set; } = Enumerable.Empty<PersonDetails>();
    }
}