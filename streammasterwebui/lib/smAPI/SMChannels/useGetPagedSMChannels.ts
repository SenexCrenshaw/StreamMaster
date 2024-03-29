import { QueryHookResult, GetApiArgument } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetPagedSMChannels, intSetGetPagedSMChannelsIsLoading, updateGetPagedSMChannels } from './GetPagedSMChannelsSlice';
import { useEffect } from 'react';
import { fetchGetPagedSMChannels } from './SMChannelsFetch';
import { FieldData, SMChannelDto, PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMChannelDto> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetPagedSMChannelsField: (fieldData: FieldData) => void;
  refreshGetPagedSMChannels: () => void;
  setGetPagedSMChannelsIsLoading: (isLoading: boolean) => void;
}
const useGetPagedSMChannels = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedSMChannels.data[query]);
  const isLoading = useAppSelector((state) => state.GetPagedSMChannels.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.GetPagedSMChannels.isError[query] ?? false);
  const error = useAppSelector((state) => state.GetPagedSMChannels.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || query === undefined) return;
    const state = store.getState().GetPagedSMChannels;

    // if (state.data[query] !== undefined || state.isLoading[query]) return;
    if (data !== undefined || state.isLoading[query]) return;
    dispatch(fetchGetPagedSMChannels(query));
  }, [data, dispatch, params, query]);

  const setGetPagedSMChannelsField = (fieldData: FieldData): void => {
    dispatch(updateGetPagedSMChannels({ fieldData: fieldData }));
  };

  const refreshGetPagedSMChannels = (): void => {
    dispatch(clearGetPagedSMChannels());
  };

  const setGetPagedSMChannelsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetPagedSMChannelsIsLoading({ isLoading: isLoading }));
  };

  return { data, error, isError, isLoading, refreshGetPagedSMChannels, setGetPagedSMChannelsField, setGetPagedSMChannelsIsLoading };
};

export default useGetPagedSMChannels;
