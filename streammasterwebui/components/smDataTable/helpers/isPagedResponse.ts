import { PagedResponse } from '@lib/smAPI/smapiTypes';

function isPagedResponse<T>(value: unknown): value is PagedResponse<T> {
  if (!value || typeof value !== 'object' || Array.isArray(value)) {
    return false;
  }

  const value_ = value as any;

  return (
    value_ &&
    (value_.Data === undefined || Array.isArray(value_.Data)) &&
    typeof value_.First === 'number' &&
    typeof value_.PageNumber === 'number' &&
    typeof value_.PageSize === 'number' &&
    typeof value_.TotalItemCount === 'number' &&
    typeof value_.TotalPageCount === 'number'
  );
}

export default isPagedResponse;
