import { QueryHookResult } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
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
  const isForced = useAppSelector((state) => state.GetIsSystemReady.isForced ?? false);

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
const selectData = (state: RootState) => {
    return state.GetIsSystemReady.data;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    return state.GetIsSystemReady.error;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    return state.GetIsSystemReady.isError;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    return state.GetIsSystemReady.isLoading;
  };
const isLoading = useAppSelector(selectIsLoading);


  useEffect(() => {
    const state = store.getState().GetIsSystemReady;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data]);

useEffect(() => {
  const state = store.getState().GetIsSystemReady;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetIsSystemReady());
}, [SetIsLoading, data, dispatch, isForced]);

const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  Clear,
  data,
  error,
  isError,
  isLoading,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetIsSystemReady;