/* eslint-disable @typescript-eslint/no-explicit-any */
import { type ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import ExportButton from '@components/export/ExportButton';
import GlobalSearch from '@components/search/GlobalSearch';
import { SMFileTypes } from '@lib/common/streammaster_enums';

import { FetchBaseQueryError } from '@reduxjs/toolkit/query';
import { Checkbox } from 'primereact/checkbox';
import { type DataTableFilterMeta, type DataTableFilterMetaData } from 'primereact/datatable';
import { type TooltipOptions } from 'primereact/tooltip/tooltipoptions';
import * as React from 'react';
import { FormattedMessage, useIntl } from 'react-intl';
import { baseHostURL, isDev as isDevelopment } from '../settings';
import { getColor } from './colors';

export function isFetchBaseQueryError(error: unknown): error is FetchBaseQueryError {
  return (error as FetchBaseQueryError).data !== undefined;
}

export function formatToFourDigits(number_: number): string {
  // Convert number to string
  let string_ = number_.toString();

  // Ensure the number is 1 or 2 digits
  if (string_.length > 2) {
    throw new Error('Input number should be 1 or 2 digits.');
  }

  // Adjust the number to be in the center of four digits
  if (string_.length === 1) {
    string_ = `0${string_}00`;
  } else if (string_.length === 2) {
    string_ += '00';
  }

  return string_;
}

export const getTopToolOptions = {
  autoHide: true,
  hideDelay: 100,
  position: 'top',
  showDelay: 400
} as TooltipOptions;
export const getLeftToolOptions = {
  autoHide: true,
  hideDelay: 100,
  position: 'left',
  showDelay: 400
} as TooltipOptions;
export const getRightToolOptions = {
  autoHide: true,
  hideDelay: 100,
  position: 'right',
  showDelay: 400
} as TooltipOptions;
<FormattedMessage defaultMessage="Stream Master" id="app.title" />;

export function compareStationPreviews(source: StationPreview[], changes: StationPreview[]): { added: StationPreview[]; removed: StationPreview[] } {
  const added: StationPreview[] = [];
  const removed: StationPreview[] = [];

  // Find added items by comparing the id property
  for (const change of changes) {
    if (!source.some((src) => src.id === change.id)) {
      added.push(change);
    }
  }

  // Find removed items by comparing the id property
  for (const src of source) {
    if (!changes.some((change) => change.id === src.id)) {
      removed.push(src);
    }
  }

  return { added, removed };
}

export const hasValidAdditionalProps = (additionalFilterProperties: AdditionalFilterProperties | undefined) => additionalFilterProperties?.values;

export function getChannelGroupMenuItem(colorIndex: string | undefined, toDisplay: string): React.ReactNode {
  return (
    <div className="gap-2">
      <div>
        <i className="pi pi-circle-fill pr-2" style={{ color: getColor(colorIndex ?? '') }} />
        {toDisplay}
      </div>
    </div>
  );
}
export type MatchMode =
  | 'between'
  | 'contains'
  | 'custom'
  | 'dateAfter'
  | 'dateBefore'
  | 'dateIs'
  | 'dateIsNot'
  | 'endsWith'
  | 'equals'
  | 'gt'
  | 'gte'
  | 'in'
  | 'lt'
  | 'lte'
  | 'notContains'
  | 'notEquals'
  | 'startsWith'
  | undefined;

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

    const aData = a[key] as SMDataTableFilterMetaData;
    const bData = b[key] as SMDataTableFilterMetaData;

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

export function toCamelCase(string_: string): string {
  return string_
    .trim()
    .split(/[\s_-]+/)
    .map((word, index) => (index === 0 ? word.toLowerCase() : word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()))
    .join('');
}

// export function GetMessagea(id: string): string {
//   const intl = useIntl();
//   const message = intl.formatMessage({ id: id });

//   return message;
// }

export function GetMessage(...arguments_: string[]): string {
  const intl = useIntl();

  if (arguments_ === undefined || arguments_.length === 0 || arguments_[0] === '') {
    return '';
  }
  const ids: string[] = arguments_.flatMap((argument) => argument.split(' '));

  const message = ids.map((x) => intl.formatMessage({ id: x })).join(' ');

  if (message === toCamelCase(message)) {
    return arguments_.join('');
  }

  return message;
}

export interface AdditionalFilterProperties {
  field: string;
  matchMode: MatchMode;
  values: string[] | undefined;
}

export function areAdditionalFilterPropsEqual(a: AdditionalFilterProperties | undefined, b: AdditionalFilterProperties | undefined): boolean {
  // If both are undefined, return true
  if (!a && !b) return true;

  // If only one of them is undefined, return false
  if (!a || !b) return false;

  // Check if fields are the same
  if (a.field !== b.field) return false;

  // Check if matchModes are the same
  if (a.matchMode !== b.matchMode) return false;

  // If values are undefined, treat them as empty arrays for comparison
  const aValues = a.values ?? [];
  const bValues = b.values ?? [];

  // Check if values array lengths are the same
  if (aValues.length !== bValues.length) return false;

  // Check if all values are the same
  for (const [index, aValue] of aValues.entries()) {
    if (aValue !== bValues[index]) return false;
  }

  // If all checks passed, the objects are equivalent
  return true;
}

export function removeValueForField(data: SMDataTableFilterMetaData[], targetFieldName: string): void {
  const index = data.findIndex((item) => item.fieldName === targetFieldName);

  if (index !== -1) {
    data.splice(index, 1);
  }
}

export function addOrUpdateValueForField(
  data: SMDataTableFilterMetaData[],
  targetFieldName: string,
  matchMode: MatchMode,
  newValue: boolean | string | null | undefined
): void {
  let itemFound = false;

  for (const item of data) {
    if (item.fieldName === targetFieldName) {
      item.matchMode = matchMode;
      item.value = newValue;
      itemFound = true;
    }
  }

  if (!itemFound) {
    data.push({
      fieldName: targetFieldName,
      matchMode: matchMode as MatchMode,
      value: newValue
    });
  }
}

export function areDataTableFilterMetaDataEqual(a: SMDataTableFilterMetaData, b: SMDataTableFilterMetaData): boolean {
  // Compare simple string properties
  if (a.fieldName !== b.fieldName) return false;
  if (a.matchMode !== b.matchMode) return false;

  // Deep comparison of 'value'. This assumes simple equality check; for objects or arrays, you might need a deeper comparison.
  if (a.value !== b.value) return false;

  return true;
}

export function areDataTableFilterMetaDatasEqual(array1: SMDataTableFilterMetaData[], array2: SMDataTableFilterMetaData[]): boolean {
  if (array1.length !== array2.length) return false;

  for (const [index, element] of array1.entries()) {
    if (!areDataTableFilterMetaDataEqual(element, array2[index])) {
      return false;
    }
  }

  return true;
}

export interface SimpleQueryApiArgument {
  count?: number;
  first?: number;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
  last?: number;
  name?: string;
  orderBy?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface IDIsHidden {
  id: string;
  isHidden: boolean;
}

export interface HasId {
  [key: string]: any;
  id: number | string;
}

export function compareIconFileDto(a: IconFileDto, b: IconFileDto): number {
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

export function arraysContainSameStrings(array1: string[] | undefined, array2: string[] | undefined): boolean {
  if (!array1 || !array2) return false;

  // If the arrays are not of the same length, they can't contain the same strings
  if (array1.length !== array2.length) return false;

  // Sort both arrays and compare them

  const sortedArray1 = [...array1].sort();

  const sortedArray2 = [...array2].sort();

  for (const [index, element] of sortedArray1.entries()) {
    if (element !== sortedArray2[index]) return false;
  }

  return true;
}

export function areIconFileDtosEqual(array1: IconFileDto[], array2: IconFileDto[]): boolean {
  if (array1.length !== array2.length) {
    return false;
  }

  for (const [index, element] of array1.entries()) {
    if (compareIconFileDto(element, array2[index]) !== 0) {
      return false;
    }
  }

  return true;
}

export type SMDataTableFilterMetaData = DataTableFilterMetaData & {
  fieldName: string;
  matchMode: MatchMode;
};

export const doSetsContainSameIds = (set1: Set<number | string>, set2: Set<number | string>): boolean => {
  if (set1.size !== set2.size) return false;

  for (const id of set1) {
    if (!set2.has(id)) return false;
  }

  return true;
};

export function isVideoStreamDto(value: unknown): value is VideoStreamDto {
  // Perform the necessary type checks to determine if 'value' is of type 'VideoStreamDto'
  if (typeof value === 'object' && value !== null) {
    const dto = value as VideoStreamDto;

    return dto.rank !== undefined;
  }

  return false;
}

export const GetMessageDiv = (id: string, upperCase?: boolean | null): React.ReactNode => {
  const intl = useIntl();
  const message = intl.formatMessage({ id });

  if (upperCase) {
    return <div>{message.toUpperCase()}</div>;
  }

  return <div>{message}</div>;
};

export function areVideoStreamsEqual(streams1: VideoStreamDto[] | VideoStreamDto[], streams2: VideoStreamDto[] | VideoStreamDto[]): boolean {
  if (streams1.length !== streams2.length) {
    return false;
  }

  for (const [index, element] of streams1.entries()) {
    if (element.id !== streams2[index].id) {
      return false;
    }

    if (isVideoStreamDto(element) && isVideoStreamDto(streams2[index]) && (element as VideoStreamDto).rank !== (streams2[index] as VideoStreamDto).rank) {
      return false;
    }
  }

  return true;
}

export function isValidUrl(string: string): boolean {
  try {
    new URL(string);

    return true;
  } catch {
    return false;
  }
}

// export async function copyTextToClipboard(text: string) {
//   if ('clipboard' in navigator) {
//     await navigator.clipboard.writeText(text);

//     return;
//   } else {
//     return document.execCommand('copy', true, text);
//   }
// }

export interface PropertiesComparator<C extends React.ComponentType> {
  (previousProperties: Readonly<React.ComponentProps<C>>, nextProperties: Readonly<React.ComponentProps<C>>): boolean;
}

export const camel2title = (camelCase: string): string =>
  camelCase
    .replaceAll(/([A-Z])/g, (match) => ` ${match}`)
    .replace(/^./, (match) => match.toUpperCase())
    .trim();

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

export const FormatDuration = ({ duration }: { duration: string | undefined }) => {
  console.log('duration', duration);
  if (!duration) return <div />;
  // Parse the duration
  // Split the duration string into days and time parts
  const [days, time] = duration.split('.');
  const [hours, minutes, seconds] = time.split(':');

  // Extract the whole seconds part (before the decimal point in seconds)
  const wholeSeconds = seconds.split('.')[0];

  // Format the duration
  const formattedDuration = `${parseInt(days, 10)} ${hours.padStart(2, '0')}:${minutes.padStart(2, '0')}:${wholeSeconds.padStart(2, '0')}`;

  return <div>{formattedDuration}</div>;
};

function getApiUrl(path: SMFileTypes, originalUrl: string): string {
  return `${isDevelopment ? baseHostURL : ''}/api/files/${path}/${encodeURIComponent(originalUrl)}`;
}

export function findDifferenceStationIdLineUps(firstArray: StationIdLineup[], secondArray: StationIdLineup[]): StationIdLineup[] {
  const missingFromFirst = secondArray.filter((item2) => !firstArray.some((item1) => item2.lineup === item1.lineup && item2.stationId === item1.stationId));
  const missingFromSecond = firstArray.filter((item1) => !secondArray.some((item2) => item1.lineup === item2.lineup && item1.stationId === item2.stationId));

  return [...missingFromFirst, ...missingFromSecond];
}
export const arraysMatch = (array1: string[], array2: string[]): boolean => {
  if (array1.length !== array2.length) {
    return false;
  }

  // Sort both arrays using localeCompare for proper string comparison
  const sortedArray1 = [...array1].sort((a, b) => a.localeCompare(b));
  const sortedArray2 = [...array2].sort((a, b) => a.localeCompare(b));

  // Compare the sorted arrays element by element
  for (const [index, element] of sortedArray1.entries()) {
    if (element !== sortedArray2[index]) {
      return false;
    }
  }

  return true;
};

export function getIconUrl(iconOriginalSource: string | null | undefined, defaultIcon: string, cacheIcon: boolean): string {
  if (!iconOriginalSource || iconOriginalSource === '') {
    iconOriginalSource = `${isDevelopment ? `${baseHostURL}/` : '/'}${defaultIcon}`;
  }

  const originalUrl = iconOriginalSource;

  if (iconOriginalSource.startsWith('/')) {
    iconOriginalSource = iconOriginalSource.slice(1);
  }

  if (iconOriginalSource.startsWith('images/')) {
    iconOriginalSource = `${isDevelopment ? `${baseHostURL}/` : ''}${iconOriginalSource}`;
  } else if (!iconOriginalSource.startsWith('http')) {
    iconOriginalSource = getApiUrl(SMFileTypes.TvLogo, originalUrl);
  } else if (cacheIcon) {
    iconOriginalSource = getApiUrl(SMFileTypes.Icon, originalUrl);
  }

  return iconOriginalSource;
}

export const removeQuotes = (string_: string) => (string_.startsWith('"') && string_.endsWith('"') ? string_.slice(1, -1) : string_);
export const hasColumns = (columns?: ColumnMeta[]) => columns && columns.length > 0;

export function isEmptyObject(value: any): boolean {
  // Check if value is an empty object
  if (value && Object.keys(value).length === 0 && value.constructor === Object) {
    return true;
  }

  // Check if value is an empty array
  if (Array.isArray(value) && value.length === 0) {
    return true;
  }

  // Check if the first item of the array is empty (including empty object)
  if (
    Array.isArray(value) &&
    value.length > 0 &&
    (value[0] === undefined || value[0] === null || value[0] === '' || (typeof value[0] === 'object' && Object.keys(value[0]).length === 0))
  ) {
    return true;
  }

  return false;
}

interface MultiSelectCheckboxProperties {
  readonly onMultiSelectClick?: (value: boolean) => void;
  readonly rowClick: boolean;
  readonly setRowClick: (value: boolean) => void;
}

/**
 * MultiSelectCheckbox component is responsible for rendering and managing
 * the multi-select checkbox based on the provided selection mode.
 *
 * @param props The properties for the MultiSelectCheckbox component.
 */
export const MultiSelectCheckbox: React.FC<MultiSelectCheckboxProperties> = (props) => {
  const { onMultiSelectClick, rowClick, setRowClick } = props;

  return (
    <Checkbox
      checked={rowClick}
      onChange={(e) => {
        onMultiSelectClick?.(e.checked ?? false);
        setRowClick(e.checked ?? false);
      }}
      tooltip="Multi Select"
      tooltipOptions={getTopToolOptions}
    />
  );
};

export const getColumnClass = (size?: number, secondSize?: number) => {
  if (size !== undefined) {
    return `col-${12 - size}`;
  }

  if (secondSize !== undefined) {
    return `col-${secondSize}`;
  }

  return 'col-6';
};

export const HeaderLeft: React.FC<{ readonly props: any }> = ({ props }) => (
  <div
    className={`flex debug flex-nowrap justify-content-end header p-0 m-0 align-items-center ${
      props?.headerLeftTemplate ? getColumnClass(props.leftColSize, 4) : ''
    }`}
  ></div>
);

export const GlobalSearchComponent: React.FC<{
  readonly clearSourceFilter: any;
  readonly globalSearchName: string;
  readonly globalSourceFilterValue: string;
  readonly onGlobalSourceFilterChange: any;
  readonly props: any;
}> = ({ clearSourceFilter, props, globalSearchName, globalSourceFilterValue, onGlobalSourceFilterChange }) =>
  // eslint-disable-next-line react/prop-types
  props.globalSearchEnabled && (
    <GlobalSearch
      clearSourceFilter={clearSourceFilter}
      // eslint-disable-next-line react/prop-types
      columns={props.columns}
      globalSearchName={globalSearchName}
      globalSourceFilterValue={globalSourceFilterValue}
      onGlobalSourceFilterChange={onGlobalSourceFilterChange}
    />
  );

export const ExportComponent: React.FC<{ readonly exportCSV: any }> = ({ exportCSV }) => <ExportButton exportCSV={exportCSV} />;

export interface UserInformation {
  IsAuthenticated: boolean;
  TokenAge: Date;
}
