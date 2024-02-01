using Mapster;
using MediatR;
using sdiagffa.application.models;
using sdiagffa.core.services;

namespace sdiagffa.application.commands.getFilmCommand
{
    public class GetFilmCommandHandler : IRequestHandler<GetFilmCommandRequest, FilmDetails?>
    {
        private readonly IStarwarsObjectProvider _provider;

        public GetFilmCommandHandler(IStarwarsObjectProvider provider)
        {
            _provider = provider;
        }

        public async Task<FilmDetails?> Handle(GetFilmCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _provider.GetFilm(request.Id);
            return response?.Adapt<FilmDetails?>();
        }
    }
}
