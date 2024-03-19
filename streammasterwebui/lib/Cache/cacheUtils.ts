export const getAllSessionStorageKeys = (): string[] => {
  const keys: string[] = [];
  for (let i = 0; i < sessionStorage.length; i++) {
    const key = sessionStorage.key(i);
    if (key !== null) {
      keys.push(key);
    }
  }
  return keys;
};
