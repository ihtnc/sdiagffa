using sdiagffa.infrastructure.http;
using sdiagffa.infrastructure.models;

namespace sdiagffa.core.services
{
    public interface IStarwarsObjectProvider
    {
        Task<Person?> GetPerson(int id);
        Task<Film?> GetFilm(int id);
        Task<Planet?> GetPlanet(int id);
        Task<IEnumerable<Film>?> GetFilmsByPerson(int id);
        Task<Planet?> GetHomeworld(int personId);
        Task<IEnumerable<Person>?> GetCharacters(int filmId);
        Task<IEnumerable<Person>?> GetResidents(int planetId);
        Task<Page<Person>?> FindPerson(string value, int? page = null);
    }

    public class StarwarsObjectProvider : IStarwarsObjectProvider
    {
        readonly ISwapiClient _client;

        readonly IDictionary<int, Person> _people;
        readonly IDictionary<int, Film> _films;
        readonly IDictionary<int, Planet> _planets;

        public StarwarsObjectProvider(ISwapiClient client)
        {
            _client = client;

            _people = new Dictionary<int, Person>();
            _films = new Dictionary<int, Film>();
            _planets = new Dictionary<int, Planet>();
        }

        public async Task<Person?> GetPerson(int id)
        {
            if (_people.ContainsKey(id)) { return _people[id]; }

            var person = await _client.GetPerson(id);
            if (person == null) { return null; }

            _people.Add(person.Id, person);

            return person;
        }

        public async Task<Film?> GetFilm(int id)
        {
            if (_films.ContainsKey(id)) { return _films[id]; }

            var film = await _client.GetFilm(id);
            if (film == null) { return null; }

            _films.Add(film.Id, film);

            return film;
        }

        public async Task<Planet?> GetPlanet(int id)
        {
            if (_planets.ContainsKey(id)) { return _planets[id]; }

            var planet = await _client.GetPlanet(id);
            if (planet == null) { return null; }

            _planets.Add(planet.Id, planet);

            return planet;
        }

        public async Task<IEnumerable<Film>?> GetFilmsByPerson(int personId)
        {
            var person = await GetPerson(personId);
            if (person == null) { return null; }

            var taskList = new List<Task<Film?>>();
            foreach (var filmId in person.FilmIds)
            {
                var getFilmTask = GetFilm(filmId);
                taskList.Add(getFilmTask);
            }

            Task.WaitAll(taskList.ToArray());

            var list = new List<Film>();
            foreach (var task in taskList)
            {
                var film = await task;
                if (film == null) { continue; }

                list.Add(film);
            }

            return list;
        }

        public async Task<Planet?> GetHomeworld(int personId)
        {
            var person = await GetPerson(personId);
            if (person == null || person.HomeworldId == null) { return null; }

            return await GetPlanet(person.HomeworldId.Value);
        }

        public async Task<IEnumerable<Person>?> GetCharacters(int filmId)
        {
            var film = await GetFilm(filmId);
            if (film == null) { return null; }

            var taskList = new List<Task<Person?>>();
            foreach (var personId in film.CharacterIds)
            {
                var getPersonTask = GetPerson(personId);
                taskList.Add(getPersonTask);
            }

            Task.WaitAll(taskList.ToArray());

            var list = new List<Person>();
            foreach (var task in taskList)
            {
                var person = await task;
                if (person == null) { continue; }

                list.Add(person);
            }

            return list;
        }

        public async Task<IEnumerable<Person>?> GetResidents(int planetId)
        {
            var planet = await GetPlanet(planetId);
            if (planet == null) { return null; }

            var taskList = new List<Task<Person?>>();
            foreach (var personId in planet.ResidentIds)
            {
                var getPersonTask = GetPerson(personId);
                taskList.Add(getPersonTask);
            }

            Task.WaitAll(taskList.ToArray());

            var list = new List<Person>();
            foreach (var task in taskList)
            {
                var person = await task;
                if (person == null) { continue; }

                list.Add(person);
            }

            return list;
        }

        public async Task<Page<Person>?> FindPerson(string value, int? page = null)
        {
            var response = await _client.FindPerson(value, page);
            return response;
        }
    }
}