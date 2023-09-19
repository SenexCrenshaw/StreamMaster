import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const CreateM3UFile = async (arg: iptv.CreateM3UFileRequest): Promise<void> => {
  if (isDebug) console.log('CreateM3UFile');
  await hubConnection.invoke('CreateM3UFile', arg);
};

export const CreateM3UFileFromForm = async (): Promise<void> => {
  if (isDebug) console.log('CreateM3UFileFromForm');
  await hubConnection.invoke('CreateM3UFileFromForm');
};

export const ChangeM3UFileName = async (arg: iptv.ChangeM3UFileNameRequest): Promise<void> => {
  if (isDebug) console.log('ChangeM3UFileName');
  await hubConnection.invoke('ChangeM3UFileName', arg);
};

export const DeleteM3UFile = async (arg: iptv.DeleteM3UFileRequest): Promise<void> => {
  if (isDebug) console.log('DeleteM3UFile');
  await hubConnection.invoke('DeleteM3UFile', arg);
};

export const ProcessM3UFile = async (arg: iptv.ProcessM3UFileRequest): Promise<void> => {
  if (isDebug) console.log('ProcessM3UFile');
  await hubConnection.invoke('ProcessM3UFile', arg);
};

export const RefreshM3UFile = async (arg: iptv.RefreshM3UFileRequest): Promise<void> => {
  if (isDebug) console.log('RefreshM3UFile');
  await hubConnection.invoke('RefreshM3UFile', arg);
};

export const ScanDirectoryForM3UFiles = async (): Promise<void> => {
  if (isDebug) console.log('ScanDirectoryForM3UFiles');
  await hubConnection.invoke('ScanDirectoryForM3UFiles');
};

export const UpdateM3UFile = async (arg: iptv.UpdateM3UFileRequest): Promise<void> => {
  if (isDebug) console.log('UpdateM3UFile');
  await hubConnection.invoke('UpdateM3UFile', arg);
};

