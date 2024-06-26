import getRecord from '../helpers/getRecord';

export function isHiddenTemplate(data: object, fieldName: string) {
  const record = getRecord(data, fieldName);

  return record ? <i className="pi pi-eye-slash text-blue-500" /> : <i className="pi pi-eye text-green-500" />;
}
