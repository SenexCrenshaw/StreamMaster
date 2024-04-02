import getHeader from '@components/smDataTable/helpers/getHeader';
import { CSSProperties } from 'react';
import { ColumnMeta } from '../types/ColumnMeta';
import { ColumnAlign, ColumnFieldType } from '../types/smDataTableTypes';

export const getHeaderFromField = (field: string, columns: ColumnMeta[]): string => {
  let col = columns.find((column) => column.field === field);
  let header = '';

  if (col) {
    header = getHeader(col.field, col.header, col.fieldType) as string;
  }
  return header;
};

export const setColumnToggle = (columns: ColumnMeta[], selectedColumns: ColumnMeta[]) => {
  const newData = [...columns];

  newData
    .filter((col) => col.removable === true)
    .forEach((col) => {
      col.removed = true;
    });

  if (selectedColumns !== undefined) {
    selectedColumns.forEach((col) => {
      let propscol = newData.find((column) => column.field === col.field);
      if (propscol) {
        console.log('Setting ' + propscol.field + ' false');
        propscol.removed = false;
      }
    });
  }

  return newData;
};

export const getAlign = (align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {
  if (fieldType === 'image') {
    return 'center';
  }

  if (fieldType === 'isHidden') {
    return 'center';
  }

  if (align === undefined || align === null) {
    return 'left';
  }

  return align;
};

export const getAlignHeader = (align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
  if (fieldType === 'image') {
    return 'center';
  }

  if (fieldType === 'isHidden') {
    return 'center';
  }

  if (!align) {
    return 'center';
  }

  return align;
};

export const getStyle = (col: ColumnMeta): CSSProperties | undefined => {
  if (col.fieldType === 'blank') {
    return {
      maxWidth: '1rem',
      width: '1rem'
    } as CSSProperties;
  }

  if (col.fieldType === 'image') {
    return {
      maxWidth: '5rem',
      width: '5rem'
    } as CSSProperties;
  }

  if (col.fieldType === 'm3ulink' || col.fieldType === 'epglink' || col.fieldType === 'url') {
    return {
      maxWidth: '3rem',
      width: '3rem'
    } as CSSProperties;
  }

  if (col.width !== undefined && col.width !== '') {
    return {
      ...col.style,
      flexGrow: 0,
      flexShrink: 1,
      maxWidth: col.width,
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',
      width: col.width
    } as CSSProperties;
  }

  return {
    ...col.style,
    flexGrow: 0,
    flexShrink: 1,
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    whiteSpace: 'nowrap'
  } as CSSProperties;
};
