import { hubConnection } from "../../app/signalr";
import type * as iptv from "../../store/iptvApi";

export const CreateVideoStream = async (arg: iptv.CreateVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('CreateVideoStream', arg);
};

export const ChangeVideoStreamChannel = async (arg: iptv.ChangeVideoStreamChannelRequest): Promise<void> => {
  await hubConnection.invoke('ChangeVideoStreamChannel', arg);
};

export const DeleteVideoStream = async (arg: iptv.DeleteVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('DeleteVideoStream', arg);
};

export const FailClient = async (arg: iptv.FailClientRequest): Promise<void> => {
  await hubConnection.invoke('FailClient', arg);
};

export const ReSetVideoStreamsLogo = async (arg: iptv.ReSetVideoStreamsLogoRequest): Promise<void> => {
  await hubConnection.invoke('ReSetVideoStreamsLogo', arg);
};

export const SetVideoStreamChannelNumbers = async (arg: iptv.SetVideoStreamChannelNumbersRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamChannelNumbers', arg);
};

export const SetVideoStreamsLogoFromEpg = async (arg: iptv.SetVideoStreamsLogoFromEpgRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamsLogoFromEpg', arg);
};

export const UpdateVideoStream = async (arg: iptv.UpdateVideoStreamRequest): Promise<void> => {
  await hubConnection.invoke('UpdateVideoStream', arg);
};

export const UpdateVideoStreams = async (arg: iptv.UpdateVideoStreamsRequest): Promise<void> => {
  await hubConnection.invoke('UpdateVideoStreams', arg);
};

export const UpdateAllVideoStreamsFromParameters = async (arg: iptv.UpdateAllVideoStreamsFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('UpdateAllVideoStreamsFromParameters', arg);
};

export const DeleteAllVideoStreamsFromParameters = async (arg: iptv.DeleteAllVideoStreamsFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('DeleteAllVideoStreamsFromParameters', arg);
};

export const SetVideoStreamChannelNumbersFromParameters = async (arg: iptv.SetVideoStreamChannelNumbersFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamChannelNumbersFromParameters', arg);
};

export const SetVideoStreamsLogoFromEpgFromParameters = async (arg: iptv.SetVideoStreamsLogoFromEpgFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('SetVideoStreamsLogoFromEpgFromParameters', arg);
};

export const ReSetVideoStreamsLogoFromParameters = async (arg: iptv.ReSetVideoStreamsLogoFromParametersRequest): Promise<void> => {
  await hubConnection.invoke('ReSetVideoStreamsLogoFromParameters', arg);
};

export const SimulateStreamFailureForAll = async (): Promise<void> => {
  await hubConnection.invoke('SimulateStreamFailureForAll');
};

export const SimulateStreamFailure = async (arg: iptv.SimulateStreamFailureRequest): Promise<void> => {
  await hubConnection.invoke('SimulateStreamFailure', arg);
};

