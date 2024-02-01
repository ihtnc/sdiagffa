using sdiagffa.core.models;
using sdiagffa.infrastructure.models;

namespace sdiagffa.core.services
{
    public interface IConnectionCrawler
    {
        Task<IConnectionPathNode?> FindShortestConnection(int referencePersonId, int targetPersonId);
    }

    public class ConnectionCrawler : IConnectionCrawler
    {
        private readonly IStarwarsObjectProvider _provider;
        private readonly IRelationshipManager _manager;
        private readonly HashSet<int> _crawledPerson;

        public ConnectionCrawler(IStarwarsObjectProvider provider, IRelationshipManager manager)
        {
            _provider = provider;
            _manager = manager;
            _crawledPerson = new HashSet<int>();
        }

        public async Task<IConnectionPathNode?> FindShortestConnection(int referencePersonId, int targetPersonId)
        {
            var referenceNode = await TryFetchNode(referencePersonId);
            var targetNode = await TryFetchNode(targetPersonId);
            if (referenceNode == null || targetNode == null) { return null; }

            // this will contain a list of processed nodes and their stack level
            // this ensures that we don't process nodes that already exist on lower stacks
            //   but can still process nodes that exist on higher stacks
            // allowing nodes to be processed this way ensures we get the shortest path
            // however, this is not the optimised approach
            var processedNodes = new Dictionary<int, int>();

            var initialTree = new Stack<Stack<IRelationshipConnection>>();
            var rootLevel = new Stack<IRelationshipConnection>();
            var root = new RelationshipConnection(ConnectionType.None, 0, referenceNode);
            rootLevel.Push(root);
            initialTree.Push(rootLevel);
            processedNodes.Add(referenceNode.PersonId, 0);

            // traverse the connections from the referenceNode
            //   and get the first tree containing the targetNode
            // this tree represents the stack of all the nodes
            //   that are to be traversed at each depth
            //   with the last node being the targetNode
            // this effectively contains the path to take to reach the targetNode
            var tree = await GetConnectionTree(initialTree, targetNode, processedNodes);
            
            var smallestTree = Copy(tree);
            while(tree.Count > 1)
            {
                // since the last node of the tree is the targetNode,
                //   remove it and find a smaller tree that contains the targetNode
                tree.Pop();
                tree = await GetConnectionTree(tree, targetNode, processedNodes, tree.Count);

                // continue doing this until we can't find a smaller tree anymore
                if (tree.Any()) { smallestTree = Copy(tree); }

                // at the end of this process, we would have found the smallest tree
                //   which contains the shortest path to the targetNode
            }

            // get the first node at each depth in the tree
            //   to obtain the nodes traversed to reach the targetNode
            var path = BuildConnectionPath(smallestTree);

            // connect these nodes to form a contiguous connection
            var connection = await BuildRelationship(path);
            return connection;
        }

        private async Task<Stack<Stack<IRelationshipConnection>>> GetConnectionTree(Stack<Stack<IRelationshipConnection>> initialTree, IRelationshipNode targetNode, Dictionary<int, int> processedNodes, int? maxLevel = null)
        {
            var pathStack = initialTree;
            while (pathStack.Any())
            {
                // traverse each node in the current stack
                var currentLevel = pathStack.Peek();
                while (currentLevel.Any())
                {
                    var currentNode = currentLevel.Peek();
                    if (currentNode.Target.PersonId == targetNode.PersonId)
                    {
                        return pathStack;
                    }

                    // since we are looking for a smaller tree from the one provided,
                    //   we can't look any further than the depth of the provided tree
                    if (maxLevel != null && pathStack.Count >= maxLevel)
                    {
                        currentLevel.Pop();
                        continue;
                    }

                    var nextLevel = new Stack<IRelationshipConnection>();
                    var connections = await GetConnections(currentNode.Target);
                    if (connections.Any() == false)
                    {
                        currentLevel.Pop();
                        continue;
                    }

                    while (connections.Any())
                    {
                        var connection = connections.Pop();
                        var isProcessed = processedNodes.ContainsKey(connection.Target.PersonId);
                        var isOnLowerStack = isProcessed && processedNodes[connection.Target.PersonId] <= pathStack.Count;
                        
                        // we allow reprocessing of nodes if they were previously processed on a higher stack
                        //   to ensure we find the shortest connection
                        if (isOnLowerStack) { continue; }

                        nextLevel.Push(connection);

                        // we don't want the target node to be tagged as processed
                        //  otherwise, it won't be added as a connection on succeeding traversals
                        if (connection.Target.PersonId == targetNode.PersonId) { break; }

                        if (isProcessed)
                        {
                            // update processed node's stack level
                            //   to ignore any further processing of this node on higher stacks
                            processedNodes[connection.Target.PersonId] = pathStack.Count;
                        }
                        else
                        {
                            processedNodes.Add(connection.Target.PersonId, pathStack.Count);
                        }
                    }

                    // if there are no new connections for this node to add to the stack,
                    //   go to the next node in the current stack
                    if (nextLevel.Any() == false)
                    {
                        currentLevel.Pop();
                        continue;
                    }

                    // otherwise, traverse this new stack of connections
                    pathStack.Push(nextLevel);
                    currentLevel = nextLevel;
                }

                // to reach this means we've traversed all the nodes in the current stack,
                //   so traverse the nodes on the previous stack
                pathStack.Pop();
            }

            var noConnection = new Stack<Stack<IRelationshipConnection>>();
            return noConnection;
        }

        private static Stack<Stack<IRelationshipConnection>> Copy(Stack<Stack<IRelationshipConnection>> source)
        {
            var copy = new Stack<Stack<IRelationshipConnection>>(source);
            var reversed = new Stack<Stack<IRelationshipConnection>>();
            while(copy.Any())
            {
                var item = new Stack<IRelationshipConnection>(copy.Pop());
                var reversedItem = new Stack<IRelationshipConnection>(item);
                reversed.Push(reversedItem);
            }

            return reversed;
        }

        private static Queue<(int, ConnectionType, int)> BuildConnectionPath(Stack<Stack<IRelationshipConnection>> connectionTree)
        {
            var stack = new Stack<(int, ConnectionType, int)>();

            while (connectionTree.Any())
            {
                var currentLevel = connectionTree.Pop();
                var currentItem = currentLevel.Pop();
                stack.Push((
                    currentItem.Target.PersonId,
                    currentItem.Connection,
                    currentItem.ConnectionId
                ));
            }

            var path = new Queue<(int, ConnectionType, int)>();
            while (stack.Any()) { path.Enqueue(stack.Pop()); }
            return path;
        }

        private async Task<IConnectionPathNode?> BuildRelationship(Queue<(int, ConnectionType, int)> connectionPath)
        {
            IConnectionPathNode? rootNode = null;
            IConnectionPathNode? previousNode = null;

            while (connectionPath.Any())
            {
                var (personId, connection, connectionId) = connectionPath.Dequeue();

                var person = await _provider.GetPerson(personId);
                var connectionName = string.Empty;

                switch (connection)
                {
                    case ConnectionType.Film:
                        var film = await _provider.GetFilm(connectionId);
                        connectionName = film.Title;
                        break;

                    case ConnectionType.Homeworld:
                        var planet = await _provider.GetPlanet(connectionId);
                        connectionName = planet.Name;
                        break;
                }

                var currentNode = new ConnectionPathNode
                {
                    PersonId = personId,
                    Name = person.Name,
                    Connection = connection,
                    ConnectionId = connectionId,
                    ConnectionName = connectionName
                };

                if (rootNode == null) { rootNode = currentNode; }
                if (previousNode != null) { previousNode.Next = currentNode; }
                
                previousNode = currentNode;
            }

            return rootNode;
        }

        private async Task<IRelationshipNode?> TryFetchNode(int personId)
        {
            var node = _manager.GetNode(personId);
            if (node == null)
            {
                var person = await _provider.GetPerson(personId);
                if (person == null) { return null; }

                _manager.AddNode(personId);
            }

            return _manager.GetNode(personId);
        }

        private async Task PopulateFilmConnections(IRelationshipNode node)
        {
            var films = await _provider.GetFilmsByPerson(node.PersonId);
            if (films == null) { return; }

            var taskList = new Dictionary<int, Task<IEnumerable<Person>?>>();
            foreach (var film in films)
            {
                var task = _provider.GetCharacters(film.Id);
                taskList.Add(film.Id, task);
            }

            Task.WaitAll(taskList.Values.ToArray());

            var coStars = new List<(int, Person)>();
            foreach(var task in taskList)
            {
                var result = await task.Value;
                if (result == null) { continue; }

                coStars.AddRange(result.Select(r => (task.Key, r)));
            }

            foreach(var (connectionId, person) in coStars)
            {
                _manager.AddNode(node.PersonId, person.Id, ConnectionType.Film, connectionId);
            }
        }

        private async Task PopulateHomeworldConnections(IRelationshipNode node)
        {
            var homeworld = await _provider.GetHomeworld(node.PersonId);
            if (homeworld == null) { return; }

            var residents = await _provider.GetResidents(homeworld.Id);
            if (residents == null) {  return; }
            
            foreach (var person in residents)
            {
                _manager.AddNode(node.PersonId, person.Id, ConnectionType.Homeworld, homeworld.Id);
            }
        }

        private async Task<Stack<IRelationshipConnection>> GetConnections(IRelationshipNode source)
        {
            var connections = new Stack<IRelationshipConnection>();

            if (!_crawledPerson.Contains(source.PersonId))
            {
                var task1 = PopulateFilmConnections(source);
                var task2 = PopulateHomeworldConnections(source);

                Task.WaitAll(task1, task2);

                _crawledPerson.Add(source.PersonId);
            }

            if (source?.Connections?.Any() != true) { return connections; }

            foreach(var c in source.Connections)
            {
                connections.Push(c.Value);
            }

            return await Task.FromResult(connections);
        }
    }
}
