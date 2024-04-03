import { CSSProperties } from 'react';
import { ColumnMeta } from '../types/ColumnMeta';

export const getStyle = (col: ColumnMeta, noMatch?: boolean): CSSProperties | undefined => {
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

  if (noMatch !== true) {
    return;
  }

  const widthStyle = col.width !== undefined && col.width !== '' ? { width: col.width } : {};

  const maxWidthStyle =
    col.maxWidth !== undefined && col.maxWidth !== ''
      ? { maxWidth: col.maxWidth }
      : col.width !== undefined && col.width !== ''
      ? { maxWidth: col.width }
      : { maxWidth: '3rem' };

  return {
    ...col.style,
    flexGrow: 0,
    flexShrink: 1,
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    whiteSpace: 'nowrap',
    ...widthStyle,
    ...maxWidthStyle
  } as CSSProperties;
};
