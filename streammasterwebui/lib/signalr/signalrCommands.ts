import { singletonCacheHandlerListener } from './singletonListeners';
/**
 * Clears a specific item from the cache stored in local storage by key.
 *
 * @param key - The key of the cache item to clear.
 */
function clearCacheItemByKey(key: string): void {
  const cacheRaw = localStorage.getItem('cache');
  if (cacheRaw) {
    const cache = new Map<string, any>(JSON.parse(cacheRaw));

    if (cache.has(key)) {
      cache.delete(key);
      localStorage.setItem('cache', JSON.stringify(Array.from(cache.entries())));
    }
  }
}

function updateCachedDataWithResults(data: any): void {
  console.log(data);
  if (data === 'epgSelector') {
    clearCacheItemByKey(data);
  }
}

export function AddConnections() {
  singletonCacheHandlerListener.addListener(updateCachedDataWithResults);
}

export function RemoveConnections() {
  singletonCacheHandlerListener.removeListener(updateCachedDataWithResults);
}
