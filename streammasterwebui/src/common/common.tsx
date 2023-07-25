import * as React from 'react';

import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { FormattedMessage, useIntl } from 'react-intl';

import { type VideoStreamDto } from '../store/iptvApi';
import { type ChildVideoStreamDto } from '../store/iptvApi';
import { baseHostURL, isDebug } from '../settings';
import { type StreamMasterSettingResponse } from '../store/signlar/StreamMasterSetting';
import { SMFileTypes } from '../store/streammaster_enums';

export const getTopToolOptions = { autoHide: true, hideDelay: 100, position: 'top', showDelay: 400 } as TooltipOptions;
export const getLeftToolOptions = { autoHide: true, hideDelay: 100, position: 'left', showDelay: 400 } as TooltipOptions;

<FormattedMessage defaultMessage="Stream Master" id="app.title" />;

export function GetMessage(id: string): string {
  const intl = useIntl();
  const message = intl.formatMessage({ id: id });

  return message;
}

export function isChildVideoStreamDto(value: unknown): value is ChildVideoStreamDto {
  // Perform the necessary type checks to determine if 'value' is of type 'ChildVideoStreamDto'
  if (typeof value === 'object' && value !== null) {
    const dto = value as ChildVideoStreamDto;
    return (
      typeof dto.rank !== undefined
    );
  }

  return false;
}

export const GetMessageDiv = (id: string, upperCase?: boolean | null): React.ReactNode => {
  const intl = useIntl();
  const message = intl.formatMessage({ id: id });
  if (upperCase) {
    return <div>{message.toUpperCase()}</div>;
  }

  return <div>{message}</div>;
}

export function areVideoStreamsEqual(
  streams1: ChildVideoStreamDto[] | VideoStreamDto[],
  streams2: ChildVideoStreamDto[] | VideoStreamDto[]
): boolean {
  if (streams1.length !== streams2.length) {
    return false;
  }

  for (let i = 0; i < streams1.length; i++) {
    if (streams1[i].id !== streams2[i].id) {
      return false;
    }

    if (isChildVideoStreamDto(streams1[i]) && isChildVideoStreamDto(streams2[i])) {
      if ((streams1[i] as ChildVideoStreamDto).rank !== (streams2[i] as ChildVideoStreamDto).rank) {
        return false;
      }
    }
  }

  return true;
}


export function isValidUrl(string: string): boolean {
  try {
    new URL(string);
    return true;
  } catch (err) {
    return false;
  }
}


export async function copyTextToClipboard(text: string) {
  if ('clipboard' in navigator) {
    await navigator.clipboard.writeText(text);
    return;
  } else {
    return document.execCommand('copy', true, text);
  }
}

export type PropsComparator<C extends React.ComponentType> = (
  prevProps: Readonly<React.ComponentProps<C>>,
  nextProps: Readonly<React.ComponentProps<C>>,
) => boolean;



export const camel2title = (camelCase: string): string => camelCase
  .replace(/([A-Z])/g, (match) => ` ${match}`)
  .replace(/^./, (match) => match.toUpperCase())
  .trim();

export function formatJSONDateString(jsonDate: string | undefined): string {
  if (!jsonDate) return '';
  const date = new Date(jsonDate);
  const ret = date.toLocaleDateString('en-US', {
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    month: '2-digit',
    second: '2-digit',
    year: 'numeric',
  });

  return ret;
}

function getApiUrl(path: SMFileTypes, originalUrl: string): string {
  return `${isDebug ? baseHostURL : ''}/api/files/${path}/${encodeURIComponent(originalUrl)}`;
}

export const arraysMatch = (arr1: string[], arr2: string[]): boolean => {
  if (arr1.length !== arr2.length) {
    return false;
  }

  // Sort both arrays using localeCompare for proper string comparison
  const sortedArr1 = arr1.slice().sort((a, b) => a.localeCompare(b));
  const sortedArr2 = arr2.slice().sort((a, b) => a.localeCompare(b));

  // Compare the sorted arrays element by element
  for (let i = 0; i < sortedArr1.length; i++) {
    if (sortedArr1[i] !== sortedArr2[i]) {
      return false;
    }
  }

  return true;
}


export function getIconUrl(iconOriginalSource: string, setting: StreamMasterSettingResponse): string {
  if (!iconOriginalSource) {
    iconOriginalSource = `${isDebug ? baseHostURL + '/' : '/'}${setting.defaultIcon}`;
  }

  let originalUrl = iconOriginalSource;

  if (iconOriginalSource.startsWith('/')) {
    iconOriginalSource = iconOriginalSource.substring(1);
  }

  if (iconOriginalSource.startsWith('images/')) {
    iconOriginalSource = `${isDebug ? baseHostURL + '/' : ''}${iconOriginalSource}`;
  } else if (!iconOriginalSource.startsWith('http')) {
    iconOriginalSource = getApiUrl(SMFileTypes.TvLogo, originalUrl);
  } else if (setting.cacheIcon) {
    iconOriginalSource = getApiUrl(SMFileTypes.Icon, originalUrl);
  }

  return iconOriginalSource;
}

export type UserInformation = {
  IsAuthenticated: boolean;
  TokenAge: Date
}
