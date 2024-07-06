import { QueryHookResult } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetPagedM3UFilesSlice';
import { useCallback,useEffect } from 'react';
import { SkipToken } from '@reduxjs/toolkit/query';
import { getParameters } from '@lib/common/getParameters';
import { fetchGetPagedM3UFiles } from './GetPagedM3UFilesFetch';
import {FieldData, M3UFileDto,PagedResponse,QueryStringParameters } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<M3UFileDto> | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetPagedM3UFiles = (params?: QueryStringParameters | undefined | SkipToken): Result => {
  const dispatch = useAppDispatch();
  const query = getParameters(params);
  const isForced = useAppSelector((state) => state.GetPagedM3UFiles.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );
  const ClearByTag = useCallback(
    (tag: string): void => {
      dispatch(clearByTag({tag: tag }));
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
    return state.GetPagedM3UFiles.data[query] || undefined;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    if (query === undefined) return undefined;
    return state.GetPagedM3UFiles.error[query] || undefined;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    if (query === undefined) return false;
    return state.GetPagedM3UFiles.isError[query] || false;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    if (query === undefined) return false;
    return state.GetPagedM3UFiles.isLoading[query] || false;
  };
const isLoading = useAppSelector(selectIsLoading);


useEffect(() => {
  if (query === undefined) return;
  const state = store.getState().GetPagedM3UFiles;

  if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [data, query, SetIsForced]);

useEffect(() => {
  if (query === undefined) return;
  const state = store.getState().GetPagedM3UFiles;
  if (state.isLoading[query]) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true, query);
  dispatch(fetchGetPagedM3UFiles(query));
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

export default useGetPagedM3UFiles;
