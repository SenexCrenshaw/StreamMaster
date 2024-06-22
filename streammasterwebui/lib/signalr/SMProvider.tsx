import SMLoader from '@components/loader/SMLoader';
import { DataRefreshAll } from '@lib/smAPI/DataRefreshAll';

import { Logger } from '@lib/common/logger';

import { GetIsSystemReady } from '@lib/smAPI/General/GeneralCommands';
import useGetIsSystemReady from '@lib/smAPI/General/useGetIsSystemReady';
import useGetTaskIsRunning from '@lib/smAPI/General/useGetTaskIsRunning';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { GetChannelStreamingStatistics, GetClientStreamingStatistics, GetStreamStreamingStatistics } from '@lib/smAPI/Statistics/StatisticsCommands';

import { ChannelStreamingStatistics, ClientStreamingStatistics, SettingDto, StreamStreamingStatistic } from '@lib/smAPI/smapiTypes';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useContext, useEffect, useState } from 'react';

interface SMContextState {
  clientStreamingStatistics: ClientStreamingStatistics[];
  channelStreamingStatistics: ChannelStreamingStatistics[];
  isSystemReady: boolean;
  isTaskRunning: boolean;
  setSettings: React.Dispatch<React.SetStateAction<SettingDto>>;
  setSystemReady: React.Dispatch<React.SetStateAction<boolean>>;
  settings: SettingDto;
  streamStreamingStatistics: StreamStreamingStatistic[];
}

const SMContext = createContext<SMContextState | undefined>(undefined);

interface SMProviderProps {
  children: ReactNode;
}

export const SMProvider: React.FC<SMProviderProps> = ({ children }) => {
  const [isSystemReady, setSystemReady] = useState<boolean>(false);
  const [settings, setSettings] = useState<SettingDto>({} as SettingDto);
  const [channelStreamingStatistics, setChannelStreamingStatistics] = useState<ChannelStreamingStatistics[]>([]);
  const [clientStreamingStatistics, setClientStreamingStatistics] = useState<ClientStreamingStatistics[]>([]);
  const [streamStreamingStatistics, setStreamStreamingStatistics] = useState<StreamStreamingStatistic[]>([]);
  const settingsQuery = useGetSettings();
  const { data: isSystemReadyQ } = useGetIsSystemReady();
  const { data: isTaskRunning } = useGetTaskIsRunning();

  useEffect(() => {
    if (settingsQuery.data) {
      setSettings(settingsQuery.data);
    }
  }, [settingsQuery.data]);

  const value = {
    channelStreamingStatistics,
    clientStreamingStatistics,
    isSystemReady: isSystemReadyQ === true && settingsQuery.data !== undefined,
    isTaskRunning: isTaskRunning ?? false,
    setSettings,
    setSystemReady,
    settings,
    streamStreamingStatistics
  };

  useEffect(() => {
    const checkSystemReady = async () => {
      try {
        const t = await GetChannelStreamingStatistics();
        if (t !== undefined) {
          setChannelStreamingStatistics(t);
        }
        const c = await GetClientStreamingStatistics();
        if (c !== undefined) {
          setClientStreamingStatistics(c);
        }
        const s = await GetStreamStreamingStatistics();
        if (s !== undefined) {
          setStreamStreamingStatistics(s);
        }
        // getInputStatistics.SetIsForced(true);
        const result = await GetIsSystemReady();
        if (result !== isSystemReady) {
          setSystemReady(result ?? false);
          if (result === true && settingsQuery.data !== undefined) {
            await DataRefreshAll();
          }
        }
      } catch (error) {
        Logger.error('Error checking system readiness', { error });
        setSystemReady(false);
      }
    };

    const intervalId = setInterval(checkSystemReady, 1000);

    return () => clearInterval(intervalId);
  }, [isSystemReady, settingsQuery.data]);

  // return (
  //   <SMContext.Provider value={value}>
  //     {value.isSystemReady && <SMLoader />}
  //     <BlockUI blocked={value.isSystemReady}>{children}</BlockUI>
  //   </SMContext.Provider>
  // );

  return (
    <SMContext.Provider value={value}>
      {isSystemReady !== true && <SMLoader />}
      <BlockUI blocked={isSystemReady !== true}>{children}</BlockUI>
    </SMContext.Provider>
  );
};

export const useSMContext = (): SMContextState => {
  const context = useContext(SMContext);
  if (!context) {
    throw new Error('useSMContext must be used within a SMProvider');
  }
  return context;
};
