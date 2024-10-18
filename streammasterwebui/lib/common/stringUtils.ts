export function removeExtension(filename: string): string {
  return filename.replace(/\.[^.]+/g, '');
}
