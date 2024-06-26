import { QueryStringParameters } from './smAPI/smapiTypes';

export interface QueryHookResult<T> {
  data?: T;
  error?: Error | string | null;
  isError: boolean;
  isLoading: boolean;
  isFetching?: boolean | undefined;
}

export interface CSharpException {
  message: string;
  stack: string;
}

export function isCSharpException(error: any): error is CSharpException {
  return error !== null && typeof error === 'object' && 'message' in error && 'stack' in error;
}

export type IFormFile = Blob;

export interface QueryHook<T> {
  (): QueryHookResult<T>;
}

export interface CacheItem<T> {
  data: T;
  timestamp: number;
}

// export interface QueryStringParameters extends QueryStringParameters {
//   // // count?: number;
//   // // first?: number;
//   // JSONArgumentString: string;
//   // JSONFiltersString: string;
//   // // last?: number;
//   // // name?: string;
//   // OrderBy: string;
//   // PageNumber: number;
//   // PageSize: number;
//   // // StreamGroupId?: number;
// }

const compareProperties = (propertyName: keyof QueryStringParameters, object1: QueryStringParameters, object2: QueryStringParameters) => {
  const value1 = object1[propertyName];
  const value2 = object2[propertyName];

  const result = value1 === value2 || (value1 === undefined && value2 === undefined);

  return result;
};

export function areGetApiArgsEqual(object1?: QueryStringParameters, object2?: QueryStringParameters): boolean {
  // Handle cases where one or both arguments are undefined
  if (!object1 && !object2) return true;
  if (!object1 || !object2) return false;

  return (
    // compareProperties('count', object1, object2) &&
    // compareProperties('first', object1, object2) &&
    compareProperties('JSONArgumentString', object1, object2) &&
    compareProperties('JSONFiltersString', object1, object2) &&
    // compareProperties('last', object1, object2) &&
    // compareProperties('name', object1, object2) &&
    compareProperties('OrderBy', object1, object2) &&
    compareProperties('PageNumber', object1, object2) &&
    compareProperties('PageSize', object1, object2)
    // compareProperties('StreamGroupId', object1, object2)
  );
}

export function removeKeyFromData<T extends Record<string, any>>(data: T, keyToRemove: keyof T): Omit<T, typeof keyToRemove> {
  const { [keyToRemove]: _, ...rest } = data;
  return rest;
}
