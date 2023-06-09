import { channels } from "./channels";
import { epg } from "./epg";

export const fetchChannels = async () =>
  await new Promise((res) => setTimeout(() => res(channels), 400));

export const fetchEpg = async () =>
  await new Promise((res) => setTimeout(() => res(epg), 500));
