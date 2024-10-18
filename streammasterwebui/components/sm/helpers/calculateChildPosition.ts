// calculateChildPosition.ts

import { Direction, Shape } from '@components/sm/Interfaces/SMSpeedDialTypes'; // Import the types

export const calculateChildPosition = (index: number, total: number, shape: Shape, direction?: Direction) => {
  const distance = 70; // distance between each child item
  let x = 0;
  let y = 0;
  const padding = 10;

  switch (shape) {
    case 'line':
      switch (direction) {
        case 'top':
          y = -distance * (index + 1);
          break;
        case 'bottom':
          y = distance * (index + 1);
          break;
        case 'left':
          x = -distance * (index + 1);
          y = -padding;
          break;
        case 'right':
          x = distance * (index + 1);
          y = -padding;
          break;
      }
      break;
    case 'circle':
      const angleStep = (2 * Math.PI) / total;
      const angle = angleStep * index;
      x = Math.cos(angle) * distance;
      y = Math.sin(angle) * distance - padding;
      break;
    case 'semicircle':
      const semicircleAngleStep = Math.PI / (total - 1);
      let semicircleStartAngle = 0;
      switch (direction) {
        case 'top': // Show children along the top
          semicircleStartAngle = Math.PI;
          break;
        case 'bottom': // Show children along the bottom
          semicircleStartAngle = 0;
          break;
        case 'left': // Show children along the left
          semicircleStartAngle = Math.PI / 2;
          break;
        case 'right': // Show children along the right
          semicircleStartAngle = -Math.PI / 2;
          break;
      }
      const semicircleAngle = semicircleStartAngle + semicircleAngleStep * index;
      x = Math.cos(semicircleAngle) * distance;
      y = Math.sin(semicircleAngle) * distance - padding;
      break;
    case 'quarter-circle':
      const quarterCircleAngleStep = Math.PI / 2 / (total - 1);
      let quarterCircleStartAngle = 0;
      switch (direction) {
        case 'top-left':
          quarterCircleStartAngle = Math.PI;
          break;
        case 'top':
          quarterCircleStartAngle = -Math.PI / 4;
          break;
        case 'top-right':
          quarterCircleStartAngle = -Math.PI / 2;
          break;
        case 'right':
          quarterCircleStartAngle = -Math.PI / 3;
          break;
        case 'bottom':
          quarterCircleStartAngle = Math.PI / 4;
          break;
        case 'bottom-left':
          quarterCircleStartAngle = Math.PI / 2;
          break;
        case 'bottom-right':
          quarterCircleStartAngle = 0;
          break;
      }
      const quarterCircleAngle = quarterCircleStartAngle + quarterCircleAngleStep * index;
      x = Math.cos(quarterCircleAngle) * distance;
      y = Math.sin(quarterCircleAngle) * distance;
      break;
  }

  return { x, y };
};
