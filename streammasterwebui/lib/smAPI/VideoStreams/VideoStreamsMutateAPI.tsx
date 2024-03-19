/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const CreateVideoStream = async (argument: iptv.CreateVideoStreamRequest): Promise<void | null> => {
  await invokeHubCommand<void>('CreateVideoStream', argument);
};
export const ChangeVideoStreamChannel = async (argument: iptv.ChangeVideoStreamChannelRequest): Promise<void | null> => {
  await invokeHubCommand<void>('ChangeVideoStreamChannel', argument);
};
export const DeleteVideoStream = async (argument: iptv.DeleteVideoStreamRequest): Promise<void | null> => {
  await invokeHubCommand<void>('DeleteVideoStream', argument);
};
export const FailClient = async (argument: iptv.FailClientRequest): Promise<void | null> => {
  await invokeHubCommand<void>('FailClient', argument);
};
export const ReadAndWrite = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('ReadAndWrite', argument);
};
export const GetVideoStreamStreamHEAD = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamHEAD', argument);
};
export const GetVideoStreamStreamHEAD2 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamHEAD2', argument);
};
export const GetVideoStreamStreamHEAD3 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamHEAD3', argument);
};
export const GetVideoStreamStreamHEAD4 = async (argument: string): Promise<void | null> => {
  await invokeHubCommand<void>('GetVideoStreamStreamHEAD4', argument);
};
export const ReSetVideoStreamsLogo = async (argument: iptv.ReSetVideoStreamsLogoRequest): Promise<void | null> => {
  await invokeHubCommand<void>('ReSetVideoStreamsLogo', argument);
};
export const SetVideoStreamChannelNumbers = async (argument: iptv.SetVideoStreamChannelNumbersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamChannelNumbers', argument);
};
export const SetVideoStreamsLogoFromEpg = async (argument: iptv.SetVideoStreamsLogoFromEpgRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamsLogoFromEpg', argument);
};
export const UpdateVideoStream = async (argument: iptv.UpdateVideoStreamRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateVideoStream', argument);
};
export const UpdateVideoStreams = async (argument: iptv.UpdateVideoStreamsRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateVideoStreams', argument);
};
export const UpdateAllVideoStreamsFromParameters = async (argument: iptv.UpdateAllVideoStreamsFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateAllVideoStreamsFromParameters', argument);
};
export const DeleteAllVideoStreamsFromParameters = async (argument: iptv.DeleteAllVideoStreamsFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('DeleteAllVideoStreamsFromParameters', argument);
};
export const SetVideoStreamChannelNumbersFromParameters = async (argument: iptv.SetVideoStreamChannelNumbersFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamChannelNumbersFromParameters', argument);
};
export const SetVideoStreamsLogoFromEpgFromParameters = async (argument: iptv.SetVideoStreamsLogoFromEpgFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamsLogoFromEpgFromParameters', argument);
};
export const ReSetVideoStreamsLogoFromParameters = async (argument: iptv.ReSetVideoStreamsLogoFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('ReSetVideoStreamsLogoFromParameters', argument);
};
export const SimulateStreamFailureForAll = async (): Promise<void | null> => {
  await invokeHubCommand<void>('SimulateStreamFailureForAll');
};
export const SimulateStreamFailure = async (argument: iptv.SimulateStreamFailureRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SimulateStreamFailure', argument);
};
export const AutoSetEpg = async (argument: iptv.AutoSetEpgRequest): Promise<void | null> => {
  await invokeHubCommand<void>('AutoSetEpg', argument);
};
export const AutoSetEpgFromParameters = async (argument: iptv.AutoSetEpgFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('AutoSetEpgFromParameters', argument);
};
export const SetVideoStreamTimeShifts = async (argument: iptv.SetVideoStreamTimeShiftsRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamTimeShifts', argument);
};
export const SetVideoStreamTimeShiftFromParameters = async (argument: iptv.SetVideoStreamTimeShiftFromParametersRequest): Promise<void | null> => {
  await invokeHubCommand<void>('SetVideoStreamTimeShiftFromParameters', argument);
};
