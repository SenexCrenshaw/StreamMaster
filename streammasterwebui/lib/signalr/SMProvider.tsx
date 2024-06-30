import SMLoader from '@components/loader/SMLoader';
import { Logger } from '@lib/common/logger';
import { GetIsSystemReady } from '@lib/smAPI/General/GeneralCommands';
import useGetIsSystemReady from '@lib/smAPI/General/useGetIsSystemReady';
import useGetTaskIsRunning from '@lib/smAPI/General/useGetTaskIsRunning';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useCallback, useContext, useEffect, useState } from 'react';

interface SMContextState {
  // clientStreamingStatistics: ClientStreamingStatistics[];
  // channelStreamingStatistics: ChannelStreamingStatistics[];
  isSystemReady: boolean;
  isTaskRunning: boolean;
  // setSettings: React.Dispatch<React.SetStateAction<SettingDto>>;
  // setSystemReady: React.Dispatch<React.SetStateAction<boolean>>;
  settings: SettingDto;
  // streamStreamingStatistics: StreamStreamingStatistic[];
}

const SMContext = createContext<SMContextState | undefined>(undefined);

interface SMProviderProps {
  children: ReactNode;
}

export const SMProvider: React.FC<SMProviderProps> = ({ children }) => {
  const [isSystemReady, setSystemReady] = useState<boolean>(false);
  const [settings, setSettings] = useState<SettingDto>({} as SettingDto);

  const settingsQuery = useGetSettings();
  const { data: isSystemReadyQ } = useGetIsSystemReady();
  const { data: isTaskRunning } = useGetTaskIsRunning();

  useEffect(() => {
    if (settingsQuery.data) {
      setSettings(settingsQuery.data);
    }
  }, [settingsQuery.data]);

  // Function to check system readiness and update statistics
  const checkSystemReady = useCallback(async () => {
    try {
      const systemReady = await GetIsSystemReady();
      if (systemReady !== isSystemReady) {
        setSystemReady(systemReady ?? false);
        if (systemReady === true && settings.EnableSSL !== undefined) {
          // await DataRefreshAll();
        }
      }

      // const [channelStats, clientStats, streamStats] = await Promise.all([
      //   GetChannelStreamingStatistics(),
      //   GetClientStreamingStatistics(),
      //   GetStreamStreamingStatistics()
      // ]);

      // setChannelStreamingStatistics(channelStats ?? []);
      // setClientStreamingStatistics(clientStats ?? []);
      // setStreamStreamingStatistics(streamStats ?? []);
    } catch (error) {
      Logger.error('Error checking system readiness', { error });
      setSystemReady(false);
    }
  }, [isSystemReady, settings.EnableSSL]);

  useEffect(() => {
    // Initial check
    checkSystemReady();

    // Interval to check system readiness
    const intervalId = setInterval(checkSystemReady, 1000);

    // Clean up interval on component unmount
    return () => clearInterval(intervalId);
  }, [checkSystemReady]);

  const contextValue = {
    // channelStreamingStatistics,
    // clientStreamingStatistics,
    isSystemReady: isSystemReady === true && settings.EnableSSL !== undefined,
    isTaskRunning: isTaskRunning ?? false,
    settings
    // streamStreamingStatistics
  };

  return (
    <SMContext.Provider value={contextValue}>
      {!contextValue.isSystemReady && <SMLoader />}
      <BlockUI blocked={!contextValue.isSystemReady}>{children}</BlockUI>
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
