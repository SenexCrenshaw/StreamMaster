/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@/lib/iptvApi';
import { invokeHubConnection } from '@/lib/signalr/signalr';

export const CreateEpgFile = async (argument: iptv.CreateEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('CreateEpgFile', argument);
};
export const CreateEpgFileFromForm = async (): Promise<void | null> => {
  await invokeHubConnection<void>('CreateEpgFileFromForm');
};
export const DeleteEpgFile = async (argument: iptv.DeleteEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('DeleteEpgFile', argument);
};
export const ProcessEpgFile = async (argument: iptv.ProcessEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('ProcessEpgFile', argument);
};
export const RefreshEpgFile = async (argument: iptv.RefreshEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('RefreshEpgFile', argument);
};
export const ScanDirectoryForEpgFiles = async (): Promise<void | null> => {
  await invokeHubConnection<void>('ScanDirectoryForEpgFiles');
};
export const UpdateEpgFile = async (argument: iptv.UpdateEpgFileRequest): Promise<void | null> => {
  await invokeHubConnection<void>('UpdateEpgFile', argument);
};
