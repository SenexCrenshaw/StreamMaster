export function formatJSONDateString(jsonDate: string | undefined): string {
  if (!jsonDate) return '';
  const date = new Date(jsonDate);
  const returnValue = date.toLocaleDateString('en-US', {
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    month: '2-digit',
    second: '2-digit',
    year: 'numeric'
  });

  return returnValue;
}
