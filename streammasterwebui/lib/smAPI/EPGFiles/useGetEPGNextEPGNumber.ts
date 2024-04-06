import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetEPGNextEPGNumberSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetEPGNextEPGNumber } from './GetEPGNextEPGNumberFetch';
import {FieldData,  } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<number | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetEPGNextEPGNumber = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetEPGNextEPGNumber.data);
  const error = useAppSelector((state) => state.GetEPGNextEPGNumber.error ?? '');
  const isError = useAppSelector((state) => state.GetEPGNextEPGNumber.isError?? false);
  const isForced = useAppSelector((state) => state.GetEPGNextEPGNumber.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetEPGNextEPGNumber.isLoading ?? false);

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
  const state = store.getState().GetEPGNextEPGNumber;

  if (data === undefined && state.isLoading !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch]);
useEffect(() => {
  const state = store.getState().GetEPGNextEPGNumber;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetEPGNextEPGNumber());
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

export default useGetEPGNextEPGNumber;
