using Mapster;
using MediatR;
using sdiagffa.application.models;
using sdiagffa.core.services;

namespace sdiagffa.application.commands.getPersonCommand
{
    public class GetPersonCommandHandler : IRequestHandler<GetPersonCommandRequest, PersonDetails?>
    {
        private readonly IStarwarsObjectProvider _provider;

        public GetPersonCommandHandler(IStarwarsObjectProvider provider)
        {
            _provider = provider;
        }

        public async Task<PersonDetails?> Handle(GetPersonCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _provider.GetPerson(request.Id);
            return response?.Adapt<PersonDetails?>();
        }
    }
}
