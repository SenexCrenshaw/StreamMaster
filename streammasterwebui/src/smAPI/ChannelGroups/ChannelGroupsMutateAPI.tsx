import { hubConnection } from "../../app/signalr";
import { isDebug } from "../../settings";
import type * as iptv from "../../store/iptvApi";

export const CreateChannelGroup = async (arg: iptv.CreateChannelGroupRequest): Promise<void> => {
  if (isDebug) console.log('CreateChannelGroup');
  await hubConnection.invoke('CreateChannelGroup', arg);
};

export const DeleteAllChannelGroupsFromParameters = async (arg: iptv.DeleteAllChannelGroupsFromParametersRequest): Promise<void> => {
  if (isDebug) console.log('DeleteAllChannelGroupsFromParameters');
  await hubConnection.invoke('DeleteAllChannelGroupsFromParameters', arg);
};

export const DeleteChannelGroup = async (arg: iptv.DeleteChannelGroupRequest): Promise<void> => {
  if (isDebug) console.log('DeleteChannelGroup');
  await hubConnection.invoke('DeleteChannelGroup', arg);
};

export const UpdateChannelGroup = async (arg: iptv.UpdateChannelGroupRequest): Promise<void> => {
  if (isDebug) console.log('UpdateChannelGroup');
  await hubConnection.invoke('UpdateChannelGroup', arg);
};

export const UpdateChannelGroups = async (arg: iptv.UpdateChannelGroupsRequest): Promise<void> => {
  if (isDebug) console.log('UpdateChannelGroups');
  await hubConnection.invoke('UpdateChannelGroups', arg);
};

