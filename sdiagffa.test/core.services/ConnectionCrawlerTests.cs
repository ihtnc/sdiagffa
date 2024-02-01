using Xunit;
using sdiagffa.core.services;
using AutoFixture;
using FluentAssertions;
using sdiagffa.core.models;
using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using sdiagffa.infrastructure.models;
using NSubstitute.ClearExtensions;

namespace sdiagffa.test.core.services
{
    public class ConnectionCrawlerTests
    {
        readonly IStarwarsObjectProvider _provider;
        readonly IRelationshipManager _manager;
        readonly IConnectionCrawler _crawler;
        readonly Fixture _fixture;

        public ConnectionCrawlerTests()
        {
            _provider = Substitute.For<IStarwarsObjectProvider>();
            _manager = new RelationshipManager();
            _crawler = new ConnectionCrawler(_provider, _manager);
            _fixture = new Fixture();

            _provider.GetPerson(Arg.Any<int>()).Returns(new Person());
            _provider.GetFilm(Arg.Any<int>()).Returns(new Film());
            _provider.GetPlanet(Arg.Any<int>()).Returns(new Planet());
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Direct_Connection()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var node1 = _manager.AddNode(id1);
            var node2 = _manager.AddNode(id2);
            _manager.AddNode(id1, id2, connectionType, connectionId);

            // act
            var rootNode = await _crawler.FindShortestConnection(node1.PersonId, node2.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int)>();
            expected.Enqueue((id1, ConnectionType.None, 0));
            expected.Enqueue((id2, connectionType, connectionId));

            ValidateConnections(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Direct_Connection_Reversed()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var node1 = _manager.AddNode(id1);
            var node2 = _manager.AddNode(id2);
            _manager.AddNode(id1, id2, connectionType, connectionId);

            // act
            var rootNode = await _crawler.FindShortestConnection(node2.PersonId, node1.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int)>();
            expected.Enqueue((id2, ConnectionType.None, 0));
            expected.Enqueue((id1, connectionType, connectionId));

            ValidateConnections(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Add_Person_Name()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id1Name = _fixture.Create<string>();
            var id2 = unchecked(id1 + 1);
            var id2Name = _fixture.Create<string>();
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var node1 = _manager.AddNode(id1);
            var node2 = _manager.AddNode(id2);
            _manager.AddNode(id1, id2, connectionType, connectionId);

            _provider.GetPerson(id1).Returns(new Person { Id = id1, Name = id1Name });
            _provider.GetPerson(id2).Returns(new Person { Id = id2, Name = id2Name });

            // act
            var rootNode = await _crawler.FindShortestConnection(node1.PersonId, node2.PersonId);

            // assert
            var expected = new Queue<(int, string, ConnectionType, int)>();
            expected.Enqueue((id1, id1Name, ConnectionType.None, 0));
            expected.Enqueue((id2, id2Name, connectionType, connectionId));

            ValidateConnectionsWithPersonName(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Add_Film_Title()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var connectionName = _fixture.Create<string>();
            var node1 = _manager.AddNode(id1);
            var node2 = _manager.AddNode(id2);
            _manager.AddNode(id1, id2, connectionType, connectionId);

            _provider.GetFilm(connectionId).Returns(new Film { Id = connectionId, Title = connectionName });

            // act
            var rootNode = await _crawler.FindShortestConnection(node1.PersonId, node2.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int, string)>();
            expected.Enqueue((id1, ConnectionType.None, 0, string.Empty));
            expected.Enqueue((id2, connectionType, connectionId, connectionName));

            ValidateConnectionsWithConnectionName(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Add_Planet_Name()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var connectionType = ConnectionType.Homeworld;
            var connectionId = _fixture.Create<int>();
            var connectionName = _fixture.Create<string>();
            var node1 = _manager.AddNode(id1);
            var node2 = _manager.AddNode(id2);
            _manager.AddNode(id1, id2, connectionType, connectionId);

            _provider.GetPlanet(connectionId).Returns(new Planet { Id = connectionId, Name = connectionName });

            // act
            var rootNode = await _crawler.FindShortestConnection(node1.PersonId, node2.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int, string)>();
            expected.Enqueue((id1, ConnectionType.None, 0, string.Empty));
            expected.Enqueue((id2, connectionType, connectionId, connectionName));

            ValidateConnectionsWithConnectionName(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Single_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var node = _manager.AddNode(id);
            
            // act
            var rootNode = await _crawler.FindShortestConnection(node.PersonId, node.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int)>();
            expected.Enqueue((id, ConnectionType.None, 0));

            ValidateConnections(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Contiguous_Nodes()
        {
            // arrange
            var nodes = new Queue<(int, ConnectionType, int)>();

            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();

            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var ids = CreateSimpleConnection(referenceId, numberOfNodes, connection, connectionId);
            var currentId = referenceId;
            while(ids.Any())
            {
                currentId = ids.Dequeue();
                nodes.Enqueue((
                    currentId,
                    currentId == referenceId ? ConnectionType.None : connection,
                    currentId == referenceId ? 0 : connectionId
                ));
            }

            var targetId = unchecked(currentId + 1);
            _manager.AddNode(targetId, currentId, connection, connectionId);
            nodes.Enqueue((
                targetId,
                connection,
                connectionId
            ));

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Contiguous_Nodes_Reversed()
        {
            // arrange
            var nodes = new Stack<(int, ConnectionType, int)>();

            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();

            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var ids = CreateSimpleConnection(referenceId, numberOfNodes, connection, connectionId);
            var currentId = referenceId;
            while (ids.Any())
            {
                currentId = ids.Dequeue();
                nodes.Push((
                    currentId,
                    connection,
                    connectionId
                ));
            }

            var targetId = unchecked(currentId + 1);
            _manager.AddNode(targetId, currentId, connection, connectionId);
            nodes.Push((
                targetId,
                ConnectionType.None,
                0
            ));

            // act
            var referenceNode = _manager.GetNode(targetId)!;
            var targetNode = _manager.GetNode(referenceId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var reversed = new Queue<(int, ConnectionType, int)>();
            while(nodes.Any()) { reversed.Enqueue(nodes.Pop()); }
            ValidateConnections(rootNode, reversed);
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Disconnected_Nodes()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var node1 = _manager.AddNode(id1);
            var node2 = _manager.AddNode(id2);

            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var parentId = unchecked(id2 + 1);
            for (var i = 0; i < numberOfNodes; i++)
            {
                var newId = parentId + 1;
                _manager.AddNode(newId, parentId, connectionType, connectionId);
                parentId = newId;
            }

            // act
            var rootNode = await _crawler.FindShortestConnection(node1.PersonId, node2.PersonId);

            // assert
            rootNode.Should().BeNull();
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Disconnected_Reference_Node()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var referenceNode = _manager.AddNode(referenceId);

            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var parentId = unchecked(referenceId + 1);
            var created = CreateSimpleConnection(parentId, numberOfNodes, connectionType, connectionId);

            var lastId = 0;
            while(created.Any()) { lastId = created.Dequeue(); }

            // act
            var lastNode = _manager.GetNode(lastId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, lastNode.PersonId);

            // assert
            rootNode.Should().BeNull();
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Disconnected_Target_Node()
        {
            // arrange
            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();
            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var created = CreateSimpleConnection(referenceId, numberOfNodes, connection, connectionId);

            var lastId = 0;
            while (created.Any()) { lastId = created.Dequeue(); }

            var targetId = unchecked(lastId + 1);
            _manager.AddNode(targetId);

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            rootNode.Should().BeNull();
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Shortest_Path()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var targetId = unchecked(referenceId + 1);

            var longConnection = ConnectionType.Film;
            var longConnectionId = _fixture.Create<int>();
            var longPathId = unchecked(targetId + 1);
            var longNumberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(5, 10);
            var longCreated = CreateSimpleConnection(longPathId, longNumberOfNodes, longConnection, longConnectionId);
            var longLastId = 0;
            while (longCreated.Any()) { longLastId = longCreated.Dequeue(); }

            _manager.AddNode(longPathId, referenceId, longConnection, longConnectionId);
            _manager.AddNode(longLastId, targetId, longConnection, longConnectionId);

            var shortConnection = ConnectionType.Homeworld;
            var shortConnectionId = unchecked(longConnectionId + 1);
            var shortPathId = unchecked(longLastId + 1);
            var shortNumberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 3);
            var shortCreated = CreateSimpleConnection(shortPathId, shortNumberOfNodes, shortConnection, shortConnectionId);

            var nodes = new Queue<(int, ConnectionType, int)>();
            _manager.AddNode(shortPathId, referenceId, shortConnection, shortConnectionId);
            nodes.Enqueue((
                referenceId,
                ConnectionType.None,
                0
            ));
            
            var shortLastId = 0;
            while (shortCreated.Any())
            {
                shortLastId = shortCreated.Dequeue();
                nodes.Enqueue((
                    shortLastId,
                    shortConnection,
                    shortConnectionId
                ));
            }

            _manager.AddNode(shortLastId, targetId, shortConnection, shortConnectionId);
            nodes.Enqueue((
                targetId,
                shortConnection,
                shortConnectionId
            ));

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_First_Shortest_Path()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var targetId = unchecked(referenceId + 1);

            var nodes = new Queue<(int, ConnectionType, int)>();
            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(5, 10);
            
            var connection1 = ConnectionType.Film;
            var connection1Id = _fixture.Create<int>();
            var pathId1 = unchecked(targetId + 1);
            var created1 = CreateSimpleConnection(pathId1, numberOfNodes, connection1, connection1Id);

            _manager.AddNode(pathId1, referenceId, connection1, connection1Id);
            nodes.Enqueue((
                referenceId,
                ConnectionType.None,
                0
            ));

            var lastId1 = 0;
            while (created1.Any())
            {
                lastId1 = created1.Dequeue();
                nodes.Enqueue((
                    lastId1,
                    connection1,
                    connection1Id
                ));
            }

            _manager.AddNode(lastId1, targetId, connection1, connection1Id);
            nodes.Enqueue((
                targetId,
                connection1,
                connection1Id
            ));

            var connection2 = ConnectionType.Homeworld;
            var connection2Id = unchecked(connection1Id + 1);
            var pathId2 = unchecked(lastId1 + 1);
            var created2 = CreateSimpleConnection(pathId2, numberOfNodes, connection2, connection2Id);
            var lastId2 = 0;
            while (created2.Any()) { lastId2 = created2.Dequeue(); }

            _manager.AddNode(pathId2, referenceId, connection2, connection2Id);
            _manager.AddNode(lastId2, targetId, connection2, connection2Id);

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Shortest_Path_Reversed()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var targetId = unchecked(referenceId + 1);

            var longConnection = ConnectionType.Film;
            var longConnectionId = _fixture.Create<int>();
            var longPathId = unchecked(targetId + 1);
            var longNumberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(5, 10);
            var longCreated = CreateSimpleConnection(longPathId, longNumberOfNodes, longConnection, longConnectionId);
            var longLastId = 0;
            while (longCreated.Any()) { longLastId = longCreated.Dequeue(); }

            _manager.AddNode(longPathId, referenceId, longConnection, longConnectionId);
            _manager.AddNode(longLastId, targetId, longConnection, longConnectionId);

            var shortConnection = ConnectionType.Homeworld;
            var shortConnectionId = unchecked(longConnectionId + 1);
            var shortPathId = unchecked(longLastId + 1);
            var shortNumberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 3);
            var shortCreated = CreateSimpleConnection(shortPathId, shortNumberOfNodes, shortConnection, shortConnectionId);

            var nodes = new Stack<(int, ConnectionType, int)>();
            _manager.AddNode(shortPathId, referenceId, shortConnection, shortConnectionId);
            nodes.Push((
                referenceId,
                shortConnection,
                shortConnectionId
            ));

            var shortLastId = 0;
            while (shortCreated.Any())
            {
                shortLastId = shortCreated.Dequeue();
                nodes.Push((
                    shortLastId,
                    shortConnection,
                    shortConnectionId
                ));
            }

            _manager.AddNode(shortLastId, targetId, shortConnection, shortConnectionId);
            nodes.Push((
                targetId,
                ConnectionType.None,
                0
            ));

            // act
            var referenceNode = _manager.GetNode(targetId)!;
            var targetNode = _manager.GetNode(referenceId)!;

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var reversed = new Queue<(int, ConnectionType, int)>();
            while (nodes.Any()) { reversed.Enqueue(nodes.Pop()); }

            ValidateConnections(rootNode, reversed);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_First_Shortest_Path_Reversed()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var targetId = unchecked(referenceId + 1);

            var nodes = new Stack<(int, ConnectionType, int)>();
            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(5, 10);

            var connection1 = ConnectionType.Film;
            var connection1Id = _fixture.Create<int>();
            var pathId1 = unchecked(targetId + 1);
            var created1 = CreateSimpleConnection(pathId1, numberOfNodes, connection1, connection1Id);

            _manager.AddNode(pathId1, referenceId, connection1, connection1Id);
            nodes.Push((
                referenceId,
                connection1,
                connection1Id
            ));

            var lastId1 = 0;
            while (created1.Any())
            {
                lastId1 = created1.Dequeue();
                nodes.Push((
                    lastId1,
                    connection1,
                    connection1Id
                ));
            }

            _manager.AddNode(lastId1, targetId, connection1, connection1Id);
            nodes.Push((
                targetId,
                ConnectionType.None,
                0
            ));

            var connection2 = ConnectionType.Homeworld;
            var connection2Id = unchecked(connection1Id + 1);
            var pathId2 = unchecked(lastId1 + 1);
            var created2 = CreateSimpleConnection(pathId2, numberOfNodes, connection2, connection2Id);
            var lastId2 = 0;
            while (created2.Any()) { lastId2 = created2.Dequeue(); }

            _manager.AddNode(pathId2, referenceId, connection2, connection2Id);
            _manager.AddNode(lastId2, targetId, connection2, connection2Id);

            // act
            var referenceNode = _manager.GetNode(targetId)!;
            var targetNode = _manager.GetNode(referenceId)!;

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var reversed = new Queue<(int, ConnectionType, int)>();
            while (nodes.Any()) { reversed.Enqueue(nodes.Pop()); }

            ValidateConnections(rootNode, reversed);
        }

        [Fact]
        public async void FindShortestConnection_Should_Find_Shortest_Path()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var targetId = unchecked(referenceId + 1);

            var nodes = new Queue<(int, ConnectionType, int)>();
            var numberOfNodes1 = new Random(Guid.NewGuid().GetHashCode()).Next(2, 4);

            var connection1 = ConnectionType.Film;
            var connection1Id = _fixture.Create<int>();
            var pathId1 = unchecked(targetId + 1);
            var created1 = CreateSimpleConnection(pathId1, numberOfNodes1, connection1, connection1Id);

            _manager.AddNode(pathId1, referenceId, connection1, connection1Id);
            nodes.Enqueue((
                referenceId,
                ConnectionType.None,
                0
            ));

            var lastId1 = 0;
            while (created1.Any())
            {
                lastId1 = created1.Dequeue();
                nodes.Enqueue((
                    lastId1,
                    connection1,
                    connection1Id
                ));
            }

            _manager.AddNode(lastId1, targetId, connection1, connection1Id);
            nodes.Enqueue((
                targetId,
                connection1,
                connection1Id
            ));

            var numberOfNodes2 = new Random(Guid.NewGuid().GetHashCode()).Next(5, 10);
            var connection2 = ConnectionType.Homeworld;
            var connection2Id = _fixture.Create<int>();
            var pathId2 = unchecked(lastId1 + 1);
            CreateSimpleConnection(pathId2, numberOfNodes2, connection2, connection2Id);
 
            _manager.AddNode(pathId2, referenceId, connection2, connection2Id);

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Find_Shortest_Path_Reversed()
        {
            // arrange
            var referenceId = _fixture.Create<int>();
            var targetId = unchecked(referenceId + 1);

            var nodes = new Stack<(int, ConnectionType, int)>();
            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(5, 10);

            var connection1 = ConnectionType.Film;
            var connection1Id = _fixture.Create<int>();
            var pathId1 = unchecked(targetId + 1);
            var created1 = CreateSimpleConnection(pathId1, numberOfNodes, connection1, connection1Id);

            _manager.AddNode(pathId1, referenceId, connection1, connection1Id);
            nodes.Push((
                referenceId,
                connection1,
                connection1Id
            ));

            var lastId1 = 0;
            while (created1.Any())
            {
                lastId1 = created1.Dequeue();
                nodes.Push((
                    lastId1,
                    connection1,
                    connection1Id
                ));
            }

            _manager.AddNode(lastId1, targetId, connection1, connection1Id);
            nodes.Push((
                targetId,
                ConnectionType.None,
                0
            ));

            var connection2 = ConnectionType.Homeworld;
            var connection2Id = unchecked(connection1Id + 1);
            var pathId2 = unchecked(lastId1 + 1);
            CreateSimpleConnection(pathId2, numberOfNodes, connection2, connection2Id);

            _manager.AddNode(pathId2, referenceId, connection2, connection2Id);

            // act
            var referenceNode = _manager.GetNode(targetId)!;
            var targetNode = _manager.GetNode(referenceId)!;

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var reversed = new Queue<(int, ConnectionType, int)>();
            while (nodes.Any()) { reversed.Enqueue(nodes.Pop()); }

            ValidateConnections(rootNode, reversed);
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Circular_Connections()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var id3 = unchecked(id2 + 1);
            var id4 = unchecked(id3 + 1);
            var id5 = unchecked(id4 + 1);

            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            _manager.AddNode(id1, id2, connectionType, connectionId);
            _manager.AddNode(id1, id3, connectionType, connectionId);
            _manager.AddNode(id1, id4, connectionType, connectionId);
            _manager.AddNode(id2, id3, connectionType, connectionId);
            _manager.AddNode(id3, id2, connectionType, connectionId);
            _manager.AddNode(id3, id4, connectionType, connectionId);
            _manager.AddNode(id4, id5, connectionType, connectionId);
            
            // act
            var referenceNode = _manager.AddNode(id1);
            var targetNode = _manager.AddNode(id5);

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int)>();
            expected.Enqueue((id1, ConnectionType.None, 0));
            expected.Enqueue((id4, connectionType, connectionId));
            expected.Enqueue((id5, connectionType, connectionId));

            ValidateConnections(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Handle_Circular_Connections_Reversed()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1);
            var id3 = unchecked(id2 + 1);
            var id4 = unchecked(id3 + 1);
            var id5 = unchecked(id4 + 1);

            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            _manager.AddNode(id1, id2, connectionType, connectionId);
            _manager.AddNode(id1, id3, connectionType, connectionId);
            _manager.AddNode(id1, id4, connectionType, connectionId);
            _manager.AddNode(id2, id3, connectionType, connectionId);
            _manager.AddNode(id3, id2, connectionType, connectionId);
            _manager.AddNode(id3, id4, connectionType, connectionId);
            _manager.AddNode(id4, id5, connectionType, connectionId);

            // act
            var referenceNode = _manager.AddNode(id5);
            var targetNode = _manager.AddNode(id1);

            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var expected = new Queue<(int, ConnectionType, int)>();
            expected.Enqueue((id5, ConnectionType.None, 0));
            expected.Enqueue((id4, connectionType, connectionId));
            expected.Enqueue((id1, connectionType, connectionId));

            ValidateConnections(rootNode, expected);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Shortest_Path_From_Circular_Connection()
        {
            // arrange
            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();

            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var ids = CreateSimpleConnection(referenceId, numberOfNodes, connection, connectionId);
            var currentId = referenceId;
            while (ids.Any())
            {
                currentId = ids.Dequeue();
            }

            var targetId = unchecked(currentId + 1);
            _manager.AddNode(targetId, currentId, connection, connectionId);

            _manager.AddNode(targetId, referenceId, connection, connectionId);

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var nodes = new Queue<(int, ConnectionType, int)>();
            nodes.Enqueue((referenceId, ConnectionType.None, 0));
            nodes.Enqueue((targetId, connection, connectionId));

            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Shortest_Path_From_Circular_Connection_Reversed()
        {
            // arrange
            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();

            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10);
            var ids = CreateSimpleConnection(referenceId, numberOfNodes, connection, connectionId);
            var currentId = referenceId;
            while (ids.Any())
            {
                currentId = ids.Dequeue();
            }

            var targetId = unchecked(currentId + 1);
            _manager.AddNode(targetId, currentId, connection, connectionId);

            _manager.AddNode(targetId, referenceId, connection, connectionId);

            // act
            var referenceNode = _manager.GetNode(targetId)!;
            var targetNode = _manager.GetNode(referenceId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var nodes = new Queue<(int, ConnectionType, int)>();
            nodes.Enqueue((targetId, ConnectionType.None, 0));
            nodes.Enqueue((referenceId, connection, connectionId));

            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Shortest_Path_From_Similar_Connection()
        {
            // arrange
            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();
            var similarId = unchecked(referenceId + 1);

            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 5);
            var ids = CreateSimpleConnection(similarId, numberOfNodes, connection, connectionId);
            var targetId = similarId;
            var similarIds = new Queue<int>();
            while (ids.Any())
            {
                targetId = ids.Dequeue();
                similarIds.Enqueue(targetId);
            }

            var longerId1 = unchecked(targetId + 1);
            var longerId2 = unchecked(longerId1 + 1);
            _manager.AddNode(longerId1, referenceId, connection, connectionId);
            _manager.AddNode(longerId2, longerId1, connection, connectionId);
            _manager.AddNode(similarId, longerId2, connection, connectionId);

            var shorterId = unchecked(longerId2 + 1);
            _manager.AddNode(shorterId, referenceId, connection, connectionId);
            _manager.AddNode(similarId, shorterId, connection, connectionId);

            // act
            var referenceNode = _manager.GetNode(referenceId)!;
            var targetNode = _manager.GetNode(targetId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var nodes = new Queue<(int, ConnectionType, int)>();
            nodes.Enqueue((referenceId, ConnectionType.None, 0));
            nodes.Enqueue((shorterId, connection, connectionId));
            while(similarIds.Any()) { nodes.Enqueue((similarIds.Dequeue(), connection, connectionId)); }

            ValidateConnections(rootNode, nodes);
        }

        [Fact]
        public async void FindShortestConnection_Should_Get_Shortest_Path_From_Similar_Connection_Reversed()
        {
            // arrange
            var connection = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            var referenceId = _fixture.Create<int>();
            var similarId = unchecked(referenceId + 1);

            var numberOfNodes = new Random(Guid.NewGuid().GetHashCode()).Next(1, 5);
            var ids = CreateSimpleConnection(similarId, numberOfNodes, connection, connectionId);
            var lastId = similarId;
            var similarIds = new Queue<int>();
            while (ids.Any())
            {
                lastId = ids.Dequeue();
                similarIds.Enqueue(lastId);
            }

            var targetId = unchecked(lastId + 1);

            var longerId1 = unchecked(targetId + 1);
            var longerId2 = unchecked(longerId1 + 1);
            _manager.AddNode(longerId1, referenceId, connection, connectionId);
            _manager.AddNode(longerId2, longerId1, connection, connectionId);
            _manager.AddNode(similarId, longerId2, connection, connectionId);
            _manager.AddNode(targetId, lastId, connection, connectionId);

            var shorterId = unchecked(longerId2 + 1);
            _manager.AddNode(shorterId, referenceId, connection, connectionId);
            _manager.AddNode(similarId, shorterId, connection, connectionId);
            _manager.AddNode(targetId, lastId, connection, connectionId);

            // act
            var referenceNode = _manager.GetNode(targetId)!;
            var targetNode = _manager.GetNode(referenceId)!;
            var rootNode = await _crawler.FindShortestConnection(referenceNode.PersonId, targetNode.PersonId);

            // assert
            var nodes = new Stack<(int, ConnectionType, int)>();
            nodes.Push((referenceId, connection, connectionId));
            nodes.Push((shorterId, connection, connectionId));
            while (similarIds.Any()) { nodes.Push((similarIds.Dequeue(), connection, connectionId)); }
            nodes.Push((targetId, ConnectionType.None, 0));

            var reversed = new Queue<(int, ConnectionType, int)>();
            while (nodes.Any()) { reversed.Enqueue(nodes.Pop()); }
            ValidateConnections(rootNode, reversed);
        }

        private static void ValidateConnections(IConnectionPathNode? rootNode, Queue<(int, ConnectionType, int)> expectedConnections)
        {
            rootNode.Should().NotBeNull();
            var currentNode = rootNode;

            while (expectedConnections.Any())
            {
                var (expectedPersonId, expectedConnection, expectedConnectionId) = expectedConnections.Dequeue();
                
                currentNode!.PersonId.Should().Be(expectedPersonId);
                currentNode.Connection.Should().Be(expectedConnection);
                currentNode.ConnectionId.Should().Be(expectedConnectionId);
                currentNode = currentNode.Next;
            }

            currentNode.Should().BeNull();
        }

        private static void ValidateConnectionsWithPersonName(IConnectionPathNode? rootNode, Queue<(int, string, ConnectionType, int)> expectedConnections)
        {
            rootNode.Should().NotBeNull();
            var currentNode = rootNode;

            while (expectedConnections.Any())
            {
                var (expectedPersonId, expectedPersonName, expectedConnection, expectedConnectionId) = expectedConnections.Dequeue();

                currentNode!.PersonId.Should().Be(expectedPersonId);
                currentNode.Name.Should().Be(expectedPersonName);
                currentNode.Connection.Should().Be(expectedConnection);
                currentNode.ConnectionId.Should().Be(expectedConnectionId);
                currentNode = currentNode.Next;
            }

            currentNode.Should().BeNull();
        }

        private static void ValidateConnectionsWithConnectionName(IConnectionPathNode? rootNode, Queue<(int, ConnectionType, int, string)> expectedConnections)
        {
            rootNode.Should().NotBeNull();
            var currentNode = rootNode;

            while (expectedConnections.Any())
            {
                var (expectedPersonId, expectedConnection, expectedConnectionId, expectedConnectionName) = expectedConnections.Dequeue();

                currentNode!.PersonId.Should().Be(expectedPersonId);
                currentNode.Connection.Should().Be(expectedConnection);
                currentNode.ConnectionId.Should().Be(expectedConnectionId);
                currentNode.ConnectionName.Should().Be(expectedConnectionName);
                currentNode = currentNode.Next;
            }

            currentNode.Should().BeNull();
        }

        private Queue<int> CreateSimpleConnection(int startId, int numberOfRelatedNodes, ConnectionType connection, int connectionId)
        {
            var created = new Queue<int>();
            created.Enqueue(startId);

            var parentId = startId;
            for (var i = 0; i < numberOfRelatedNodes; i++)
            {
                var newId = parentId + 1;
                _manager.AddNode(newId, parentId, connection, connectionId);
                created.Enqueue(newId);
                parentId = newId;
            };

            return created;
        }
    }
}