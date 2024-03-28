import { FieldData, PagedResponse, M3UFileDto } from '@lib/smAPI/smapiTypes';
import { GetApiArgument, QueryHookResult } from '@lib/apiDefs';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { fetchGetPagedM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesFetch';
import { clearM3UFiles, intSetM3UFilesIsLoading, updateM3UFiles } from '@lib/smAPI/M3UFiles/M3UFilesSlice';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<M3UFileDto> | undefined> {}

interface M3UFileDtoResult extends ExtendedQueryHookResult {
  setM3UFilesField: (fieldData: FieldData) => void;
  refreshM3UFiles: () => void;
  setM3UFilesIsLoading: (isLoading: boolean) => void;
}

const useM3UFiles = (params?: GetApiArgument | undefined): M3UFileDtoResult => {
  const query = JSON.stringify(params);
  const dispatch = useAppDispatch();

  const data = useAppSelector((state) => state.M3UFiles.data[query]);
  const isLoading = useAppSelector((state) => state.M3UFiles.isLoading[query] ?? false);
  const isError = useAppSelector((state) => state.M3UFiles.isError[query] ?? false);
  const error = useAppSelector((state) => state.M3UFiles.error[query] ?? '');

  useEffect(() => {
    if (params === undefined || data !== undefined) return;
    dispatch(fetchGetPagedM3UFiles(query));
  }, [data, dispatch, params, query]);

  const setM3UFilesField = (fieldData: FieldData): void => {
    dispatch(updateM3UFiles({ fieldData: fieldData }));
  };

  const refreshM3UFiles = (): void => {
    dispatch(clearM3UFiles());
  };

  const setM3UFilesIsLoading = (isLoading: boolean): void => {
    dispatch(intSetM3UFilesIsLoading( {isLoading: isLoading} ));
  };

  return { data, error, isError, isLoading, refreshM3UFiles, setM3UFilesField, setM3UFilesIsLoading };
};

export default useM3UFiles;
