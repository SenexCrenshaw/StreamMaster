function getRecord(data: object, fieldName: string) {
  type ObjectKey = keyof typeof data;
  const record = data[fieldName as ObjectKey];
  return record;
}

export default getRecord;
