export const propertyExists = <T extends object>(obj: T, key: keyof any): key is keyof T => {
  if (obj === undefined) {
    return false;
  }
  return key in obj;
};
