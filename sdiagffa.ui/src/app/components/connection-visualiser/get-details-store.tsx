import { create } from 'zustand';
import {
  getPerson, getFilm, getPlanet,
  Person, Film, Planet
} from '@/api/api-client';

export interface Details {
  id: number,
  type: DetailsType,
  heading: string,
  additionalDetails: object
}

export enum DetailsType {
  Character='Character',
  Film='Film',
  Homeworld='Homeworld'
}

export interface GetDetailsStore {
  isBusy: boolean,
  isSearchDone: boolean,
  getDetails: (id: number, type: DetailsType) => Promise<void>,
  results: Details | null,
  detailsCache: Array<Details>
}

const convertFromPerson = (person: Person): Details => {
  const details: Details = {
    id: person.id,
    type: DetailsType.Character,
    heading: person.name,
    additionalDetails: {
      gender: person.gender,
      birthYear: person.birthYear,
      height: person.height,
      mass: person.mass,
      skinColor: person.skinColor,
      eyeColor: person.eyeColor
    }
  };

  return details;
};

const convertFromFilm = (film: Film): Details => {
  const details: Details = {
    id: film.id,
    type: DetailsType.Film,
    heading: film.title,
    additionalDetails: {
      releaseDate: film.releaseDate,
      episodeId: film.episodeId,
      director: film.director,
      producer: film.producer
    }
  };

  return details;
};

const convertFromPlanet = (planet: Planet): Details => {
  const details: Details = {
    id: planet.id,
    type: DetailsType.Homeworld,
    heading: planet.name,
    additionalDetails: {
      terrain: planet.terrain,
      climate: planet.climate,
      surfaceWater: planet.surfaceWater,
      gravity: planet.gravity,
      population: planet.population,
      diameter: planet.diameter
    }
  };

  return details;
};

const useGetDetails = create<GetDetailsStore>((set, get) => ({
  isBusy: false,
  isSearchDone: false,
  results: null,
  detailsCache: [],

  getDetails: async (id: number, type: DetailsType) => {
    set({
      isBusy: true,
      isSearchDone: false,
      results: null
    });

    const { detailsCache } = get();
    let details = detailsCache.find((c) => c?.id === id && c?.type === type);
    if (details) {
      set({
        isBusy: false,
        isSearchDone: true,
        results: details
      });
      return;
    }

    switch(type) {
      case DetailsType.Character:
        const person = await getPerson(id);
        details = convertFromPerson(person);
        break;

      case DetailsType.Film:
        const film = await getFilm(id);
        details = convertFromFilm(film);
        break;

      case DetailsType.Homeworld:
        const planet = await getPlanet(id);
        details = convertFromPlanet(planet);
        break;
    }

    detailsCache.push(details);

    set({
      isBusy: false,
      isSearchDone: true,
      results: details,
      detailsCache: detailsCache
    });
  }
}));

export default useGetDetails;