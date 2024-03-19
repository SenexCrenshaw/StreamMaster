/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const CreateStreamGroup = async (argument: iptv.CreateStreamGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('CreateStreamGroup', argument);
};
export const DeleteStreamGroup = async (argument: iptv.DeleteStreamGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('DeleteStreamGroup', argument);
};
export const GetVideoStreamStreamFromAutoHEAD = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamFromAutoHEAD', argument);
};
export const UpdateStreamGroup = async (argument: iptv.UpdateStreamGroupRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateStreamGroup', argument);
};
