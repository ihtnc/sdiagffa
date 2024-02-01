using Xunit;
using sdiagffa.core.services;
using AutoFixture;
using FluentAssertions;
using sdiagffa.core.models;
using System.Linq;

namespace sdiagffa.test.core.services
{
    public class RelationshipManagerTests
    {
        readonly IRelationshipManager _manager;
        readonly Fixture _fixture;

        public RelationshipManagerTests()
        {
            _manager = new RelationshipManager();
            _fixture = new Fixture();
        }

        [Fact]
        public void GetNode_Should_Handle_Empty_Nodes()
        {
            // arrange
            var id = _fixture.Create<int>();

            // act
            var node = _manager.GetNode(id);

            // assert
            node.Should().BeNull();
        }

        [Fact]
        public void GetNode_Should_Handle_NonExisting_Nodes()
        {
            // arrange
            var id = _fixture.Create<int>();
            _manager.AddNode(id);

            var invalidId = unchecked(id + 1);

            // act
            var node = _manager.GetNode(invalidId);

            // assert
            node.Should().BeNull();
        }

        [Fact]
        public void GetNode_Should_Get_Existing_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var otherId = unchecked(id + 1);

            var expected = _manager.AddNode(id);
            _manager.AddNode(otherId);

            // act
            var node = _manager.GetNode(id);

            // assert
            node.Should().Be(expected);
        }

        [Fact]
        public void AddNode_Should_Add_Node()
        {
            // arrange
            var id = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id);

            // assert
            node.Should().NotBeNull();
            node.PersonId.Should().Be(id);
            node.Connections.Should().BeEmpty();
        }

        [Fact]
        public void AddNode_Should_Add_New_Nodes()
        {
            // arrange
            var existingId = _fixture.Create<int>();
            var id = unchecked(existingId + 1);

            // act
            var existing = _manager.AddNode(existingId);
            var node = _manager.AddNode(id);

            // assert
            node.Should().NotBe(existing);
            node.PersonId.Should().Be(id);
        }

        [Fact]
        public void AddNode_Should_Not_Add_Duplicate()
        {
            // arrange
            var id = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id);
            var newNode = _manager.AddNode(id);

            // assert
            node.Should().Be(newNode);
            var added = _manager.GetNode(id);
            node.Should().Be(added);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Add_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var parentId = unchecked(id + 1);
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, parentId, ConnectionType.Film, connectionId);

            // assert
            node.Should().NotBeNull();
            node.PersonId.Should().Be(id);
            node.Connections.Should().NotBeEmpty();
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Add_New_Nodes()
        {
            // arrange
            var existingId = _fixture.Create<int>();
            var id = unchecked(existingId + 1);
            var parentId = unchecked(id + 1);
            var connectionId = _fixture.Create<int>();

            // act
            var existing = _manager.AddNode(existingId, parentId, ConnectionType.Homeworld, connectionId);
            var node = _manager.AddNode(id, parentId, ConnectionType.Film, connectionId);

            // assert
            node.Should().NotBe(existing);
            node.PersonId.Should().Be(id);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Not_Add_Duplicate()
        {
            // arrange
            var id = _fixture.Create<int>();
            var parentId = unchecked(id + 1);
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, parentId, ConnectionType.Film, connectionId);
            var newNode = _manager.AddNode(id, parentId, ConnectionType.Homeworld, connectionId);

            // assert
            node.Should().Be(newNode);
            var added = _manager.GetNode(id);
            node.Should().Be(added);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Relate_New_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var relatedId = unchecked(id + 1);
            _manager.GetNode(relatedId).Should().BeNull();
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, relatedId, ConnectionType.Homeworld, connectionId);

            // assert
            var related = _manager.GetNode(relatedId)!;
            related.Should().NotBeNull();
            related.PersonId.Should().Be(relatedId);
            related.Connections.Should().NotBeNullOrEmpty();

            node.Should().NotBe(related);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Relate_Existing_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var relatedId = unchecked(id + 1);
            var existing = _manager.AddNode(relatedId);
            existing.Connections.Should().BeEmpty();
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, relatedId, ConnectionType.Homeworld, connectionId);

            // assert
            var related = _manager.GetNode(relatedId)!;
            related.Should().NotBeNull();
            related.Should().Be(existing);
            related.Connections.Should().NotBeEmpty();

            node.Should().NotBe(related);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Not_Relate_To_Self()
        {
            // arrange
            var id = _fixture.Create<int>();
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, id, ConnectionType.Homeworld, connectionId);

            // assert
            node.Connections.Should().BeEmpty();
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Not_Relate_None_ConnectionType()
        {
            // arrange
            var id = _fixture.Create<int>();
            var relatedId = unchecked(id + 1);
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, relatedId, ConnectionType.None, connectionId);

            // assert
            node.Connections.Should().BeEmpty();
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Add_Connection_To_Related_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var relatedId = unchecked(id + 1);
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, relatedId, connectionType, connectionId);

            // assert
            var related = _manager.GetNode(relatedId)!;

            node.Connections.Should().NotBeEmpty();
            node.Connections.Should().HaveCount(1);

            var connection = node.Connections.First();
            connection.Should().NotBeNull();
            connection.Key.Should().Be(related.PersonId);
            connection.Value.Should().NotBeNull();
            connection.Value.Connection.Should().Be(connectionType);
            connection.Value.ConnectionId.Should().Be(connectionId);
            connection.Value.Target.Should().Be(related);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Add_Connection_From_Related_Node()
        {
            // arrange
            var id = _fixture.Create<int>();
            var relatedId = unchecked(id + 1);
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();

            // act
            var node = _manager.AddNode(id, relatedId, connectionType, connectionId);
            var related = _manager.GetNode(relatedId)!;

            // assert
            related.Connections.Should().NotBeEmpty();
            related.Connections.Should().HaveCount(1);

            var connection = related.Connections.First();
            connection.Should().NotBeNull();
            connection.Key.Should().Be(node.PersonId);
            connection.Value.Should().NotBeNull();
            connection.Value.Connection.Should().Be(connectionType);
            connection.Value.ConnectionId.Should().Be(connectionId);
            connection.Value.Target.Should().Be(node);
        }

        [Fact]
        public void AddNode_With_Related_Node_Should_Append_Connection_From_Related_Node()
        {
            // arrange
            var id1 = _fixture.Create<int>();
            var id2 = unchecked(id1 + 1); 
            var relatedId = unchecked(id2 + 1);
            var connectionType = ConnectionType.Film;
            var connectionId = _fixture.Create<int>();
            _manager.AddNode(id1, relatedId, connectionType, connectionId);

            // act
            var node2 = _manager.AddNode(id2, relatedId, connectionType, connectionId);
            var related = _manager.GetNode(relatedId)!;

            // assert
            node2.Connections.Should().HaveCount(1);
            related.Connections.Should().NotBeEmpty();
            related.Connections.Should().HaveCount(2);

            var connection = related.Connections[node2.PersonId];
            connection.Should().NotBeNull();
            connection.Connection.Should().Be(connectionType);
            connection.ConnectionId.Should().Be(connectionId);
            connection.Target.Should().Be(node2);
        }
    }
}