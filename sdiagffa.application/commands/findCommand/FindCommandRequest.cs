using MediatR;
using sdiagffa.core.models;

namespace sdiagffa.application.commands.findCommand
{
    public class FindCommandRequest : IRequest<IConnectionPathNode?>
    {
        public int ReferencePersonId { get; set; }
        public int TargetPersonId { get; set; }
    }
}
