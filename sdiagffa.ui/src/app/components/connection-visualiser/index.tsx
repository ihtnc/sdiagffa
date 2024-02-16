import { useState } from "react";
import { Connection } from "@/api/api-client";
import { DetailsType } from "./get-details-store";
import ConnectionDetails from "./connection-details";
import ConnectionGraph from "./connection-graph";

interface ConnectionVisualiserProps {
  connection: Connection,
}

export interface ConnectionId {
  id: number,
  type: 'Character' | 'Film' | 'Homeworld'
}

const ConnectionVisualiser = ({connection}: ConnectionVisualiserProps) => {
  const [ selectedNodeId, setSelectedNodeId ] = useState(0);
  const [ selectedNodeType, setSelectedNodeType ] = useState<DetailsType>(null);

  const handleSelect = (selected: ConnectionId) => {
    let type: DetailsType = null;
    switch (selected.type) {
      case 'Character': type = DetailsType.Character; break;
      case 'Film': type = DetailsType.Film; break;
      case 'Homeworld': type = DetailsType.Homeworld; break;
    }

    if (selected.id > 0 && type !== null) {
      setSelectedNodeId(selected.id);
      setSelectedNodeType(type);
    }
  };

  return connection && (
    <div className='relative
      w-full h-80
      border-swblue dark:border-swyellow
      rounded-md border-2
    '>
      <ConnectionGraph connection={connection} onSelect={handleSelect} />
      <ConnectionDetails id={selectedNodeId} type={selectedNodeType} />
    </div>
  );
};

export default ConnectionVisualiser;