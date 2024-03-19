import { ReactNode, createContext, useContext } from 'react';

/**
 * Interface for the Cache Context
 */
export interface CacheContextType {
  // updateCache: (key: string, data: any[]) => void;
  // removeFromCache: (key: string) => void;
  // clearCache: () => void;
  isExpired: (timestamp: number) => boolean;
}

/**
 * Creating a context for the cache.
 */
export const CacheContext = createContext<CacheContextType | undefined>(undefined);

const EXPIRATION_TIME = 4 * 24 * 60 * 60 * 1000; // 4 days in milliseconds

export const CacheProvider = ({ children }: { children: ReactNode }) => {
  // const loadCache = (): Map<string, CacheItem<T>> => {
  //   const savedCache = sessionStorage.getItem('cache');
  //   return savedCache ? new Map<string, CacheItem<T>>(JSON.parse(savedCache)) : new Map<string, CacheItem<T>>();
  // };

  // const [cache, setCache] = useState<Map<string, CacheItem<T>>>(loadCache());

  // useEffect(() => {
  //   const loadedCache = loadCache();
  //   setCache(loadedCache);
  // }, []);

  const isExpired = (timestamp: number) => Date.now() - timestamp > EXPIRATION_TIME;

  // const updateCache = (key: string, data: T[]) => {
  //   // Update cache with new data and current timestamp
  //   const newCache = new Map(cache);
  //   newCache.set(key, { data, timestamp: Date.now() });
  //   setCache(newCache);
  //   sessionStorage.setItem('cache', JSON.stringify(Array.from(newCache.entries())));
  // };

  // const removeFromCache = (key: string) => {
  //   const newCache = new Map(cache);
  //   newCache.delete(key);
  //   setCache(newCache);
  //   sessionStorage.setItem('cache', JSON.stringify(Array.from(newCache.entries())));
  // };

  // const clearCache = () => {
  //   const newCache = new Map<string, CacheItem<T>>();
  //   setCache(newCache);
  //   sessionStorage.removeItem('cache'); // Clear the cache in local storage
  // };

  // const checkAndCleanCache = () => {
  //   // Check and clear expired items from the cache
  //   const newCache = new Map(cache);
  //   newCache.forEach((value, key) => {
  //     if (isExpired(value.timestamp)) {
  //       newCache.delete(key);
  //     }
  //   });
  //   return newCache;
  // };

  return <CacheContext.Provider value={{ isExpired }}>{children}</CacheContext.Provider>;
};

/**
 * Custom hook to use the cache.
 * Provides functionalities to interact with the cache.
 *
 * @param key - Key to identify the cache data.
 * @param querySelectedItem - Function to query an item if not present in the cache.
 */
export const useCache = <T extends { id: string | number }>(key: string, querySelectedItem: (argument: { value: string }) => Promise<T | null>) => {
  const context = useContext(CacheContext) as CacheContextType;
  if (context === null) {
    throw new Error('useCache must be used within a CacheProvider');
  }

  const isExpired = context.isExpired;

  // const cacheData = context.cache.get(key)?.data ?? [];

  // // Update the cache
  // const updateCache = (newData: T[]) => context.updateCache(key, newData);

  // // Add an item to the cache
  // const addItem = (item: T) => {
  //   if (!cacheData.some((cachedItem) => cachedItem.id === item.id)) {
  //     updateCache([...cacheData, item]);
  //   }
  // };

  // // Remove an item from the cache based on a predicate
  // const removeItem = (predicate: (item: T) => boolean) => {
  //   updateCache(cacheData.filter((item) => !predicate(item)));
  // };

  // // Fetch and add an item to the cache
  // const fetchAndAddItem = async (arg: { value: string }): Promise<T | undefined> => {
  //   if (cacheData.some((item) => item.id.toString() === arg.value)) {
  //     return cacheData.find((item) => item.id.toString() === arg.value);
  //   } else {
  //     const newItem = await querySelectedItem(arg);
  //     if (newItem) {
  //       addItem(newItem);
  //       return newItem;
  //     }
  //   }
  //   return undefined;
  // };

  return { isExpired };
};
