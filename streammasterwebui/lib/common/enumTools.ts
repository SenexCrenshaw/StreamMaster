export const getEnumKeyByValue = <T>(enumObj: T, value: number): string | null => {
  const entries = Object.entries(enumObj as unknown as Record<string, number>);
  for (const [key, val] of entries) {
    if (val === value) {
      return key;
    }
  }
  return null;
};

export const getEnumValueByKey = <T extends object>(enumType: T, key: keyof T): T[keyof T] => {
  return enumType[key];
};

export function getEnumValueByName<T extends Record<string, any>>(enumType: T, enumName: string): keyof typeof enumType | undefined {
  // Check if the provided name exists in the enum
  if (enumName in enumType) {
    return enumType[enumName as keyof typeof enumType];
  }
  return undefined;
}
