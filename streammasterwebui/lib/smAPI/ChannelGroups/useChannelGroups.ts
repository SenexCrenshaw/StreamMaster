import { FieldData, PagedResponse,  } from '@lib/smAPI/smapiTypes';
import { GetApiArgument, QueryHookResult } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearChannelGroups, intSetChannelGroupsIsLoading, updateChannelGroups } from '@lib/smAPI/ChannelGroups/ChannelGroupsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setChannelGroupsField: (fieldData: FieldData) => void;
  refreshChannelGroups: () => void;
  setChannelGroupsIsLoading: (isLoading: boolean) => void;
}

const useChannelGroups = (params?: GetApiArgument | undefined): Result => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.ChannelGroups.data[query]);
  const isLoading = useAppSelector((state) => state.ChannelGroups.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.ChannelGroups.isError[query] ?? false);
  const error = useAppSelector((state) => state.ChannelGroups.error[query] ?? '');

  const setChannelGroupsField = (fieldData: FieldData): void => {
    dispatch(updateChannelGroups({ fieldData: fieldData }));
  };

  const refreshChannelGroups = (): void => {
    dispatch(clearChannelGroups());
  };

  const setChannelGroupsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetChannelGroupsIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshChannelGroups, setChannelGroupsField, setChannelGroupsIsLoading };
};

export default useChannelGroups;
