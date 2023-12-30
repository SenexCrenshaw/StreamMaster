import { HasId } from '@lib/common/common';
import { StringArgument } from '@lib/common/dataTypes';
import { ReactNode, createContext, useContext, useState } from 'react';

interface CacheContextType<T> {
  cache: Map<string, T[]>;
  updateCache: (key: string, data: T[]) => void;
  removeFromCache: (key: string) => void;
}
const CacheContext = createContext<CacheContextType<any> | null>(null);

export const CacheProvider = <T extends unknown>({ children }: { children: ReactNode }) => {
  const [cache, setCache] = useState<Map<string, T[]>>(new Map());

  const updateCache = (key: string, data: T[]) => {
    setCache(new Map(cache.set(key, data)));
  };

  const removeFromCache = (key: string) => {
    const newCache = new Map(cache);
    newCache.delete(key);
    setCache(newCache);
  };

  return <CacheContext.Provider value={{ cache, updateCache, removeFromCache }}>{children}</CacheContext.Provider>;
};

export const useCache = <T extends HasId>(
  key: string,
  querySelectedItem: (argument: StringArgument) => Promise<T | null>
): {
  cacheData: T[];
  updateCache: (data: T[]) => void;
  addItem: (item: T) => void;
  removeItem: (predicate: (item: T) => boolean) => void;
  fetchAndAddItem: (arg: StringArgument) => Promise<T | undefined>;
} => {
  const context = useContext(CacheContext) as CacheContextType<T>;
  if (context === null) {
    throw new Error('useCache must be used within a CacheProvider');
  }

  // Determine the cache type based on the key
  const isIconSelector = key === 'iconSelector';
  const cache = context.cache;

  // Cast the cache to the appropriate type
  const cacheData = cache.get(key) ?? [];

  const updateCache = (newData: T[]) => context.updateCache(key, newData);

  const addItem = (item: T) => {
    if (!itemExistsByValue(item.id)) {
      const updatedData = [...cacheData, item];
      updateCache(updatedData);
    }
  };

  const removeItem = (predicate: (item: T) => boolean) => {
    const updatedData = cacheData.filter((item) => !predicate(item));
    updateCache(updatedData);
  };

  const itemExistsByValue = (toMatch: string | number) => {
    if (cacheData === undefined || cacheData.length === 0) {
      return false;
    }

    let existingItem = false;

    if (isIconSelector) {
      existingItem = cacheData.some((item) => item.source.toString() === toMatch);
    } else {
      existingItem = cacheData.some((item) => item.id.toString() === toMatch);
    }

    return existingItem;
  };

  const fetchAndAddItem = async (arg: StringArgument): Promise<T | undefined> => {
    // Check if the item already exists in the cache and return it if found
    if (itemExistsByValue(arg.value)) {
      if (isIconSelector) {
        return cacheData.find((item) => item.source === arg.value);
      }
      return cacheData.find((item) => item.id.toString() === arg.value);
    } else {
      // Item is not in the cache, fetch it
      const newItem = await querySelectedItem(arg);
      if (newItem) {
        if (!itemExistsByValue(arg.value)) {
          addItem(newItem);
        }
        return newItem; // Return the new item
      }
    }
    return undefined; // Return undefined if the item is not found and cannot be fetched
  };

  return { cacheData, updateCache, addItem, removeItem, fetchAndAddItem };
};
