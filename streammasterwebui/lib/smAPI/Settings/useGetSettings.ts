import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetSettings, intSetGetSettingsIsLoading, updateGetSettings } from './GetSettingsSlice';
import { useEffect } from 'react';
import { fetchGetSettings } from './SettingsFetch';
import {FieldData, SettingDto } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SettingDto | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetSettingsField: (fieldData: FieldData) => void;
  refreshGetSettings: () => void;
  setGetSettingsIsLoading: (isLoading: boolean) => void;
}
const useGetSettings = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetSettings.data);
  const isLoading = useAppSelector((state) => state.GetSettings.isLoading ?? false);
  const isError = useAppSelector((state) => state.GetSettings.isError ?? false);
  const error = useAppSelector((state) => state.GetSettings.error ?? '');

  useEffect(() => {
    const test = store.getState().GetSettings;
    if (test.data !== undefined || test.isLoading) return;
    dispatch(fetchGetSettings());
  }, [data, dispatch]);

  const setGetSettingsField = (fieldData: FieldData): void => {
    dispatch(updateGetSettings({ fieldData: fieldData }));
  };

  const refreshGetSettings = (): void => {
    dispatch(clearGetSettings());
  };

  const setGetSettingsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetSettingsIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshGetSettings, setGetSettingsField, setGetSettingsIsLoading };
};

export default useGetSettings;
