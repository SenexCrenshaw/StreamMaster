import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetPagedSMChannelsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetPagedSMChannels } from './GetPagedSMChannelsFetch';
import {FieldData, SMChannelDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMChannelDto> | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetPagedSMChannels = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const isForced = useAppSelector((state) => state.GetPagedSMChannels.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );

  const SetIsLoading = useCallback(
    (isLoading: boolean, query: string): void => {
      dispatch(setIsLoading({ isLoading: isLoading, query: query }));
    },
    [dispatch]
  );

const selectData = (state: RootState) => {
    const defaultData = {} as PagedResponse<SMChannelDto>;
    if (query === undefined) return defaultData;
    return state.GetPagedSMChannels.data[query] || defaultData;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    if (query === undefined) return undefined;
    return state.GetPagedSMChannels.error[query] || undefined;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    if (query === undefined) return false;
    return state.GetPagedSMChannels.isError[query] || false;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    if (query === undefined) return false;
    return state.GetPagedSMChannels.isLoading[query] || false;
  };
const isLoading = useAppSelector(selectIsLoading);


useEffect(() => {
  if (query === undefined) return;
  const state = store.getState().GetPagedSMChannels;

  if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [data, query, SetIsForced]);

useEffect(() => {
  const state = store.getState().GetPagedSMChannels;
  if (state.isLoading[query]) return;
  if (query === undefined) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, query);
  dispatch(fetchGetPagedSMChannels(query));
}, [data, dispatch, isForced, query, SetIsLoading]);

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

export default useGetPagedSMChannels;
