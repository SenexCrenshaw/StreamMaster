import { useCallback, useContext } from 'react';
import { CacheContext, CacheContextType } from './CacheProvider';
import { getCacheItem } from './utils/getCacheItem';
import { removeCacheItem } from './utils/removeCacheItem';
import { setCacheItem } from './utils/setCacheItem';

function useStorageCache<T>() {
  const setItemCallback = useCallback((key: string, data: T) => setCacheItem(key, data), []);
  const getItemCallback = useCallback((key: string) => getCacheItem<T>(key), []);
  const removeItemCallback = useCallback((key: string) => removeCacheItem(key), []);

  const context = useContext(CacheContext) as CacheContextType;
  if (context === null) {
    throw new Error('useStorageCache must be used within a CacheProvider');
  }
  const isExpired = context.isExpired;

  return { setItem: setItemCallback, getItem: getItemCallback, removeItem: removeItemCallback, isExpired };
}

export default useStorageCache;
