/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@lib/common/dataTypes';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetM3UFile = async (argument: iptv.M3UFileDto): Promise<iptv.M3UFileDto | null> =>
  invokeHubConnection<iptv.M3UFileDto>('GetM3UFile', argument);
export const GetPagedM3UFiles = async (argument: iptv.PagedResponseOfM3UFileDto): Promise<iptv.M3UFileDto[] | null> =>
  invokeHubConnection<iptv.M3UFileDto[]>('GetPagedM3UFiles', argument);
export const GetM3UFileNames = async (): Promise<void | null> => {
  await invokeHubConnection<void>('GetM3UFileNames');
};
