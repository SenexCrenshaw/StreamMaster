import { removeQuotes } from '@lib/common/common';

function getRecordString(data: object, fieldName: string): string {
  const record = data[fieldName as keyof typeof data];

  return record ? removeQuotes(JSON.stringify(record)) : '';
}

export default getRecordString;
