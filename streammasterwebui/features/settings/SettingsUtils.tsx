import { SettingDto, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

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

// Function to update a nested property value
export function updateNestedProperty(obj: Record<string, any>, path: string, value: any): void {
  const keys = path.split('.');
  let currentObj = obj;

  keys.slice(0, -1).forEach((key) => {
    if (!currentObj[key]) {
      currentObj[key] = {};
    }
    currentObj = currentObj[key];
  });

  currentObj[keys[keys.length - 1]] = value;
}

type UpdateChangesProps = {
  field: string;
  warning?: string | null;
  currentSettingRequest: SettingDto;
  onChange: (existing: SettingDto, updatedValues: UpdateSettingRequest) => void;
  value: boolean | string | number | null;
};

// Component to update settings and call onChange handler with updated values
export function UpdateChanges({ field, currentSettingRequest, onChange, value }: UpdateChangesProps): void {
  const updatedSettingDto: SettingDto = {
    ...currentSettingRequest,
    SDSettings: {
      ...currentSettingRequest.SDSettings
    }
  };

  updateNestedProperty(updatedSettingDto, field, value);

  const updateRequest: UpdateSettingRequest = {} as UpdateSettingRequest;
  updateNestedProperty(updateRequest, field, value);

  onChange(updatedSettingDto, updateRequest);
}
