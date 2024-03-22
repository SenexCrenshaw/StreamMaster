import { FieldData, GetApiArgument, PagedResponse, QueryHookResult,SMChannelDto } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { fetchGetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsFetch';
import { clearSMChannels, updateSMChannels } from '@lib/smAPI/SMChannels/SMChannelsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMChannelDto> | undefined> {}

interface SMChannelDtoResult extends ExtendedQueryHookResult {
  setSMChannelsField: (fieldData: FieldData) => void;
  refreshSMChannels: () => void;
}

const useSMChannels = (params?: GetApiArgument | undefined): SMChannelDtoResult => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.SMChannels.data[query]);
  const isLoading = useAppSelector((state) => state.SMChannels.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.SMChannels.isError[query] ?? false);
  const error = useAppSelector((state) => state.SMChannels.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || data !== undefined) return;
    dispatch(fetchGetPagedSMChannels(query));
  }, [data, dispatch, params, query]);

  const setSMChannelsField = (fieldData: FieldData): void => {
    dispatch(updateSMChannels({ fieldData: fieldData }));
  };

  const refreshSMChannels = (): void => {
    dispatch(clearSMChannels());
  };

  return { data, error, isError, isLoading, refreshSMChannels, setSMChannelsField };
};

export default useSMChannels;
