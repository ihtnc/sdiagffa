"use client";

import React, { KeyboardEvent, MouseEvent, Fragment, useState } from 'react';
import { useShallow } from 'zustand/react/shallow';
import { useDebounceCallback } from 'usehooks-ts';
import { Combobox } from '@headlessui/react';
import { Person } from '@/api/api-client';
import Loader from '@/components/loader';
import useCharacterSearchStoreFactory from './character-search-store-factory';

interface CharacterPickerProps {
  id: string,
  placeholderText?: string,
  searchDelay?: number,
  onSelect?: (selectedItem: Person)
    => void
}

const CharacterPicker = ({
  id,
  placeholderText = 'Search for a character...',
  searchDelay = 1500,
  onSelect
  }: CharacterPickerProps) => {
  const getStore = useCharacterSearchStoreFactory(state => state.getStore);

  const useCharacterSearchStore = getStore(id);

  const {
    search, loadMoreResults,
    isBusy, isSearchDone,
    results, hasMoreResults, count
  } = useCharacterSearchStore(useShallow(state => ({
    search: state.search,
    loadMoreResults: state.loadMoreResults,
    isBusy: state.isBusy,
    isSearchDone: state.isSearchDone,
    results: state.results,
    hasMoreResults: state.hasMoreResults,
    count: state.count
  })))

  const [selectedValue, setSelectedValue] = useState<Person>(null);

  const delayedSearch = useDebounceCallback(search, searchDelay);

  const handleKeyDown = (e: KeyboardEvent<HTMLInputElement>, activeOption: Person | null) => {
    if (e.key !== 'Enter'
      || activeOption === null || activeOption.id !== null) { return; }

    if (!isBusy) { loadMoreResults(); }

    e.preventDefault();
  };

  const handleClick = (e: MouseEvent<HTMLDivElement>) => {
    loadMoreResults();
    e.preventDefault();
  };

  return (
    <div className='flex flex-col items-stretch'>
      <Combobox
        value={selectedValue}
        onChange={(p) => {
          setSelectedValue(p);
          if (onSelect) { onSelect(p); }
        }}
      >
        {({open, activeOption}) => (
          <>
          <div className='flex flex-row'>
            <Combobox.Input
              className='p-2 pr-10 mb-1
                rounded-md
                focus-visible:border-0
                focus-visible:outline-2
                focus-visible:outline-swblue dark:focus-visible:outline-swyellow
                bg-white dark:bg-swblack
                text-swblue dark:text-swyellow
                caret-swblue dark:caret-swyellow'
              onChange={(e) => delayedSearch(e.target.value)}
              onKeyDown={(e) => handleKeyDown(e, activeOption)}
              displayValue={(p: Person) => p?.name}
              placeholder={placeholderText} />

            {(open && isBusy && !isSearchDone) && (
              <Loader className='w-10 p-1 -ml-10 -mt-1
                fill-swblue dark:fill-swyellow' />
            )}
          </div>

          {(open && (isSearchDone && count > 0)) && (
            <div className='relative flex z-10'>
              <div className='absolute
                rounded-md
                border-2
                w-full
                border-swblue dark:border-swyellow'>
                <Combobox.Options static
                  className='p-2
                    rounded-md
                    divide-y divide-y-2
                    bg-swgray/80 dark:bg-swblack/80
                    divide-swblue dark:divide-swyellow
                    '>
                  <div className={`mb-1
                    max-h-64
                    overscroll-contain
                    ${isBusy ? `after:absolute
                      after:top-0 after:left-0
                      after:w-full after:h-full
                      after:opacity-80
                      after:bg-swgray dark:after:bg-swblack
                      after:mouse-events-none
                      after:select-none` : ''}
                    overflow-y-auto`}>
                    {results.map((p) => (
                      <Combobox.Option key={p.id} value={p}
                        as={Fragment}
                        disabled={isBusy}>
                        {({ active }) => (
                          <li
                            className={`
                              cursor-pointer
                              ${active ? 'text-swblue' : 'text-swblack'}
                              ${active ? 'dark:text-swyellow' : 'dark:text-white'}`}>
                            {p.name}
                          </li>
                        )}
                      </Combobox.Option>
                    ))}
                  </div>

                  {hasMoreResults && (
                    <div className='pt-1'>
                      <Combobox.Option key={null} value={{ id: null, name: ''}}
                        disabled={isBusy}
                        className='
                          flex place-content-center
                          w-full h-8
                          cursor-pointer'>
                        {({active}) => (
                          <>
                            {isBusy &&
                              <Loader
                                className='h-8
                                  fill-swblue dark:fill-swyellow'
                              />
                            }

                            {!isBusy &&
                              <div
                                className={`flex
                                  items-center
                                  text-swblack dark:text-white
                                  ${active ? 'text-swblue' : 'text-swblack'}
                                  ${active ? 'dark:text-swyellow' : 'dark:text-white'}
                                  hover:text-swblue dark:hover:text-swyellow`}
                                onClick={handleClick}>
                                More
                              </div>
                            }
                          </>
                        )}
                      </Combobox.Option>
                    </div>
                  )}
                </Combobox.Options>
              </div>
            </div>
          )}
          </>
        )}
      </Combobox>
    </div>
  );
}

export default CharacterPicker;