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

  // Extracted values with defaults
  let width = col.width !== '' ? col.width : undefined;
  let maxWidth = col.maxWidth !== '' ? col.maxWidth : width;
  const minWidth = col.minWidth !== '' ? col.minWidth : width;

  if (width === undefined && maxWidth === undefined && minWidth === undefined) {
    return;
  }
  // if (width === undefined) {
  //   if (minWidth !== undefined) {
  //     width = minWidth;
  //   }
  // }
  // if (width === undefined) {
  //   if (maxWidth !== undefined) {
  //     width = maxWidth;
  //   }
  // }

  const widthStyle = width ? { width: width } : undefined;
  const maxWidthStyle = maxWidth ? { maxWidth: maxWidth } : undefined;
  const minWidthStyle = minWidth ? { minWidth: minWidth } : undefined;

  // if (maxWidth) {
  //   const parsedMaxWidth = parseFloat(maxWidth);
  //   const parsedMinWidth = parseFloat(styles.minWidth as string);

  //   if (!isNaN(parsedMaxWidth) && !isNaN(parsedMinWidth) && parsedMaxWidth <= parsedMinWidth) {
  //     styles = { ...styles, maxWidth: `${parsedMinWidth + 0.1}rem` };
  //   } else {
  //     styles = { ...styles, maxWidth: `${parsedMaxWidth}rem` };
  //   }
  // }

  // const test = { ...widthStyle, ...minWidthStyle, ...maxWidthStyle };
  // if (col.field === 'M3UFileName' || col.field === 'Name' || col.field === 'Group') {
  //   console.log(col.field, test);
  // }

  return {
    ...col.style,
    flexGrow: 0,
    flexShrink: 1,
    ...widthStyle,
    ...minWidthStyle,
    ...maxWidthStyle
  } as CSSProperties;
};
