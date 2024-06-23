import { CSSProperties } from 'react';
import { ColumnMeta } from '../types/ColumnMeta';

export const getStyle = (width?: string | number, minWidth?: string | number, maxWidth?: string | number): CSSProperties => {
  // Helper function to convert to CSS unit strings
  const toPx = (value: string | number): string => {
    if (typeof value === 'number') {
      return `${value}px`;
    }
    // If value is a string and does not end in a CSS unit, assume 'px'
    if (typeof value === 'string' && !/^\d+(\.\d+)?(px|rem|em|%)$/.test(value)) {
      return `${value}px`;
    }
    return value;
  };

  // Initialize the style object with overflow properties
  const style: CSSProperties = {
    overflow: 'hidden',
    textOverflow: 'ellipsis',
    whiteSpace: 'nowrap'
  };

  // Determine the effective values for width, minWidth, and maxWidth
  const effectiveWidth = width !== undefined ? toPx(width) : '10px';
  const effectiveMinWidth = minWidth !== undefined ? toPx(minWidth) : effectiveWidth;
  const effectiveMaxWidth = maxWidth !== undefined ? toPx(maxWidth) : effectiveWidth;

  // Apply the properties to the style object
  style.width = effectiveWidth;
  style.minWidth = effectiveMinWidth;
  style.maxWidth = effectiveMaxWidth;

  return style;
};

export const getColumnStyles = (col: ColumnMeta): CSSProperties => {
  // if (col.field === 'Group') {
  //   Logger.debug('getColumnStyles', col);
  // }
  // Default style for specific field types
  const defaultWidth = 40;

  // if (col.fieldType === 'blank' || col.fieldType === 'm3ulink' || col.fieldType === 'epglink' || col.fieldType === 'url') {
  //   return getStyle(defaultWidth);
  // }

  // Check if all width properties are undefined and apply default width if true
  if (col.width === undefined && col.minWidth === undefined && col.maxWidth === undefined) {
    return getStyle(defaultWidth);
  }

  // Use the provided width, or fallback to minWidth or maxWidth
  return getStyle(col.width, col.minWidth, col.maxWidth);
};
