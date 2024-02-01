using sdiagffa.infrastructure.models;

namespace sdiagffa.core.models
{
    public class CoreConfig
    {
        public ApiClientConfig ApiClient { get; set; } = new();
    }
}
