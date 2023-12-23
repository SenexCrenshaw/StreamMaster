/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetEpgColors = async (): Promise<iptv.EpgColorDto[] | null> => invokeHubConnection<iptv.EpgColorDto[]>('GetEpgColors');
export const GetEpgFile = async (argument: iptv.EpgFileDto): Promise<iptv.EpgFileDto | null> => invokeHubConnection<iptv.EpgFileDto>('GetEpgFile', argument);
export const GetEpgFilePreviewById = async (argument: iptv.EpgFilePreviewDto[]): Promise<iptv.EpgFilePreviewDto[] | null> =>
  invokeHubConnection<iptv.EpgFilePreviewDto[]>('GetEpgFilePreviewById', argument);
export const GetPagedEpgFiles = async (argument: iptv.PagedResponseOfEpgFileDto): Promise<iptv.EpgFileDto[] | null> =>
  invokeHubConnection<iptv.EpgFileDto[]>('GetPagedEpgFiles', argument);
