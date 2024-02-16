const baseUrl = 'https://localhost:44300';

export interface Person {
  id: number,
  name: string,
  gender: string,
  birthYear: string,
  height: string,
  mass: string,
  skinColor: string,
  eyeColor: string
};

export interface Film {
  id: number,
  title: string,
  releaseDate: string,
  episodeId: number,
  director: string,
  producer: string
};

export interface Planet {
  id: number,
  name: string,
  terrain: string,
  climate: string,
  surfaceWater: string,
  gravity: string,
  population: string,
  diameter: string
};

export interface Connection {
  personId: number,
  name: string,
  connection: 'Film' | 'Homeworld' | 'None',
  connectionId: number | null,
  connectionName: string | null,
  next: Connection | null
};

export interface PagedResults<T> {
  count: number,
  nextPageUrl: string | null,
  previousPageUrl: string | null,
  results: Array<T>
};

export const searchPeople = async function (search: string, page: number | null = null)
  : Promise<PagedResults<Person>> {
  const findUrl = '/details/people';

  const queries = [];
  if (search.length > 0) { queries.push(`search=${search}`); }
  if (page !== null) { queries.push(`page=${page}`); }
  const queryString = queries.length > 0 ? `?${queries.join('&')}` : '';

  const data = await fetch(`${baseUrl}${findUrl}${queryString}`);
  return data.json() as unknown as PagedResults<Person>;
};

export const customPeopleSearch = async function (peopleSearchUrl: string)
  : Promise<PagedResults<Person>> {
  const data = await fetch(`${baseUrl}${peopleSearchUrl}`);
  return data.json() as unknown as PagedResults<Person>;
};

export const findConnection = async function (referencePersonId: number, targetPersonId: number)
  : Promise<Connection> {
    const findConnectionUrl = '/find-connection';
    const requestOptions = {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        referencePersonId,
        targetPersonId
      })
    };
    const data = await fetch(`${baseUrl}${findConnectionUrl}`, requestOptions);
    return data.json() as unknown as Connection;
};

export const getPerson = async function (id: number)
  : Promise<Person> {
  const getUrl = '/details/people';

  const data = await fetch(`${baseUrl}${getUrl}/${id}`);
  return data.json() as unknown as Person;
};


export const getFilm = async function (id: number)
  : Promise<Film> {
  const getUrl = '/details/films';

  const data = await fetch(`${baseUrl}${getUrl}/${id}`);
  return data.json() as unknown as Film;
};

export const getPlanet = async function (id: number)
  : Promise<Planet> {
  const getUrl = '/details/planets';

  const data = await fetch(`${baseUrl}${getUrl}/${id}`);
  return data.json() as unknown as Planet;
};