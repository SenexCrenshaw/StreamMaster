import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clearGetPagedM3UFiles, intSetGetPagedM3UFilesIsLoading, updateGetPagedM3UFiles } from './GetPagedM3UFilesSlice';
import { useEffect } from 'react';
import { fetchGetPagedM3UFiles } from './M3UFilesFetch';
import {FieldData, M3UFileDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<M3UFileDto> | undefined> {}

interface Result extends ExtendedQueryHookResult {
  setGetPagedM3UFilesField: (fieldData: FieldData) => void;
  refreshGetPagedM3UFiles: () => void;
  setGetPagedM3UFilesIsLoading: (isLoading: boolean) => void;
}
const useGetPagedM3UFiles = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedM3UFiles.data[query]);
  const isLoading = useAppSelector((state) => state.GetPagedM3UFiles.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.GetPagedM3UFiles.isError[query] ?? false);
  const error = useAppSelector((state) => state.GetPagedM3UFiles.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || query === undefined) return;
    const state = store.getState().GetPagedM3UFiles;
    if (state.data[query] !== undefined || state.isLoading[query]) return;
    dispatch(fetchGetPagedM3UFiles(query));
  }, [data, dispatch, params, query]);

  const setGetPagedM3UFilesField = (fieldData: FieldData): void => {
    dispatch(updateGetPagedM3UFiles({ fieldData: fieldData }));
  };

  const refreshGetPagedM3UFiles = (): void => {
    dispatch(clearGetPagedM3UFiles());
  };

  const setGetPagedM3UFilesIsLoading = (isLoading: boolean): void => {
    dispatch(intSetGetPagedM3UFilesIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshGetPagedM3UFiles, setGetPagedM3UFilesField, setGetPagedM3UFilesIsLoading };
};

export default useGetPagedM3UFiles;
