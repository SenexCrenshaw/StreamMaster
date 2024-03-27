import { useCallback } from 'react';

import { getCacheItem } from './utils/getCacheItem';
import { removeCacheItem } from './utils/removeCacheItem';
import { setCacheItem } from './utils/setCacheItem';

function useStorageCache<T>() {
  const setItemCallback = useCallback((key: string, data: T) => setCacheItem(key, data), []);
  const getItemCallback = useCallback((key: string) => getCacheItem<T>(key), []);
  const removeItemCallback = useCallback((key: string) => removeCacheItem(key), []);

  return { setItem: setItemCallback, getItem: getItemCallback, removeItem: removeItemCallback };
}

export default useStorageCache;
