import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetPagedSMStreamsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetPagedSMStreams } from './SMStreamsFetch';
import {FieldData, SMStreamDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMStreamDto> | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetPagedSMStreams = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedSMStreams.data[query]);
  const error = useAppSelector((state) => state.GetPagedSMStreams.error[query] ?? '');
  const isError = useAppSelector((state) => state.GetPagedSMStreams.isError[query] ?? false);
  const isForced = useAppSelector((state) => state.GetPagedSMStreams.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetPagedSMStreams.isLoading[query] ?? false);

const SetIsForced = useCallback(
  (forceRefresh: boolean, query?: string): void => {
    dispatch(setIsForced({ force: forceRefresh }));
  },
  [dispatch]
);


const SetIsLoading = useCallback(
  (isLoading: boolean, query: string): void => {
    dispatch(setIsLoading({ query: query, isLoading: isLoading }));
  },
  [dispatch]
);
useEffect(() => {
  if (query === undefined) return;
  const state = store.getState().GetPagedSMStreams;

  if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch, query]);

useEffect(() => {
  const state = store.getState().GetPagedSMStreams;
  if (state.isLoading[query]) return;
  if (query === undefined && !isForced) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, query);
  dispatch(fetchGetPagedSMStreams(query));
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

export default useGetPagedSMStreams;
