/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetM3UFile = async (argument: iptv.M3UFileDto): Promise<iptv.M3UFileDto | null> => invokeHubCommand<iptv.M3UFileDto>('GetM3UFile', argument);
export const GetPagedM3UFiles = async (argument: iptv.PagedResponseOfM3UFileDto): Promise<iptv.M3UFileDto[] | null> =>
  invokeHubCommand<iptv.M3UFileDto[]>('GetPagedM3UFiles', argument);
export const GetM3UFileNames = async (): Promise<void | null> => {
  await invokeHubCommand<void>('GetM3UFileNames');
};
