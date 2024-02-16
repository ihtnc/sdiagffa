import { beforeEach, afterEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor, waitForElementToBeRemoved } from '@testing-library/react';
import { useDebounceCallback } from 'usehooks-ts';
import * as ApiClient from "@/api/api-client";
import CharacterPicker from ".";

vi.mock('usehooks-ts', () => {
  const useDebounceCallback = vi.fn();
  useDebounceCallback.mockImplementation((func) => { return func; });
  return { useDebounceCallback };
});

describe('CharacterPicker component', () => {
  let searchPeopleSpy;
  let customPeopleSearchSpy;

  beforeEach(() => {
    searchPeopleSpy = vi.spyOn(ApiClient, 'searchPeople');
    customPeopleSearchSpy = vi.spyOn(ApiClient, 'customPeopleSearch');
  });

  afterEach(() => {
    searchPeopleSpy.mockReset();
    customPeopleSearchSpy.mockReset();
  });

  it('should render default state', () => {
    const component = render(<CharacterPicker id="test" />);
    expect(component).toMatchSnapshot();
  });

  it('should render with custom placeholder text', () => {
    const placeholderText = 'placeholderText123qwe'
    const component = render(<CharacterPicker id="test" placeholderText={placeholderText} />);

    expect(component).toMatchSnapshot();
    const input = screen.getByPlaceholderText(placeholderText);
    expect(input).toBeInTheDocument();
  });

  it('should setup debounce function with default value', () => {
    render(<CharacterPicker id="test" />);

    expect(useDebounceCallback).toHaveBeenCalledWith(expect.any(Function), 1500);
  });

  it('should setup debounce function with searchDelay value', () => {
    const searchDelay = 100;
    render(<CharacterPicker id="test" searchDelay={searchDelay} />);

    expect(useDebounceCallback).toHaveBeenCalledWith(expect.any(Function), searchDelay);
  });

  it('should call search on text change', () => {
    render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');
    const searchText = 'test';

    searchPeopleSpy.mockReturnValue({});
    expect(searchPeopleSpy).not.toHaveBeenCalled();

    fireEvent.change(input, { target: { value: searchText } });

    expect(searchPeopleSpy).toHaveBeenCalledWith(searchText);
  });

  it('should render search in progress', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const promise = new Promise(() => { });
    searchPeopleSpy.mockReturnValue(promise);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByTitle('Loading')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should render result', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: null,
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should render result with more result indicator', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: 'nextPageUrl',
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

   fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
    const more = screen.getByText('More');
    expect(more).toBeInTheDocument();
  });

  it('should call customPeopleSearch on more result indicator click', async () => {
    render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');
    const nextPageUrl = 'testNextPageUrl';

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: nextPageUrl,
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    customPeopleSearchSpy.mockReturnValue({});
    expect(customPeopleSearchSpy).not.toHaveBeenCalled();

    const more = screen.getByText('More');
    fireEvent.click(more);

    expect(customPeopleSearchSpy).toHaveBeenCalledWith(nextPageUrl);
  });

  it('should render customPeopleSearch in progress', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: 'nextPageUrl',
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

    const promise = new Promise(() => { });
    customPeopleSearchSpy.mockReturnValue(promise);;

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    const more = component.getByText('More');
    fireEvent.click(more);

    await waitFor(() => {
      expect(screen.getByTitle('Loading')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should render results with additional results from loading more', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: 'nextPageUrl',
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

    const moreResult: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: null,
      previousPageUrl: null,
      results: [{
        id: 2,
        name: 'C-3PO',
        birthYear: '112BBY',
        eyeColor: 'yellow',
        gender: 'n/a',
        height: '167',
        mass: '75',
        skinColor: 'gold'
      }]
    };
    customPeopleSearchSpy.mockResolvedValue(moreResult);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    const more = screen.getByText('More');
    fireEvent.click(more);

    await waitFor(() => {
      expect(screen.getByText('C-3PO')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should render results with more result indicator after loading more', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: 'nextPageUrl1',
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

    const moreResult: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: 'nextPageUrl2',
      previousPageUrl: null,
      results: [{
        id: 2,
        name: 'C-3PO',
        birthYear: '112BBY',
        eyeColor: 'yellow',
        gender: 'n/a',
        height: '167',
        mass: '75',
        skinColor: 'gold'
      }]
    };
    customPeopleSearchSpy.mockResolvedValue(moreResult);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    const more = screen.getByText('More');
    fireEvent.click(more);

    await waitFor(() => {
      expect(screen.getByText('C-3PO')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
  });

  it('should call onSelect when result is selected', async () => {
    const onSelect = vi.fn();
    render(<CharacterPicker id="test" onSelect={onSelect} />);
    const input = screen.getByRole('combobox');

    const person1 = {
      id: 1,
      name: 'Luke Skywalker',
      birthYear: '19BBY',
      eyeColor: 'blue',
      gender: 'male',
      height: '172',
      mass: '77',
      skinColor: 'fair'
    };

    const person2 = {
      id: 2,
      name: 'C-3PO',
      birthYear: '112BBY',
      eyeColor: 'yellow',
      gender: 'n/a',
      height: '167',
      mass: '75',
      skinColor: 'gold'
    };

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: null,
      previousPageUrl: null,
      results: [person1, person2]
    };
    searchPeopleSpy.mockResolvedValue(result);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    const item = screen.getByText('C-3PO');
    fireEvent.click(item);

    expect(onSelect).toHaveBeenCalledWith(person2);
  });

  it('should render after selection', async () => {
    const component = render(<CharacterPicker id="test" />);
    const input = screen.getByRole('combobox');

    const result: ApiClient.PagedResults<ApiClient.Person> = {
      count: 1,
      nextPageUrl: null,
      previousPageUrl: null,
      results: [{
        id: 1,
        name: 'Luke Skywalker',
        birthYear: '19BBY',
        eyeColor: 'blue',
        gender: 'male',
        height: '172',
        mass: '77',
        skinColor: 'fair'
      }]
    };
    searchPeopleSpy.mockResolvedValue(result);

    fireEvent.change(input, { target: { value: 'test' } });

    await waitFor(() => {
      expect(screen.getByRole('listbox')).toBeInTheDocument();
    });

    const item = component.getByText('Luke Skywalker');
    fireEvent.click(item);

    await waitForElementToBeRemoved(() => {
      return screen.getByRole('listbox')
    });

    expect(component).toMatchSnapshot();
  });
});