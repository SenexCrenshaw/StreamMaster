/* eslint unused-imports/no-unused-imports-ts: off */
/* eslint @typescript-eslint/no-unused-vars: off */
import { StringArgument } from '@components/selectors/BaseSelector';
import type * as iptv from '@lib/iptvApi';
import { invokeHubConnection } from '@lib/signalr/signalr';

export const GetProgramme = async (argument: iptv.XmltvProgramme[]): Promise<iptv.XmltvProgramme[] | null> =>
  invokeHubConnection<iptv.XmltvProgramme[]>('GetProgramme', argument);
export const GetProgrammes = async (argument: iptv.XmltvProgramme[]): Promise<iptv.XmltvProgramme[] | null> =>
  invokeHubConnection<iptv.XmltvProgramme[]>('GetProgrammes', argument);
