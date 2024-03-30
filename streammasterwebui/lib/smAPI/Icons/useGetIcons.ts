import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetIconsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetIcons } from './IconsFetch';
import {FieldData, IconFileDto } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<IconFileDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query?: string) => void;
}
const useGetIcons = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetIcons.data);
  const error = useAppSelector((state) => state.GetIcons.error ?? '');
  const isError = useAppSelector((state) => state.GetIcons.isError?? false);
  const isForced = useAppSelector((state) => state.GetIcons.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetIcons.isLoading ?? false);

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
  const state = store.getState().GetIcons;
  if (data === undefined && state.isLoading !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch]);

useEffect(() => {
  const state = store.getState().GetIcons;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetIcons());
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

export default useGetIcons;
