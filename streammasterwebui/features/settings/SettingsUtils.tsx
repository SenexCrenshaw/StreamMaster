import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

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

type UpdateChangesProps = {
  field: string;
  warning?: string | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void | undefined;
  value: boolean | string | number | null;
};

export function UpdateChanges({ field, currentSettingRequest, onChange, value }: UpdateChangesProps) {
  let toReturn: UpdateSettingRequest = {};
  let updatedSettingDto = {} as SettingDto;

  if (currentSettingRequest?.SDSettings !== undefined) {
    updatedSettingDto = { ...currentSettingRequest, SDSettings: { ...currentSettingRequest.SDSettings } };
    updateNestedProperty(updatedSettingDto, field, value);
  }

  updateNestedProperty(toReturn, field, value);
  onChange(updatedSettingDto, toReturn);
}
