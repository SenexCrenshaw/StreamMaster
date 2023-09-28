import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const CreateVideoStream = async (arg: iptv.CreateVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('CreateVideoStream');
  await hubConnection.invoke('CreateVideoStream', arg);
};

export const ChangeVideoStreamChannel = async (arg: iptv.ChangeVideoStreamChannelRequest): Promise<void> => {
  if (isDebug) console.log('ChangeVideoStreamChannel');
  await hubConnection.invoke('ChangeVideoStreamChannel', arg);
};

export const DeleteVideoStream = async (arg: iptv.DeleteVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('DeleteVideoStream');
  await hubConnection.invoke('DeleteVideoStream', arg);
};

export const FailClient = async (arg: iptv.FailClientRequest): Promise<void> => {
  if (isDebug) console.log('FailClient');
  await hubConnection.invoke('FailClient', arg);
};

export const ReSetVideoStreamsLogo = async (arg: iptv.ReSetVideoStreamsLogoRequest): Promise<void> => {
  if (isDebug) console.log('ReSetVideoStreamsLogo');
  await hubConnection.invoke('ReSetVideoStreamsLogo', arg);
};

export const SetVideoStreamChannelNumbers = async (arg: iptv.SetVideoStreamChannelNumbersRequest): Promise<void> => {
  if (isDebug) console.log('SetVideoStreamChannelNumbers');
  await hubConnection.invoke('SetVideoStreamChannelNumbers', arg);
};

export const SetVideoStreamsLogoFromEpg = async (arg: iptv.SetVideoStreamsLogoFromEpgRequest): Promise<void> => {
  if (isDebug) console.log('SetVideoStreamsLogoFromEpg');
  await hubConnection.invoke('SetVideoStreamsLogoFromEpg', arg);
};

export const UpdateVideoStream = async (arg: iptv.UpdateVideoStreamRequest): Promise<void> => {
  if (isDebug) console.log('UpdateVideoStream');
  await hubConnection.invoke('UpdateVideoStream', arg);
};

export const UpdateVideoStreams = async (arg: iptv.UpdateVideoStreamsRequest): Promise<void> => {
  if (isDebug) console.log('UpdateVideoStreams');
  await hubConnection.invoke('UpdateVideoStreams', arg);
};

export const UpdateAllVideoStreamsFromParameters = async (arg: iptv.UpdateAllVideoStreamsFromParametersRequest): Promise<void> => {
  if (isDebug) console.log('UpdateAllVideoStreamsFromParameters');
  await hubConnection.invoke('UpdateAllVideoStreamsFromParameters', arg);
};

export const DeleteAllVideoStreamsFromParameters = async (arg: iptv.DeleteAllVideoStreamsFromParametersRequest): Promise<void> => {
  if (isDebug) console.log('DeleteAllVideoStreamsFromParameters');
  await hubConnection.invoke('DeleteAllVideoStreamsFromParameters', arg);
};

export const SetVideoStreamChannelNumbersFromParameters = async (arg: iptv.SetVideoStreamChannelNumbersFromParametersRequest): Promise<void> => {
  if (isDebug) console.log('SetVideoStreamChannelNumbersFromParameters');
  await hubConnection.invoke('SetVideoStreamChannelNumbersFromParameters', arg);
};

export const SetVideoStreamsLogoFromEpgFromParameters = async (arg: iptv.SetVideoStreamsLogoFromEpgFromParametersRequest): Promise<void> => {
  if (isDebug) console.log('SetVideoStreamsLogoFromEpgFromParameters');
  await hubConnection.invoke('SetVideoStreamsLogoFromEpgFromParameters', arg);
};

export const ReSetVideoStreamsLogoFromParameters = async (arg: iptv.ReSetVideoStreamsLogoFromParametersRequest): Promise<void> => {
  if (isDebug) console.log('ReSetVideoStreamsLogoFromParameters');
  await hubConnection.invoke('ReSetVideoStreamsLogoFromParameters', arg);
};

export const SimulateStreamFailureForAll = async (): Promise<void> => {
  if (isDebug) console.log('SimulateStreamFailureForAll');
  await hubConnection.invoke('SimulateStreamFailureForAll');
};

export const SimulateStreamFailure = async (arg: iptv.SimulateStreamFailureRequest): Promise<void> => {
  if (isDebug) console.log('SimulateStreamFailure');
  await hubConnection.invoke('SimulateStreamFailure', arg);
};

