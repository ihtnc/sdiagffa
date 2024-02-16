import { Connection } from "@/api/api-client";
import Loader from "@/components/loader";
import { useEffect } from "react";
import useFindConnection from "./find-connection-store";

interface ConnectionFinderProps {
  referencePersonId: number,
  targetPersonId: number,
  onSearchComplete?: (results: Connection) => void
}

const ConnectionFinder = ({
  referencePersonId,
  targetPersonId,
  onSearchComplete
}: ConnectionFinderProps) => {
  const findConnection = useFindConnection((state) => state.findConnection);
  const isBusy = useFindConnection((state) => state.isBusy);
  const isSearchDone = useFindConnection((state) => state.isSearchDone);
  const results = useFindConnection((state) => state.results)

  useEffect(() => {
    if (isSearchDone && onSearchComplete) {
      onSearchComplete(results);
    }
  }, [isSearchDone]);

  return (
    <button
      className='rounded-md
        p-2
        w-60
        text-white dark:text-swblack
        bg-swblue dark:bg-swyellow
        flex place-content-center'
      disabled={isBusy}
      onClick={() => findConnection(referencePersonId, targetPersonId)}>
        {isBusy &&
          <Loader
            className='h-8 fill-swgray dark:fill-swblack'
            />}

        {!isBusy &&
          <div className='h-8 flex items-center font-semibold'>
            Find Connection
          </div>}
    </button>
  );
};

export default ConnectionFinder;