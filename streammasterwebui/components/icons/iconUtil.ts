import { baseHostURL, isDev } from '@lib/settings';

function devUrl(url: string): string {
  return isDev ? `${baseHostURL}${url}` : url;
}

export const SMLogo = '/images/streammaster_logo.png';

export function getIconUrl(source?: string): string {
  if (!source) {
    return SMLogo;
  }

  //   if (!source.startsWith('http')) {
  //     source = devUrl(source);
  //   }
  return devUrl(source);
}
