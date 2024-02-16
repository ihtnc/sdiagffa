import { create } from 'zustand';
import { findConnection, Connection } from '@/api/api-client';

interface FindConnectionStore {
  isBusy: boolean,
  isSearchDone: boolean,
  findConnection: (referencePersonId: number, targetPersonId: number) => Promise<void>,

  results: Connection | null
}

const useFindConnection = create<FindConnectionStore>((set) => ({
  isBusy: false,
  isSearchDone: false,
  results: null,

  findConnection: async (referencePersonId: number, targetPersonId: number) => {
    set({
      isBusy: true,
      isSearchDone: false,
      results: null
    });

    const response = await findConnection(referencePersonId, targetPersonId);

    set({
      isBusy: false,
      isSearchDone: true,
      results: response
    });
  }
}));

export default useFindConnection;