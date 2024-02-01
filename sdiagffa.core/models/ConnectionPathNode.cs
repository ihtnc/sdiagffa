namespace sdiagffa.core.models
{
    public interface IConnectionPathNode
    {
        int PersonId { get; set; }
        string Name { get; set; }
        ConnectionType Connection { get; set; }
        int ConnectionId { get; set; }
        string ConnectionName { get; set; }
        IConnectionPathNode? Next { get; set; }
    }

    public class ConnectionPathNode: IConnectionPathNode
    {
        public int PersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ConnectionType Connection { get; set; } = ConnectionType.None;
        public int ConnectionId { get; set; }
        public string ConnectionName { get; set; } = string.Empty;
        public IConnectionPathNode? Next { get; set; }
    }
}
