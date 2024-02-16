import { create, StoreApi, UseBoundStore } from 'zustand';
import { Person, searchPeople, customPeopleSearch } from '@/api/api-client';

interface CharacterSearchStore {
  isBusy: boolean,

  isSearchDone: boolean,
  search: (search: string) => Promise<void>,

  hasMoreResults: boolean,
  loadMoreUrl: string | null,
  loadMoreResults: () => Promise<void>,

  count: number,
  results: Array<Person>
}

interface CharacterSearchStoreFactory {
  stores: object,
  getStore: (storeId: string) => UseBoundStore<StoreApi<CharacterSearchStore>>
}

const initialiseStore = () => create<CharacterSearchStore>((set, get) => ({
  isBusy: false,
  isSearchDone: false,

  hasMoreResults: false,
  loadMoreUrl: null,

  count: 1,
  results: [],

  search: async (search: string) => {
    set({
      isBusy: true,
      isSearchDone: false,

      hasMoreResults: false,
      loadMoreUrl: null,

      count: 0,
      results: []
    });

    const response = await searchPeople(search);

    set({
      isBusy: false,
      isSearchDone: true,

      hasMoreResults: response.nextPageUrl != null,
      loadMoreUrl: response.nextPageUrl,

      count: response.count,
      results: response.results
    });
  },

  loadMoreResults: async () => {
    const {
      hasMoreResults,
      loadMoreUrl,
      results
     } = get();

    if (hasMoreResults === false) { return; }

    set({ isBusy: true });
    const response = await customPeopleSearch(loadMoreUrl);

    set({
      isBusy: false,

      hasMoreResults: response.nextPageUrl != null,
      loadMoreUrl: response.nextPageUrl,

      count: response.count,
      results: results.concat(response.results)
    });
  }
}));

const useCharacterSearchStoreFactory = create<CharacterSearchStoreFactory>((set, get) => ({
  stores: {},

  getStore: (storeId: string) => {
    const { stores } = get();
    if (stores[storeId] !== undefined) { return stores[storeId]; }

    const store = initialiseStore();
    stores[storeId] = store;
    set({ stores });
    return store;
  }
}));

export default useCharacterSearchStoreFactory;