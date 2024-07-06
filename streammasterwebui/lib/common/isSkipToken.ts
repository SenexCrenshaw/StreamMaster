import { skipToken, SkipToken } from '@reduxjs/toolkit/query';

export function isSkipToken(param: any): param is SkipToken {
  return param === skipToken;
}
