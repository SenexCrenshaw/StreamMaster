import SMLoader from '@components/loader/SMLoader';
import { Logger } from '@lib/common/logger';
import { DataRefreshAll } from '@lib/smAPI/DataRefreshAll';
import { GetIsSystemReady } from '@lib/smAPI/Settings/SettingsCommands';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SettingDto } from '@lib/smAPI/smapiTypes';
import { BlockUI } from 'primereact/blockui';
import React, { ReactNode, createContext, useContext, useEffect, useState } from 'react';

interface SMContextState {
  isSystemReady: boolean;
  setSystemReady: React.Dispatch<React.SetStateAction<boolean>>;
  settings: SettingDto | null;
  setSettings: React.Dispatch<React.SetStateAction<SettingDto | null>>;
}

const SMContext = createContext<SMContextState | undefined>(undefined);

interface SMProviderProps {
  children: ReactNode;
}

export const SMProvider: React.FC<SMProviderProps> = ({ children }) => {
  const [isSystemReady, setSystemReady] = useState<boolean>(false);
  const [settings, setSettings] = useState<SettingDto | null>(null);
  const settingsQuery = useGetSettings();

  useEffect(() => {
    if (settingsQuery.data) {
      setSettings(settingsQuery.data);
    }
  }, [settingsQuery.data]);

  const value = {
    isSystemReady: isSystemReady && settings !== null,
    setSystemReady,
    settings,
    setSettings
  };

  useEffect(() => {
    const checkSystemReady = async () => {
      try {
        const result = await GetIsSystemReady();
        if (result !== isSystemReady) {
          setSystemReady(result ?? false);
          if (result === true && settings !== null) {
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
  }, [isSystemReady, settings]);

  return (
    <SMContext.Provider value={value}>
      {!value.isSystemReady && <SMLoader />}
      <BlockUI blocked={!value.isSystemReady}>{children}</BlockUI>
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
