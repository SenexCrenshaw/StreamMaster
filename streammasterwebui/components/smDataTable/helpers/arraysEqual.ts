export const arraysEqualByKey = <T>(arr1: T[], arr2: T[], key: keyof T = 'Id' as keyof T): boolean => {
  if (arr1 == null && arr2 == null) return true;
  if (arr1 == null || arr2 == null) return false;

  if (arr1.length !== arr2.length) {
    return false;
  }

  for (let i = 0; i < arr1.length; i++) {
    if (arr1[i][key] !== arr2[i][key]) {
      return false;
    }
  }

  return true;
};

export function arraysEqual<T>(arr1: T[] | undefined, arr2: T[] | undefined): boolean {
  if (arr1 == null && arr2 == null) return true;
  if (arr1 == null || arr2 == null) return false;
  if (arr1.length !== arr2.length) return false;

  for (let i = 0; i < arr1.length; i++) {
    if (arr1[i] !== arr2[i]) return false;
  }
  return true;
}
