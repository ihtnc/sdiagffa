using sdiagffa.core.models;

namespace sdiagffa.core.services
{
    public interface IRelationshipManager
    {
        IRelationshipNode AddNode(int personId);
        IRelationshipNode AddNode(int personId, int relatedId, ConnectionType connectionType, int connectionId);
        IRelationshipNode? GetNode(int personId);
    }

    public class RelationshipManager : IRelationshipManager
    {
        readonly Dictionary<int, IRelationshipNode> _index = new();

        public IRelationshipNode? GetNode(int personId)
        {
            return _index.ContainsKey(personId)
                ? _index[personId] 
                : null;
        }

        public IRelationshipNode AddNode(int personId)
        {
            if (!_index.ContainsKey(personId))
            {
                _index.Add(
                    personId,
                    new RelationshipNode
                    {
                        PersonId = personId
                    }
                );
            }

            return _index[personId];
        }

        public IRelationshipNode AddNode(int personId, int relatedId, ConnectionType connectionType, int connectionId)
        {
            var person = AddNode(personId);
            if (personId == relatedId) { return person; }

            var related = AddNode(relatedId);

            if (related.Connections.ContainsKey(personId)) { return person; }
            if (connectionType == ConnectionType.None) { return person; }

            var relatedConnection = new RelationshipConnection (connectionType, connectionId, related);
            var personConnection = new RelationshipConnection (connectionType, connectionId, person);

            related.Connections.Add(personId, personConnection);
            person.Connections.Add(related.PersonId, relatedConnection);

            return person;
        }
    }
}
