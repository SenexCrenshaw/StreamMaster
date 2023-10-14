/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const CreateStreamGroup = async (arg: iptv.CreateStreamGroupRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('CreateStreamGroup', arg);
};

export const DeleteStreamGroup = async (arg: iptv.DeleteStreamGroupRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('DeleteStreamGroup', arg);
};

export const UpdateStreamGroup = async (arg: iptv.UpdateStreamGroupRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateStreamGroup', arg);
};

