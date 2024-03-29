import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetPagedChannelGroups, intSetGetPagedChannelGroupsIsLoading, updateGetPagedChannelGroups } from './GetPagedChannelGroupsSlice';
import { useEffect } from 'react';
import { fetchGetPagedChannelGroups } from './ChannelGroupsFetch';
import {FieldData, ChannelGroupDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<ChannelGroupDto> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetPagedChannelGroupsField: (fieldData: FieldData) => void;
  refreshGetPagedChannelGroups: () => void;
  setGetPagedChannelGroupsIsLoading: (isLoading: boolean) => void;
}
const useGetPagedChannelGroups = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedChannelGroups.data[query]);
  const isLoading = useAppSelector((state) => state.GetPagedChannelGroups.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.GetPagedChannelGroups.isError[query] ?? false);
  const error = useAppSelector((state) => state.GetPagedChannelGroups.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || query === undefined) return;
    const state = store.getState().GetPagedChannelGroups;
    if (state.data[query] !== undefined || state.isLoading[query]) return;
    dispatch(fetchGetPagedChannelGroups(query));
  }, [data, dispatch, params, query]);

  const setGetPagedChannelGroupsField = (fieldData: FieldData): void => {
    dispatch(updateGetPagedChannelGroups({ fieldData: fieldData }));
  };

  const refreshGetPagedChannelGroups = (): void => {
    dispatch(clearGetPagedChannelGroups());
  };

  const setGetPagedChannelGroupsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetPagedChannelGroupsIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshGetPagedChannelGroups, setGetPagedChannelGroupsField, setGetPagedChannelGroupsIsLoading };
};

export default useGetPagedChannelGroups;
