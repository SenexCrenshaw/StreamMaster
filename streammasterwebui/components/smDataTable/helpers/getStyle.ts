import { CSSProperties } from 'react';
import { ColumnMeta } from '../types/ColumnMeta';

export const getColumnClassNames = (col: ColumnMeta) => {
  if (col.fieldType === 'blank') {
    return 'sm-w-1rem';
  }

  if (col.fieldType === 'image') {
    return 'sm-w-3rem';
  }

  if (col.fieldType === 'm3ulink' || col.fieldType === 'epglink' || col.fieldType === 'url') {
    return 'sm-w-3rem';
  }

  if (col.width === undefined && col.minWidth === undefined && col.maxWidth === undefined) {
    return;
  }

  const classNames = [];

  if (col.width) {
    classNames.push(`sm-w-${col.width}`);
  }

  if (col.minWidth) {
    classNames.push(`sm-w-min-${col.minWidth}`);
  }

  if (col.maxWidth) {
    classNames.push(`sm-w-max-${col.maxWidth}`);
  }

  return classNames.join(' ');
};

export const getStyle = (col: ColumnMeta, noMatch?: boolean): CSSProperties | undefined => {
  if (col.fieldType === 'blank') {
    return {
      className: 'sm-w-1rem'
    } as CSSProperties;
  }

  if (col.fieldType === 'image') {
    return {
      className: 'sm-w-3rem'
    } as CSSProperties;
  }

  if (col.fieldType === 'm3ulink' || col.fieldType === 'epglink' || col.fieldType === 'url') {
    return {
      className: 'sm-w-3rem'
    } as CSSProperties;
  }

  if (noMatch !== true) {
    return;
  }

  const classNames = getColumnClassNames(col);

  return {
    ...col.style,
    className: classNames,
    flexGrow: 0,
    flexShrink: 1
  } as CSSProperties;
};
