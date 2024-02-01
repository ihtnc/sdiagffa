using Flurl.Util;
using Mapster;
using MediatR;
using sdiagffa.core.services;

namespace sdiagffa.application.commands.findPersonCommand
{
    public class FindPersonCommandHandler : IRequestHandler<FindPersonCommandRequest, FindPersonCommandResponse?>
    {
        private readonly IStarwarsObjectProvider _provider;

        public FindPersonCommandHandler(IStarwarsObjectProvider provider)
        {
            _provider = provider;
        }

        public async Task<FindPersonCommandResponse?> Handle(FindPersonCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _provider.FindPerson(request.Search, request.Page);
            return response?.Adapt<FindPersonCommandResponse?>();
        }
    }
}
