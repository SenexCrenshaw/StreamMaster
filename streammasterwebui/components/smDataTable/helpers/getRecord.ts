interface getRecordProps {
  data: any;
  fieldName: string;
  dataKey?: string;
}

export function getRecord({ data, fieldName, dataKey = 'Id' }: getRecordProps): any {
  // if (dataKey !== 'Id') {
  //   console.log('dataKey is not Id', dataKey);
  // }
  const keys = fieldName.split('.');
  let currentData = data;
  if (data[dataKey] === null || data[dataKey] === undefined) {
    // Logger.debug('SMStreamDataSelector', 'rowClass', 'Id is undefined');
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
