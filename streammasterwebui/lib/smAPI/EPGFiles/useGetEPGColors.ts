import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetEPGColorsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetEPGColors } from './GetEPGColorsFetch';
import {FieldData, EPGColorDto } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<EPGColorDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetEPGColors = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetEPGColors.data);
  const error = useAppSelector((state) => state.GetEPGColors.error ?? '');
  const isError = useAppSelector((state) => state.GetEPGColors.isError?? false);
  const isForced = useAppSelector((state) => state.GetEPGColors.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetEPGColors.isLoading ?? false);

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
    const state = store.getState().GetEPGColors;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data, dispatch]);
useEffect(() => {
  const state = store.getState().GetEPGColors;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetEPGColors());
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

export default useGetEPGColors;
