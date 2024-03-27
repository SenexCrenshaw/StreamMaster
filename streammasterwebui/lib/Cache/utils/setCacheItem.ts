import { CacheItem } from '@lib/apiDefs';

export function setCacheItem<T>(key: string, data: T): void {
  const cacheItem: CacheItem<T> = {
    data,
    timestamp: Date.now()
  };
  sessionStorage.setItem(key, JSON.stringify(cacheItem));
}
