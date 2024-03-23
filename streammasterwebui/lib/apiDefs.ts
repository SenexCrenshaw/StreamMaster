export interface SMChannelRankRequest {
  smChannelId: number;
  smStreamId: string;
  rank: number;
}

export interface QueryHookResult<T> {
  data?: T;
  error?: Error | string | null;
  isError: boolean;
  isLoading: boolean;
  isFetching?: boolean | undefined;
}

export type IFormFile = Blob;

export interface QueryHook<T> {
  (): QueryHookResult<T>;
}

export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalPageCount: number;
  totalItemCount: number;
  first: number;
}

export interface APIResponse<T> {
  message?: string;
  errorMessage?: string;
  isError?: boolean;
  pagedResponse?: PagedResponse<T>;
}

export type QueryStringParameters = {
  pageNumber?: number;
  pageSize?: number;
  orderBy?: string;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
};

export type SMStream = {
  id?: string;
  filePosition?: number;
  isHidden?: boolean;
  isUserCreated?: boolean;
  m3UFileId?: number;
  tvg_chno?: number;
  m3UFileName?: string;
  shortId?: string;
  group?: string;
  epgid?: string;
  logo?: string;
  name?: string;
  url?: string;
  stationId?: string;
};

export type SMStreamDto = SMStream & {
  realUrl?: string;
  rank: number;
  [key: string]: any;
};

export type StreamingProxyTypes = 0 | 1 | 2 | 3;
export type VideoStreamHandlers = 0 | 1 | 2;

export type SMChannelDto = {
  id: number;
  streamingProxyType: StreamingProxyTypes;
  videoStreamHandler: VideoStreamHandlers;
  realUrl?: string;
  isHidden?: boolean;
  channelNumber?: number;
  shortId?: string;
  group?: string;
  epgid?: string;
  logo?: string;
  name?: string;
  smStreams: SMStreamDto[];
  [key: string]: any;
};

export interface CacheItem<T> {
  data: T;
  timestamp: number;
}

export interface GetApiArgument {
  count?: number;
  first?: number;
  jsonArgumentString?: string | null;
  jsonFiltersString?: string | null;
  last?: number;
  name?: string;
  orderBy?: string;
  pageNumber?: number;
  pageSize?: number;
  streamGroupId?: number | undefined;
}

export interface DefaultAPIResponse extends APIResponse<{}> {}

const compareProperties = (propertyName: keyof GetApiArgument, object1: GetApiArgument, object2: GetApiArgument) => {
  const value1 = object1[propertyName];
  const value2 = object2[propertyName];

  const result = value1 === value2 || (value1 === undefined && value2 === undefined);

  return result;
};

export function areGetApiArgsEqual(object1?: GetApiArgument, object2?: GetApiArgument): boolean {
  // Handle cases where one or both arguments are undefined
  if (!object1 && !object2) return true;
  if (!object1 || !object2) return false;

  return (
    compareProperties('count', object1, object2) &&
    compareProperties('first', object1, object2) &&
    compareProperties('jsonArgumentString', object1, object2) &&
    compareProperties('jsonFiltersString', object1, object2) &&
    compareProperties('last', object1, object2) &&
    compareProperties('name', object1, object2) &&
    compareProperties('orderBy', object1, object2) &&
    compareProperties('pageNumber', object1, object2) &&
    compareProperties('pageSize', object1, object2) &&
    compareProperties('streamGroupId', object1, object2)
  );
}

export function removeKeyFromData<T extends Record<string, any>>(data: T, keyToRemove: keyof T): Omit<T, typeof keyToRemove> {
  const { [keyToRemove]: _, ...rest } = data;
  return rest;
}

export type FieldData = {
  entity: string;
  id: string;
  field: string;
  value: any;
};

export interface SMChannelLogoRequest {
  smChannelId: number;
  logo: string;
}

export type M3UFileDto = BaseFileDto & {
  vodTags: string[];
  overwriteChannelNumbers: boolean;
  startingChannelNumber: number;
  maxStreamCount: number;
  stationCount: number;
};

export type BaseFileDto = {
  source: string;
  autoUpdate: boolean;
  description: string;
  downloadErrors: number;
  hoursToUpdate: number;
  id: number;
  lastDownloadAttempt: string;
  lastDownloaded: string;
  name: string;
  needsUpdate: boolean;
  url: string;
};
export type EpgFileDto = BaseFileDto & {
  timeShift: number;
  epgNumber: number;
  color: string;
  channelCount: number;
  epgStartDate: string;
  epgStopDate: string;
  programmeCount: number;
};
