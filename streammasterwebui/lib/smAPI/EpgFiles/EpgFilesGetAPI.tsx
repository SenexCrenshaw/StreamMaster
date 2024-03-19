/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetEpgColors = async (): Promise<iptv.EpgColorDto[] | null> => invokeHubCommand<iptv.EpgColorDto[]>('GetEpgColors');
export const GetEpgFile = async (argument: iptv.EpgFileDto): Promise<iptv.EpgFileDto | null> => invokeHubCommand<iptv.EpgFileDto>('GetEpgFile', argument);
export const GetEpgFilePreviewById = async (argument: iptv.EpgFilePreviewDto[]): Promise<iptv.EpgFilePreviewDto[] | null> =>
  invokeHubCommand<iptv.EpgFilePreviewDto[]>('GetEpgFilePreviewById', argument);

export const GetEpgNextEpgNumber = async (): Promise<number | null> => invokeHubCommand<number>('GetEpgNextEpgNumber');

export const GetPagedEpgFiles = async (argument: iptv.PagedResponseOfEpgFileDto): Promise<iptv.EpgFileDto[] | null> =>
  invokeHubCommand<iptv.EpgFileDto[]>('GetPagedEpgFiles', argument);
