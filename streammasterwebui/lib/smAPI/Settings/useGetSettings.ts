import { QueryHookResult } from '@lib/apiDefs';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import store from '@lib/redux/store';
import { FieldData, SettingDto } from '@lib/smAPI/smapiTypes';
import { useCallback, useEffect } from 'react';
import { clear, setField, setIsForced, setIsLoading } from './GetSettingsSlice';
import { fetchGetSettings } from './SettingsFetch';

interface ExtendedQueryHookResult extends QueryHookResult<SettingDto | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetSettings = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetSettings.data);
  const error = useAppSelector((state) => state.GetSettings.error ?? '');
  const isError = useAppSelector((state) => state.GetSettings.isError ?? false);
  const isForced = useAppSelector((state) => state.GetSettings.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetSettings.isLoading ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean, query?: string): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );

  const SetIsLoading = useCallback(
    (isLoading: boolean): void => {
      dispatch(setIsLoading({ isLoading: isLoading }));
    },
    [dispatch]
  );
  useEffect(() => {
    const state = store.getState().GetSettings;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data, dispatch]);

  useEffect(() => {
    const state = store.getState().GetSettings;
    if (state.isLoading) return;
    if (data !== undefined && !isForced) return;

    SetIsLoading(true);
    dispatch(fetchGetSettings());
  }, [data, dispatch, isForced, isLoading, SetIsLoading]);

  const SetField = (fieldData: FieldData): void => {
    dispatch(setField({ fieldData: fieldData }));
  };

  const Clear = (): void => {
    dispatch(clear());
  };

  return {
    data,
    error,
    isError,
    isLoading,
    Clear,
    SetField,
    SetIsForced,
    SetIsLoading
  };
};

export default useGetSettings;
