/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { invokeHubConnection } from '@/lib/signalr/signalr';
import type * as iptv from '@/lib/iptvApi';

export const CreateEpgFile = async (arg: iptv.CreateEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('CreateEpgFile', arg);
};

export const CreateEpgFileFromForm = async (): Promise<void | null> => {
  await invokeHubConnection<void>('CreateEpgFileFromForm');
};

export const DeleteEpgFile = async (arg: iptv.DeleteEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('DeleteEpgFile', arg);
};

export const ProcessEpgFile = async (arg: iptv.ProcessEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('ProcessEpgFile', arg);
};

export const RefreshEpgFile = async (arg: iptv.RefreshEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('RefreshEpgFile', arg);
};

export const ScanDirectoryForEpgFiles = async (): Promise<void | null> => {
  await invokeHubConnection<void>('ScanDirectoryForEpgFiles');
};

export const UpdateEpgFile = async (arg: iptv.UpdateEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('UpdateEpgFile', arg);
};
