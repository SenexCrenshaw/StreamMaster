/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetM3UFile = async (arg: iptv.M3UFileDto): Promise<iptv.M3UFileDto | null> => {
  return await invokeHubConnection<iptv.M3UFileDto>('GetM3UFile', arg);
};

export const GetPagedM3UFiles = async (arg: iptv.PagedResponseOfM3UFileDto): Promise<iptv.M3UFileDto[] | null> => {
  return await invokeHubConnection<iptv.M3UFileDto[]>('GetPagedM3UFiles', arg);
};

export const GetM3UFileNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetM3UFileNames');
};
