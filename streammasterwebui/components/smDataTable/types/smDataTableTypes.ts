export type DataSelectorSelectionMode = 'checkbox' | 'multiple' | 'multipleNoCheckBox' | 'multipleNoRowCheckBox' | 'selectable' | 'single' | undefined;

export interface PagedTableInformation {
  first: number;
  pageNumber: number;
  pageSize: number;
  totalItemCount: number;
  totalPageCount: number;
}

export type PagedTableDto<T> = PagedDataDto<T> & PagedTableInformation & {};

export interface PagedDataDto<T> {
  data?: T[];
}

export type ColumnFieldType = 'blank' | 'deleted' | 'epg' | 'epglink' | 'image' | 'isHidden' | 'm3ulink' | 'streams' | 'url' | 'actions' | undefined;
export type ColumnAlign = 'center' | 'left' | 'right' | null | undefined;
