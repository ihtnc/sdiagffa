namespace sdiagffa.infrastructure.models
{
    public class Page<T>
    {
        public int Count { get; set; }
        public int? NextPageId { get; set; }
        public int? PreviousPageId { get; set; }
        public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();
    }
}