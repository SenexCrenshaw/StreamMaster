import { FieldData, PagedResponse, SMStreamDto } from '@lib/apiDefs';
import { getAllSessionStorageKeys } from '../cacheUtils';
import { getCacheItem } from './getCacheItem';
import { setCacheItem } from './setCacheItem';

export function setCacheField(fieldData: FieldData): void {
  console.log('setCacheField', fieldData);
  const allKeys = getAllSessionStorageKeys();
  const keys = allKeys.filter((key) => key.startsWith(fieldData.entity));

  if (fieldData.entity === 'SMStreamDto') {
    keys.forEach((key) => {
      const item = getCacheItem<PagedResponse<SMStreamDto>>(key);
      if (item) {
        const found = item.data.find((x) => x.id === fieldData.id);
        if (found) {
          console.log('found', found);
          found[fieldData.field] = fieldData.value;
          setCacheItem(key, item);
        }
      }
    });
  }
}
