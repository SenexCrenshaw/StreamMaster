import { Logger } from '@lib/common/logger';
import useGetSettings from '@lib/smAPI/Settings/useGetSettings';
import { SettingDto, UpdateSettingParameters, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';
import React, { ReactNode, createContext, useCallback, useContext, useEffect, useState } from 'react';

interface SettingsProviderState {
  currentSetting: SettingDto;
  setCurrentSetting: (setting: SettingDto) => void;
  updateSettingRequest: UpdateSettingRequest;
  setUpdateSettingRequest: (updateSettingRequest: UpdateSettingRequest) => void;
  updateStateAndRequest: (updatedFields: Partial<UpdateSettingParameters>) => void;
}

const SettingsContext = createContext<SettingsProviderState | undefined>(undefined);

interface SettingsProviderProps {
  children: ReactNode;
}

export const SettingsProvider: React.FC<SettingsProviderProps> = ({ children }) => {
  const [currentSetting, setIntCurrentSetting] = useState<SettingDto>({} as SettingDto);
  const [updateSettingRequest, setUpdateSettingRequest] = useState<UpdateSettingRequest>({} as UpdateSettingRequest);

  const data = useGetSettings();

  const setCurrentSetting = useCallback((setting: SettingDto) => {
    setIntCurrentSetting(setting);
  }, []);

  useEffect(() => {
    if (data.data !== undefined) {
      setCurrentSetting(data.data);
      // setUpdateSettingRequest({} as UpdateSettingRequest);
    }
  }, [data, setCurrentSetting]);

  const updateStateAndRequest = useCallback(
    (updatedFields: Partial<UpdateSettingParameters>) => {
      // Initialize the updateSettingParameters with existing settings and updated fields
      const updateSettingParameters = {
        ...updateSettingRequest.Parameters,
        ...updatedFields,
        SDSettings: updatedFields.SDSettings
      };
      // Get a list of updated fields including nested fields
      const updatedFieldsList = getUpdatedFieldsList(updatedFields);

      // Iterate over the updated fields to handle nested SDSettings updates
      updatedFieldsList.forEach((field) => {
        if (field.key.startsWith('SDSettings.')) {
          Logger.debug('SettingsProvider', field.key, field.value);
          const key = field.key.replace('SDSettings.', '');

          // Ensure SDSettings object exists
          if (!updateSettingParameters.SDSettings) {
            updateSettingParameters.SDSettings = {};
          }

          // Type assertion for SDSettings object
          (updateSettingParameters.SDSettings as any)[key] = field.value;
        } else {
          Logger.debug('SettingsProvider', field.key, field.value);

          // Type assertion for updateSettingParameters object
          (updateSettingParameters as any)[field.key] = field.value;
        }
      });

      Logger.debug('SettingsProvider', updateSettingParameters, { toSet: updatedFields });

      const request = {
        Parameters: updateSettingParameters
      } as UpdateSettingRequest;

      setUpdateSettingRequest(request);
    },
    [updateSettingRequest]
  );

  // Utility function to return a list of { key, value } objects
  function getUpdatedFieldsList(fields: any): { key: string; value: any }[] {
    const updatedFieldsList: { key: string; value: any }[] = [];

    function flattenObject(obj: any, prefix = '') {
      for (const [key, value] of Object.entries(obj)) {
        const newKey = prefix ? `${prefix}.${key}` : key;
        if (typeof value === 'object' && value !== null && !Array.isArray(value)) {
          flattenObject(value, newKey);
        } else {
          updatedFieldsList.push({ key: newKey, value });
        }
      }
    }

    flattenObject(fields);
    return updatedFieldsList;
  }

  const contextValue = {
    currentSetting: currentSetting,
    setCurrentSetting: setCurrentSetting,
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
