import Graph from "react-graph-vis";
import { useDarkMode } from "usehooks-ts";
import config from "@tailwind.config";
import { Connection } from "@/api/api-client";
import { ConnectionId } from ".";

interface Node {
  id: string,
  label: string
}

interface Edge {
  id: string,
  from: string,
  to: string,
  label: string,
}

interface GraphValue {
  nodes: Array<Node>,
  edges: Array<Edge>
}

interface ConnectionGraphProps {
  connection: Connection,
  onSelect: (id: ConnectionId) => void
}

const typeParser = new RegExp('(Character|Film|Homeworld)(?=_\\d+)', 'i');
const idParser = new RegExp('(?<=Character_|Film_|Homeworld_)(\\d+)', 'i');

const parseConnectionId = (id: string): ConnectionId | null => {
  const idResult = idParser.exec(id);
  const typeResult = typeParser.exec(id);
  if (idResult === null || typeResult === null) { return null; }

  let type = null;
  switch(typeResult[0]) {
    case "Character": type = 'Character'; break;
    case "Film": type = 'Film'; break;
    case "Homeworld": type = 'Homeworld'; break;
  }
  if (type === null) { return null; }

  const connection: ConnectionId = {
    id: parseInt(idResult[0]),
    type: type
  };

  return connection;
}

const addNode = (graph: GraphValue, { id, label }: Node): GraphValue => {
  graph.nodes.push({ id, label });
  return graph;
};

const addEdge = (graph: GraphValue, {id, from, to, label }: Edge): GraphValue => {
  graph.edges.push({ id, from, to, label });
  return graph;
};

const buildGraph = (connection: Connection): GraphValue => {
  let previousId = 0;
  let current = connection;
  let graph: GraphValue = {
    nodes:[],
    edges:[]
  };

  while(current !== null) {
    graph = addNode(graph, {
      id: `Character_${current.personId}`,
      label: current.name
    });

    if (previousId > 0 && current.connection !== "None") {
      graph = addEdge(graph, {
        id: `${current.connection}_${current.connectionId}`,
        from: `Character_${previousId}`,
        to: `Character_${current.personId}`,
        label: `${current.connectionName} (${current.connection})`
      });
    }

    previousId = current.personId;
    current = current.next
  }

  return graph;
}

const getGraphOptions = (isDarkMode: boolean) => {
  const swblue = config.theme.colors["swblue"];
  const swyellow = config.theme.colors["swyellow"];
  const swblack = config.theme.colors["swblack"];
  const white = config.theme.colors["white"];

  const node = isDarkMode ? white : swblack;
  const edge = isDarkMode ? white: swblack;
  const active = isDarkMode ? swyellow : swblue;

  const options = {
    nodes: {
      shape: 'icon',
      icon: {
        face: 'FontAwesome',
        code: '\uf183',
        color: node,
        weight: 'normal'
      },
      font: { color: node },
      chosen: {
        label: (values) => { values.color = active; }
      },
      fixed: { x: false, y: false }
    },

    edges: {
      color: edge,
      font: { color: edge, strokeWidth: 0 },
      chosen: {
        edge: (values) => { values.color = active; },
        label: (values) => { values.color = active; }
      },
      width: 2
    },

    physics: { enabled: false },
    interaction: { selectConnectedEdges: false }
  };

  return options;
};

const getEventHandlers = (onSelectCallback: (id: ConnectionId) => void) => ({
  select: ({nodes, edges}) => {
    let selectedId = null;
    if (Array.isArray(nodes) && nodes.length > 0) {
      selectedId = nodes[0];
    } else if (Array.isArray(edges) && edges.length > 0) {
      selectedId = edges[0];
    }

    if (selectedId === null) { return; }

    const id = parseConnectionId(selectedId);
    if (onSelectCallback) { onSelectCallback(id); }
  }
});

const ConnectionGraph = ({ connection, onSelect }: ConnectionGraphProps) => {
  const { isDarkMode } = useDarkMode();

  const graph = buildGraph(connection);
  const options = getGraphOptions(isDarkMode);
  const events = getEventHandlers(onSelect);
  const hasNodes = graph.nodes.length > 0

  return hasNodes && (
    <div className='w-full h-full absolute top-0 left-0'>
      <Graph graph={graph} options={options} events={events} />
    </div>
  );
};

export default ConnectionGraph;