import { beforeEach, afterEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor, waitForElementToBeRemoved } from '@testing-library/react'
import * as ApiClient from "@/api/api-client";
import ConnectionDetails from "./connection-details";
import useGetDetails, { Details, DetailsType } from "./get-details-store";

vi.mock('usehooks-ts', async () => {
  const useOnClickOutside = vi.fn();
  const useBoolean = (await vi.importActual<typeof import('usehooks-ts')>('usehooks-ts')).useBoolean;
  return { useOnClickOutside, useBoolean };
});

describe('ConnectionDetails component', () => {
  let getPersonSpy;
  let getFilmSpy;
  let getPlanetSpy;

  beforeEach(() => {
    getPersonSpy = vi.spyOn(ApiClient, 'getPerson');
    getFilmSpy = vi.spyOn(ApiClient, 'getFilm');
    getPlanetSpy = vi.spyOn(ApiClient, 'getPlanet');
    useGetDetails.setState({ detailsCache: [] })
  });

  afterEach(() => {
    getPersonSpy.mockReset();
    getFilmSpy.mockReset();
    getPlanetSpy.mockReset();
  });

  it('should render default state', () => {
    getPersonSpy.mockReturnValue({ });
    const component = render(<ConnectionDetails id={1} type={DetailsType.Character} />);
    expect(component).toMatchSnapshot();
  });

  it('should call getPerson on initial load if type is Character', () => {
    getPersonSpy.mockReturnValue({});
    expect(getPersonSpy).not.toHaveBeenCalled();

    const id = 1;
    const type = DetailsType.Character;
    render(<ConnectionDetails id={id} type={type} />);

    expect(getPersonSpy).toHaveBeenCalledWith(id);
  });

  it('should call getFilm on initial load if type is Film', () => {
    getFilmSpy.mockReturnValue({});
    expect(getFilmSpy).not.toHaveBeenCalled();

    const id = 1;
    const type = DetailsType.Film;
    render(<ConnectionDetails id={id} type={type} />);

    expect(getFilmSpy).toHaveBeenCalledWith(id);
  });

  it('should call getPlanet on initial load if type is Homeworld', () => {
    getPlanetSpy.mockReturnValue({});
    expect(getPlanetSpy).not.toHaveBeenCalled();

    const id = 1;
    const type = DetailsType.Homeworld;
    render(<ConnectionDetails id={id} type={type} />);

    expect(getPlanetSpy).toHaveBeenCalledWith(id);
  });

  it('should render character details', async () => {
    var person: ApiClient.Person = {
      id: 1,
      name: 'Luke Skywalker',
      birthYear: '19BBY',
      eyeColor: 'blue',
      gender: 'male',
      height: '172',
      mass: '77',
      skinColor: 'fair'
    };
    getPersonSpy.mockResolvedValue(person);

    const id = 1;
    const type = DetailsType.Character;
    const component = render(<ConnectionDetails id={id} type={type} />);

    await waitFor(() => {
      expect(screen.getByText('Luke Skywalker')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
    const icon = component.container.querySelector('.fa-user');
    expect(icon).toBeInTheDocument();
  });

  it('should render film details', async () => {
    var film: ApiClient.Film = {
      id: 1,
      title: 'A New Hope',
      episodeId: 4,
      director: 'George Lucas',
      producer: 'Gary Kurtz, Rick McCallum',
      releaseDate: '1977-05-25'
    };
    getFilmSpy.mockResolvedValue(film);

    const id = 1;
    const type = DetailsType.Film;
    const component = render(<ConnectionDetails id={id} type={type} />);

    await waitFor(() => {
      expect(screen.getByText('A New Hope')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
    const icon = component.container.querySelector('.fa-film');
    expect(icon).toBeInTheDocument();
  });

  it('should render planet details', async () => {
    var planet: ApiClient.Planet = {
      id: 1,
      name: 'Tatooine',
      climate: 'arid',
      terrain: 'desert',
      surfaceWater: '1',
      diameter: '10465',
      gravity: '1 standard',
      population: '200000'
    };
    getPlanetSpy.mockResolvedValue(planet);

    const id = 1;
    const type = DetailsType.Homeworld;
    const component = render(<ConnectionDetails id={id} type={type} />);

    await waitFor(() => {
      expect(screen.getByText('Tatooine')).toBeInTheDocument();
    });

    expect(component).toMatchSnapshot();
    const icon = component.container.querySelector('.fa-globe');
    expect(icon).toBeInTheDocument();
  });

  it('should retrieve details when id property changes', () => {
    getPersonSpy.mockReturnValueOnce({ id: 1 });
    getPersonSpy.mockReturnValueOnce({ id: 2 });

    const id = 1;
    const type = DetailsType.Character;
    const { rerender } = render(<ConnectionDetails id={id} type={type} />);
    expect(getPersonSpy).toHaveBeenCalledWith(id);

    const newId = 2;
    rerender(<ConnectionDetails id={newId} type={type} />)
    expect(getPersonSpy).toHaveBeenCalledWith(newId);
  });

  it('should retrieve details when type property changes', () => {
    getPersonSpy.mockReturnValueOnce({ id: 1 });
    getFilmSpy.mockReturnValueOnce({ id: 1 });

    const id = 1;
    const type = DetailsType.Character;
    const { rerender } = render(<ConnectionDetails id={id} type={type} />);
    expect(getPersonSpy).toHaveBeenCalledWith(id);

    const newType = DetailsType.Film;
    rerender(<ConnectionDetails id={id} type={newType} />)
    expect(getFilmSpy).toHaveBeenCalledWith(id);
  });

  it('should not retrieve details when id was previously retrieved', () => {
    const details: Array<Details> = [{
      id: 1,
      type: DetailsType.Character,
      heading: '',
      additionalDetails: { }
    }];
    useGetDetails.setState({ detailsCache: details});

    getPersonSpy.mockReturnValueOnce({ id: 1 });

    const id = 1;
    const type = DetailsType.Character;
    render(<ConnectionDetails id={id} type={type} />);

    expect(getPersonSpy).not.toHaveBeenCalled();
  });

  it('should render when clicked outside', async () => {
    const useHooks = await import('usehooks-ts');
    let clickOutsideCallback;
    useHooks.useOnClickOutside = vi.fn().mockImplementation((_, func) => {
      clickOutsideCallback = func;
    });

    var person: ApiClient.Person = {
      id: 1,
      name: 'Luke Skywalker',
      birthYear: '19BBY',
      eyeColor: 'blue',
      gender: 'male',
      height: '172',
      mass: '77',
      skinColor: 'fair'
    };
    getPersonSpy.mockResolvedValue(person);

    const component = render(<ConnectionDetails id={1} type={DetailsType.Character} />);

    await waitFor(() => {
      expect(screen.getByText('Luke Skywalker')).toBeInTheDocument();
    });

    clickOutsideCallback();

    await waitForElementToBeRemoved(() => screen.getByText('Luke Skywalker'));

    expect(component).toMatchSnapshot();
  });
});