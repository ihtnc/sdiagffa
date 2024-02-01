using AutoFixture;
using FluentAssertions;
using Flurl.Http;
using MediatR;
using NSubstitute;
using sdiagffa.application.commands.findPersonCommand;
using sdiagffa.host.models;
using sdiagffa.test.host.utilities;
using Xunit;

namespace sdiagffa.test.host
{
    public class SixDegreesControllerTests : BaseControllerTests
    {
        readonly IMediator _mediator;
        readonly IFixture _fixture;

        public SixDegreesControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData("abc", 1, "/details/people?search=abc&page=1")]
        [InlineData("abc", null, null)]
        public async void FindPerson_Should_Populate_NextPageUrl_From_NextPageId(string search, int? nextPageIdResponse, string? expectedUrl)
        {
            var request = _fixture.Build<FindPersonCommandRequest>()
                .With(r => r.Search, search)
                .Create();
            var response = _fixture.Build<FindPersonCommandResponse>()
                .With(r => r.NextPageId, nextPageIdResponse)
                .Create();

            _mediator.Send(Arg.Any<FindPersonCommandRequest>()).Returns(response);

            var client = CreateClient(services =>
                services.SwapTransient(_ => _mediator)
            );

            var actual = await client.Request("/details/people")
                .AppendQueryParam("search", request.Search)
                .GetJsonAsync<FindPersonResponse>();

            actual!.NextPageUrl.Should().Be(expectedUrl);
        }

        [Theory]
        [InlineData("abc", 1, "/details/people?search=abc&page=1")]
        [InlineData("abc", null, null)]
        public async void FindPerson_Should_Populate_PreviousPageUrl_From_PreviousPageId(string search, int? previousPageIdResponse, string? expectedUrl)
        {
            var request = _fixture.Build<FindPersonCommandRequest>()
                .With(r => r.Search, search)
                .Create();
            var response = _fixture.Build<FindPersonCommandResponse>()
                .With(r => r.PreviousPageId, previousPageIdResponse)
                .Create();

            _mediator.Send(Arg.Any<FindPersonCommandRequest>()).Returns(response);

            var client = CreateClient(services =>
                services.SwapTransient(_ => _mediator)
            );

            var actual = await client.Request("/details/people")
                .AppendQueryParam("search", request.Search)
                .GetJsonAsync<FindPersonResponse>();

            actual!.PreviousPageUrl.Should().Be(expectedUrl);
        }

        [Fact]
        public async void FindPerson_Should_Return_FindPersonResponse()
        {
            var request = _fixture.Create<FindPersonCommandRequest>();
            var response = _fixture.Create<FindPersonCommandResponse>();

            _mediator.Send(Arg.Any<FindPersonCommandRequest>()).Returns(response);

            var client = CreateClient(services =>
                services.SwapTransient(_ => _mediator)
            );

            var actual = await client.Request("/details/people")
                .AppendQueryParam("search", request.Search)
                .GetJsonAsync<FindPersonResponse>();

            actual!.Count.Should().Be(response.Count);
            actual.Results.Should().HaveSameCount(response.Results);
        }
    }
}
