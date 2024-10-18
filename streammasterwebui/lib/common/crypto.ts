import CryptoJS from 'crypto-js';

/**
 * Generates a cryptographically secure random key of the specified length.
 * @param keySize The size of the key in bits. Default is 128 bits.
 * @returns A Base64-encoded string representing the generated key.
 */
export function generateKey(keySize: number = 128): string {
  const keyBytesSize = keySize / 8;
  const key = CryptoJS.lib.WordArray.random(keyBytesSize);
  return CryptoJS.enc.Base64.stringify(key);
}
