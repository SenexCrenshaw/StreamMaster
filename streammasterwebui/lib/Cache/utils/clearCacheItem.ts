import { getAllSessionStorageKeys } from '../cacheUtils';
import { removeCacheItem } from './removeCacheItem';

export function clearCacheItem(keyPrefix: string): void {
  try {
    const allKeys = getAllSessionStorageKeys(); // Retrieve all cache keys
    const keysToClear = allKeys.filter((key) => key.startsWith(keyPrefix));

    keysToClear.forEach((key) => {
      removeCacheItem(key);
    });
  } catch (error) {
    // setError(error instanceof Error ? error : new Error('An error occurred during cache clearing'));
  }
}
