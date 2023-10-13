import { type FilterMatchMode } from 'primereact/api';
import { type ColumnEditorOptions, type ColumnEvent, type ColumnFilterElementTemplateOptions } from 'primereact/column';
import { type DataTableFilterMeta } from 'primereact/datatable';
import { type CSSProperties } from 'react';

export type ColumnFieldType = 'blank' | 'deleted' | 'epg' | 'epglink' | 'image' | 'isHidden' | 'm3ulink' | 'streams' | 'url' | undefined;
export type ColumnAlign = 'center' | 'left' | 'right' | null | undefined;
export type DataSelectorSelectionMode = 'checkbox' | 'multiple' | 'multipleNoCheckBox' | 'multipleNoRowCheckBox' | 'selectable' | 'single' | undefined;

export type LazyTableState = {
  filters: DataTableFilterMeta;
  first: number;
  jsonFiltersString: string | null | undefined;
  page: number;
  rows: number;
  sortField?: string;
  sortOrder?: -1 | 0 | 1 | null | undefined;
  sortString: string;
  // streamGroupId?: number;
};

/**
 * The metadata for a column in the table.
 */
export type ColumnMeta = {
  /**
   * The type of alignment for the content of the column.
   */
  align?: ColumnAlign | undefined;
  /**
   * A function that returns a JSX element for the column's body content.
   */

  bodyTemplate?: (data: any) => JSX.Element;
  /**
   * A boolean value that specifies whether to convert the field name to camel case.
   */
  camelize?: boolean;
  /**
   * The CSS class name for the column.
   */
  className?: string;
  /**
   * A React node or function that provides the input for the cell editor.
   */
  editor?: React.ReactNode | ((options: ColumnEditorOptions) => React.ReactNode);

  /**
   * The name of the field for the column.
   */
  field: string;
  /**
   * The type of field for the column.
   */
  fieldType?: ColumnFieldType;
  /**
   * A boolean value that specifies whether to enable filtering for the column.
   */
  filter?: boolean;
  filterElement?: (options: ColumnFilterElementTemplateOptions) => React.ReactNode;
  filterField?: string;
  /**
   * The type of matching to use for filtering.
   */
  filterMatchMode?: FilterMatchMode;
  /**
   * The type of field used for filtering.
   */
  filterType?: ColumnFieldType;
  /**
   * A function that is called when the cell editor is submitted.
   */
  handleOnCellEditComplete?: (event: ColumnEvent) => void;
  /**
   * The text to display in the column header.
   */
  header?: string;
  isHidden?: boolean;
  resizeable?: boolean | undefined;
  /**
   * A boolean value that specifies whether the column can be sorted.
   */
  sortable?: boolean;
  /**
   * The inline style for the column.
   */
  style?: CSSProperties;
  width?: string;
};
