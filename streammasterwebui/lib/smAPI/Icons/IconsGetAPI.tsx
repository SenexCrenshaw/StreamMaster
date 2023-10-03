/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';
import { type StringArg } from '@/src/components/selectors/BaseSelector';


export const GetIcon = async (arg: iptv.IconFileDto): Promise<iptv.IconFileDto | null> => {
    return await invokeHubConnection<iptv.IconFileDto> ('GetIcon', arg);
};

export const GetIconFromSource = async (arg: StringArg): Promise<iptv.IconFileDto | null> => {
    return await invokeHubConnection<iptv.IconFileDto> ('GetIconFromSource', arg);
};

export const GetPagedIcons = async (arg: iptv.PagedResponseOfIconFileDto): Promise<iptv.IconFileDto[] | null> => {
    return await invokeHubConnection<iptv.IconFileDto[]> ('GetPagedIcons', arg);
};

export const GetIconsSimpleQuery = async (arg: iptv.IconFileDto[]): Promise<iptv.IconFileDto[] | null> => {
    return await invokeHubConnection<iptv.IconFileDto[]> ('GetIconsSimpleQuery', arg);
};

