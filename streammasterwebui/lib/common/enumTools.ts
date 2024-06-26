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
