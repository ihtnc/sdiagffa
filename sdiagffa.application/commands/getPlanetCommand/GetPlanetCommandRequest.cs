using MediatR;
using sdiagffa.application.models;

namespace sdiagffa.application.commands.getPlanetCommand
{
    public class GetPlanetCommandRequest : IRequest<PlanetDetails?>
    {
        public int Id { get; set; }
    }
}
