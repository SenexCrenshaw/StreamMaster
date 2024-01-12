function stringToHash(string_: string): number {
  let hash = 0;
  for (let index = 0; index < string_.length; index++) {
    const char = string_.charCodeAt(index);
    hash = (hash << 5) - hash + char;
    hash &= hash; // Convert to 32bit integer
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

function hslToRgb(h: number, s: number, l: number): [number, number, number] {
  s /= 100;
  l /= 100;
  let c = (1 - Math.abs(2 * l - 1)) * s;
  let x = c * (1 - Math.abs(((h / 60) % 2) - 1));
  let m = l - c / 2;
  let r = 0;
  let g = 0;
  let b = 0;

  if (0 <= h && h < 60) {
    r = c;
    g = x;
    b = 0;
  } else if (60 <= h && h < 120) {
    r = x;
    g = c;
    b = 0;
  } else if (120 <= h && h < 180) {
    r = 0;
    g = c;
    b = x;
  } else if (180 <= h && h < 240) {
    r = 0;
    g = x;
    b = c;
  } else if (240 <= h && h < 300) {
    r = x;
    g = 0;
    b = c;
  } else if (300 <= h && h < 360) {
    r = c;
    g = 0;
    b = x;
  }
  r = Math.round((r + m) * 255);
  g = Math.round((g + m) * 255);
  b = Math.round((b + m) * 255);

  return [r, g, b];
}

function rgbToHex(r: number, g: number, b: number): string {
  const hex = (r << 16) + (g << 8) + b;
  return '#' + hex.toString(16).padStart(6, '0');
}

export function getColorHex(index: number): string {
  const hsl = getColorByNumber(index).match(/(\d+)/g);
  if (hsl) {
    const [h, s, l] = hsl.map(Number);
    const [r, g, b] = hslToRgb(h, s, l);
    return rgbToHex(r, g, b);
  }
  return '#000000'; // Fallback color
}

export function getColor2(index: number): string {
  const PRIME1 = 137;
  const PRIME2 = 157;
  const hue = (index * PRIME1) % 360;
  const saturation = 90 + ((index * PRIME2) % 10); // Values between 90% and 100%
  const lightness = 65 + (index % 5); // Values between 65% and 70%
  return `hsl(${hue}, ${saturation}%, ${lightness}%)`;
}
