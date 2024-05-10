import { QueryHookResult, QueryStringParameters } from '@lib/apiDefs';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import store, { RootState } from '@lib/redux/store';
import { ChannelGroupDto, FieldData, PagedResponse } from '@lib/smAPI/smapiTypes';
import { useCallback, useEffect } from 'react';
import { fetchGetPagedChannelGroups } from './GetPagedChannelGroupsFetch';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetPagedChannelGroupsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<ChannelGroupDto> | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetPagedChannelGroups = (params?: QueryStringParameters | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const isForced = useAppSelector((state) => state.GetPagedChannelGroups.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );
  const ClearByTag = useCallback(
    (tag: string): void => {
      dispatch(clearByTag({ tag: tag }));
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
    if (query === undefined) return undefined;
    return state.GetPagedChannelGroups.data[query] || undefined;
  };
  const data = useAppSelector(selectData);

  const selectError = (state: RootState) => {
    if (query === undefined) return undefined;
    return state.GetPagedChannelGroups.error[query] || undefined;
  };
  const error = useAppSelector(selectError);

  const selectIsError = (state: RootState) => {
    if (query === undefined) return false;
    return state.GetPagedChannelGroups.isError[query] || false;
  };
  const isError = useAppSelector(selectIsError);

  const selectIsLoading = (state: RootState) => {
    if (query === undefined) return false;
    return state.GetPagedChannelGroups.isLoading[query] || false;
  };
  const isLoading = useAppSelector(selectIsLoading);

  useEffect(() => {
    if (query === undefined) return;
    const state = store.getState().GetPagedChannelGroups;

    if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [data, query, SetIsForced]);

  useEffect(() => {
    const state = store.getState().GetPagedChannelGroups;
    if (state.isLoading[query]) return;
    if (query === undefined) return;
    if (data !== undefined && !isForced) return;

    SetIsLoading(true, query);
    dispatch(fetchGetPagedChannelGroups(query));
  }, [data, dispatch, isForced, query, SetIsLoading]);

  const SetField = (fieldData: FieldData): void => {
    dispatch(setField({ fieldData: fieldData }));
  };

  const Clear = (): void => {
    dispatch(clear());
  };

  return {
    Clear,
    ClearByTag,
    data,
    error,
    isError,
    isLoading,
    SetField,
    SetIsForced,
    SetIsLoading
  };
};

export default useGetPagedChannelGroups;
