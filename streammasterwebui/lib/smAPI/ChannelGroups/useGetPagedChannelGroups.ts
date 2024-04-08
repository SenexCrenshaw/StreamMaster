import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetPagedChannelGroupsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetPagedChannelGroups } from './GetPagedChannelGroupsFetch';
import {FieldData, ChannelGroupDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<ChannelGroupDto> | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetPagedChannelGroups = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedChannelGroups.data[query]);
  const error = useAppSelector((state) => state.GetPagedChannelGroups.error[query] ?? '');
  const isError = useAppSelector((state) => state.GetPagedChannelGroups.isError[query] ?? false);
  const isForced = useAppSelector((state) => state.GetPagedChannelGroups.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetPagedChannelGroups.isLoading[query] ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
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
  const state = store.getState().GetPagedChannelGroups;

  if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch, query]);
useEffect(() => {
  const state = store.getState().GetPagedChannelGroups;
  if (state.isLoading[query]) return;
  if (query === undefined && !isForced) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, query);
  dispatch(fetchGetPagedChannelGroups(query));
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

export default useGetPagedChannelGroups;
