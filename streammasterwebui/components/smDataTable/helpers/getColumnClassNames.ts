import { ColumnMeta } from '../types/ColumnMeta';

export const getColumnClassNames = (col: ColumnMeta) => {
  const prefix = 'rem';
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
    classNames.push(`sm-w-min-${col.width}${prefix} sm-w-${col.width}${prefix}`);
  }

  // if (col.minWidth) {
  //   classNames.push(`sm-w-${col.minWidth}${prefix}`);
  //   classNames.push(`sm-w-min-${col.minWidth}${prefix}`);
  // }

  // if (col.maxWidth) {
  //   classNames.push(`sm-w-max-${col.maxWidth}${prefix}`);
  // }

  return classNames.join(' ');
};
