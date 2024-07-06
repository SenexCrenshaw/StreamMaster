import { isSkipToken } from './isSkipToken';

export function getParameters(param: any): any {
  return isSkipToken(param) ? undefined : param ? JSON.stringify(param) : undefined;
}
