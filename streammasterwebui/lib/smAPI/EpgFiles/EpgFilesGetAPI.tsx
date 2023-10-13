/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetEpgFile = async (arg: iptv.EpgFileDto): Promise<iptv.EpgFileDto | null> => {
  return await invokeHubConnection<iptv.EpgFileDto>('GetEpgFile', arg);
};

export const GetPagedEpgFiles = async (arg: iptv.PagedResponseOfEpgFileDto): Promise<iptv.EpgFileDto[] | null> => {
  return await invokeHubConnection<iptv.EpgFileDto[]>('GetPagedEpgFiles', arg);
};
