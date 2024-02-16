import { beforeEach, afterEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import * as ApiClient from "@/api/api-client";
import ConnectionFinder from ".";

describe('CharacterPicker component', () => {
  let findConnectionSpy;

  beforeEach(() => {
    findConnectionSpy = vi.spyOn(ApiClient, 'findConnection');
  });

  afterEach(() => {
    findConnectionSpy.mockReset();
  });

  it('should render default state', () => {
    const component = render(<ConnectionFinder referencePersonId={1} targetPersonId={2} />);
    expect(component).toMatchSnapshot();
  });

  it('should call findConnection on button click', () => {
    const referencePersonId = 1;
    const targetPersonId = 2;
    render(<ConnectionFinder
      referencePersonId={referencePersonId}
      targetPersonId={targetPersonId} />);
    const button = screen.getByRole('button');

    findConnectionSpy.mockReturnValue({});
    expect(findConnectionSpy).not.toHaveBeenCalled();

    fireEvent.click(button);

    expect(findConnectionSpy).toHaveBeenCalledWith(referencePersonId, targetPersonId);
  });

  it('should render findConnection in progress', async () => {
    const component = render(<ConnectionFinder referencePersonId={1} targetPersonId={2} />);
    const button = screen.getByRole('button');

    findConnectionSpy.mockReturnValue(new Promise(() => {}));

    fireEvent.click(button);

    await waitFor(() => {
      expect(screen.getByTitle('Loading')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should render after findConnection', async () => {
    const component = render(<ConnectionFinder referencePersonId={1} targetPersonId={2} />);
    const button = screen.getByRole('button');

    const result: ApiClient.Connection = {
      connection: 'None',
      connectionId: 0,
      connectionName: null,
      name: 'test',
      personId: 1,
      next: null
    };
    findConnectionSpy.mockResolvedValue(result);

    fireEvent.click(button);

    await waitFor(() => {
      expect(screen.getByText('Find Connection')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should call onSearchComplete after button click', async () => {
    const onSearchComplete = vi.fn();
    render(<ConnectionFinder
      referencePersonId={1} targetPersonId={2}
      onSearchComplete={onSearchComplete} />);
    const button = screen.getByRole('button');

    const result: ApiClient.Connection = {
      connection: 'None',
      connectionId: 0,
      connectionName: null,
      name: 'test',
      personId: 1,
      next: null
    };
    findConnectionSpy.mockResolvedValue(result);

    fireEvent.click(button);

    await waitFor(() => {
      expect(screen.getByText('Find Connection')).toBeInTheDocument();
    });

    expect(onSearchComplete).toHaveBeenCalledWith(result);
  });
});