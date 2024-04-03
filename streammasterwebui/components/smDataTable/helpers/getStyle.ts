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
