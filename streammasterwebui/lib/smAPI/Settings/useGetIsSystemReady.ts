import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetIsSystemReady, intSetGetIsSystemReadyIsLoading, updateGetIsSystemReady } from './GetIsSystemReadySlice';
import { useEffect } from 'react';
import { fetchGetIsSystemReady } from './SettingsFetch';
import {FieldData,  } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<boolean | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetIsSystemReadyField: (fieldData: FieldData) => void;
  refreshGetIsSystemReady: () => void;
  setGetIsSystemReadyIsLoading: (isLoading: boolean) => void;
}
const useGetIsSystemReady = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetIsSystemReady.data);
  const isLoading = useAppSelector((state) => state.GetIsSystemReady.isLoading ?? false);
  const isError = useAppSelector((state) => state.GetIsSystemReady.isError ?? false);
  const error = useAppSelector((state) => state.GetIsSystemReady.error ?? '');

  useEffect(() => {
    const test = store.getState().GetIsSystemReady;
    if (test.data !== undefined || test.isLoading) return;
    dispatch(fetchGetIsSystemReady());
  }, [data, dispatch]);

  const setGetIsSystemReadyField = (fieldData: FieldData): void => {
    dispatch(updateGetIsSystemReady({ fieldData: fieldData }));
  };

  const refreshGetIsSystemReady = (): void => {
    dispatch(clearGetIsSystemReady());
  };

  const setGetIsSystemReadyIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetIsSystemReadyIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshGetIsSystemReady, setGetIsSystemReadyField, setGetIsSystemReadyIsLoading };
};

export default useGetIsSystemReady;
