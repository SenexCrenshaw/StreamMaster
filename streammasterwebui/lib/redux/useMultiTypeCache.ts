import { StringArgument } from '@lib/common/dataTypes';
import { IconFileDto, StationChannelName } from '@lib/iptvApi';
import { useContext } from 'react';
import { MultiTypeCacheContext } from './MultiTypeCacheProvider';

/**
 * Custom hook to interact with multiple types of caches provided by MultiTypeCacheProvider.
 * @param key The key to identify the specific cache data.
 * @param querySelectedItem Function to fetch an item if it's not present in the cache.
 * @param cacheTypeKey Key to identify the type of cache to be used.
 * @returns An object containing methods and data for cache manipulation and retrieval.
 */
export const useMultiTypeCache = (
  key: string,
  querySelectedItem: (argument: StringArgument) => Promise<StationChannelName | IconFileDto | null>,
  cacheTypeKey: 'iconFileDtoCache' | 'stationChannelNameCache' // Extend with more cache types as needed
): {
  cacheData: StationChannelName[] | IconFileDto[];
  updateCache: (data: StationChannelName[] | IconFileDto[]) => void;
  addItem: (item: StationChannelName | IconFileDto) => void;
  removeItem: (predicate: (item: StationChannelName | IconFileDto) => boolean) => void;
  fetchAndAddItem: (arg: StringArgument) => Promise<StationChannelName | IconFileDto | undefined>;
} => {
  // Access the MultiTypeCacheProvider context
  const multiTypeCacheContext = useContext(MultiTypeCacheContext);
  if (!multiTypeCacheContext) {
    throw new Error('useMultiTypeCache must be used within a MultiTypeCacheProvider');
  }

  // Determine the correct cache based on the cacheTypeKey
  const cacheContext = multiTypeCacheContext[cacheTypeKey];
  const isIconSelector = key === 'iconSelector';

  const cacheData = cacheContext.cache.get(key) ?? [];

  const updateCache = (newData: (IconFileDto | StationChannelName)[]) => {
    if (isIconSelector) {
      multiTypeCacheContext.iconFileDtoCache.updateCache(key, newData as unknown as IconFileDto[]);
    } else {
      multiTypeCacheContext.stationChannelNameCache.updateCache(key, newData as unknown as StationChannelName[]);
    }
  };
  // Function to add an item to the cache if it doesn't exist
  const addItem = (item: StationChannelName | IconFileDto) => {
    const itemExists = cacheData.some((existingItem) => existingItem.id === item.id);
    if (!itemExists) {
      const updatedData = [...cacheData, item];
      updateCache(updatedData);
    }
  };

  // Function to remove an item from the cache based on a predicate
  const removeItem = (predicate: (item: StationChannelName | IconFileDto) => boolean) => {
    const updatedData = cacheData.filter((item) => !predicate(item));
    updateCache(updatedData);
  };

  // Function to fetch and add an item to the cache if it's not already present
  const fetchAndAddItem = async (arg: StringArgument): Promise<StationChannelName | IconFileDto | undefined> => {
    const existingItem = cacheData.find((item) => item.id === arg.value);
    if (!existingItem) {
      const newItem = await querySelectedItem(arg);
      if (newItem) {
        addItem(newItem);
        return newItem;
      }
    }
    return existingItem;
  };

  return { cacheData, updateCache, addItem, removeItem, fetchAndAddItem };
};
