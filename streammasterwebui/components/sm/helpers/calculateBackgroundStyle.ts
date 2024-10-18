// helpers/calculateBackgroundStyle.ts

import { Direction, Shape } from '@components/sm/Interfaces/SMSpeedDialTypes';
import { Logger } from '@lib/common/logger';

export const calculateBackgroundStyle = (
  isOpen: boolean,
  shape: Shape,
  direction: Direction,
  itemsLength: number,
  backgroundWidth?: string // New optional parameter for manually setting background width
) => {
  if (!isOpen) return {};

  let height = '0%';
  let width = '0%';
  let top = '0%';
  let left = '0%';
  let borderRadius = '10px';
  const padding = 10;
  const distance = 70;
  const mainItemWidth = distance + padding; // Width for the main item

  Logger.debug('calculateBackgroundStyle', { direction, isOpen, itemsLength, padding, shape });
  switch (shape) {
    case 'line':
      if (direction === 'bottom') {
        height = `${(itemsLength + 1) * distance + padding}px`;
        width = backgroundWidth || `${mainItemWidth}px`; // Use provided width for children or default for main item
        top = `0px`;
      } else if (direction === 'top') {
        height = `${(itemsLength + 1) * distance + padding}px`;
        width = backgroundWidth || `${mainItemWidth}px`; // Use provided width for children or default for main item
        top = `calc(100% - ${(itemsLength + 1) * distance + padding}px)`;
      } else if (direction === 'left') {
        height = `${mainItemWidth}px`; // Initial height to cover the main item
        width = backgroundWidth || `${(itemsLength + 1) * distance + padding}px`; // Use provided width for children or default for main item
        left = `calc(100% - ${(itemsLength + 1) * distance + padding}px)`;
      } else if (direction === 'right') {
        height = `${mainItemWidth}px`; // Initial height to cover the main item
        width = backgroundWidth || `${(itemsLength + 1) * distance + padding}px`; // Use provided width for children or default for main item
        left = `0px`;
      }
      break;
    case 'circle':
      const radius = 4 * (distance - padding * 1.5);
      const size = radius;

      height = `${size}px`;
      width = `${size}px`;
      top = `calc(50% - ${size / 2}px)`;
      left = `calc(50% - ${size / 2}px)`;
      borderRadius = '50%'; // Make the background circular
      break;
    case 'semicircle':
      let semiRadius = 4 * (distance - padding);
      semiRadius = semiRadius / 2;
      height = `${semiRadius + padding * 1.5}px`;
      width = `${semiRadius * 2}px`;
      if (direction === 'top') {
        top = `calc(100% - ${semiRadius + padding * 1.5}px)`;
        left = `calc(50% - ${semiRadius}px)`;
        borderRadius = '100% 100% 0 0';
      } else if (direction === 'bottom') {
        top = `0px`;
        left = `calc(50% - ${semiRadius}px)`;
        borderRadius = '0 0 100% 100%';
      } else if (direction === 'left') {
        height = `${semiRadius * 2 - padding * 2}px`;
        width = `${semiRadius + padding * 1.5}px`;
        top = `calc(50% - ${semiRadius - padding}px)`;
        left = `calc(100% - ${semiRadius + padding * 1.5}px)`;
        borderRadius = '100% 0 0 100%';
      } else if (direction === 'right') {
        height = `${semiRadius * 2 - padding * 2}px`;
        width = `${semiRadius + padding * 1.5}px`;
        top = `calc(50% - ${semiRadius - padding}px)`;
        left = `0px`;
        borderRadius = '0 100% 100% 0';
      }
      break;
    case 'quarter-circle':
      const quarterRadius = (itemsLength * distance) / 2;
      height = `${quarterRadius + padding}px`;
      width = `${quarterRadius + padding}px`;
      switch (direction) {
        case 'top-left':
          top = `calc(100% - ${quarterRadius + padding}px)`;
          left = `calc(100% - ${quarterRadius + padding}px)`;
          borderRadius = '100% 0 0 0';
          break;
        case 'top':
          top = `calc(100% - ${quarterRadius + padding}px)`;
          left = `0px`;
          borderRadius = '0 100% 0 0';
          break;
        case 'top-right':
          top = `calc(100% - ${quarterRadius + padding}px)`;
          left = `0px`;
          borderRadius = '0 100% 0 0';
          break;
        case 'bottom-left':
          top = `0px`;
          left = `calc(100% - ${quarterRadius + padding}px)`;
          borderRadius = '0 0 0 100%';
          break;
        case 'bottom-right':
          top = `0px`;
          left = `0px`;
          borderRadius = '0 0 100% 0';
          break;
      }
      break;
  }

  return {
    background: 'rgba(0, 0, 0, 0.5)',
    borderRadius: borderRadius,
    height: height,
    left: left,
    position: 'absolute',
    top: top,
    width: isOpen ? backgroundWidth || width : `${mainItemWidth}px`, // Use provided width for children or default for main item
    zIndex: 2
  };
};
