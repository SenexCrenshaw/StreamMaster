/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const CreateVideoStream = async (arg: iptv.CreateVideoStreamRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('CreateVideoStream', arg);
};

export const ChangeVideoStreamChannel = async (arg: iptv.ChangeVideoStreamChannelRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('ChangeVideoStreamChannel', arg);
};

export const DeleteVideoStream = async (arg: iptv.DeleteVideoStreamRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('DeleteVideoStream', arg);
};

export const FailClient = async (arg: iptv.FailClientRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('FailClient', arg);
};

export const GetVideoStreamStreamHEAD = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetVideoStreamStreamHEAD', arg);
};

export const GetVideoStreamStreamHEAD2 = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetVideoStreamStreamHEAD2', arg);
};

export const GetVideoStreamStreamHEAD3 = async (arg: string): Promise<void | null> => {
    await invokeHubConnection<void> ('GetVideoStreamStreamHEAD3', arg);
};

export const ReSetVideoStreamsLogo = async (arg: iptv.ReSetVideoStreamsLogoRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('ReSetVideoStreamsLogo', arg);
};

export const SetVideoStreamChannelNumbers = async (arg: iptv.SetVideoStreamChannelNumbersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('SetVideoStreamChannelNumbers', arg);
};

export const SetVideoStreamsLogoFromEpg = async (arg: iptv.SetVideoStreamsLogoFromEpgRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('SetVideoStreamsLogoFromEpg', arg);
};

export const UpdateVideoStream = async (arg: iptv.UpdateVideoStreamRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateVideoStream', arg);
};

export const UpdateVideoStreams = async (arg: iptv.UpdateVideoStreamsRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateVideoStreams', arg);
};

export const UpdateAllVideoStreamsFromParameters = async (arg: iptv.UpdateAllVideoStreamsFromParametersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateAllVideoStreamsFromParameters', arg);
};

export const DeleteAllVideoStreamsFromParameters = async (arg: iptv.DeleteAllVideoStreamsFromParametersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('DeleteAllVideoStreamsFromParameters', arg);
};

export const SetVideoStreamChannelNumbersFromParameters = async (arg: iptv.SetVideoStreamChannelNumbersFromParametersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('SetVideoStreamChannelNumbersFromParameters', arg);
};

export const SetVideoStreamsLogoFromEpgFromParameters = async (arg: iptv.SetVideoStreamsLogoFromEpgFromParametersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('SetVideoStreamsLogoFromEpgFromParameters', arg);
};

export const ReSetVideoStreamsLogoFromParameters = async (arg: iptv.ReSetVideoStreamsLogoFromParametersRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('ReSetVideoStreamsLogoFromParameters', arg);
};

export const SimulateStreamFailureForAll = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('SimulateStreamFailureForAll');
};

export const SimulateStreamFailure = async (arg: iptv.SimulateStreamFailureRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('SimulateStreamFailure', arg);
};

