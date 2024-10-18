// Generic function to get a nested property value
export function getRecord<T, R>(fieldName: string, newData: T): R | undefined {
  const record = fieldName.split('.').reduce((obj, key) => obj?.[key], newData as Record<string, any>);

  return record as R | undefined;
}

// Function to get a nested property value as a string
export function getRecordString<T>(fieldName: string, data: T): string | undefined {
  const record = getRecord<T, string>(fieldName, data);
  let toDisplay = JSON.stringify(record);

  if (!toDisplay || toDisplay === 'null' || toDisplay === 'undefined' || toDisplay === '') {
    return '';
  }

  if (toDisplay.startsWith('"') && toDisplay.endsWith('"')) {
    toDisplay = toDisplay.slice(1, -1);
  }

  return toDisplay;
}
