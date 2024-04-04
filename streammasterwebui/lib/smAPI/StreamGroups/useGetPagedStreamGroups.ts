import { QueryHookResult,GetApiArgument } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetPagedStreamGroupsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetPagedStreamGroups } from './GetPagedStreamGroupsFetch';
import {FieldData, StreamGroupDto,PagedResponse } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<PagedResponse<StreamGroupDto> | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetPagedStreamGroups = (params?: GetApiArgument | undefined): Result => {
  const dispatch = useAppDispatch();
  const query = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetPagedStreamGroups.data[query]);
  const error = useAppSelector((state) => state.GetPagedStreamGroups.error[query] ?? '');
  const isError = useAppSelector((state) => state.GetPagedStreamGroups.isError[query] ?? false);
  const isForced = useAppSelector((state) => state.GetPagedStreamGroups.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetPagedStreamGroups.isLoading[query] ?? false);

const SetIsForced = useCallback(
  (forceRefresh: boolean, query?: string): void => {
    dispatch(setIsForced({ force: forceRefresh }));
  },
  [dispatch]
);


const SetIsLoading = useCallback(
  (isLoading: boolean, query: string): void => {
    dispatch(setIsLoading({ query: query, isLoading: isLoading }));
  },
  [dispatch]
);
useEffect(() => {
  if (query === undefined) return;
  const state = store.getState().GetPagedStreamGroups;

  if (data === undefined && state.isLoading[query] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch, query]);


const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  data,
  error,
  isError,
  isLoading,
  Clear,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetPagedStreamGroups;
