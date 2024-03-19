/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const CreateM3UFile = async (argument: iptv.CreateM3UFileRequest): Promise<void | null> => {
  await invokeHubCommand<void>('CreateM3UFile', argument);
};
export const CreateM3UFileFromForm = async (): Promise<void | null> => {
  await invokeHubCommand<void>('CreateM3UFileFromForm');
};
export const ChangeM3UFileName = async (argument: iptv.ChangeM3UFileNameRequest): Promise<void | null> => {
  await invokeHubCommand<void>('ChangeM3UFileName', argument);
};
export const DeleteM3UFile = async (argument: iptv.DeleteM3UFileRequest): Promise<void | null> => {
  await invokeHubCommand<void>('DeleteM3UFile', argument);
};
export const ProcessM3UFile = async (argument: iptv.ProcessM3UFileRequest): Promise<void | null> => {
  await invokeHubCommand<void>('ProcessM3UFile', argument);
};
export const RefreshM3UFile = async (argument: iptv.RefreshM3UFileRequest): Promise<void | null> => {
  await invokeHubCommand<void>('RefreshM3UFile', argument);
};
export const ScanDirectoryForM3UFiles = async (): Promise<void | null> => {
  await invokeHubCommand<void>('ScanDirectoryForM3UFiles');
};
export const UpdateM3UFile = async (argument: iptv.UpdateM3UFileRequest): Promise<void | null> => {
  await invokeHubCommand<void>('UpdateM3UFile', argument);
};
