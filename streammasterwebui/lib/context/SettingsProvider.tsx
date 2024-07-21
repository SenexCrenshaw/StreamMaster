import { Logger } from '@lib/common/logger';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SettingDto, UpdateSettingParameters, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import React, { ReactNode, createContext, useCallback, useContext, useEffect, useState } from 'react';

interface SettingsProviderState {
  currentSettingRequest: SettingDto;
  setCurrentSettingRequest: (setting: SettingDto) => void;
  updateSettingRequest: UpdateSettingRequest;
  setUpdateSettingRequest: (updateSettingRequest: UpdateSettingRequest) => void;
  updateStateAndRequest: (updatedFields: Partial<UpdateSettingParameters>) => void;
}

const SettingsContext = createContext<SettingsProviderState | undefined>(undefined);

interface SettingsProviderProps {
  children: ReactNode;
}

export const SettingsProvider: React.FC<SettingsProviderProps> = ({ children }) => {
  const [currentSettingRequest, setIntCurrentSettingRequest] = useState<SettingDto>({} as SettingDto);
  const [updateSettingRequest, setUpdateSettingRequest] = useState<UpdateSettingRequest>({} as UpdateSettingRequest);

  const data = useGetSettings();

  const setCurrentSettingRequest = useCallback((setting: SettingDto) => {
    setIntCurrentSettingRequest(setting);
  }, []);

  useEffect(() => {
    if (data.data !== undefined) {
      setCurrentSettingRequest(data.data);
      // setUpdateSettingRequest({} as UpdateSettingRequest);
    }
  }, [data, setCurrentSettingRequest]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<UpdateSettingParameters>) => {
      const updateSettingParameters = { ...updateSettingRequest.Parameters, ...updatedFields };
      Logger.debug('SettingsProvider', updateSettingParameters, { toSet: updatedFields });

      const request = {
        Parameters: updateSettingParameters
      } as UpdateSettingRequest;

      setUpdateSettingRequest(request);
    },
    [updateSettingRequest]
  );

  const contextValue = {
    currentSettingRequest: currentSettingRequest,
    setCurrentSettingRequest: setCurrentSettingRequest,
    setUpdateSettingRequest: setUpdateSettingRequest,
    updateSettingRequest: updateSettingRequest,
    updateStateAndRequest: updateStateAndRequest
  };

  return <SettingsContext.Provider value={contextValue}>{children}</SettingsContext.Provider>;
};

export const useSettingsContext = (): SettingsProviderState => {
  const context = useContext(SettingsContext);
  if (!context) {
    throw new Error('useSettingsContext must be used within a SettingsContext');
  }
  return context;
};
