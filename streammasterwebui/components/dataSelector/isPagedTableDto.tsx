import { PagedTableDto } from './DataSelectorTypes';

function isPagedTableDto<T>(value: unknown): value is PagedTableDto<T> {
  if (!value || typeof value !== 'object' || Array.isArray(value)) {
    return false;
  }

  // You'll need to assert value as any here, since TS doesn't know if the value is an object or not.

  const value_ = value as any;

  return (
    value_ &&
    (value_.data === undefined || Array.isArray(value_.data)) &&
    typeof value_.first === 'number' &&
    typeof value_.pageNumber === 'number' &&
    typeof value_.pageSize === 'number' &&
    typeof value_.totalItemCount === 'number' &&
    typeof value_.totalPageCount === 'number'
  );
}

export default isPagedTableDto;
