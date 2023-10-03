/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { hubConnection, invokeHubConnection } from '@/lib/signalr/signalr';
import { isDebug } from '@/lib/settings';
import type * as iptv from '@/lib/iptvApi';

export const CreateM3UFile = async (arg: iptv.CreateM3UFileRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('CreateM3UFile', arg);
};

export const CreateM3UFileFromForm = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('CreateM3UFileFromForm');
};

export const ChangeM3UFileName = async (arg: iptv.ChangeM3UFileNameRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('ChangeM3UFileName', arg);
};

export const DeleteM3UFile = async (arg: iptv.DeleteM3UFileRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('DeleteM3UFile', arg);
};

export const ProcessM3UFile = async (arg: iptv.ProcessM3UFileRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('ProcessM3UFile', arg);
};

export const RefreshM3UFile = async (arg: iptv.RefreshM3UFileRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('RefreshM3UFile', arg);
};

export const ScanDirectoryForM3UFiles = async (): Promise<void | null> => {
    await invokeHubConnection<void> ('ScanDirectoryForM3UFiles');
};

export const UpdateM3UFile = async (arg: iptv.UpdateM3UFileRequest): Promise<void | null> => {
    await invokeHubConnection<void> ('UpdateM3UFile', arg);
};

