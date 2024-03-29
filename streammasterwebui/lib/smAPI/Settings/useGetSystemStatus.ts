import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetSystemStatus, intSetGetSystemStatusIsLoading, updateGetSystemStatus } from './GetSystemStatusSlice';
import { useEffect } from 'react';
import { fetchGetSystemStatus } from './SettingsFetch';
import {FieldData, SDSystemStatus } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SDSystemStatus | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetSystemStatusField: (fieldData: FieldData) => void;
  refreshGetSystemStatus: () => void;
  setGetSystemStatusIsLoading: (isLoading: boolean) => void;
}
const useGetSystemStatus = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetSystemStatus.data);
  const isLoading = useAppSelector((state) => state.GetSystemStatus.isLoading ?? false);
  const isError = useAppSelector((state) => state.GetSystemStatus.isError ?? false);
  const error = useAppSelector((state) => state.GetSystemStatus.error ?? '');

  useEffect(() => {
    const test = store.getState().GetSystemStatus;
    if (test.data !== undefined || test.isLoading) return;
    dispatch(fetchGetSystemStatus());
  }, [data, dispatch]);

  const setGetSystemStatusField = (fieldData: FieldData): void => {
    dispatch(updateGetSystemStatus({ fieldData: fieldData }));
  };

  const refreshGetSystemStatus = (): void => {
    dispatch(clearGetSystemStatus());
  };

  const setGetSystemStatusIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetSystemStatusIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshGetSystemStatus, setGetSystemStatusField, setGetSystemStatusIsLoading };
};

export default useGetSystemStatus;
