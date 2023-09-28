export type PagedTableInformation = {
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
};

export type PagedDataDto<T> = {
  data?: T[];
};

export type PagedTableDto<T> = PagedDataDto<T> & PagedTableInformation & {
};

function isPagedTableDto<T>(value: PagedTableDto<T> | T[]): value is PagedTableDto<T> {
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
    typeof value.totalPageCount === 'number'
  );
}

export default isPagedTableDto;
