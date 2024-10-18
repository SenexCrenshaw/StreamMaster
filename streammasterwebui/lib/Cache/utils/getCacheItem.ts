import { CacheItem } from '@lib/apiDefs';

export function getCacheItem<T>(key: string): T | null {
  const item = sessionStorage.getItem(key);
  if (!item) {
    return null;
  }
  try {
    const parsedItem: CacheItem<T> = JSON.parse(item);
    return parsedItem.data;
  } catch {
    console.error('Failed to parse the stored item.');
    return null;
  }
}
