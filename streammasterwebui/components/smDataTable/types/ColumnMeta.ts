import { FilterMatchMode } from 'primereact/api';
import { ColumnEditorOptions, ColumnEvent, ColumnFilterElementTemplateOptions } from 'primereact/column';
import { CSSProperties } from 'react';
import { ColumnAlign, ColumnFieldType } from './smDataTableTypes';

/**
 * The metadata for a column in the table.
 */
export interface ColumnMeta {
  noAutoStyle?: boolean;
  /**
   * The type of alignment for the content of the column.
   */
  align?: ColumnAlign | undefined;
  alignHeader?: ColumnAlign | undefined;
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
  removed?: boolean;
  removable?: boolean;
  maxWidth?: string;
  minWidth?: string;
}
