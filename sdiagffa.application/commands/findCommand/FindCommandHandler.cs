using MediatR;
using sdiagffa.core.models;
using sdiagffa.core.services;

namespace sdiagffa.application.commands.findCommand
{
    public class FindCommandHandler : IRequestHandler<FindCommandRequest, IConnectionPathNode?>
    {
        private readonly IConnectionCrawler _crawler;

        public FindCommandHandler(IConnectionCrawler crawler)
        {
            _crawler = crawler;
        }

        public async Task<IConnectionPathNode?> Handle(FindCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _crawler.FindShortestConnection(request.ReferencePersonId, request.TargetPersonId);
            return response;
        }
    }
}
