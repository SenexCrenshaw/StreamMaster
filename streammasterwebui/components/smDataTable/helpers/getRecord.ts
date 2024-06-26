export function getRecord(data: any, fieldName: string): any {
  const keys = fieldName.split('.');
  let currentData = data;

  for (const key of keys) {
    if (currentData === undefined || typeof currentData !== 'object') {
      return undefined;
    }
    currentData = currentData[key];
  }

  return currentData;
}

export default getRecord;
