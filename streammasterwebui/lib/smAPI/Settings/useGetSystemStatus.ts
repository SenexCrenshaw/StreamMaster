import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetSystemStatusSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetSystemStatus } from './GetSystemStatusFetch';
import {FieldData, SDSystemStatus } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SDSystemStatus | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetSystemStatus = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetSystemStatus.data);
  const error = useAppSelector((state) => state.GetSystemStatus.error ?? '');
  const isError = useAppSelector((state) => state.GetSystemStatus.isError?? false);
  const isForced = useAppSelector((state) => state.GetSystemStatus.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetSystemStatus.isLoading ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
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
    const state = store.getState().GetSystemStatus;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data, dispatch]);
useEffect(() => {
  const state = store.getState().GetSystemStatus;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetSystemStatus());
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

export default useGetSystemStatus;
