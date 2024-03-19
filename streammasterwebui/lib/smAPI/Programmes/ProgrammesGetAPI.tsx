/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import type * as iptv from '@lib/iptvApi';
import { invokeHubCommand } from '@lib/signalr/signalr';

export const GetProgramme = async (argument: iptv.XmltvProgramme[]): Promise<iptv.XmltvProgramme[] | null> =>
  invokeHubCommand<iptv.XmltvProgramme[]>('GetProgramme', argument);
export const GetProgrammes = async (argument: iptv.XmltvProgramme[]): Promise<iptv.XmltvProgramme[] | null> =>
  invokeHubCommand<iptv.XmltvProgramme[]>('GetProgrammes', argument);
