using Mapster;
using sdiagffa.infrastructure.models;
using System.Text.RegularExpressions;

namespace sdiagffa.infrastructure.http
{
    public class MappingConfig : IRegister
    {
        readonly Regex _personIdParser;
        readonly Regex _filmIdParser;
        readonly Regex _planetIdParser;
        readonly Regex _pageNumberParser;

        public MappingConfig()
        {
            _personIdParser = new Regex($"(?<=\\{ApiClientConfig.PEOPLE_ENDPOINT}\\/)\\d+(?=\\/?)");
            _filmIdParser = new Regex($"(?<=\\{ApiClientConfig.FILMS_ENDPOINT}\\/)\\d+(?=\\/?)");
            _planetIdParser = new Regex($"(?<=\\{ApiClientConfig.PLANETS_ENDPOINT}\\/)\\d+(?=\\/?)");
            _pageNumberParser = new Regex($"(?<=&page=)\\d+");
        }

        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<PersonDto, Person>()
                .Map(dst => dst.Id, src => GetIntValue(src.Url, _personIdParser))
                .Map(dst => dst.FilmIds, src => GetIntValues(src.Films, _filmIdParser))
                .Map(dst => dst.HomeworldId, src => GetNullableIntValue(src.Homeworld, _planetIdParser));

            config
                .NewConfig<FilmDto, Film>()
                .Map(dst => dst.Id, src => GetIntValue(src.Url, _filmIdParser))
                .Map(dst => dst.CharacterIds, src => GetIntValues(src.Characters, _personIdParser));

            config
                .NewConfig<PlanetDto, Planet>()
                .Map(dst => dst.Id, src => GetIntValue(src.Url, _planetIdParser))
                .Map(dst => dst.ResidentIds, src => GetIntValues(src.Residents, _personIdParser));

            config
                .NewConfig<PageDto<PersonDto>, Page<Person>>()
                .Map(dst => dst.PreviousPageId, src => GetNullableIntValue(src.Previous, _pageNumberParser))
                .Map(dst => dst.NextPageId, src => GetNullableIntValue(src.Next, _pageNumberParser));
        }

        private static IEnumerable<int> GetIntValues(IEnumerable<string> urls, Regex parser)
        {
            var parsed = urls.Select(url => GetNullableIntValue(url, parser));

            return parsed.Where(v => v != null).Select(v => v!.Value);
        }

        private static int GetIntValue(string value, Regex parser)
        {
            var match = parser.Match(value);
            return match.Success ? int.Parse(match.Value) : default;
        }

        private static int? GetNullableIntValue(string? value, Regex parser)
        {
            if (value == null) { return null; }

            var match = parser.Match(value);
            return match.Success ? int.Parse(match.Value) : null;
        }
    }
}
