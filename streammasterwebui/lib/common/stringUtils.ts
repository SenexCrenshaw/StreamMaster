export function removeExtension(filename: string): string {
  const lastDotIndex = filename.lastIndexOf('.');
  if (lastDotIndex === -1) {
    return filename; // No extension found
  }
  return filename.substring(0, lastDotIndex);
}
