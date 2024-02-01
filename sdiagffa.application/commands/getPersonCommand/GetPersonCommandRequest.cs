using MediatR;
using sdiagffa.application.models;

namespace sdiagffa.application.commands.getPersonCommand
{
    public class GetPersonCommandRequest : IRequest<PersonDetails?>
    {
        public int Id { get; set; }
    }
}
