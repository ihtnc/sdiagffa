using MediatR;

namespace sdiagffa.application.commands.findPersonCommand
{
    public class FindPersonCommandRequest : IRequest<FindPersonCommandResponse?>
    {
        public string Search { get; set; } = string.Empty;
        public int? Page { get; set; }
    }
}
