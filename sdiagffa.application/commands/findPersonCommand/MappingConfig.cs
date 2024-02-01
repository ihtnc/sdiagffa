using Mapster;
using sdiagffa.application.models;
using sdiagffa.infrastructure.models;

namespace sdiagffa.application.commands.findPersonCommand
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config
                .NewConfig<Page<Person>, FindPersonCommandResponse>()
                .Map(dst => dst.Results, src => src.Results.Adapt<IEnumerable<PersonDetails>>());
        }
    }
}
