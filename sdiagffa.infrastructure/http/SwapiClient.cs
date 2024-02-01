using Flurl.Http;
using Mapster;
using sdiagffa.infrastructure.models;

namespace sdiagffa.infrastructure.http
{
    public interface ISwapiClient
    {
        Task<Person?> GetPerson(int id);
        Task<Film?> GetFilm(int id);
        Task<Planet?> GetPlanet(int id);
        Task<Page<Person>?> FindPerson(string value, int? page = null);
    }

    public class SwapiClient : ISwapiClient
    {
        readonly IFlurlClient _client;

        public SwapiClient(IFlurlClient client)
        {
            _client = client;
        }

        public async Task<Person?> GetPerson(int id)
        {
            var response = await _client
                .Request(ApiClientConfig.PEOPLE_ENDPOINT, id)
                .GetJsonAsync<PersonDto?>();

            return response?.Adapt<Person>();
        }

        public async Task<Film?> GetFilm(int id)
        {
            var response = await _client
                .Request(ApiClientConfig.FILMS_ENDPOINT, id)
                .GetJsonAsync<FilmDto?>();

            return response?.Adapt<Film>();
        }

        public async Task<Planet?> GetPlanet(int id)
        {
            var response = await _client
                .Request(ApiClientConfig.PLANETS_ENDPOINT, id)
                .GetJsonAsync<PlanetDto?>();

            return response?.Adapt<Planet>();
        }

        public async Task<Page<Person>?> FindPerson(string value, int? page = null)
        {
            var request = _client
                .Request(ApiClientConfig.PEOPLE_ENDPOINT)
                .SetQueryParam("search", value);

            if (page != null)
            {
                request.SetQueryParam("page", page);
            }

            var response = await request.GetJsonAsync<PageDto<PersonDto>?>();

            return response?.Adapt<Page<Person>>();
        }
    }
}