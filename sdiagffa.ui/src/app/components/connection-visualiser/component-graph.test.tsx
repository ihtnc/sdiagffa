import { beforeEach, afterEach, describe, expect, it, vi } from "vitest";
import { render } from '@testing-library/react'
import ConnectionGraph from "./connection-graph";
import { Connection } from "@/api/api-client";
import { ConnectionId } from ".";

vi.mock('react-graph-vis', () => {
  const mock = vi.fn().mockImplementation(({ graph, options, events }) => {
    const nodes = [{
      id: 'Person_1',
      label: 'Luke Skywalker'
    }];
    events.select({nodes});

    return (<>
      <div>Graph: {JSON.stringify(graph)}</div>
      <div>Options: {JSON.stringify(options)}</div>
    </>);
  });
  return { default: mock };
});

vi.mock('usehooks-ts', () => {
  const useDarkMode = vi.fn();
  return { useDarkMode };
});

describe('ConnectionGraph component', () => {
  let useDarkModeMock;

  beforeEach(async () => {
    useDarkModeMock = vi.fn();
    const useHooks = await import('usehooks-ts');
    useHooks.useDarkMode = useDarkModeMock;

    useDarkModeMock.mockImplementation(() => {
      return { isDarkMode: false }
    });
  });

  afterEach(() => {
    useDarkModeMock.mockReset();
  });

  it('should render default state', async () => {
    const connection: Connection = {
      personId: 1,
      name: 'Luke Skywalker',
      connectionId: 0,
      connection: "None",
      connectionName: null,
      next: null
    };
    const component = render(<ConnectionGraph connection={connection} onSelect={vi.fn()} />);
    expect(component).toMatchSnapshot();
  });

  it('should render default state in darkmode', async () => {
    useDarkModeMock.mockImplementation(() => {
      return { isDarkMode: true }
    });

    const connection: Connection = {
      personId: 1,
      name: 'Luke Skywalker',
      connectionId: 0,
      connection: "None",
      connectionName: null,
      next: null
    };
    const component = render(<ConnectionGraph connection={connection} onSelect={vi.fn()} />);
    expect(component).toMatchSnapshot();
  });

  it('should call onSelect', async () => {
    const onSelect = vi.fn<[id: ConnectionId], void>();

    const connection: Connection = {
      personId: 1,
      name: 'Luke Skywalker',
      connectionId: 0,
      connection: "None",
      connectionName: null,
      next: null
    };
    render(<ConnectionGraph connection={connection} onSelect={onSelect} />);

    expect(onSelect).toHaveBeenCalled();
  });
});