import { type PagedTableDto } from './DataSelector';

function isPagedTableDto<T>(value: unknown): value is PagedTableDto<T> {
  if (!value || typeof value !== 'object' || Array.isArray(value)) {
    return false;
  }

  // You'll need to assert value as any here, since TS doesn't know if the value is an object or not.

  const val = value as any;

  return (
    val &&
    (val.data === undefined || Array.isArray(val.data)) &&
    typeof val.first === 'number' &&
    typeof val.pageNumber === 'number' &&
    typeof val.pageSize === 'number' &&
    typeof val.totalItemCount === 'number' &&
    typeof val.totalPageCount === 'number'
  );
}

export default isPagedTableDto;
