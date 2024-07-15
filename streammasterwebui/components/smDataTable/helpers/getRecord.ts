import { Logger } from '@lib/common/logger';

export function getRecord(data: any, fieldName: string): any {
  const keys = fieldName.split('.');
  let currentData = data;
  if (data.Id === null || data.Id === undefined) {
    Logger.debug('SMStreamDataSelector', 'rowClass', 'Id is undefined');
    return '';
  }
  for (const key of keys) {
    if (currentData === undefined || typeof currentData !== 'object') {
      return undefined;
    }
    currentData = currentData[key];
  }

  return currentData;
}

export default getRecord;
