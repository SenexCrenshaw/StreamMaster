/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { type StringArg } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetIcon = async (argument: iptv.IconFileDto): Promise<iptv.IconFileDto | null> => invokeHubCommand<iptv.IconFileDto>('GetIcon', argument);
export const GetIconFromSource = async (argument: StringArg): Promise<iptv.IconFileDto | null> =>
  invokeHubCommand<iptv.IconFileDto>('GetIconFromSource', argument);
export const GetPagedIcons = async (argument: iptv.PagedResponseOfIconFileDto): Promise<iptv.IconFileDto[] | null> =>
  invokeHubCommand<iptv.IconFileDto[]>('GetPagedIcons', argument);
export const GetIconsSimpleQuery = async (argument: iptv.IconFileDto[]): Promise<iptv.IconFileDto[] | null> =>
  invokeHubCommand<iptv.IconFileDto[]>('GetIconsSimpleQuery', argument);
