using Mapster;
using MediatR;
using sdiagffa.application.models;
using sdiagffa.core.services;

namespace sdiagffa.application.commands.getPlanetCommand
{
    public class GetPlanetCommandHandler : IRequestHandler<GetPlanetCommandRequest, PlanetDetails?>
    {
        private readonly IStarwarsObjectProvider _provider;

        public GetPlanetCommandHandler(IStarwarsObjectProvider provider)
        {
            _provider = provider;
        }

        public async Task<PlanetDetails?> Handle(GetPlanetCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _provider.GetPlanet(request.Id);
            return response?.Adapt<PlanetDetails?>();
        }
    }
}
