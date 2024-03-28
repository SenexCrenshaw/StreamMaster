import { FieldData, PagedResponse,  } from '@lib/smAPI/smapiTypes';
import { GetApiArgument, QueryHookResult } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearSettings, intSetSettingsIsLoading, updateSettings } from '@lib/smAPI/Settings/SettingsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setSettingsField: (fieldData: FieldData) => void;
  refreshSettings: () => void;
  setSettingsIsLoading: (isLoading: boolean) => void;
}

const useSettings = (params?: GetApiArgument | undefined): Result => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.Settings.data[query]);
  const isLoading = useAppSelector((state) => state.Settings.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.Settings.isError[query] ?? false);
  const error = useAppSelector((state) => state.Settings.error[query] ?? '');

  const setSettingsField = (fieldData: FieldData): void => {
    dispatch(updateSettings({ fieldData: fieldData }));
  };

  const refreshSettings = (): void => {
    dispatch(clearSettings());
  };

  const setSettingsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetSettingsIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshSettings, setSettingsField, setSettingsIsLoading };
};

export default useSettings;
