import { FieldData, GetApiArgument, PagedResponse, QueryHookResult,SMStreamDto } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { fetchGetPagedSMStreams } from '@lib/smAPI/SMStreams/SMStreamsFetch';
import { clearSMStreams, intSetSMStreamsIsLoading, updateSMStreams } from '@lib/smAPI/SMStreams/SMStreamsSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMStreamDto> | undefined> {}

interface SMStreamDtoResult extends ExtendedQueryHookResult {
  setSMStreamsField: (fieldData: FieldData) => void;
  refreshSMStreams: () => void;
  setSMStreamsIsLoading: (isLoading: boolean) => void;
}

const useSMStreams = (params?: GetApiArgument | undefined): SMStreamDtoResult => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.SMStreams.data[query]);
  const isLoading = useAppSelector((state) => state.SMStreams.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.SMStreams.isError[query] ?? false);
  const error = useAppSelector((state) => state.SMStreams.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || data !== undefined) return;
    dispatch(fetchGetPagedSMStreams(query));
  }, [data, dispatch, params, query]);

  const setSMStreamsField = (fieldData: FieldData): void => {
    dispatch(updateSMStreams({ fieldData: fieldData }));
  };

  const refreshSMStreams = (): void => {
    dispatch(clearSMStreams());
  };

  const setSMStreamsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetSMStreamsIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshSMStreams, setSMStreamsField, setSMStreamsIsLoading };
};

export default useSMStreams;
