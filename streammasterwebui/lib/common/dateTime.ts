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

export const getElapsedTimeString = (start: string, stop: string, noms: boolean = false): string => {
  const startDate = new Date(start);

  let stopDate = new Date(stop);

  // Check if the stop time is less than the start time
  if (stopDate.getTime() < startDate.getTime()) {
    // startDate.setMilliseconds(0);
    stopDate = new Date();
  }

  const elapsedMs = stopDate.getTime() - startDate.getTime();

  const milliseconds = elapsedMs % 1000;
  const totalSeconds = Math.floor(elapsedMs / 1000);
  const seconds = totalSeconds % 60;
  const totalMinutes = Math.floor(totalSeconds / 60);
  const minutes = totalMinutes % 60;
  const hours = Math.floor(totalMinutes / 60);

  if (noms === true) {
    return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;
  }
  // Format to "hh:mm:ss:ms"
  return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}:${String(milliseconds).padStart(3, '0')}`;
};
