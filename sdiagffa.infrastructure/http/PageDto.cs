namespace sdiagffa.infrastructure.http
{
    internal class PageDto<T>
    {
        public int Count { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }
        public IEnumerable<T> Results { get; set; } = Enumerable.Empty<T>();
    }
}