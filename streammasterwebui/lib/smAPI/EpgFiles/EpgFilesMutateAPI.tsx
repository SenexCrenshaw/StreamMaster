/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const CreateEpgFile = async (arg: iptv.CreateEpgFileRequest): Promise<void> => {
  if (isDebug) console.log('CreateEpgFile');
  await hubConnection.invoke('CreateEpgFile', arg);
};

export const CreateEpgFileFromForm = async (): Promise<void> => {
  if (isDebug) console.log('CreateEpgFileFromForm');
  await hubConnection.invoke('CreateEpgFileFromForm');
};

export const DeleteEpgFile = async (arg: iptv.DeleteEpgFileRequest): Promise<void> => {
  if (isDebug) console.log('DeleteEpgFile');
  await hubConnection.invoke('DeleteEpgFile', arg);
};

export const ProcessEpgFile = async (arg: iptv.ProcessEpgFileRequest): Promise<void> => {
  if (isDebug) console.log('ProcessEpgFile');
  await hubConnection.invoke('ProcessEpgFile', arg);
};

export const RefreshEpgFile = async (arg: iptv.RefreshEpgFileRequest): Promise<void> => {
  if (isDebug) console.log('RefreshEpgFile');
  await hubConnection.invoke('RefreshEpgFile', arg);
};

export const ScanDirectoryForEpgFiles = async (): Promise<void> => {
  if (isDebug) console.log('ScanDirectoryForEpgFiles');
  await hubConnection.invoke('ScanDirectoryForEpgFiles');
};

export const UpdateEpgFile = async (arg: iptv.UpdateEpgFileRequest): Promise<void> => {
  if (isDebug) console.log('UpdateEpgFile');
  await hubConnection.invoke('UpdateEpgFile', arg);
};

