export function getRecord<T, R>(fieldName: string, newData: T): R | undefined {
  const record = fieldName.split('.').reduce((obj, key) => {
    return obj?.[key];
  }, newData as Record<string, any>);

  if (record === undefined || record === null) {
    return undefined;
  }

  return record as R;
}

export function getRecordString<T>(fieldName: string, newData: T): string | undefined {
  const record = getRecord<T, string>(fieldName, newData);
  let toDisplay = JSON.stringify(record);

  if (!toDisplay || toDisplay === undefined || toDisplay === '') {
    return '';
  }

  if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
    toDisplay = toDisplay.substring(1, toDisplay.length - 1);
  }

  return toDisplay;
}

export function updateNestedProperty(obj: Record<string, any>, path: string, value: any) {
  const keys = path.split('.');
  let currentObj = obj;

  for (let i = 0; i < keys.length - 1; i++) {
    const key = keys[i];
    if (!currentObj[key]) {
      currentObj[key] = {};
    }
    currentObj = currentObj[key];
  }

  const lastKey = keys[keys.length - 1];
  currentObj[lastKey] = value;
}
