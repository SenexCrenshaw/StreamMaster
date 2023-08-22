import { type PagedTableDto } from "./DataSelector";

function isPagedTableDto<T>(value: PagedTableDto<T>): value is PagedTableDto<T> {
  if (!value || Array.isArray(value)) {
    return false;
  }

  return (
    value &&
    (value.data === undefined || Array.isArray(value.data)) &&
    typeof value.first === 'number' &&
    typeof value.pageNumber === 'number' &&
    typeof value.pageSize === 'number' &&
    typeof value.totalItemCount === 'number' &&
    typeof value.totalPageCount === 'number' &&
    typeof value.totalRecords === 'number'
  );
}

export default isPagedTableDto;
