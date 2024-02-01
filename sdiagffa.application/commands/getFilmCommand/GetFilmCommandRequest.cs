using MediatR;
using sdiagffa.application.models;

namespace sdiagffa.application.commands.getFilmCommand
{
    public class GetFilmCommandRequest : IRequest<FilmDetails?>
    {
        public int Id { get; set; }
    }
}
