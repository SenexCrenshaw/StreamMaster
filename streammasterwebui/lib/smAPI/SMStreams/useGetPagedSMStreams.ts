import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetPagedSMStreams, intSetGetPagedSMStreamsIsLoading, updateGetPagedSMStreams } from './GetPagedSMStreamsSlice';
import { useEffect } from 'react';
import { fetchGetPagedSMStreams } from './SMStreamsFetch';
import {FieldData, SMStreamDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<SMStreamDto> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetPagedSMStreamsField: (fieldData: FieldData) => void;
  refreshGetPagedSMStreams: () => void;
  setGetPagedSMStreamsIsLoading: (isLoading: boolean) => void;
}
const useGetPagedSMStreams = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedSMStreams.data[query]);
  const isLoading = useAppSelector((state) => state.GetPagedSMStreams.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.GetPagedSMStreams.isError[query] ?? false);
  const error = useAppSelector((state) => state.GetPagedSMStreams.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || data !== undefined) return;
    dispatch(fetchGetPagedSMStreams(query));
  }, [data, dispatch, params, query]);

  const setGetPagedSMStreamsField = (fieldData: FieldData): void => {
    dispatch(updateGetPagedSMStreams({ fieldData: fieldData }));
  };

  const refreshGetPagedSMStreams = (): void => {
    dispatch(clearGetPagedSMStreams());
  };

  const setGetPagedSMStreamsIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetPagedSMStreamsIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshGetPagedSMStreams, setGetPagedSMStreamsField, setGetPagedSMStreamsIsLoading };
};

export default useGetPagedSMStreams;
