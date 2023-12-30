export interface PagedResponseDto<T> {
  data: T[];
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
}

export interface StringArgument {
  value: string;
}
export interface PagedResponseDtoData<T> {
  data?: PagedResponseDto<T>;
}

export interface SimpleQueryResponse<T> {
  data?: T[];
}
