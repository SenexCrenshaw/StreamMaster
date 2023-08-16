import * as React from 'react';

import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import { FormattedMessage, useIntl } from 'react-intl';

import { type IconSimpleDto } from '../store/iptvApi';
import { type VideoStreamDto } from '../store/iptvApi';
import { type ChildVideoStreamDto } from '../store/iptvApi';
import { baseHostURL, isDebug } from '../settings';
import { SMFileTypes } from '../store/streammaster_enums';
import { type DataTableFilterMeta } from 'primereact/datatable';

export const getTopToolOptions = { autoHide: true, hideDelay: 100, position: 'top', showDelay: 400 } as TooltipOptions;
export const getLeftToolOptions = { autoHide: true, hideDelay: 100, position: 'left', showDelay: 400 } as TooltipOptions;

<FormattedMessage defaultMessage="Stream Master" id="app.title" />;

export function areFilterMetaEqual(a: DataTableFilterMeta, b: DataTableFilterMeta): boolean {
  const aKeys = Object.keys(a);
  const bKeys = Object.keys(b);

  // Compare if both objects have the same keys
  if (aKeys.length !== bKeys.length) {
    return false;
  }

  for (const key of aKeys) {
    if (!b[key]) {
      return false; // Key doesn't exist in 'b'
    }

    const aData = a[key] as DataTableFilterMetaData;
    const bData = b[key] as DataTableFilterMetaData;

    // Compare 'matchMode'
    if (aData.matchMode !== bData.matchMode) {
      return false;
    }

    // Compare 'value' (this assumes a simple comparison; for deep object comparison, consider using lodash's isEqual or similar)
    if (aData.value !== bData.value) {
      return false;
    }
  }

  return true;
}

export function GetMessage(id: string): string {
  const intl = useIntl();
  const message = intl.formatMessage({ id: id });

  return message;
}

export type MatchMode = 'between' | 'channelGroups' | 'contains' | 'custom' | 'dateAfter' | 'dateBefore' | 'dateIs' | 'dateIsNot' | 'endsWith' | 'equals' | 'gt' | 'gte' | 'in' | 'lt' | 'lte' | 'notContains' | 'notEquals' | 'startsWith' | undefined;

export function addOrUpdateValueForField(
  data: DataTableFilterMetaData[],
  targetFieldName: string,
  matchMode: MatchMode,
  newValue: string
): void {

  // let itemFound = false;

  // data.forEach(item => {
  //   if (item.fieldName === targetFieldName) {
  //     item.matchMode = matchMode;
  //     item.value = newValue;
  //     item.valueType = typeof newValue;
  //     itemFound = true;
  //   }
  // });

  // if (!itemFound) {
  data.push({
    fieldName: targetFieldName,
    matchMode: matchMode,
    value: newValue,
    valueType: typeof newValue,
  });
  // }
}

export function areDataTableFilterMetaDataEqual(a: DataTableFilterMetaData, b: DataTableFilterMetaData): boolean {
  // Compare simple string properties
  if (a.fieldName !== b.fieldName) return false;
  if (a.matchMode !== b.matchMode) return false;
  if (a.valueType !== b.valueType) return false;

  // Deep comparison of 'value'. This assumes simple equality check; for objects or arrays, you might need a deeper comparison.
  if (a.value !== b.value) return false;

  return true;
}

export function areDataTableFilterMetaDatasEqual(arr1: DataTableFilterMetaData[], arr2: DataTableFilterMetaData[]): boolean {
  if (arr1.length !== arr2.length) return false;

  for (let i = 0; i < arr1.length; i++) {
    if (!areDataTableFilterMetaDataEqual(arr1[i], arr2[i])) {
      return false;
    }
  }

  return true;
}

export function compareIconSimpleDto(a: IconSimpleDto, b: IconSimpleDto): number {
  // Compare by id
  if (a.id !== undefined && b.id !== undefined) {
    if (a.id < b.id) return -1;
    if (a.id > b.id) return 1;
  }

  // Compare by source
  if (a.source !== undefined && b.source !== undefined) {
    const sourceComparison = a.source.localeCompare(b.source);
    if (sourceComparison !== 0) return sourceComparison;
  }

  // Compare by name
  if (a.name !== undefined && b.name !== undefined) {
    return a.name.localeCompare(b.name);
  }

  return 0;
}

export function areIconSimpleDtosEqual(array1: IconSimpleDto[], array2: IconSimpleDto[]): boolean {
  if (array1.length !== array2.length) {
    return false;
  }

  for (let i = 0; i < array1.length; i++) {
    if (compareIconSimpleDto(array1[i], array2[i]) !== 0) {
      return false;
    }
  }

  return true;
}

export type DataTableFilterMetaData = {
  fieldName: string;
  matchMode: MatchMode;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  value: any;
  valueType: string;
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

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function checkData(data: any): boolean {
  if (data === null || data === undefined || data.data === null || data.data === undefined) {
    return false;
  }

  return true;

}


export function getIconUrl(iconOriginalSource: string | null | undefined, defaultIcon: string, cacheIcon: boolean): string {
  if (!iconOriginalSource || iconOriginalSource === '') {
    iconOriginalSource = `${isDebug ? baseHostURL + '/' : '/'}${defaultIcon}`;
  }

  let originalUrl = iconOriginalSource;

  if (iconOriginalSource.startsWith('/')) {
    iconOriginalSource = iconOriginalSource.substring(1);
  }

  if (iconOriginalSource.startsWith('images/')) {
    iconOriginalSource = `${isDebug ? baseHostURL + '/' : ''}${iconOriginalSource}`;
  } else if (!iconOriginalSource.startsWith('http')) {
    iconOriginalSource = getApiUrl(SMFileTypes.TvLogo, originalUrl);
  } else if (cacheIcon) {
    iconOriginalSource = getApiUrl(SMFileTypes.Icon, originalUrl);
  }

  return iconOriginalSource;
}

export type UserInformation = {
  IsAuthenticated: boolean;
  TokenAge: Date
}
