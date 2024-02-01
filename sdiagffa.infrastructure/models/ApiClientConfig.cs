namespace sdiagffa.infrastructure.models
{
    public class ApiClientConfig
    {
        public const string PEOPLE_ENDPOINT = "/people";
        public const string FILMS_ENDPOINT = "/films";
        public const string PLANETS_ENDPOINT = "/planets";

        public string BaseUrl { get; set; } = string.Empty;
    }
}
