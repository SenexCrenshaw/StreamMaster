import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetIsSystemReadySlice';
import { useCallback,useEffect } from 'react';
import { fetchGetIsSystemReady } from './GetIsSystemReadyFetch';
import {FieldData,  } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<boolean | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetIsSystemReady = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetIsSystemReady.data);
  const error = useAppSelector((state) => state.GetIsSystemReady.error ?? '');
  const isError = useAppSelector((state) => state.GetIsSystemReady.isError?? false);
  const isForced = useAppSelector((state) => state.GetIsSystemReady.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetIsSystemReady.isLoading ?? false);

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
  const state = store.getState().GetIsSystemReady;

  if (data === undefined && state.isLoading !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch]);
useEffect(() => {
  const state = store.getState().GetIsSystemReady;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetIsSystemReady());
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

export default useGetIsSystemReady;
