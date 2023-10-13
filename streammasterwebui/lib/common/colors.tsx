function stringToHash(str: string): number {
  let hash = 0;
  for (let i = 0; i < str.length; i++) {
    const char = str.charCodeAt(i);
    hash = (hash << 5) - hash + char;
    hash = hash & hash; // Convert to 32bit integer
  }

  return Math.abs(hash);
}

export function getColor(text: string): string {
  const hash = stringToHash(text);
  const hue = hash % 360;
  const saturation = (hash % 30) + 70; // Values between 70% and 100%
  const lightness = (hash % 20) + 65; // Values between 65% and 85%
  return `hsl(${hue}, ${saturation}%, ${lightness}%)`;
}

export function getColorByNumber(index: number): string {
  const STEP = 50; // This will determine the number of segments in the hue spectrum
  const segments = Math.floor(360 / STEP);

  // Calculate a hue value that jumps around the spectrum to ensure variation
  const hue = (index * STEP + Math.floor(index / segments) * STEP) % 360;

  return `hsl(${hue}, 100%, 70%)`;
}

export function getColor2(index: number): string {
  const PRIME1 = 137;
  const PRIME2 = 157;
  const hue = (index * PRIME1) % 360;
  const saturation = 90 + ((index * PRIME2) % 10); // Values between 90% and 100%
  const lightness = 65 + (index % 5); // Values between 65% and 70%
  return `hsl(${hue}, ${saturation}%, ${lightness}%)`;
}
