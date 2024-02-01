namespace sdiagffa.core.models
{
    public interface IRelationshipNode
    {
        int PersonId { get; set; }
        IDictionary<int, IRelationshipConnection> Connections { get; set; }
    }

    public class RelationshipNode : IRelationshipNode
    {
        public int PersonId { get; set; }
        public IDictionary<int, IRelationshipConnection> Connections { get; set; } = new Dictionary<int, IRelationshipConnection>();
    }
}
