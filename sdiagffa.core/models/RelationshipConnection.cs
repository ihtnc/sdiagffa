namespace sdiagffa.core.models
{
    public interface IRelationshipConnection
    {
        ConnectionType Connection { get; set; }
        int ConnectionId { get; set; }
        IRelationshipNode Target { get; set; }
    }

    public class RelationshipConnection : IRelationshipConnection
    {
        public ConnectionType Connection { get; set; }
        public int ConnectionId { get; set; }
        public IRelationshipNode Target { get; set; }

        public RelationshipConnection(ConnectionType connection, int connectionId, IRelationshipNode target)
        {
            Connection = connection;
            ConnectionId = connectionId;
            Target = target;
        }
    }
}
