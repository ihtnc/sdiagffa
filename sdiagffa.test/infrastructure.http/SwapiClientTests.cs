using AutoFixture;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using Mapster;
using sdiagffa.infrastructure.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace sdiagffa.test.infrastructure.http
{
    public class SwapiClientTests
    {
        readonly ISwapiClient _client;

        readonly HttpTest _server;
        readonly Fixture _fixture;

        const string TEST_BASE_URL = "https://localhost";

        public SwapiClientTests()
        {
            _server = new HttpTest();
            _fixture = new Fixture();

            var flurlClient = new FlurlClient(TEST_BASE_URL);
            _client = new SwapiClient(flurlClient);

            TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfig).Assembly);
        }

        [Fact]
        public async void GetPerson_Should_Call_Correct_EndPoint()
        {
            var id = _fixture.Create<int>();
            var expectedUrl = $"{TEST_BASE_URL}/people/{id}";

            var person = _fixture.Create<PersonDto>();
            _server
                .ForCallsTo(expectedUrl)
                .RespondWithJson(person, 200);

            await _client.GetPerson(id);

            _server.ShouldHaveCalled(expectedUrl)
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async void GetPerson_Should_Return_Correctly()
        {
            var person = _fixture.Create<PersonDto>();
            _server.RespondWithJson(person, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response!.Name.Should().Be(person.Name);
        }

        [Theory]
        [InlineData("/people/1", 1)]
        [InlineData("/people/abc", 0)]
        [InlineData("2", 0)]
        public async void GetPerson_Should_Populate_Id_From_Url(string url, int expectedId)
        {
            var person = _fixture.Build<PersonDto>()
                .With(p => p.Url, url)
                .Create();
            
            _server.RespondWithJson(person, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response!.Id.Should().Be(expectedId);
        }

        [Fact]
        public async void GetPerson_Should_Parse_FilmIds()
        {
            var filmIds = new [] { 1, 2, 3 };
            var filmUrls = filmIds.Select(id => $"{TEST_BASE_URL}/films/{id}");

            var person = _fixture.Build<PersonDto>()
                .With(p => p.Films, filmUrls)
                .Create();

            _server.RespondWithJson(person, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response!.FilmIds.Should().HaveSameCount(filmIds);
            response.FilmIds.Should().BeEquivalentTo(filmIds);
        }

        [Theory]
        [InlineData($"{TEST_BASE_URL}/films/abc")]
        [InlineData("2")]
        [InlineData("")]
        [InlineData("   ")]
        public async void GetPerson_Should_Ignore_Invalid_FilmIds(string invalidFilmUrl)
        {
            var filmIds = new[] { 1, 3 };
            var filmUrls = new[]
            {
                $"{TEST_BASE_URL}/films/1",
                invalidFilmUrl,
                $"{TEST_BASE_URL}/films/3",
            };

            var person = _fixture.Build<PersonDto>()
                .With(p => p.Films, filmUrls)
                .Create();

            _server.RespondWithJson(person, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response!.FilmIds.Should().HaveSameCount(filmIds);
            response.FilmIds.Should().BeEquivalentTo(filmIds);
        }

        [Fact]
        public async void GetPerson_Should_Handle_Empty_Films()
        {
            var person = _fixture.Build<PersonDto>()
                .With(p => p.Films, Array.Empty<string>())
                .Create();

            _server.RespondWithJson(person, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response!.FilmIds.Should().BeEmpty();
        }

        [Theory]
        [InlineData("/planets/1", 1)]
        [InlineData("/planets/abc", null)]
        [InlineData("2", null)]
        [InlineData("", null)]
        [InlineData("   ", null)]
        public async void GetPerson_Should_Parse_Homeworld(string planetUrl, int? expectedId)
        {
            var person = _fixture.Build<PersonDto>()
                .With(p => p.Homeworld, planetUrl)
                .Create();

            _server.RespondWithJson(person, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response!.HomeworldId.Should().Be(expectedId);
        }

        [Fact]
        public async void GetPerson_Should_Handle_Null_Response()
        {
            _server.RespondWithJson(null, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPerson(id);

            response.Should().BeNull();
        }

        [Fact]
        public async void GetFilm_Should_Call_Correct_EndPoint()
        {
            var id = _fixture.Create<int>();
            var expectedUrl = $"{TEST_BASE_URL}/films/{id}";

            var film = _fixture.Create<FilmDto>();
            _server
                .ForCallsTo(expectedUrl)
                .RespondWithJson(film, 200);

            await _client.GetFilm(id);

            _server.ShouldHaveCalled(expectedUrl)
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async void GetFilm_Should_Return_Correctly()
        {
            var film = _fixture.Create<FilmDto>();
            _server.RespondWithJson(film, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetFilm(id);

            response!.Title.Should().Be(film.Title);
        }

        [Theory]
        [InlineData("/films/1", 1)]
        [InlineData("/films/abc", 0)]
        [InlineData("2", 0)]
        public async void GetFilm_Should_Populate_Id_From_Url(string url, int expectedId)
        {
            var film = _fixture.Build<FilmDto>()
                .With(p => p.Url, url)
                .Create();

            _server.RespondWithJson(film, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetFilm(id);

            response!.Id.Should().Be(expectedId);
        }

        [Fact]
        public async void GetFilm_Should_Parse_PersonIds()
        {
            var characterIds = new[] { 1, 2, 3 };
            var characterUrls = characterIds.Select(id => $"{TEST_BASE_URL}/people/{id}");

            var film = _fixture.Build<FilmDto>()
                .With(p => p.Characters, characterUrls)
                .Create();

            _server.RespondWithJson(film, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetFilm(id);

            response!.CharacterIds.Should().HaveSameCount(characterIds);
            response.CharacterIds.Should().BeEquivalentTo(characterIds);
        }

        [Theory]
        [InlineData($"{TEST_BASE_URL}/people/abc")]
        [InlineData("2")]
        [InlineData("")]
        [InlineData("   ")]
        public async void GetFilm_Should_Ignore_Invalid_PersonIds(string invalidPersonUrl)
        {
            var personIds = new[] { 1, 3 };
            var personUrls = new[]
            {
                $"{TEST_BASE_URL}/people/1",
                invalidPersonUrl,
                $"{TEST_BASE_URL}/people/3",
            };

            var film = _fixture.Build<FilmDto>()
                .With(p => p.Characters, personUrls)
                .Create();

            _server.RespondWithJson(film, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetFilm(id);

            response!.CharacterIds.Should().HaveSameCount(personIds);
            response.CharacterIds.Should().BeEquivalentTo(personIds);
        }

        [Fact]
        public async void GetFilm_Should_Handle_Empty_Characters()
        {
            var film = _fixture.Build<FilmDto>()
                .With(p => p.Characters, Array.Empty<string>())
                .Create();

            _server.RespondWithJson(film, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetFilm(id);

            response!.CharacterIds.Should().BeEmpty();
        }

        [Fact]
        public async void GetFilm_Should_Handle_Null_Response()
        {
            _server.RespondWithJson(null, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetFilm(id);

            response.Should().BeNull();
        }

        [Fact]
        public async void GetPlanet_Should_Call_Correct_EndPoint()
        {
            var id = _fixture.Create<int>();
            var expectedUrl = $"{TEST_BASE_URL}/planets/{id}";

            var planet = _fixture.Create<PlanetDto>();
            _server
                .ForCallsTo(expectedUrl)
                .RespondWithJson(planet, 200);

            await _client.GetPlanet(id);

            _server.ShouldHaveCalled(expectedUrl)
                .WithVerb(HttpMethod.Get);
        }

        [Fact]
        public async void GetPlanet_Should_Return_Correctly()
        {
            var planet = _fixture.Create<PlanetDto>();
            _server.RespondWithJson(planet, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPlanet(id);

            response!.Name.Should().Be(planet.Name);
        }

        [Theory]
        [InlineData("/planets/1", 1)]
        [InlineData("/planet/abc", 0)]
        [InlineData("2", 0)]
        public async void GetPlanet_Should_Populate_Id_From_Url(string url, int expectedId)
        {
            var planet = _fixture.Build<PlanetDto>()
                .With(p => p.Url, url)
                .Create();

            _server.RespondWithJson(planet, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPlanet(id);

            response!.Id.Should().Be(expectedId);
        }

        [Fact]
        public async void GetPlanet_Should_Parse_PersonIds()
        {
            var residentIds = new[] { 1, 2, 3 };
            var residentUrls = residentIds.Select(id => $"{TEST_BASE_URL}/people/{id}");

            var planet = _fixture.Build<PlanetDto>()
                .With(p => p.Residents, residentUrls)
                .Create();

            _server.RespondWithJson(planet, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPlanet(id);

            response!.ResidentIds.Should().HaveSameCount(residentIds);
            response.ResidentIds.Should().BeEquivalentTo(residentIds);
        }

        [Theory]
        [InlineData($"{TEST_BASE_URL}/people/abc")]
        [InlineData("2")]
        [InlineData("")]
        [InlineData("   ")]
        public async void GetPlanet_Should_Ignore_Invalid_PersonIds(string invalidPersonUrl)
        {
            var personIds = new[] { 1, 3 };
            var personUrls = new[]
            {
                $"{TEST_BASE_URL}/people/1",
                invalidPersonUrl,
                $"{TEST_BASE_URL}/people/3",
            };

            var planet = _fixture.Build<PlanetDto>()
                .With(p => p.Residents, personUrls)
                .Create();

            _server.RespondWithJson(planet, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPlanet(id);

            response!.ResidentIds.Should().HaveSameCount(personIds);
            response.ResidentIds.Should().BeEquivalentTo(personIds);
        }

        [Fact]
        public async void GetPlanet_Should_Handle_Empty_Characters()
        {
            var planet = _fixture.Build<PlanetDto>()
                .With(p => p.Residents, Array.Empty<string>())
                .Create();

            _server.RespondWithJson(planet, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPlanet(id);

            response!.ResidentIds.Should().BeEmpty();
        }

        [Fact]
        public async void GetPlanet_Should_Handle_Null_Response()
        {
            _server.RespondWithJson(null, 200);

            var id = _fixture.Create<int>();
            var response = await _client.GetPlanet(id);

            response.Should().BeNull();
        }

        [Fact]
        public async void FindPerson_Should_Call_Correct_EndPoint()
        {
            var searchQuery = _fixture.Create<string>();
            var pageQuery = _fixture.Create<int>();
            var expectedUrl = $"{TEST_BASE_URL}/people";

            var results = _fixture.Create<PageDto<PersonDto>>();
            _server
                .ForCallsTo(expectedUrl)
                .WithQueryParam("search", searchQuery)
                .WithQueryParam("page", pageQuery)
                .RespondWithJson(results, 200);

            await _client.FindPerson(searchQuery, pageQuery);

            _server.ShouldHaveCalled(expectedUrl)
                .WithVerb(HttpMethod.Get)
                .WithQueryParam("search", searchQuery)
                .WithQueryParam("page", pageQuery);
        }

        [Fact]
        public async void FindPerson_Should_Return_Correctly()
        {
            var results = _fixture.Create<PageDto<PersonDto>>();
            _server.RespondWithJson(results, 200);

            var search = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var response = await _client.FindPerson(search, page);

            response!.Count.Should().Be(results.Count);
        }

        [Theory]
        [InlineData("&page=1", 1)]
        [InlineData("page=1", null)]
        [InlineData("&page=abc", null)]
        [InlineData("2", null)]
        public async void FindPerson_Should_Populate_PreviousPageId_From_Previous(string url, int? expectedId)
        {
            var results = _fixture.Build<PageDto<PersonDto>>()
                .With(p => p.Previous, url)
                .Create();

            _server.RespondWithJson(results, 200);

            var search = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var response = await _client.FindPerson(search, page);

            response!.PreviousPageId.Should().Be(expectedId);
        }

        [Theory]
        [InlineData("&page=1", 1)]
        [InlineData("page=1", null)]
        [InlineData("&page=abc", null)]
        [InlineData("2", null)]
        public async void FindPerson_Should_Populate_NextPageId_From_Next(string url, int? expectedId)
        {
            var results = _fixture.Build<PageDto<PersonDto>>()
                .With(p => p.Next, url)
                .Create();

            _server.RespondWithJson(results, 200);

            var search = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var response = await _client.FindPerson(search, page);

            response!.NextPageId.Should().Be(expectedId);
        }

        [Fact]
        public async void FindPerson_Should_Return_Person_Results()
        {
            var characterIds = new[] { 1, 2, 3 };

            var list = new List<PersonDto>();
            foreach(var id in characterIds)
            {
                list.Add(_fixture.Build<PersonDto>()
                    .With(p => p.Url, $"{TEST_BASE_URL}/people/{id}")
                    .Create());
            }

            var results = _fixture.Build<PageDto<PersonDto>>()
                .With(p => p.Results, list)
                .Create();

            _server.RespondWithJson(results, 200);

            var search = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var response = await _client.FindPerson(search, page);

            response!.Results.Should().HaveSameCount(characterIds);

            foreach (var id in characterIds)
            {
                var expected = results.Results.First(p => p.Url.EndsWith($"/{id}"));
                var actual = response.Results.First(p => p.Id == id);

                actual.Name.Should().Be(expected.Name);
            }
        }

        [Fact]
        public async void FindPerson_Should_Handle_Empty_Results()
        {
            var results = _fixture.Build<PageDto<PersonDto>>()
                .With(p => p.Results, Array.Empty<PersonDto>())
                .Create();

            _server.RespondWithJson(results, 200);

            var search = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var response = await _client.FindPerson(search, page);

            response!.Results.Should().BeEmpty();
        }

        [Fact]
        public async void FindPerson_Should_Handle_Null_Response()
        {
            _server.RespondWithJson(null, 200);

            var search = _fixture.Create<string>();
            var page = _fixture.Create<int>();
            var response = await _client.FindPerson(search, page);

            response.Should().BeNull();
        }
    }
}
