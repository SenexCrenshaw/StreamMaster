import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetSystemStatusSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetSystemStatus } from './SettingsFetch';
import {FieldData, SDSystemStatus } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SDSystemStatus | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query?: string) => void;
}
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetSystemStatus.data[query]);
  const error = useAppSelector((state) => state.GetSystemStatus.error[query] ?? '');
  const isError = useAppSelector((state) => state.GetSystemStatus.isError[query] ?? false);
  const isForced = useAppSelector((state) => state.GetSystemStatus.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetSystemStatus.isLoading[query] ?? false);

const SetIsForced = useCallback(
  (forceRefresh: boolean, query?: string): void => {
    dispatch(setIsForced({ force: forceRefresh }));
  },
  [dispatch]
);


const SetIsLoading = useCallback(
  (isLoading: boolean, query?: string): void => {
    dispatch(setIsLoading({ query: query, isLoading: isLoading }));
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
  if (isLoading) return;
  if (query === undefined && !isForced) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetSystemStatus(query));
}, [data, dispatch, query, isForced, isLoading, SetIsLoading]);

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
